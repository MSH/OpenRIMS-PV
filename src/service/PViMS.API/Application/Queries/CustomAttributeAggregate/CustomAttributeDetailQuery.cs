using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.CustomAttributeAggregate
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
