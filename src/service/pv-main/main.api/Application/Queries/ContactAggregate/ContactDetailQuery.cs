using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.ContactAggregate
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
