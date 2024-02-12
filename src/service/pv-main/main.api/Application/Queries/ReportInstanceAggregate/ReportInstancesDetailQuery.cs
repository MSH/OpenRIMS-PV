using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.ReportInstanceAggregate
{
    [DataContract]
    public class ReportInstancesDetailQuery
        : IRequest<LinkedCollectionResourceWrapperDto<ReportInstanceDetailDto>>
    {
        [DataMember]
        public Guid WorkFlowGuid { get; private set; }

        [DataMember]
        public bool NewReportsOnly { get; private set; }

        [DataMember]
        public bool FeedbackReportsOnly { get; private set; }

        [DataMember]
        public bool ActiveReportsOnly { get; private set; }

        [DataMember]
        public DateTime SearchFrom { get; private set; }

        [DataMember]
        public DateTime SearchTo { get; private set; }

        [DataMember]
        public string QualifiedName { get; private set; }

        [DataMember]
        public string SearchTerm { get; private set; }

        [DataMember]
        public int PageNumber { get; private set; }

        [DataMember]
        public int PageSize { get; private set; }

        public ReportInstancesDetailQuery()
        {
        }

        public ReportInstancesDetailQuery(Guid workFlowGuid, bool newReportsOnly, bool feedbackReportsOnly, bool activeReportsOnly, DateTime searchFrom, DateTime searchTo, string searchTerm, string qualifiedName, int pageNumber, int pageSize) : this()
        {
            WorkFlowGuid = workFlowGuid;
            NewReportsOnly = newReportsOnly;
            FeedbackReportsOnly = feedbackReportsOnly;
            ActiveReportsOnly = activeReportsOnly;
            SearchFrom = searchFrom;
            SearchTo = searchTo;
            SearchTerm = searchTerm;
            QualifiedName = qualifiedName;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
