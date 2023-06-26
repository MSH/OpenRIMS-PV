using MediatR;
using PVIMS.API.Models;
using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.WorkFlowAggregate
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
