using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.WorkFlowAggregate
{
    [DataContract]
    public class WorkFlowSummaryQuery
        : IRequest<WorkFlowSummaryDto>
    {
        [DataMember]
        public Guid WorkFlowGuid { get; private set; }

        public WorkFlowSummaryQuery()
        {
        }

        public WorkFlowSummaryQuery(Guid workFlowGuid) : this()
        {
            WorkFlowGuid = workFlowGuid;
        }
    }
}
