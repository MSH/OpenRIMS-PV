using Dapper;
using MySqlConnector;
using PVIMS.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.AppointmentAggregate
{
    public class AppointmentQueries
        : IAppointmentQueries
    {
        private string _connectionString = string.Empty;

        public AppointmentQueries(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<AppointmentSearchDto>> SearchAppointmentsAsync(
            int criteriaId,
            int? searchFacilityId,
            int? searchPatientId,
            string searchFirstName,
            string searchLastName,
            DateTime? searchFrom,
            DateTime? searchTo,
            string customAttributeKey, 
            string customAttributeValue)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var joinCustomAttribute = !String.IsNullOrWhiteSpace(customAttributeKey) && !String.IsNullOrWhiteSpace(customAttributeValue) ? $"CROSS APPLY p.CustomAttributesXmlSerialised.nodes('CustomAttributeSet/CustomStringAttribute') as X(Y)" : "";

                var whereCriteria = PrepareCriteriaQuery(criteriaId);
                var whereFacility = searchFacilityId.HasValue ? $"AND pf.Facility_Id = {searchFacilityId}" : "";
                var wherePatient = searchPatientId.HasValue ? $"AND p.Id = {searchPatientId}" : "";
                var whereFirstName = !String.IsNullOrWhiteSpace(searchFirstName) ? $"AND p.FirstName LIKE '%{searchFirstName}%'" : "";
                var whereLastName = !String.IsNullOrWhiteSpace(searchLastName) ? $"AND p.Surname LIKE '%{searchLastName}%'" : "";
                var whereSearchFrom = searchFrom.HasValue ? $"AND a.AppointmentDate >= '%{searchFrom.Value.ToString("yyyy-MM-dd")}%'" : "";
                var whereSearchTo = searchTo.HasValue ? $"AND a.AppointmentDate<= '%{searchTo.Value.ToString("yyyy-MM-dd")}%'" : "";
                var whereCustomAttribute = !String.IsNullOrWhiteSpace(customAttributeKey) && !String.IsNullOrWhiteSpace(customAttributeValue) ? $"AND X.Y.value('(Key)[1]', 'VARCHAR(MAX)') = '%{customAttributeKey}%' AND X.Y.value('(Value)[1]', 'VARCHAR(MAX)')  LIKE '%{customAttributeValue}%'" : "";

                var sql = $@"SELECT	
                                a.Id,
                                DATE_FORMAT(a.AppointmentDate, '%Y-%m-%d %T.%f') AS AppointmentDate,
				                p.Id as PatientId,
				                p.FirstName,
				                p.Surname AS LastName,
				                f.FacilityName,
				                a.Reason,
				                CASE 
                                    WHEN a.Cancelled = 1 THEN 'Cancelled' 
                                    WHEN e.Id IS NOT NULL THEN 'Appointment met' 
                                    WHEN e.Id IS NULL AND DATE_ADD(a.AppointmentDate, INTERVAL 3 DAY) >= NOW() THEN 'Appointment' 
                                    WHEN e.Id IS NULL AND DATE_ADD(a.AppointmentDate, INTERVAL 3 DAY) < NOW() AND a.DNA = 0 THEN 'MISSED' 
                                    WHEN e.Id IS NULL AND DATE_ADD(a.AppointmentDate, INTERVAL 3 DAY) < NOW() AND a.DNA = 1 THEN 'Did Not Arrive' 
                                END AS AppointmentStatus,
				                IFNULL(e.Id, 0) as EncounterId
			                FROM Appointment a
				                INNER JOIN Patient p ON a.Patient_Id = p.Id
                                INNER JOIN PatientFacility pf ON p.Id = pf.Patient_Id 
                                    AND pf.Id = (SELECT Id FROM PatientFacility ipf WHERE Patient_Id = p.Id ORDER BY EnrolledDate DESC limit 0,10) 
				                INNER JOIN Facility f ON pf.Facility_Id = f.Id
				                LEFT JOIN Encounter e ON e.Patient_Id = p.Id 
                                    AND e.EncounterDate >= DATE_ADD(a.AppointmentDate, INTERVAL 1 DAY) 
                                    AND e.EncounterDate <= DATE_ADD(a.AppointmentDate, INTERVAL 5 DAY) 
			                    {joinCustomAttribute} 
			                WHERE a.Archived = 0 AND p.Archived = 0 
                            {whereCriteria} 
                            {whereFacility} 
                            {wherePatient} 
                            {whereFirstName} 
                            {whereLastName} 
                            {whereSearchFrom} 
                            {whereSearchTo} 
                            {whereCustomAttribute} 
		            ORDER BY a.Id desc";

                return await connection.QueryAsync<AppointmentSearchDto>(sql);
            }
        }

        private string PrepareCriteriaQuery(int criteriaId)
        {
            if (criteriaId == 3)
            {
                return $@"AND DATEADD(dd, 3, a.AppointmentDate) < NOW() AND a.DNA = 0 AND e.Id IS NULL ";
            }
            if (criteriaId == 4)
            {
                return $@"AND DATEADD(dd, 3, a.AppointmentDate) < NOW() AND a.DNA = 1 AND e.Id IS NULL ";
            }
            if (criteriaId == 5)
            {
                return $@"AND e.Id IS NOT NULL ";
            }
            if (criteriaId == 5)
            {
                return $@"AND a.Cancelled = 0 ";
            }

            return "";
        }

        public async Task<IEnumerable<OutstandingVisitReportDto>> GetOutstandingVisitsAsync(DateTime searchFrom, DateTime searchTo, int facilityId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string where = facilityId > 0 ? " AND pf.Facility_Id = " + facilityId.ToString() : "";

                return await connection.QueryAsync<OutstandingVisitReportDto>(
                    @$"SELECT   p.Id AS PatientId, 
                                p.FirstName, 
                                p.Surname as LastName, 
                                a.AppointmentDate,
                                f.FacilityName as Facility
                       FROM Patient p 
	                        INNER JOIN Appointment a ON p.Id = a.Patient_Id 
                            INNER JOIN PatientFacility pf ON pf.Id = (select top 1 Id from PatientFacility ipf where ipf.Patient_Id = p.Id and ipf.EnrolledDate <= GETDATE() order by ipf.EnrolledDate desc, ipf.Id desc)
                            INNER JOIN Facility f ON pf.Facility_Id = f.Id
                       WHERE a.AppointmentDate < DATEADD(dd, 3, GETDATE())
	                        AND a.AppointmentDate BETWEEN '{searchFrom.ToString("yyyy-MM-dd")}' AND '{searchTo.ToString("yyyy-MM-dd")}' {where} 
	                        AND a.Cancelled = 0
                            AND p.Archived = 0 and a.Archived = 0
	                        AND NOT EXISTS(SELECT Id FROM Encounter ie WHERE ie.Patient_Id = p.Id AND ie.Archived = 0 AND ie.EncounterDate BETWEEN DATEADD(dd, -3, a.AppointmentDate) AND DATEADD(dd, 3, a.AppointmentDate))
                       ORDER BY a.AppointmentDate desc");
            }
        }
    }
}