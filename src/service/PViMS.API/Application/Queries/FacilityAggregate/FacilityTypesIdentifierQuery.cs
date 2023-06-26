using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.FacilityAggregate
{
    [DataContract]
    public class FacilityTypesIdentifierQuery
        : IRequest<LinkedCollectionResourceWrapperDto<FacilityTypeIdentifierDto>>
    {
        [DataMember]
        public string OrderBy { get; private set; }

        [DataMember]
        public int PageNumber { get; private set; }

        [DataMember]
        public int PageSize { get; private set; }

        public FacilityTypesIdentifierQuery()
        {
        }

        public FacilityTypesIdentifierQuery(string orderBy, int pageNumber, int pageSize) : this()
        {
            OrderBy = orderBy;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
