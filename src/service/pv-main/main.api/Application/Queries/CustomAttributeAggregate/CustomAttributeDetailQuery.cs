using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.CustomAttributeAggregate
{
    [DataContract]
    public class CustomAttributeDetailQuery
        : IRequest<CustomAttributeDetailDto>
    {
        [DataMember]
        public int CustomAttributeId { get; private set; }

        public CustomAttributeDetailQuery()
        {
        }

        public CustomAttributeDetailQuery(int customAttributeId) : this()
        {
            CustomAttributeId = customAttributeId;
        }
    }
}
