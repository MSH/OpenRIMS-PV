using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.OrgUnitAggregate
{
    [DataContract]
    public class OrgUnitsIdentifierQuery
        : IRequest<LinkedCollectionResourceWrapperDto<OrgUnitIdentifierDto>>
    {
        [DataMember]
        public string OrderBy { get; private set; }

        [DataMember]
        public int PageNumber { get; private set; }

        [DataMember]
        public int PageSize { get; private set; }

        public OrgUnitsIdentifierQuery()
        {
        }

        public OrgUnitsIdentifierQuery(string orderBy, int pageNumber, int pageSize) : this()
        {
            OrderBy = orderBy;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
