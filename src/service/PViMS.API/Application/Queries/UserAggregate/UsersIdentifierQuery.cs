using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.UserAggregate
{
    [DataContract]
    public class UsersIdentifierQuery
        : IRequest<LinkedCollectionResourceWrapperDto<UserIdentifierDto>>
    {
        [DataMember]
        public string OrderBy { get; private set; }

        [DataMember]
        public int PageNumber { get; private set; }

        [DataMember]
        public int PageSize { get; private set; }

        [DataMember]
        public string SearchTerm { get; private set; }

        public UsersIdentifierQuery()
        {
        }

        public UsersIdentifierQuery(string orderBy, int pageNumber, int pageSize, string searchTerm) : this()
        {
            OrderBy = orderBy;
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchTerm = searchTerm;
        }
    }
}
