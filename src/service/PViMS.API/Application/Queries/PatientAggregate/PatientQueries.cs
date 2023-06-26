using Dapper;
using MySqlConnector;
using PVIMS.API.Application.Models.Patient;
using PVIMS.API.Models;
using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    public class PatientQueries
        : IPatientQueries
    {
        private string _connectionString = string.Empty;

        public PatientQueries(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<PatientSearchDto>> SearchPatientsAsync(
            int currentUserId, 
            int? searchFacilityId, 
            int? searchPatientId, 
            string searchFirstName, 
            string searchLastName, 
            string caseNumber, 
            DateTime? dateOfBirth, 
            string customAttributeKey, 
            string customAttributeValue)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var joinCustomAttribute = !String.IsNullOrWhiteSpace(customAttributeKey) && !String.IsNullOrWhiteSpace(customAttributeValue) ? $"CROSS APPLY p.CustomAttributesXmlSerialised.nodes('CustomAttributeSet/CustomStringAttribute') as X(Y)" : "";

                var whereFacility = searchFacilityId.HasValue ? $"AND pf.Facility_Id = {searchFacilityId}" : $"AND pf.Facility_Id IN (SELECT f.Id FROM UserFacility uf INNER JOIN Facility f ON uf.Facility_Id = f.Id WHERE uf.User_Id = {currentUserId})";
                var wherePatient = searchPatientId.HasValue ? $"AND p.Id = {searchPatientId}" : "";
                var whereFirstName = !String.IsNullOrWhiteSpace(searchFirstName) ? $"AND p.FirstName LIKE '%{searchFirstName}%'" : "";
                var whereLastName = !String.IsNullOrWhiteSpace(searchLastName) ? $"AND p.Surname LIKE '%{searchLastName}%'" : "";
                var whereCaseNumber = !String.IsNullOrWhiteSpace(caseNumber) ? $"AND EXISTS(SELECT Id FROM PatientCondition pc WHERE pc.Patient_Id = p.id AND pc.CaseNumber LIKE '%{caseNumber}%')" : "";
                var whereDateOfBirth = dateOfBirth.HasValue ? $"AND p.DateOfBirth = '%{dateOfBirth.Value.ToString("yyyy-MM-dd")}%'" : "";
                var whereCustomAttribute = !String.IsNullOrWhiteSpace(customAttributeKey) && !String.IsNullOrWhiteSpace(customAttributeValue) ? $"AND X.Y.value('(Key)[1]', 'VARCHAR(MAX)') = '%{customAttributeKey}%' AND X.Y.value('(Value)[1]', 'VARCHAR(MAX)')  LIKE '%{customAttributeValue}%'" : "";

                var sql = $@"
		            SELECT	p.Id AS PatientId, 
				            p.FirstName, 
				            p.Surname, 
				            f.FacilityName, 
				            DATE_FORMAT(p.DateOfBirth, '%Y-%m-%d %T.%f') AS DateOfBirth,
				            IFNULL(FLOOR(DATEDIFF(p.DateofBirth, NOW()) / 365.25), 0) AS Age,
				            IFNULL(DATE_FORMAT((SELECT MAX(EncounterDate) FROM Encounter e WHERE e.Patient_Id = p.Id), '%Y-%m-%d %T.%f'),'') AS LatestEncounterDate
			            FROM Patient p
				            INNER JOIN PatientFacility pf ON p.Id = pf.Patient_Id AND pf.Id = (SELECT Id FROM PatientFacility ipf WHERE Patient_Id = p.Id ORDER BY EnrolledDate DESC limit 0,10) 
				            INNER JOIN Facility f ON pf.Facility_Id = f.Id
			                {joinCustomAttribute} 
			            WHERE p.Archived = 0 
                            {whereFacility} 
                            {wherePatient} 
                            {whereFirstName} 
                            {whereLastName} 
                            {whereCaseNumber} 
                            {whereDateOfBirth} 
				            {whereCustomAttribute} 
		            ORDER BY p.Id desc";

                return await connection.QueryAsync<PatientSearchDto>(sql);
            }
        }

        public async Task<IEnumerable<PatientSearchDto>> SearchPatientsByConditionCaseNumberAsync(string caseNumber)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var sql = $@"
		            SELECT	p.Id AS PatientId, 
				            p.FirstName, 
				            p.Surname, 
				            f.FacilityName, 
				            ISNULL(CONVERT(VARCHAR(10), p.DateOfBirth, 101), '') AS DateOfBirth,
				            CAST(ISNULL(FLOOR(DATEDIFF(DAY, p.DateofBirth, GETDATE()) / 365.25), 0) AS VARCHAR) AS Age,
				            ISNULL(CONVERT(VARCHAR(10), (SELECT MAX(EncounterDate) FROM Encounter e WHERE e.Patient_Id = p.Id), 101), '') AS LatestEncounterDate
			            FROM Patient p 
                            INNER JOIN PatientCondition pc ON p.Id = pc.Patient_Id 
				            INNER JOIN PatientFacility pf ON p.Id = pf.Patient_Id AND pf.Id = (SELECT TOP 1 Id FROM PatientFacility ipf WHERE Patient_Id = p.Id ORDER BY EnrolledDate DESC) 
				            INNER JOIN Facility f ON pf.Facility_Id = f.Id 
			            WHERE p.Archived = 0 AND pc.CaseNumber = '%{caseNumber}%' 
		            ORDER BY p.Id desc";

                return await connection.QueryAsync<PatientSearchDto>(sql);
            }
        }

        public async Task<IEnumerable<AdverseEventFrequencyReportDto>> GetAdverseEventsByAnnualAsync(DateTime searchFrom, DateTime searchTo)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var sql = @$"SELECT c.PeriodYear AS PeriodDisplay, 
                                    c.MedDraTerm AS SystemOrganClass,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 1' THEN c.PatientCount ELSE 0 END) AS Grade1Count,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 2' THEN c.PatientCount ELSE 0 END) AS Grade2Count,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 3' THEN c.PatientCount ELSE 0 END) AS Grade3Count,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 4' THEN c.PatientCount ELSE 0 END) AS Grade4Count,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 5' THEN c.PatientCount ELSE 0 END) AS Grade5Count,
		                            SUM(CASE WHEN c.SeverityGrade = '' THEN c.PatientCount ELSE 0 END) AS GradeUnknownCount
				            FROM (SELECT b.PeriodYear, tm1.MedDraTerm, b.SeverityGrade, b.PatientCount
					            FROM TerminologyMedDra tm1 
						            LEFT JOIN 
					            (SELECT DATEPART(YEAR, pce.OnsetDate) as [PeriodYear], 
						            t5.MedDraTerm AS 'Description',
						            ISNULL(mpce.SeverityGrade, '') AS SeverityGrade,
						            COUNT(*) AS PatientCount
					            FROM PatientClinicalEvent pce 
						            INNER JOIN ReportInstance ri on pce.PatientClinicalEventGuid = ri.ContextGuid 
                                    INNER JOIN MetaPatientClinicalEvent mpce on pce.PatientClinicalEventGuid = mpce.PatientClinicalEventGuid 
						            INNER JOIN Patient p ON pce.Patient_Id = p.Id
						            INNER JOIN TerminologyMedDra t ON ri.TerminologyMedDra_Id = t.Id
						            INNER JOIN TerminologyMedDra t2 ON t.Parent_Id = t2.Id
						            INNER JOIN TerminologyMedDra t3 ON t2.Parent_Id = t3.Id
						            INNER JOIN TerminologyMedDra t4 ON t3.Parent_Id = t4.Id
						            INNER JOIN TerminologyMedDra t5 ON t4.Parent_Id = t5.Id
					            WHERE pce.OnsetDate BETWEEN '{searchFrom.ToString("yyyy-MM-dd")}' AND '{searchTo.ToString("yyyy-MM-dd")}' and pce.Archived = 0 and p.Archived = 0 
					            GROUP BY DATEPART(YEAR, pce.OnsetDate), t5.MedDraTerm, mpce.SeverityGrade) as b on tm1.MedDraTerm = b.[Description] 
					            WHERE tm1.MedDraTermType = 'SOC' AND b.PeriodYear IS NOT NULL) as c 
	                        GROUP BY c.PeriodYear, c.MedDraTerm
	                        ORDER BY c.MedDraTerm, c.PeriodYear";

                return await connection.QueryAsync<AdverseEventFrequencyReportDto>(sql);
            }
        }

        public async Task<IEnumerable<AdverseEventFrequencyReportDto>> GetAdverseEventsByQuarterAsync(DateTime searchFrom, DateTime searchTo)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var sql = @$"SELECT CAST(c.PeriodYear as varchar) + ' - ' + 'Q' + CAST(c.PeriodQuarter as varchar) AS PeriodDisplay,
                                    c.MedDraTerm AS SystemOrganClass,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 1' THEN c.PatientCount ELSE 0 END) AS Grade1Count,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 2' THEN c.PatientCount ELSE 0 END) AS Grade2Count,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 3' THEN c.PatientCount ELSE 0 END) AS Grade3Count,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 4' THEN c.PatientCount ELSE 0 END) AS Grade4Count,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 5' THEN c.PatientCount ELSE 0 END) AS Grade5Count,
		                            SUM(CASE WHEN c.SeverityGrade = '' THEN c.PatientCount ELSE 0 END) AS GradeUnknownCount
	                        FROM (SELECT b.PeriodYear, b.PeriodQuarter, tm1.MedDraTerm, b.SeverityGrade, b.PatientCount
	                            FROM TerminologyMedDra tm1 
		                            LEFT JOIN 
	                            (SELECT DATEPART(YEAR, pce.OnsetDate) as [PeriodYear], DATEPART(QUARTER, pce.OnsetDate) as [PeriodQuarter],
		                            t5.MedDraTerm AS 'Description', 
                                    ISNULL(mpce.SeverityGrade, '') AS SeverityGrade,
			                        COUNT(*) AS PatientCount
	                            FROM PatientClinicalEvent pce 
                                    INNER JOIN ReportInstance ri on pce.PatientClinicalEventGuid = ri.ContextGuid 
                                    INNER JOIN MetaPatientClinicalEvent mpce on pce.PatientClinicalEventGuid = mpce.PatientClinicalEventGuid 
		                            INNER JOIN Patient p ON pce.Patient_Id = p.Id
		                            INNER JOIN TerminologyMedDra t ON ri.TerminologyMedDra_Id = t.Id
		                            INNER JOIN TerminologyMedDra t2 ON t.Parent_Id = t2.Id
		                            INNER JOIN TerminologyMedDra t3 ON t2.Parent_Id = t3.Id
		                            INNER JOIN TerminologyMedDra t4 ON t3.Parent_Id = t4.Id
		                            INNER JOIN TerminologyMedDra t5 ON t4.Parent_Id = t5.Id
	                            WHERE pce.OnsetDate BETWEEN '{searchFrom.ToString("yyyy-MM-dd")}' AND '{searchTo.ToString("yyyy-MM-dd")}' and pce.Archived = 0 and p.Archived = 0 
	                            GROUP BY DATEPART(YEAR, pce.OnsetDate), DATEPART(QUARTER, pce.OnsetDate), t5.MedDraTerm, mpce.SeverityGrade) as b on tm1.MedDraTerm = b.[Description] 
	                            WHERE tm1.MedDraTermType = 'SOC' AND b.PeriodYear IS NOT NULL) as c
	                    GROUP BY c.PeriodYear, c.PeriodQuarter, c.MedDraTerm
	                    ORDER BY c.MedDraTerm, c.PeriodYear, c.PeriodQuarter";

                return await connection.QueryAsync<AdverseEventFrequencyReportDto>(sql);
            }
        }

        public async Task<IEnumerable<AdverseEventFrequencyReportDto>> GetAdverseEventsByMonthAsync(DateTime searchFrom, DateTime searchTo)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var sql = @$"SELECT CAST(c.PeriodYear as varchar) + ' ' + CAST(c.PeriodMonth as varchar) AS PeriodDisplay,
                                    c.MedDraTerm AS SystemOrganClass,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 1' THEN c.PatientCount ELSE 0 END) AS Grade1Count,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 2' THEN c.PatientCount ELSE 0 END) AS Grade2Count,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 3' THEN c.PatientCount ELSE 0 END) AS Grade3Count,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 4' THEN c.PatientCount ELSE 0 END) AS Grade4Count,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 5' THEN c.PatientCount ELSE 0 END) AS Grade5Count,
		                            SUM(CASE WHEN c.SeverityGrade = '' THEN c.PatientCount ELSE 0 END) AS GradeUnknownCount
	                        FROM (SELECT b.PeriodYear, b.PeriodMonth, tm1.MedDraTerm, b.SeverityGrade, b.PatientCount
	                            FROM TerminologyMedDra tm1 
		                            LEFT JOIN 
	                            (SELECT DATEPART(YEAR, pce.OnsetDate) as [PeriodYear], DATENAME(mm, pce.OnsetDate) as [PeriodMonth],
		                            t5.MedDraTerm AS 'Description', 
                                    ISNULL(mpce.SeverityGrade, '') AS SeverityGrade,
			                        COUNT(*) AS PatientCount
	                            FROM PatientClinicalEvent pce 
                                    INNER JOIN ReportInstance ri on pce.PatientClinicalEventGuid = ri.ContextGuid 
                                    INNER JOIN MetaPatientClinicalEvent mpce on pce.PatientClinicalEventGuid = mpce.PatientClinicalEventGuid 
		                            INNER JOIN Patient p ON pce.Patient_Id = p.Id
		                            INNER JOIN TerminologyMedDra t ON ri.TerminologyMedDra_Id = t.Id
		                            INNER JOIN TerminologyMedDra t2 ON t.Parent_Id = t2.Id
		                            INNER JOIN TerminologyMedDra t3 ON t2.Parent_Id = t3.Id
		                            INNER JOIN TerminologyMedDra t4 ON t3.Parent_Id = t4.Id
		                            INNER JOIN TerminologyMedDra t5 ON t4.Parent_Id = t5.Id
	                            WHERE pce.OnsetDate BETWEEN '{searchFrom.ToString("yyyy-MM-dd")}' AND '{searchTo.ToString("yyyy-MM-dd")}' and pce.Archived = 0 and p.Archived = 0 
	                            GROUP BY DATEPART(YEAR, pce.OnsetDate), DATENAME(mm, pce.OnsetDate), t5.MedDraTerm, mpce.SeverityGrade) as b on tm1.MedDraTerm = b.[Description] 
	                            WHERE tm1.MedDraTermType = 'SOC' AND b.PeriodYear IS NOT NULL) as c
	                    GROUP BY c.PeriodYear, c.PeriodMonth, c.MedDraTerm
	                    ORDER BY c.MedDraTerm, c.PeriodYear, c.PeriodMonth";

                return await connection.QueryAsync<AdverseEventFrequencyReportDto>(sql);
            }
        }

        public async Task<IEnumerable<AdverseEventReportDto>> GetAdverseEventsByAgeGroupAsync(
            DateTime searchFrom, DateTime searchTo,
            AgeGroupCriteria ageGroupCriteria,
            string genderId,
            string regimenId,
            int organisationUnitId,
            string outcomeId,
            string isSeriousId,
            string seriousnessId,
            string classificationId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var whereAgeGroupCriteria = PrepareAgeWhereQuery(ageGroupCriteria);
                var whereGenderCriteria = !String.IsNullOrWhiteSpace(genderId) ? PrepareGenderWhereQuery(genderId) : "";
                var whereRegimenCriteria = !String.IsNullOrWhiteSpace(regimenId) ? PrepareRegimenWhereQuery(regimenId) : "";
                var whereFacilityRegionCriteria = organisationUnitId > 0 ? $"AND f.OrgUnit_Id = {organisationUnitId}" : "";
                var whereOutcomeCriteria = !String.IsNullOrWhiteSpace(outcomeId) ? PrepareOutcomeWhereQuery(outcomeId) : "";
                var whereIsSeriousCriteria = !String.IsNullOrWhiteSpace(isSeriousId) ? PrepareIsSeriousWhereQuery(isSeriousId) : "";
                var whereSeriousnessCriteria = !String.IsNullOrWhiteSpace(seriousnessId) ? PrepareSeriousnessWhereQuery(seriousnessId) : "";
                var whereClassificationCriteria = !String.IsNullOrWhiteSpace(classificationId) ? PrepareClassificationWhereQuery(classificationId) : "";

                var sql = @$"SELECT	
		                        CASE	WHEN FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) BETWEEN 0 AND 4 THEN '0 - 4' 
				                        WHEN FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) BETWEEN 5 AND 14 THEN '5 - 14'
				                        WHEN FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) BETWEEN 15 AND 24 THEN '15 - 24'
				                        WHEN FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) BETWEEN 25 AND 34 THEN '25 - 34'
				                        WHEN FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) BETWEEN 35 AND 44 THEN '35 - 44'
				                        WHEN FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) BETWEEN 45 AND 54 THEN '45 - 54'
				                        WHEN FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) BETWEEN 55 AND 64 THEN '55 - 64'
				                        WHEN FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) > 64 THEN '>= 65' END AS StratificationCriteria,
		                            t.MedDraTerm AS AdverseEvent, 
		                            COUNT(*) AS PatientCount
	                            FROM PatientClinicalEvent pce 
                                    INNER JOIN ReportInstance ri on pce.PatientClinicalEventGuid = ri.ContextGuid 
                                    INNER JOIN MetaPatientClinicalEvent mpce on pce.PatientClinicalEventGuid = mpce.PatientClinicalEventGuid 
		                            INNER JOIN Patient p ON pce.Patient_Id = p.Id
                                    INNER JOIN MetaPatient mp on p.PatientGuid = mp.PatientGuid
		                            INNER JOIN PatientFacility pf ON p.Id = pf.Patient_Id AND pf.Id = (SELECT TOP 1 Id FROM PatientFacility ipf WHERE Patient_Id = p.Id ORDER BY EnrolledDate DESC)
		                            INNER JOIN Facility f ON pf.Facility_Id = f.Id
		                            INNER JOIN OrgUnit ou ON f.OrgUnit_Id = ou.Id
		                            INNER JOIN TerminologyMedDra t ON ri.TerminologyMedDra_Id = t.Id
	                            WHERE pce.OnsetDate BETWEEN '{searchFrom.ToString("yyyy-MM-dd")}' AND '{searchTo.ToString("yyyy-MM-dd")}' 
                                    AND pce.Archived = 0 AND p.Archived = 0 
                                    {whereAgeGroupCriteria} 
                                    {whereGenderCriteria} 
                                    {whereRegimenCriteria} 
                                    {whereFacilityRegionCriteria} 
                                    {whereOutcomeCriteria} 
                                    {whereIsSeriousCriteria} 
                                    {whereSeriousnessCriteria} 
                                    {whereClassificationCriteria} 
	                            GROUP BY 
		                            CASE	WHEN FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) BETWEEN 0 AND 4 THEN '0 - 4' 
				                            WHEN FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) BETWEEN 5 AND 14 THEN '5 - 14'
				                            WHEN FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) BETWEEN 15 AND 24 THEN '15 - 24'
				                            WHEN FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) BETWEEN 25 AND 34 THEN '25 - 34'
				                            WHEN FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) BETWEEN 35 AND 44 THEN '35 - 44'
				                            WHEN FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) BETWEEN 45 AND 54 THEN '45 - 54'
				                            WHEN FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) BETWEEN 55 AND 64 THEN '55 - 64'
				                            WHEN FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) > 64 THEN '>= 65' END,
                                    t.MedDraTerm
	                            ORDER BY t.MedDraTerm";

                return await connection.QueryAsync<AdverseEventReportDto>(sql);
            }
        }

        public async Task<IEnumerable<AdverseEventReportDto>> GetAdverseEventsByFacilityRegionAsync(
            DateTime searchFrom, DateTime searchTo,
            AgeGroupCriteria ageGroupCriteria,
            string genderId, 
            string regimenId,
            int organisationUnitId,
            string outcomeId, 
            string isSeriousId, 
            string seriousnessId, 
            string classificationId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var whereAgeGroupCriteria = PrepareAgeWhereQuery(ageGroupCriteria);
                var whereGenderCriteria = !String.IsNullOrWhiteSpace(genderId) ? PrepareGenderWhereQuery(genderId) : "";
                var whereRegimenCriteria = !String.IsNullOrWhiteSpace(regimenId) ? PrepareRegimenWhereQuery(regimenId) : "";
                var whereFacilityRegionCriteria = organisationUnitId > 0 ? $"AND f.OrgUnit_Id = {organisationUnitId}" : "";
                var whereOutcomeCriteria = !String.IsNullOrWhiteSpace(outcomeId) ? PrepareOutcomeWhereQuery(outcomeId) : "";
                var whereIsSeriousCriteria = !String.IsNullOrWhiteSpace(isSeriousId) ? PrepareIsSeriousWhereQuery(isSeriousId) : "";
                var whereSeriousnessCriteria = !String.IsNullOrWhiteSpace(seriousnessId) ? PrepareSeriousnessWhereQuery(seriousnessId) : "";
                var whereClassificationCriteria = !String.IsNullOrWhiteSpace(classificationId) ? PrepareClassificationWhereQuery(classificationId) : "";

                var sql = @$"SELECT	ou.[Name] AS StratificationCriteria,
		                            t.MedDraTerm AS AdverseEvent, 
		                            COUNT(*) AS PatientCount
	                            FROM PatientClinicalEvent pce 
                                    INNER JOIN ReportInstance ri on pce.PatientClinicalEventGuid = ri.ContextGuid 
                                    INNER JOIN MetaPatientClinicalEvent mpce on pce.PatientClinicalEventGuid = mpce.PatientClinicalEventGuid 
		                            INNER JOIN Patient p ON pce.Patient_Id = p.Id
                                    INNER JOIN MetaPatient mp on p.PatientGuid = mp.PatientGuid
		                            INNER JOIN PatientFacility pf ON p.Id = pf.Patient_Id AND pf.Id = (SELECT TOP 1 Id FROM PatientFacility ipf WHERE Patient_Id = p.Id ORDER BY EnrolledDate DESC)
		                            INNER JOIN Facility f ON pf.Facility_Id = f.Id
		                            INNER JOIN OrgUnit ou ON f.OrgUnit_Id = ou.Id
		                            INNER JOIN TerminologyMedDra t ON ri.TerminologyMedDra_Id = t.Id
	                            WHERE pce.OnsetDate BETWEEN '{searchFrom.ToString("yyyy-MM-dd")}' AND '{searchTo.ToString("yyyy-MM-dd")}' 
                                    AND pce.Archived = 0 AND p.Archived = 0 
                                    {whereAgeGroupCriteria} 
                                    {whereGenderCriteria} 
                                    {whereRegimenCriteria} 
                                    {whereFacilityRegionCriteria} 
                                    {whereOutcomeCriteria} 
                                    {whereIsSeriousCriteria} 
                                    {whereSeriousnessCriteria} 
                                    {whereClassificationCriteria} 
	                            GROUP BY ou.[Name], t.MedDraTerm
	                            ORDER BY ou.[Name], t.MedDraTerm";

                return await connection.QueryAsync<AdverseEventReportDto>(sql);
            }
        }

        public async Task<IEnumerable<AdverseEventReportDto>> GetAdverseEventsByOutcomeAsync(
            DateTime searchFrom, DateTime searchTo,
            AgeGroupCriteria ageGroupCriteria,
            string genderId,
            string regimenId,
            int organisationUnitId,
            string outcomeId,
            string isSeriousId,
            string seriousnessId,
            string classificationId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var whereAgeGroupCriteria = PrepareAgeWhereQuery(ageGroupCriteria);
                var whereGenderCriteria = !String.IsNullOrWhiteSpace(genderId) ? PrepareGenderWhereQuery(genderId) : "";
                var whereRegimenCriteria = !String.IsNullOrWhiteSpace(regimenId) ? PrepareRegimenWhereQuery(regimenId) : "";
                var whereFacilityRegionCriteria = organisationUnitId > 0 ? $"AND f.OrgUnit_Id = {organisationUnitId}" : "";
                var whereOutcomeCriteria = !String.IsNullOrWhiteSpace(outcomeId) ? PrepareOutcomeWhereQuery(outcomeId) : "";
                var whereIsSeriousCriteria = !String.IsNullOrWhiteSpace(isSeriousId) ? PrepareIsSeriousWhereQuery(isSeriousId) : "";
                var whereSeriousnessCriteria = !String.IsNullOrWhiteSpace(seriousnessId) ? PrepareSeriousnessWhereQuery(seriousnessId) : "";
                var whereClassificationCriteria = !String.IsNullOrWhiteSpace(classificationId) ? PrepareClassificationWhereQuery(classificationId) : "";

                var sql = @$"SELECT	mpce.Outcome AS StratificationCriteria,
		                            t.MedDraTerm AS AdverseEvent, 
		                            COUNT(*) AS PatientCount
	                            FROM PatientClinicalEvent pce 
                                    INNER JOIN ReportInstance ri on pce.PatientClinicalEventGuid = ri.ContextGuid 
                                    INNER JOIN MetaPatientClinicalEvent mpce on pce.PatientClinicalEventGuid = mpce.PatientClinicalEventGuid 
		                            INNER JOIN Patient p ON pce.Patient_Id = p.Id
                                    INNER JOIN MetaPatient mp on p.PatientGuid = mp.PatientGuid
		                            INNER JOIN PatientFacility pf ON p.Id = pf.Patient_Id AND pf.Id = (SELECT TOP 1 Id FROM PatientFacility ipf WHERE Patient_Id = p.Id ORDER BY EnrolledDate DESC)
		                            INNER JOIN Facility f ON pf.Facility_Id = f.Id
		                            INNER JOIN OrgUnit ou ON f.OrgUnit_Id = ou.Id
		                            INNER JOIN TerminologyMedDra t ON ri.TerminologyMedDra_Id = t.Id
	                            WHERE pce.OnsetDate BETWEEN '{searchFrom.ToString("yyyy-MM-dd")}' AND '{searchTo.ToString("yyyy-MM-dd")}' 
                                    AND pce.Archived = 0 AND p.Archived = 0 
                                    {whereAgeGroupCriteria} 
                                    {whereGenderCriteria} 
                                    {whereRegimenCriteria} 
                                    {whereFacilityRegionCriteria} 
                                    {whereOutcomeCriteria} 
                                    {whereIsSeriousCriteria} 
                                    {whereSeriousnessCriteria} 
                                    {whereClassificationCriteria} 
	                            GROUP BY mpce.Outcome, t.MedDraTerm
	                            ORDER BY mpce.Outcome, t.MedDraTerm";

                return await connection.QueryAsync<AdverseEventReportDto>(sql);
            }
        }

        public async Task<IEnumerable<AdverseEventReportDto>> GetAdverseEventsByGenderAsync(
            DateTime searchFrom, DateTime searchTo,
            AgeGroupCriteria ageGroupCriteria,
            string genderId,
            string regimenId,
            int organisationUnitId,
            string outcomeId,
            string isSeriousId,
            string seriousnessId,
            string classificationId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var whereAgeGroupCriteria = PrepareAgeWhereQuery(ageGroupCriteria);
                var whereGenderCriteria = !String.IsNullOrWhiteSpace(genderId) ? PrepareGenderWhereQuery(genderId) : "";
                var whereRegimenCriteria = !String.IsNullOrWhiteSpace(regimenId) ? PrepareRegimenWhereQuery(regimenId) : "";
                var whereFacilityRegionCriteria = organisationUnitId > 0 ? $"AND f.OrgUnit_Id = {organisationUnitId}" : "";
                var whereOutcomeCriteria = !String.IsNullOrWhiteSpace(outcomeId) ? PrepareOutcomeWhereQuery(outcomeId) : "";
                var whereIsSeriousCriteria = !String.IsNullOrWhiteSpace(isSeriousId) ? PrepareIsSeriousWhereQuery(isSeriousId) : "";
                var whereSeriousnessCriteria = !String.IsNullOrWhiteSpace(seriousnessId) ? PrepareSeriousnessWhereQuery(seriousnessId) : "";
                var whereClassificationCriteria = !String.IsNullOrWhiteSpace(classificationId) ? PrepareClassificationWhereQuery(classificationId) : "";

                var sql = @$"SELECT	mp.Gender AS StratificationCriteria,
		                            t.MedDraTerm AS AdverseEvent, 
		                            COUNT(*) AS PatientCount
	                            FROM PatientClinicalEvent pce 
                                    INNER JOIN ReportInstance ri on pce.PatientClinicalEventGuid = ri.ContextGuid 
                                    INNER JOIN MetaPatientClinicalEvent mpce on pce.PatientClinicalEventGuid = mpce.PatientClinicalEventGuid 
		                            INNER JOIN Patient p ON pce.Patient_Id = p.Id
                                    INNER JOIN MetaPatient mp on p.PatientGuid = mp.PatientGuid
		                            INNER JOIN PatientFacility pf ON p.Id = pf.Patient_Id AND pf.Id = (SELECT TOP 1 Id FROM PatientFacility ipf WHERE Patient_Id = p.Id ORDER BY EnrolledDate DESC)
		                            INNER JOIN Facility f ON pf.Facility_Id = f.Id
		                            INNER JOIN OrgUnit ou ON f.OrgUnit_Id = ou.Id
		                            INNER JOIN TerminologyMedDra t ON ri.TerminologyMedDra_Id = t.Id
	                            WHERE pce.OnsetDate BETWEEN '{searchFrom.ToString("yyyy-MM-dd")}' AND '{searchTo.ToString("yyyy-MM-dd")}' 
                                    AND pce.Archived = 0 AND p.Archived = 0 
                                    {whereAgeGroupCriteria} 
                                    {whereGenderCriteria} 
                                    {whereRegimenCriteria} 
                                    {whereFacilityRegionCriteria} 
                                    {whereOutcomeCriteria} 
                                    {whereIsSeriousCriteria} 
                                    {whereSeriousnessCriteria} 
                                    {whereClassificationCriteria} 
	                            GROUP BY mp.Gender, t.MedDraTerm
	                            ORDER BY mp.Gender, t.MedDraTerm";

                return await connection.QueryAsync<AdverseEventReportDto>(sql);
            }
        }

        public async Task<IEnumerable<AdverseEventReportDto>> GetAdverseEventsByRegimenAsync(
            DateTime searchFrom, DateTime searchTo,
            AgeGroupCriteria ageGroupCriteria,
            string genderId,
            string regimenId,
            int organisationUnitId,
            string outcomeId,
            string isSeriousId,
            string seriousnessId,
            string classificationId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var whereAgeGroupCriteria = PrepareAgeWhereQuery(ageGroupCriteria);
                var whereGenderCriteria = !String.IsNullOrWhiteSpace(genderId) ? PrepareGenderWhereQuery(genderId) : "";
                var whereRegimenCriteria = !String.IsNullOrWhiteSpace(regimenId) ? PrepareRegimenWhereQuery(regimenId) : "";
                var whereFacilityRegionCriteria = organisationUnitId > 0 ? $"AND f.OrgUnit_Id = {organisationUnitId}" : "";
                var whereOutcomeCriteria = !String.IsNullOrWhiteSpace(outcomeId) ? PrepareOutcomeWhereQuery(outcomeId) : "";
                var whereIsSeriousCriteria = !String.IsNullOrWhiteSpace(isSeriousId) ? PrepareIsSeriousWhereQuery(isSeriousId) : "";
                var whereSeriousnessCriteria = !String.IsNullOrWhiteSpace(seriousnessId) ? PrepareSeriousnessWhereQuery(seriousnessId) : "";
                var whereClassificationCriteria = !String.IsNullOrWhiteSpace(classificationId) ? PrepareClassificationWhereQuery(classificationId) : "";

                var sql = @$"SELECT	mpce.Regimen AS StratificationCriteria,
		                            t.MedDraTerm AS AdverseEvent, 
		                            COUNT(*) AS PatientCount
	                            FROM PatientClinicalEvent pce 
                                    INNER JOIN ReportInstance ri on pce.PatientClinicalEventGuid = ri.ContextGuid 
                                    INNER JOIN MetaPatientClinicalEvent mpce on pce.PatientClinicalEventGuid = mpce.PatientClinicalEventGuid 
		                            INNER JOIN Patient p ON pce.Patient_Id = p.Id
                                    INNER JOIN MetaPatient mp on p.PatientGuid = mp.PatientGuid
		                            INNER JOIN PatientFacility pf ON p.Id = pf.Patient_Id AND pf.Id = (SELECT TOP 1 Id FROM PatientFacility ipf WHERE Patient_Id = p.Id ORDER BY EnrolledDate DESC)
		                            INNER JOIN Facility f ON pf.Facility_Id = f.Id
		                            INNER JOIN OrgUnit ou ON f.OrgUnit_Id = ou.Id
		                            INNER JOIN TerminologyMedDra t ON ri.TerminologyMedDra_Id = t.Id
	                            WHERE pce.OnsetDate BETWEEN '{searchFrom.ToString("yyyy-MM-dd")}' AND '{searchTo.ToString("yyyy-MM-dd")}' 
                                    AND pce.Archived = 0 AND p.Archived = 0 
                                    {whereAgeGroupCriteria} 
                                    {whereGenderCriteria} 
                                    {whereRegimenCriteria} 
                                    {whereFacilityRegionCriteria} 
                                    {whereOutcomeCriteria} 
                                    {whereIsSeriousCriteria} 
                                    {whereSeriousnessCriteria} 
                                    {whereClassificationCriteria} 
	                            GROUP BY mpce.Regimen, t.MedDraTerm
	                            ORDER BY mpce.Regimen, t.MedDraTerm";

                return await connection.QueryAsync<AdverseEventReportDto>(sql);
            }
        }

        public async Task<IEnumerable<AdverseEventReportDto>> GetAdverseEventsByIsSeriousAsync(
            DateTime searchFrom, DateTime searchTo,
            AgeGroupCriteria ageGroupCriteria,
            string genderId,
            string regimenId,
            int organisationUnitId,
            string outcomeId,
            string isSeriousId,
            string seriousnessId,
            string classificationId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var whereAgeGroupCriteria = PrepareAgeWhereQuery(ageGroupCriteria);
                var whereGenderCriteria = !String.IsNullOrWhiteSpace(genderId) ? PrepareGenderWhereQuery(genderId) : "";
                var whereRegimenCriteria = !String.IsNullOrWhiteSpace(regimenId) ? PrepareRegimenWhereQuery(regimenId) : "";
                var whereFacilityRegionCriteria = organisationUnitId > 0 ? $"AND f.OrgUnit_Id = {organisationUnitId}" : "";
                var whereOutcomeCriteria = !String.IsNullOrWhiteSpace(outcomeId) ? PrepareOutcomeWhereQuery(outcomeId) : "";
                var whereIsSeriousCriteria = !String.IsNullOrWhiteSpace(isSeriousId) ? PrepareIsSeriousWhereQuery(isSeriousId) : "";
                var whereSeriousnessCriteria = !String.IsNullOrWhiteSpace(seriousnessId) ? PrepareSeriousnessWhereQuery(seriousnessId) : "";
                var whereClassificationCriteria = !String.IsNullOrWhiteSpace(classificationId) ? PrepareClassificationWhereQuery(classificationId) : "";

                var sql = @$"SELECT	mpce.[Istheadverseeventserious?] AS StratificationCriteria,
		                            t.MedDraTerm AS AdverseEvent, 
		                            COUNT(*) AS PatientCount
	                            FROM PatientClinicalEvent pce 
                                    INNER JOIN ReportInstance ri on pce.PatientClinicalEventGuid = ri.ContextGuid 
                                    INNER JOIN MetaPatientClinicalEvent mpce on pce.PatientClinicalEventGuid = mpce.PatientClinicalEventGuid 
		                            INNER JOIN Patient p ON pce.Patient_Id = p.Id
                                    INNER JOIN MetaPatient mp on p.PatientGuid = mp.PatientGuid
		                            INNER JOIN PatientFacility pf ON p.Id = pf.Patient_Id AND pf.Id = (SELECT TOP 1 Id FROM PatientFacility ipf WHERE Patient_Id = p.Id ORDER BY EnrolledDate DESC)
		                            INNER JOIN Facility f ON pf.Facility_Id = f.Id
		                            INNER JOIN OrgUnit ou ON f.OrgUnit_Id = ou.Id
		                            INNER JOIN TerminologyMedDra t ON ri.TerminologyMedDra_Id = t.Id
	                            WHERE pce.OnsetDate BETWEEN '{searchFrom.ToString("yyyy-MM-dd")}' AND '{searchTo.ToString("yyyy-MM-dd")}' 
                                    AND pce.Archived = 0 AND p.Archived = 0 
                                    {whereAgeGroupCriteria} 
                                    {whereGenderCriteria} 
                                    {whereRegimenCriteria} 
                                    {whereFacilityRegionCriteria} 
                                    {whereOutcomeCriteria} 
                                    {whereIsSeriousCriteria} 
                                    {whereSeriousnessCriteria} 
                                    {whereClassificationCriteria} 
	                            GROUP BY mpce.[Istheadverseeventserious?], t.MedDraTerm
	                            ORDER BY mpce.[Istheadverseeventserious?], t.MedDraTerm";

                return await connection.QueryAsync<AdverseEventReportDto>(sql);
            }
        }

        public async Task<IEnumerable<AdverseEventReportDto>> GetAdverseEventsBySeriousnessAsync(
            DateTime searchFrom, DateTime searchTo,
            AgeGroupCriteria ageGroupCriteria,
            string genderId,
            string regimenId,
            int organisationUnitId,
            string outcomeId,
            string isSeriousId,
            string seriousnessId,
            string classificationId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var whereAgeGroupCriteria = PrepareAgeWhereQuery(ageGroupCriteria);
                var whereGenderCriteria = !String.IsNullOrWhiteSpace(genderId) ? PrepareGenderWhereQuery(genderId) : "";
                var whereRegimenCriteria = !String.IsNullOrWhiteSpace(regimenId) ? PrepareRegimenWhereQuery(regimenId) : "";
                var whereFacilityRegionCriteria = organisationUnitId > 0 ? $"AND f.OrgUnit_Id = {organisationUnitId}" : "";
                var whereOutcomeCriteria = !String.IsNullOrWhiteSpace(outcomeId) ? PrepareOutcomeWhereQuery(outcomeId) : "";
                var whereIsSeriousCriteria = !String.IsNullOrWhiteSpace(isSeriousId) ? PrepareIsSeriousWhereQuery(isSeriousId) : "";
                var whereSeriousnessCriteria = !String.IsNullOrWhiteSpace(seriousnessId) ? PrepareSeriousnessWhereQuery(seriousnessId) : "";
                var whereClassificationCriteria = !String.IsNullOrWhiteSpace(classificationId) ? PrepareClassificationWhereQuery(classificationId) : "";

                var sql = @$"SELECT	mpce.[Seriousness] AS StratificationCriteria,
		                            t.MedDraTerm AS AdverseEvent, 
		                            COUNT(*) AS PatientCount
	                            FROM PatientClinicalEvent pce 
                                    INNER JOIN ReportInstance ri on pce.PatientClinicalEventGuid = ri.ContextGuid 
                                    INNER JOIN MetaPatientClinicalEvent mpce on pce.PatientClinicalEventGuid = mpce.PatientClinicalEventGuid 
		                            INNER JOIN Patient p ON pce.Patient_Id = p.Id
                                    INNER JOIN MetaPatient mp on p.PatientGuid = mp.PatientGuid
		                            INNER JOIN PatientFacility pf ON p.Id = pf.Patient_Id AND pf.Id = (SELECT TOP 1 Id FROM PatientFacility ipf WHERE Patient_Id = p.Id ORDER BY EnrolledDate DESC)
		                            INNER JOIN Facility f ON pf.Facility_Id = f.Id
		                            INNER JOIN OrgUnit ou ON f.OrgUnit_Id = ou.Id
		                            INNER JOIN TerminologyMedDra t ON ri.TerminologyMedDra_Id = t.Id
	                            WHERE pce.OnsetDate BETWEEN '{searchFrom.ToString("yyyy-MM-dd")}' AND '{searchTo.ToString("yyyy-MM-dd")}' 
                                    AND pce.Archived = 0 AND p.Archived = 0 
                                    {whereAgeGroupCriteria} 
                                    {whereGenderCriteria} 
                                    {whereRegimenCriteria} 
                                    {whereFacilityRegionCriteria} 
                                    {whereOutcomeCriteria} 
                                    {whereIsSeriousCriteria} 
                                    {whereSeriousnessCriteria} 
                                    {whereClassificationCriteria} 
	                            GROUP BY mpce.[Seriousness], t.MedDraTerm
	                            ORDER BY mpce.[Seriousness], t.MedDraTerm";

                return await connection.QueryAsync<AdverseEventReportDto>(sql);
            }
        }

        public async Task<IEnumerable<AdverseEventReportDto>> GetAdverseEventsByClassificationAsync(
            DateTime searchFrom, DateTime searchTo,
            AgeGroupCriteria ageGroupCriteria,
            string genderId,
            string regimenId,
            int organisationUnitId,
            string outcomeId,
            string isSeriousId,
            string seriousnessId,
            string classificationId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var whereAgeGroupCriteria = PrepareAgeWhereQuery(ageGroupCriteria);
                var whereGenderCriteria = !String.IsNullOrWhiteSpace(genderId) ? PrepareGenderWhereQuery(genderId) : "";
                var whereRegimenCriteria = !String.IsNullOrWhiteSpace(regimenId) ? PrepareRegimenWhereQuery(regimenId) : "";
                var whereFacilityRegionCriteria = organisationUnitId > 0 ? $"AND f.OrgUnit_Id = {organisationUnitId}" : "";
                var whereOutcomeCriteria = !String.IsNullOrWhiteSpace(outcomeId) ? PrepareOutcomeWhereQuery(outcomeId) : "";
                var whereIsSeriousCriteria = !String.IsNullOrWhiteSpace(isSeriousId) ? PrepareIsSeriousWhereQuery(isSeriousId) : "";
                var whereSeriousnessCriteria = !String.IsNullOrWhiteSpace(seriousnessId) ? PrepareSeriousnessWhereQuery(seriousnessId) : "";
                var whereClassificationCriteria = !String.IsNullOrWhiteSpace(classificationId) ? PrepareClassificationWhereQuery(classificationId) : "";

                var sql = @$"SELECT	mpce.[Classification] AS StratificationCriteria,
		                            t.MedDraTerm AS AdverseEvent, 
		                            COUNT(*) AS PatientCount
	                            FROM PatientClinicalEvent pce 
                                    INNER JOIN ReportInstance ri on pce.PatientClinicalEventGuid = ri.ContextGuid 
                                    INNER JOIN MetaPatientClinicalEvent mpce on pce.PatientClinicalEventGuid = mpce.PatientClinicalEventGuid 
		                            INNER JOIN Patient p ON pce.Patient_Id = p.Id
                                    INNER JOIN MetaPatient mp on p.PatientGuid = mp.PatientGuid
		                            INNER JOIN PatientFacility pf ON p.Id = pf.Patient_Id AND pf.Id = (SELECT TOP 1 Id FROM PatientFacility ipf WHERE Patient_Id = p.Id ORDER BY EnrolledDate DESC)
		                            INNER JOIN Facility f ON pf.Facility_Id = f.Id
		                            INNER JOIN OrgUnit ou ON f.OrgUnit_Id = ou.Id
		                            INNER JOIN TerminologyMedDra t ON ri.TerminologyMedDra_Id = t.Id
	                            WHERE pce.OnsetDate BETWEEN '{searchFrom.ToString("yyyy-MM-dd")}' AND '{searchTo.ToString("yyyy-MM-dd")}' 
                                    AND pce.Archived = 0 AND p.Archived = 0 
                                    {whereAgeGroupCriteria} 
                                    {whereGenderCriteria} 
                                    {whereRegimenCriteria} 
                                    {whereFacilityRegionCriteria} 
                                    {whereOutcomeCriteria} 
                                    {whereIsSeriousCriteria} 
                                    {whereSeriousnessCriteria} 
                                    {whereClassificationCriteria} 
	                            GROUP BY mpce.[Classification], t.MedDraTerm
	                            ORDER BY mpce.[Classification], t.MedDraTerm";

                return await connection.QueryAsync<AdverseEventReportDto>(sql);
            }
        }

        public async Task<IEnumerable<PatientListDto>> GetPatientListByConceptAsync(int conceptId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var sql = @$"SELECT s.Id AS PatientId, s.FirstName, s.Surname, s.FacilityName  FROM 
	                            (
			                        select ip.Id, ip.FirstName, ip.Surname, ifa.FacilityName
			                        from Patient ip 
				                        inner join PatientMedication ipm on ip.Id = ipm.Patient_Id 
				                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
				                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
				                        inner join Concept ic on ipm.Concept_Id = ic.Id
			                        where ic.Id = {conceptId} and ip.Archived = 0 and ipm.Archived = 0 
			                        group by ip.Id, ip.FirstName, ip.Surname, ifa.FacilityName
		                        ) AS s";

                return await connection.QueryAsync<PatientListDto>(sql);
            }
        }

        private string PrepareAgeWhereQuery(AgeGroupCriteria ageGroupCriteria)
        {
            switch (ageGroupCriteria)
            {
                case AgeGroupCriteria.None:
                    return string.Empty;
                case AgeGroupCriteria.ZeroToFour:
                    return "AND FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) BETWEEN 0 AND 4";
                case AgeGroupCriteria.FiveToFourteen:
                    return "AND FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) BETWEEN 5 AND 14";
                case AgeGroupCriteria.FifteenToTwentyFour:
                    return "AND FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) BETWEEN 15 AND 24";
                case AgeGroupCriteria.TwentyFiveToThirtyFour:
                    return "AND FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) BETWEEN 25 AND 34";
                case AgeGroupCriteria.ThirtyFiveToFortyFour:
                    return "AND FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) BETWEEN 35 AND 44";
                case AgeGroupCriteria.FortyFiveToFivetyFour:
                    return "AND FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) BETWEEN 45 AND 54";
                case AgeGroupCriteria.FifetyFiveToSixtyFour:
                    return "AND FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) BETWEEN 55 AND 64";
                case AgeGroupCriteria.AboveSixtyFour:
                    return "AND FLOOR(DATEDIFF(DAY, p.DateofBirth, pce.OnsetDate) / 365.25) >= 65";
                default:
                    return string.Empty;
            }
        }

        private string PrepareGenderWhereQuery(string key)
        {
            switch (key)
            {
                case "1":
                    return "AND mp.Gender = 'Male'";
                case "2":
                    return "AND mp.Gender = 'Female'";
                default:
                    return string.Empty;
            }
        }

        private string PrepareRegimenWhereQuery(string key)
        {
            switch (key)
            {
                case "1":
                    return "AND mpce.Regimen = 'SSOR (3)'";
                case "2":
                    return "AND mpce.Regimen = 'SLOR FQ-S (4)'";
                case "3":
                    return "AND mpce.Regimen = 'SLOR FQ-R (5)'";
                case "4":
                    return "AND mpce.Regimen = 'ITR'";
                case "5":
                    return "AND mpce.Regimen = 'FQ susceptible MDR TB (6a)'";
                case "6":
                    return "AND mpce.Regimen = 'FQ susceptible MDR TB (6b)'";
                case "7":
                    return "AND mpce.Regimen = 'FQ susceptible MDR TB (6c)'";
                case "8":
                    return "AND mpce.Regimen = 'FQ resistant MDR TB (7a)'";
                case "9":
                    return "AND mpce.Regimen = 'FQ resistant MDR TB (7b)'";
                case "10":
                    return "AND mpce.Regimen = 'FQ resistant MDR TB (7c)'";
                case "11":
                    return "AND mpce.Regimen = 'BPaL'";
                default:
                    return string.Empty;
            }
        }

        private string PrepareOutcomeWhereQuery(string key)
        {
            switch (key)
            {
                case "1":
                    return "AND mpce.Outcome = 'Resolved'";
                case "2":
                    return "AND mpce.Outcome = 'Resolved with sequelae'";
                case "3":
                    return "AND mpce.Outcome = 'Fatal'";
                case "4":
                    return "AND mpce.Outcome = 'Resolving'";
                case "5":
                    return "AND mpce.Outcome = 'Not resolved'";
                case "6":
                    return "AND mpce.Outcome = 'Unknown'";
                default:
                    return string.Empty;
            }
        }

        private string PrepareIsSeriousWhereQuery(string key)
        {
            switch (key)
            {
                case "1":
                    return "AND mpce.[Istheadverseeventserious?] = 'Yes'";
                case "2":
                    return "AND mpce.[Istheadverseeventserious?] = 'No'";
                case "3":
                    return "AND mpce.[Istheadverseeventserious?] = 'Unknown'";
                default:
                    return string.Empty;
            }
        }

        private string PrepareSeriousnessWhereQuery(string key)
        {
            switch (key)
            {
                case "1":
                    return "AND mpce.[Seriousness] = 'A congenital anomaly or birth defect'";
                case "2":
                    return "AND mpce.[Seriousness] = 'Persistent or significant disability or incapacity'";
                case "3":
                    return "AND mpce.[Seriousness] = 'Death'";
                case "4":
                    return "AND mpce.[Seriousness] = 'Hospitalisation'";
                case "5":
                    return "AND mpce.[Seriousness] = 'Prolonged hospitalisation'";
                case "6":
                    return "AND mpce.[Seriousness] = 'Life threatening'";
                case "7":
                    return "AND mpce.[Seriousness] = 'A medically important event'";
                case "8":
                    return "AND mpce.[Seriousness] = 'N/A'";
                default:
                    return string.Empty;
            }
        }

        private string PrepareClassificationWhereQuery(string key)
        {
            switch (key)
            {
                case "1":
                    return "AND mpce.[Classification] = 'AESI'";
                case "2":
                    return "AND mpce.[Classification] = 'SAE'";
                case "3":
                    return "AND mpce.[Classification] = 'Clinically Significant'";
                default:
                    return string.Empty;
            }
        }

        public async Task<IEnumerable<PatientsOnTreatmentDto>> GetPatientsOnTreatmentByEncounterAsync(DateTime searchFrom, DateTime searchTo)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var sql =
                    @$"SELECT f.FacilityName, f.Id as FacilityId
                            ,	(
                                    select count(distinct(ip.Id))
                                    from Encounter ie 
                                        inner join Patient ip on ie.Patient_Id = ip.Id
                                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
                                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                    where ie.Archived = 0 and ip.Archived = 0 and ie.Archived = 0
                                        and ie.EncounterDate between '{searchFrom.ToString("yyyy-MM-dd")}' and '{searchTo.ToString("yyyy-MM-dd")}'
                                        and ifa.Id = f.Id
                                ) AS PatientCount
                            ,	(
                                    select count(distinct(ip.Id))
                                    from Encounter ie 
                                        inner join Patient ip on ie.Patient_Id = ip.Id
                                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
                                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                        inner join PatientClinicalEvent ipce on ip.Id = ipce.Patient_Id 
                                        inner join MetaPatientClinicalEvent impce on ipce.PatientClinicalEventGuid = impce.PatientClinicalEventGuid 
                                    where ie.Archived = 0 and ip.Archived = 0 and ipce.Archived = 0
                                        and ie.EncounterDate between '{searchFrom.ToString("yyyy-MM-dd")}' and '{searchTo.ToString("yyyy-MM-dd")}'
                                        and ifa.Id = f.Id
                                        and (impce.`Istheadverseeventserious?` <> 'Yes' or impce.`Istheadverseeventserious?` is null)
                                ) AS PatientWithNonSeriousEventCount
                            ,	(
                                    select count(distinct(ip.Id))
                                    from Encounter ie 
                                        inner join Patient ip on ie.Patient_Id = ip.Id
                                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
                                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                        inner join PatientClinicalEvent ipce on ip.Id = ipce.Patient_Id 
                                        inner join MetaPatientClinicalEvent impce on ipce.PatientClinicalEventGuid = impce.PatientClinicalEventGuid 
                                    where ie.Archived = 0 and ip.Archived = 0 and ipce.Archived = 0
                                        and ie.EncounterDate between '{searchFrom.ToString("yyyy-MM-dd")}' and '{searchTo.ToString("yyyy-MM-dd")}'
                                        and ifa.Id = f.Id
                                        and impce.`Istheadverseeventserious?` = 'Yes'
                                ) AS PatientWithSeriousEventCount
                            ,	(
                                    select count(distinct(ip.Id))
                                    from Encounter ie 
                                        inner join Patient ip on ie.Patient_Id = ip.Id
                                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
                                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                        inner join PatientClinicalEvent ipce on ip.Id = ipce.Patient_Id 
                                    where ie.Archived = 0 and ip.Archived = 0 and ipce.Archived = 0
                                        and ie.EncounterDate between '{searchFrom.ToString("yyyy-MM-dd")}' and '{searchTo.ToString("yyyy-MM-dd")}'
                                        and ifa.Id = f.Id
                                ) AS PatientWithEventCount        
                        FROM Facility f
                        ORDER BY f.FacilityName";

                return await connection.QueryAsync<PatientsOnTreatmentDto>(sql);

            }
        }

        public async Task<IEnumerable<PatientListDto>> GetPatientOnTreatmentListByEncounterAsync(DateTime searchFrom, DateTime searchTo, int facilityId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var sql =
                    @$"SELECT s.Id AS PatientId, s.FirstName + ' ' + s.Surname AS FullName, s.FacilityName  FROM 
	                        	(
			                        select ip.Id, ip.FirstName, ip.Surname, ifa.FacilityName
			                        from Encounter ie 
				                        inner join Patient ip on ie.Patient_Id = ip.Id
				                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
				                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
			                        where ie.Archived = 0 and ip.Archived = 0 
                                        and ie.EncounterDate between '{searchFrom.ToString("yyyy-MM-dd")}' and '{searchTo.ToString("yyyy-MM-dd")}'
				                        and ifa.Id = {facilityId}
				                    group by ip.Id, ip.FirstName, ip.Surname, ifa.FacilityName 
		                        ) AS s";

                return await connection.QueryAsync<PatientListDto>(sql);
            }
        }

        public async Task<IEnumerable<PatientsOnTreatmentDto>> GetPatientsOnTreatmentByFacilityAsync(DateTime searchFrom, DateTime searchTo)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var sql =
                    @$"SELECT f.FacilityName, f.Id as FacilityId
                            ,	(
                                    select count(distinct(ip.Id))
                                    from Patient ip 
                                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
                                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                    where ip.Archived = 0 and ipf.Archived = 0
                                        and ipf.EnrolledDate between '{searchFrom.ToString("yyyy-MM-dd")}' and '{searchTo.ToString("yyyy-MM-dd")}'
                                        and ifa.Id = f.Id
                                ) AS PatientCount
                            ,	(
                                    select count(distinct(ip.Id))
                                    from Patient ip
                                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
                                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                        inner join PatientClinicalEvent ipce on ip.Id = ipce.Patient_Id 
                                        inner join MetaPatientClinicalEvent impce on ipce.PatientClinicalEventGuid = impce.PatientClinicalEventGuid 
                                    where ip.Archived = 0 and ipf.Archived = 0 and ipce.Archived = 0
                                        and ipf.EnrolledDate between '{searchFrom.ToString("yyyy-MM-dd")}' and '{searchTo.ToString("yyyy-MM-dd")}'
                                        and ifa.Id = f.Id
                                        and (impce.`Istheadverseeventserious?` <> 'Yes' or impce.`Istheadverseeventserious?` is null)
                                ) AS PatientWithNonSeriousEventCount
                            ,	(
                                    select count(distinct(ip.Id))
                                    from Patient ip
                                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
                                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                        inner join PatientClinicalEvent ipce on ip.Id = ipce.Patient_Id 
                                        inner join MetaPatientClinicalEvent impce on ipce.PatientClinicalEventGuid = impce.PatientClinicalEventGuid 
                                    where ip.Archived = 0 and ipf.Archived = 0 and ipce.Archived = 0
                                        and ipf.EnrolledDate between '{searchFrom.ToString("yyyy-MM-dd")}' and '{searchTo.ToString("yyyy-MM-dd")}'
                                        and ifa.Id = f.Id
                                        and impce.`Istheadverseeventserious?` = 'Yes'
                                ) AS PatientWithSeriousEventCount
                            ,	(
                                    select count(distinct(ip.Id))
                                    from Patient ip
                                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
                                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                        inner join PatientClinicalEvent ipce on ip.Id = ipce.Patient_Id 
                                    where ip.Archived = 0 and ipf.Archived = 0 and ipce.Archived = 0
                                        and ipf.EnrolledDate between '{searchFrom.ToString("yyyy-MM-dd")}' and '{searchTo.ToString("yyyy-MM-dd")}'
                                        and ifa.Id = f.Id
                                ) AS PatientWithEventCount
                        FROM Facility f
                        ORDER BY f.FacilityName ";

                return await connection.QueryAsync<PatientsOnTreatmentDto>(sql);

            }
        }

        public async Task<IEnumerable<PatientListDto>> GetPatientOnTreatmentListByFacilityAsync(DateTime searchFrom, DateTime searchTo, int facilityId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<PatientListDto>(
                    @$"SELECT s.Id AS PatientId, s.FirstName + ' ' + s.Surname AS FullName, s.FacilityName  FROM
	                        	(
			                        select ip.Id, ip.FirstName, ip.Surname, ifa.FacilityName
			                        from Patient ip 
				                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
				                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                    where ip.Archived = 0 
			                            and ip.Created between '{searchFrom.ToString("yyyy-MM-dd")}' and '{searchTo.ToString("yyyy-MM-dd")}'
				                        and ifa.Id = {facilityId}
		                        ) AS s");
            }
        }
    }
}