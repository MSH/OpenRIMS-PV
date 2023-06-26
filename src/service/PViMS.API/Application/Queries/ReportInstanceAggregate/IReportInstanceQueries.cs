using PVIMS.API.Models;
using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.ReportInstanceAggregate
{
    public interface IReportInstanceQueries
    {
        Task<IEnumerable<ReportInstanceEventDto>> GetExecutionStatusEventsForPatientViewAsync(int patientId);

        Task<IEnumerable<ReportInstanceEventDto>> GetExecutionStatusEventsForEventViewAsync(int patientClinicalEventId);

        Task<IEnumerable<CausalityReportDto>> GetCausalityNotSetAsync(DateTime searchFrom, DateTime searchTo, CausalityConfigType causalityConfig, int facilityId, CausalityCriteria causalityCriteria);
    }
}
