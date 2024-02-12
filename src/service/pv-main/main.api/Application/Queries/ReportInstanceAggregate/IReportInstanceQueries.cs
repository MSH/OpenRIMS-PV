using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.ReportInstanceAggregate
{
    public interface IReportInstanceQueries
    {
        Task<IEnumerable<ReportInstanceEventDto>> GetExecutionStatusEventsForPatientViewAsync(int patientId);

        Task<IEnumerable<ReportInstanceEventDto>> GetExecutionStatusEventsForEventViewAsync(int patientClinicalEventId);

        Task<IEnumerable<CausalityReportDto>> GetCausalityNotSetAsync(DateTime searchFrom, DateTime searchTo, CausalityConfigType causalityConfig, int facilityId, CausalityCriteria causalityCriteria);
    }
}
