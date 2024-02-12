using OpenRIMS.PV.Main.API.Application.Models.Encounter;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.EncounterAggregate
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
