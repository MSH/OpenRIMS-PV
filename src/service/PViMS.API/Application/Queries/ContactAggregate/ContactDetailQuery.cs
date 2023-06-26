using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.ContactAggregate
{
    [DataContract]
    public class ContactDetailQuery
        : IRequest<ContactDetailDto>
    {
        [DataMember]
        public int SiteContactDetailId { get; private set; }

        public ContactDetailQuery()
        {
        }

        public ContactDetailQuery(int siteContactDetailId) : this()
        {
            SiteContactDetailId = siteContactDetailId;
        }
    }
}
