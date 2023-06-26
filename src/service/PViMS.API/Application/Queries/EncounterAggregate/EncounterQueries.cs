using Dapper;
using MySqlConnector;
using PVIMS.API.Application.Models.Encounter;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.EncounterAggregate
{
    public class EncounterQueries
        : IEncounterQueries
    {
        private string _connectionString = string.Empty;

        public EncounterQueries(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<SearchEncounterDto>> SearchEncountersAsync(
            int currentUserId, 
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

                var whereFacility = searchFacilityId.HasValue ? $"AND pf.Facility_Id = {searchFacilityId}" : $"AND pf.Facility_Id IN (SELECT f.Id FROM UserFacility uf INNER JOIN Facility f ON uf.Facility_Id = f.Id WHERE uf.User_Id = {currentUserId})";
                var wherePatient = searchPatientId.HasValue ? $"AND p.Id = {searchPatientId}" : "";
                var whereFirstName = !String.IsNullOrWhiteSpace(searchFirstName) ? $"AND p.FirstName LIKE '%{searchFirstName}%'" : "";
                var whereLastName = !String.IsNullOrWhiteSpace(searchLastName) ? $"AND p.Surname LIKE '%{searchLastName}%'" : "";
                var whereSearchFrom = searchFrom.HasValue ? $"AND e.EncounterDate >= '%{searchFrom.Value.ToString("yyyy-MM-dd")}%'" : "";
                var whereSearchTo = searchTo.HasValue ? $"AND e.EncounterDate <= '%{searchTo.Value.ToString("yyyy-MM-dd")}%'" : "";
                var whereCustomAttribute = !String.IsNullOrWhiteSpace(customAttributeKey) && !String.IsNullOrWhiteSpace(customAttributeValue) ? $"AND X.Y.value('(Key)[1]', 'VARCHAR(MAX)') = '%{customAttributeKey}%' AND X.Y.value('(Value)[1]', 'VARCHAR(MAX)')  LIKE '%{customAttributeValue}%'" : "";

                var sql = $@"
		            SELECT	e.Id AS EncounterId,
                            p.Id AS PatientId, 
				            p.FirstName, 
				            p.Surname, 
				            f.FacilityName, 
                            et.Description AS EncounterType, 
				            DATE_FORMAT(e.EncounterDate, '%Y-%m-%d %T.%f') AS EncounterDate
			            FROM Encounter e
                            INNER JOIN Patient p ON e.Patient_Id = p.Id
				            INNER JOIN PatientFacility pf ON p.Id = pf.Patient_Id AND pf.Id = (SELECT Id FROM PatientFacility ipf WHERE Patient_Id = p.Id ORDER BY EnrolledDate DESC limit 0,10) 
				            INNER JOIN Facility f ON pf.Facility_Id = f.Id 
                            INNER JOIN EncounterType et ON e.EncounterType_Id = et.Id 
			                {joinCustomAttribute} 
			            WHERE e.Archived = 0 AND p.Archived = 0 
                            {whereFacility} 
                            {wherePatient} 
                            {whereFirstName} 
                            {whereLastName} 
                            {whereSearchFrom} 
                            {whereSearchTo} 
				            {whereCustomAttribute} 
		            ORDER BY p.Id desc";

                return await connection.QueryAsync<SearchEncounterDto>(sql);
            }
        }
   }
}