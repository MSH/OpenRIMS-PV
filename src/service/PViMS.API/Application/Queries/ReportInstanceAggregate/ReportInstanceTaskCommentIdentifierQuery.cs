using MediatR;
using PVIMS.API.Models;
using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.ReportInstanceAggregate
{
    [DataContract]
    public class ReportInstanceTaskCommentIdentifierQuery
        : IRequest<ReportInstanceTaskCommentIdentifierDto>
    {
        [DataMember]
        public Guid WorkFlowGuid { get; private set; }

        [DataMember]
        public int ReportInstanceId { get; private set; }

        [DataMember]
        public int ReportInstanceTaskId { get; private set; }

        [DataMember]
        public int Id { get; private set; }

        public ReportInstanceTaskCommentIdentifierQuery()
        {
        }

        public ReportInstanceTaskCommentIdentifierQuery(Guid workFlowGuid, int reportInstanceId, int reportInstanceTaskId, int id) : this()
        {
            WorkFlowGuid = workFlowGuid;
            ReportInstanceId = reportInstanceId;
            ReportInstanceTaskId = reportInstanceTaskId;
            Id = id;
        }
    }
}
