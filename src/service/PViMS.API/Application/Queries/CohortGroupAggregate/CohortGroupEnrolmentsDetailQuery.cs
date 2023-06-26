using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.CohortGroupAggregate
{
    [DataContract]
    public class CohortGroupEnrolmentsDetailQuery
        : IRequest<LinkedCollectionResourceWrapperDto<EnrolmentDetailDto>>
    {
        [DataMember]
        public int CohortGroupId { get; private set; }

        [DataMember]
        public string OrderBy { get; private set; }

        [DataMember]
        public int PageNumber { get; private set; }

        [DataMember]
        public int PageSize { get; private set; }

        public CohortGroupEnrolmentsDetailQuery()
        {
        }

        public CohortGroupEnrolmentsDetailQuery(int cohortGroupId, string orderBy, int pageNumber, int pageSize) : this()
        {
            CohortGroupId = cohortGroupId;
            OrderBy = orderBy;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
