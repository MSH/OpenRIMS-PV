using Dapper;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using PVIMS.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.WorkFlowAggregate
{
    public class WorkFlowQueries
        : IWorkFlowQueries
    {
        private string _connectionString = string.Empty;

        public WorkFlowQueries(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<WorkFlowSummaryDto> GetWorkFlowFeedbackSummaryAsync(
            Guid workFlowGuid, List<string> userFacilityCodes)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

				var facilityCodeList = userFacilityCodes.Count > 0 ? string.Join(", ", userFacilityCodes.Select(fc => "'" + fc + "'")) : "''";

				var summarySql = @$"
						SELECT	wf.Id,
								wf.WorkFlowGuid,
								wf.Description AS WorkFlowName,
								(
									SELECT COUNT(ri.Id) 
									FROM ReportInstance ri 
									WHERE ri.WorkFlow_Id  = wf.Id
										AND ri.FacilityIdentifier IN ({facilityCodeList})
								) AS SubmissionCount,
								(
									SELECT COUNT(ri.Id) 
									FROM ReportInstance ri 
										INNER JOIN ActivityInstance ai ON ai.ReportInstance_Id = ri.Id AND ai.Current = 1
										INNER JOIN ActivityExecutionStatus aes ON ai.CurrentStatus_Id = aes.Id
									WHERE ri.WorkFlow_Id  = wf.Id
										AND ri.FacilityIdentifier IN ({facilityCodeList})
										AND aes.Description = 'DELETED'
								) AS DeletionCount,
								(
									SELECT COUNT(ri.Id) 
									FROM ReportInstance ri 
										INNER JOIN ActivityInstance ai ON ai.ReportInstance_Id = ri.Id AND ai.Current = 1
									WHERE ri.WorkFlow_Id  = wf.Id
										AND ri.FacilityIdentifier IN ({facilityCodeList})
										AND ai.QualifiedName IN ('Set MedDRA and Causality', 'Extract E2B')
								) AS ReportDataConfirmedCount,
								(
									SELECT COUNT(ri.Id) 
									FROM ReportInstance ri 
										INNER JOIN ActivityInstance ai ON ai.ReportInstance_Id = ri.Id AND ai.Current = 1
									WHERE ri.WorkFlow_Id  = wf.Id
										AND ri.FacilityIdentifier IN ({facilityCodeList})
										AND ai.QualifiedName = 'Extract E2B'
								) AS TerminologyAndCausalityConfirmedCount
						FROM WorkFlow wf
						WHERE wf.WorkFlowGuid = '{workFlowGuid}'";

				var workFlowSummary = await connection.QuerySingleAsync<WorkFlowSummaryDto>(summarySql);

				var classificationsSql = @$"
						SELECT sub.Classification, COUNT(*) AS ClassificationCount, SUM(sub.HasCausality) AS CausativeCount, SUM(sub.E2BSubmitted) AS E2BCount
							FROM 
								(SELECT  'AESI' AS Classification,
										CASE WHEN EXISTS (SELECT rim.Id FROM ReportInstanceMedication rim WHERE rim.ReportInstance_Id = ri.Id AND (rim.NaranjoCausality IN ('Definite', 'Probable', 'Possible') OR rim.WHOCausality IN ('Certain', 'Probable', 'Possible'))) THEN 1 ELSE 0 END AS HasCausality,
										CASE WHEN EXISTS (SELECT evt.Id FROM ActivityExecutionStatusEvent evt INNER JOIN ActivityExecutionStatus aes ON evt.ExecutionStatus_Id = aes.Id WHERE evt.ActivityInstance_Id = ai.Id AND aes.Description = 'E2BSUBMITTED') THEN 1 ELSE 0 END AS E2BSubmitted
								FROM ReportInstance ri 
									INNER JOIN WorkFlow wf ON ri.WorkFlow_Id = wf.Id
									INNER JOIN ActivityInstance ai ON ai.ReportInstance_Id = ri.Id AND ai.Current = 1
									INNER JOIN ActivityExecutionStatus aes ON ai.CurrentStatus_Id = aes.Id
								WHERE wf.WorkFlowGuid = '{workFlowGuid}'
									AND ri.FacilityIdentifier IN ({facilityCodeList})
									AND ri.ReportClassificationId = 1
									AND ai.QualifiedName = 'Extract E2B'
								UNION ALL 
								SELECT  'SAE' AS Classification,
										CASE WHEN EXISTS (SELECT rim.Id FROM ReportInstanceMedication rim WHERE rim.ReportInstance_Id = ri.Id AND (rim.NaranjoCausality IN ('Definite', 'Probable', 'Possible') OR rim.WHOCausality IN ('Certain', 'Probable', 'Possible'))) THEN 1 ELSE 0 END AS HasCausality,
										CASE WHEN EXISTS (SELECT evt.Id FROM ActivityExecutionStatusEvent evt INNER JOIN ActivityExecutionStatus aes ON evt.ExecutionStatus_Id = aes.Id WHERE evt.ActivityInstance_Id = ai.Id AND aes.Description = 'E2BSUBMITTED') THEN 1 ELSE 0 END AS E2BSubmitted
								FROM ReportInstance ri 
									INNER JOIN WorkFlow wf ON ri.WorkFlow_Id = wf.Id
									INNER JOIN ActivityInstance ai ON ai.ReportInstance_Id = ri.Id AND ai.Current = 1
									INNER JOIN ActivityExecutionStatus aes ON ai.CurrentStatus_Id = aes.Id
								WHERE wf.WorkFlowGuid = '{workFlowGuid}'
									AND ri.FacilityIdentifier IN ({facilityCodeList})
									AND ri.ReportClassificationId = 2
									AND ai.QualifiedName = 'Extract E2B'
								UNION ALL 
								SELECT  'Clinically Significant' AS Classification,
										CASE WHEN EXISTS (SELECT rim.Id FROM ReportInstanceMedication rim WHERE rim.ReportInstance_Id = ri.Id AND (rim.NaranjoCausality IN ('Definite', 'Probable', 'Possible') OR rim.WHOCausality IN ('Certain', 'Probable', 'Possible'))) THEN 1 ELSE 0 END AS HasCausality,
										CASE WHEN EXISTS (SELECT evt.Id FROM ActivityExecutionStatusEvent evt INNER JOIN ActivityExecutionStatus aes ON evt.ExecutionStatus_Id = aes.Id WHERE evt.ActivityInstance_Id = ai.Id AND aes.Description = 'E2BSUBMITTED') THEN 1 ELSE 0 END AS E2BSubmitted
								FROM ReportInstance ri 
									INNER JOIN WorkFlow wf ON ri.WorkFlow_Id = wf.Id
									INNER JOIN ActivityInstance ai ON ai.ReportInstance_Id = ri.Id AND ai.Current = 1
									INNER JOIN ActivityExecutionStatus aes ON ai.CurrentStatus_Id = aes.Id
								WHERE wf.WorkFlowGuid = '{workFlowGuid}'
									AND ri.FacilityIdentifier IN ({facilityCodeList})
									AND ri.ReportClassificationId = 3
									AND ai.QualifiedName = 'Extract E2B'
								UNION ALL 
								SELECT  'Unclassified' AS Classification,
										CASE WHEN EXISTS (SELECT rim.Id FROM ReportInstanceMedication rim WHERE rim.ReportInstance_Id = ri.Id AND (rim.NaranjoCausality IN ('Definite', 'Probable', 'Possible') OR rim.WHOCausality IN ('Certain', 'Probable', 'Possible'))) THEN 1 ELSE 0 END AS HasCausality,
										CASE WHEN EXISTS (SELECT evt.Id FROM ActivityExecutionStatusEvent evt INNER JOIN ActivityExecutionStatus aes ON evt.ExecutionStatus_Id = aes.Id WHERE evt.ActivityInstance_Id = ai.Id AND aes.Description = 'E2BSUBMITTED') THEN 1 ELSE 0 END AS E2BSubmitted
								FROM ReportInstance ri 
									INNER JOIN WorkFlow wf ON ri.WorkFlow_Id = wf.Id
									INNER JOIN ActivityInstance ai ON ai.ReportInstance_Id = ri.Id AND ai.Current = 1
									INNER JOIN ActivityExecutionStatus aes ON ai.CurrentStatus_Id = aes.Id
								WHERE wf.WorkFlowGuid = '{workFlowGuid}'
									AND ri.FacilityIdentifier IN ({facilityCodeList})
									AND ri.ReportClassificationId = 4
									AND ai.QualifiedName = 'Extract E2B') AS sub
						GROUP BY sub.Classification
						ORDER BY sub.Classification";

				workFlowSummary.Classifications = await connection.QueryAsync<ClassificationSummaryDto>(classificationsSql);

				return workFlowSummary;
			}
        }
    }
}