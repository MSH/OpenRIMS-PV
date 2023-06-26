using MediatR;
using PVIMS.API.Models;
using PVIMS.Core.ValueTypes;
using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.ReportInstanceAggregate
{
    [DataContract]
    public class CausalityReportQuery
        : IRequest<LinkedCollectionResourceWrapperDto<CausalityReportDto>>
    {
        [DataMember]
        public Guid WorkFlowGuid { get; private set; }

        [DataMember]
        public int PageNumber { get; private set; }

        [DataMember]
        public int PageSize { get; private set; }

        [DataMember]
        public DateTime SearchFrom { get; private set; }

        [DataMember]
        public DateTime SearchTo { get; private set; }

        [DataMember]
        public int FacilityId { get; private set; }

        [DataMember]
        public CausalityCriteria CausalityCriteria { get; private set; }

        public CausalityReportQuery()
        {
        }

        public CausalityReportQuery(Guid workFlowGuid, int pageNumber, int pageSize, DateTime searchFrom, DateTime searchTo, int facilityId, CausalityCriteria causalityCriteria) : this()
        {
            WorkFlowGuid = workFlowGuid;
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchFrom = searchFrom;
            SearchTo = searchTo;
            FacilityId = facilityId;
            CausalityCriteria = causalityCriteria;
        }
    }
}
