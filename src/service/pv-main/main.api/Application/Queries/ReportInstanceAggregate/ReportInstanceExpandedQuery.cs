using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.ReportInstanceAggregate
{
    [DataContract]
    public class ReportInstanceExpandedQuery
        : IRequest<ReportInstanceExpandedDto>
    {
        [DataMember]
        public Guid WorkFlowGuid { get; private set; }

        [DataMember]
        public int ReportInstanceId { get; private set; }

        public ReportInstanceExpandedQuery()
        {
        }

        public ReportInstanceExpandedQuery(Guid workFlowGuid, int reportInstanceId) : this()
        {
            WorkFlowGuid = workFlowGuid;
            ReportInstanceId = reportInstanceId;
        }
    }
}
