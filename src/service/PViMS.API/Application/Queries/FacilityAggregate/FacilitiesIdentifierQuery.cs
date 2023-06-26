using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.FacilityAggregate
{
    [DataContract]
    public class FacilitiesIdentifierQuery
        : IRequest<LinkedCollectionResourceWrapperDto<FacilityIdentifierDto>>
    {
        [DataMember]
        public string OrderBy { get; private set; }

        [DataMember]
        public int PageNumber { get; private set; }

        [DataMember]
        public int PageSize { get; private set; }

        public FacilitiesIdentifierQuery()
        {
        }

        public FacilitiesIdentifierQuery(string orderBy, int pageNumber, int pageSize) : this()
        {
            OrderBy = orderBy;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
