using OpenRIMS.PV.Main.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.AppointmentAggregate
{
    public interface IAppointmentQueries
    {
        Task<IEnumerable<AppointmentSearchDto>> SearchAppointmentsAsync(
            int criteriaId,
            int? searchFacilityId,
            int? searchPatientId,
            string searchFirstName,
            string searchLastName,
            DateTime? searchFrom,
            DateTime? searchTo,
            string customAttributeKey,
            string customAttributeValue);

        Task<IEnumerable<OutstandingVisitReportDto>> GetOutstandingVisitsAsync(DateTime searchFrom, DateTime searchTo, int facilityId);
    }
}
