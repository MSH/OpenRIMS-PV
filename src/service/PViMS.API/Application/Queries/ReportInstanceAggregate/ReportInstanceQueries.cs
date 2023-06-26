using Dapper;
using Microsoft.Data.SqlClient;
using PVIMS.API.Models;
using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.ReportInstanceAggregate
{
    public class ReportInstanceQueries
        : IReportInstanceQueries
    {
        private string _connectionString = string.Empty;

        public ReportInstanceQueries(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<ReportInstanceEventDto>> GetExecutionStatusEventsForPatientViewAsync(
            int patientId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<ReportInstanceEventDto>(
                    @$"SELECT	evt.Id,
		                        pce.Id AS PatientClinicalEventId,
		                        pce.SourceDescription AS AdverseEvent,
		                        convert(varchar(20), evt.EventDateTime , 120) AS ExecutedDate,
		                        u.FirstName + ' ' + u.LastName AS ExecutedBy,
		                        ai.QualifiedName AS Activity,
		                        stat.FriendlyDescription AS ExecutionEvent,
		                        evt.Comments
                        FROM PatientClinicalEvent pce
	                        INNER JOIN ReportInstance ri ON pce.PatientClinicalEventGuid = ri.ContextGuid
	                        INNER JOIN ActivityInstance ai ON ri.Id = ai.ReportInstance_Id
	                        INNER JOIN ActivityExecutionStatusEvent evt ON ai.Id = evt.ActivityInstance_Id
	                        INNER JOIN ActivityExecutionStatus stat ON evt.ExecutionStatus_Id = stat.Id
	                        INNER JOIN [User] u ON evt.EventCreatedBy_Id = u.Id
                        WHERE pce.Patient_Id = {patientId}
                        ORDER BY evt.EventDateTime desc");
            }
        }

        public async Task<IEnumerable<ReportInstanceEventDto>> GetExecutionStatusEventsForEventViewAsync(
            int patientClinicalEventId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<ReportInstanceEventDto>(
                    @$"SELECT	evt.Id,
		                        pce.Id AS PatientClinicalEventId,
		                        pce.SourceDescription AS AdverseEvent,
		                        convert(varchar(20), evt.EventDateTime , 120) AS ExecutedDate,
		                        u.FirstName + ' ' + u.LastName AS ExecutedBy,
		                        ai.QualifiedName AS Activity,
		                        stat.FriendlyDescription AS ExecutionEvent,
		                        evt.Comments
                        FROM PatientClinicalEvent pce
	                        INNER JOIN ReportInstance ri ON pce.PatientClinicalEventGuid = ri.ContextGuid
	                        INNER JOIN ActivityInstance ai ON ri.Id = ai.ReportInstance_Id
	                        INNER JOIN ActivityExecutionStatusEvent evt ON ai.Id = evt.ActivityInstance_Id
	                        INNER JOIN ActivityExecutionStatus stat ON evt.ExecutionStatus_Id = stat.Id
	                        INNER JOIN [User] u ON evt.EventCreatedBy_Id = u.Id
                        WHERE pce.Id = {patientClinicalEventId}
                        ORDER BY evt.EventDateTime desc");
            }
        }

        public async Task<IEnumerable<CausalityReportDto>> GetCausalityNotSetAsync(
            DateTime searchFrom, DateTime searchTo, CausalityConfigType causalityConfig, int facilityId, CausalityCriteria causalityCriteria)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string where = facilityId > 0 ? " AND pf.Facility_Id = " + facilityId.ToString() : "";
                switch (causalityConfig)
                {
                    case CausalityConfigType.BothScales:
                        where += (causalityCriteria == CausalityCriteria.CausalitySet) ? " AND (rim.NaranjoCausality IS NOT NULL AND LEN(RTRIM(rim.NaranjoCausality)) > 0 OR rim.WhoCausality IS NOT NULL AND LEN(RTRIM(rim.WhoCausality)) > 0) " : " AND (rim.NaranjoCausality IS NULL OR LEN(RTRIM(rim.NaranjoCausality)) = 0 OR rim.WhoCausality IS NULL OR LEN(RTRIM(rim.WhoCausality)) = 0) ";
                        break;

                    case CausalityConfigType.WHOScale:
                        where += (causalityCriteria == CausalityCriteria.CausalitySet) ? " AND rim.WhoCausality IS NOT NULL AND LEN(RTRIM(rim.WhoCausality)) > 0 " : " AND rim.WhoCausality IS NULL OR LEN(RTRIM(rim.WhoCausality)) = 0 ";
                        break;

                    case CausalityConfigType.NaranjoScale:
                        where += (causalityCriteria == CausalityCriteria.CausalitySet) ? " AND rim.NaranjoCausality IS NOT NULL AND LEN(RTRIM(rim.NaranjoCausality)) > 0 " : " AND rim.NaranjoCausality IS NULL OR LEN(RTRIM(rim.NaranjoCausality)) = 0 ";
                        break;
                }

                var sql = @$"SELECT   p.Id AS PatientId, 
                                f.FacilityName, 
                                p.FirstName, 
                                p.Surname as LastName, 
                                f.FacilityName,
                                pce.SourceTerminologyMedDra AS AdverseEvent, 
                                pce.[Istheadverseeventserious?] AS Serious,
                                pce.OnsetDate, 
                                rim.NaranjoCausality, 
                                rim.WhoCausality, 
                                rim.MedicationIdentifier
                    FROM ReportInstance ri
		                INNER JOIN MetaPatientClinicalEvent pce ON ri.ContextGuid = pce.PatientClinicalEventGuid 
		                INNER JOIN Patient p on pce.Patient_Id = p.Id
                        INNER JOIN PatientFacility pf ON pf.Id = (select top 1 Id from PatientFacility ipf where ipf.Patient_Id = p.Id and ipf.EnrolledDate <= GETDATE() order by ipf.EnrolledDate desc, ipf.Id desc)
                        INNER JOIN Facility f ON pf.Facility_Id = f.Id 
                        INNER JOIN ReportInstanceMedication rim ON ri.Id = rim.ReportInstance_Id 
                WHERE pce.OnsetDate BETWEEN '{searchFrom.ToString("yyyy-MM-dd")}' AND '{searchTo.ToString("yyyy-MM-dd")}' 
	                AND p.Archived = 0 {where} 
                ORDER BY pce.OnsetDate asc";

                return await connection.QueryAsync<CausalityReportDto>(sql);
            }
        }
    }
}
