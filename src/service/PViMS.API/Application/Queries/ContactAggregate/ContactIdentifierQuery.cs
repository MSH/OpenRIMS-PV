using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.ContactAggregate
{
    [DataContract]
    public class ContactIdentifierQuery
        : IRequest<ContactIdentifierDto>
    {
        [DataMember]
        public int SiteContactDetailId { get; private set; }

        public ContactIdentifierQuery()
        {
        }

        public ContactIdentifierQuery(int siteContactDetailId) : this()
        {
            SiteContactDetailId = siteContactDetailId;
        }
    }
}
