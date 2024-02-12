using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.ReportInstanceAggregate
{
    [DataContract]
    public class ReportInstancesFeedbackQuery
        : IRequest<LinkedCollectionResourceWrapperDto<ReportInstanceDetailDto>>
    {
        [DataMember]
        public Guid WorkFlowGuid { get; private set; }

        [DataMember]
        public string QualifiedName { get; private set; }

        [DataMember]
        public int PageNumber { get; private set; }

        [DataMember]
        public int PageSize { get; private set; }

        public ReportInstancesFeedbackQuery()
        {
        }

        public ReportInstancesFeedbackQuery(Guid workFlowGuid, string qualifiedName, int pageNumber, int pageSize) : this()
        {
            WorkFlowGuid = workFlowGuid;
            QualifiedName = qualifiedName;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
