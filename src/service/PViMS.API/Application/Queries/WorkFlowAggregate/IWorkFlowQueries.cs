using PVIMS.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.WorkFlowAggregate
{
    public interface IWorkFlowQueries
    {
        Task<WorkFlowSummaryDto> GetWorkFlowFeedbackSummaryAsync(Guid workFlowGuid, List<string> userFacilityCodes);
    }
}
