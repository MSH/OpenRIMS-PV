using MediatR;
using PVIMS.API.Models;
using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.WorkFlowAggregate
{
    [DataContract]
    public class WorkFlowDetailQuery
        : IRequest<WorkFlowDetailDto>
    {
        [DataMember]
        public Guid WorkFlowGuid { get; private set; }

        public WorkFlowDetailQuery()
        {
        }

        public WorkFlowDetailQuery(Guid workFlowGuid) : this()
        {
            WorkFlowGuid = workFlowGuid;
        }
    }
}
