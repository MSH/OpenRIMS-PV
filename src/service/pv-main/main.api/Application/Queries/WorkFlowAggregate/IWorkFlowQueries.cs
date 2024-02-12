using OpenRIMS.PV.Main.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.Queries.WorkFlowAggregate
{
    public interface IWorkFlowQueries
    {
        Task<WorkFlowSummaryDto> GetWorkFlowFeedbackSummaryAsync(Guid workFlowGuid, List<string> userFacilityCodes);
    }
}
