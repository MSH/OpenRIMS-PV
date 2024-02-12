using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.ContactAggregate
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
