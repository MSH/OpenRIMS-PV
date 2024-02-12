using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.WorkFlowAggregate
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
