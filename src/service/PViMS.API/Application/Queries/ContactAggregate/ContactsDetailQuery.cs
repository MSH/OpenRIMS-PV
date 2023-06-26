using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.ContactAggregate
{
    [DataContract]
    public class ContactsDetailQuery
        : IRequest<LinkedCollectionResourceWrapperDto<ContactDetailDto>>
    {
        [DataMember]
        public string OrderBy { get; private set; }

        [DataMember]
        public int PageNumber { get; private set; }

        [DataMember]
        public int PageSize { get; private set; }

        public ContactsDetailQuery()
        {
        }

        public ContactsDetailQuery(string orderBy, int pageNumber, int pageSize) : this()
        {
            OrderBy = orderBy;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
