using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.FacilityAggregate
{
    [DataContract]
    public class FacilitiesDetailQuery
        : IRequest<LinkedCollectionResourceWrapperDto<FacilityDetailDto>>
    {
        [DataMember]
        public string OrderBy { get; private set; }

        [DataMember]
        public int PageNumber { get; private set; }

        [DataMember]
        public int PageSize { get; private set; }

        public FacilitiesDetailQuery()
        {
        }

        public FacilitiesDetailQuery(string orderBy, int pageNumber, int pageSize) : this()
        {
            OrderBy = orderBy;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
