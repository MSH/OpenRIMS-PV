using MediatR;
using PVIMS.API.Models;
using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.WorkFlowAggregate
{
    [DataContract]
    public class WorkFlowIdentifierQuery
        : IRequest<WorkFlowIdentifierDto>
    {
        [DataMember]
        public Guid WorkFlowGuid { get; private set; }

        public WorkFlowIdentifierQuery()
        {
        }

        public WorkFlowIdentifierQuery(Guid workFlowGuid) : this()
        {
            WorkFlowGuid = workFlowGuid;
        }
    }
}
