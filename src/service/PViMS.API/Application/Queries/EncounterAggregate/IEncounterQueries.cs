using PVIMS.API.Application.Models.Encounter;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.EncounterAggregate
{
    public interface IEncounterQueries
    {
        Task<IEnumerable<SearchEncounterDto>> SearchEncountersAsync(int currentUserId,
            int? searchFacilityId,
            int? searchPatientId,
            string searchFirstName,
            string searchLastName,
            DateTime? searchFrom,
            DateTime? searchTo,
            string customAttributeKey,
            string customAttributeValue);
    }
}
