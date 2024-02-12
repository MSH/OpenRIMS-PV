using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.ReportInstanceAggregate
{
    [DataContract]
    public class ReportInstanceTaskIdentifierQuery
        : IRequest<ReportInstanceTaskIdentifierDto>
    {
        [DataMember]
        public Guid WorkFlowGuid { get; private set; }

        [DataMember]
        public int ReportInstanceId { get; private set; }

        [DataMember]
        public int Id { get; private set; }

        public ReportInstanceTaskIdentifierQuery()
        {
        }

        public ReportInstanceTaskIdentifierQuery(Guid workFlowGuid, int reportInstanceId, int id) : this()
        {
            WorkFlowGuid = workFlowGuid;
            ReportInstanceId = reportInstanceId;
            Id = id;
        }
    }
}
