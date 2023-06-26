using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.FacilityAggregate
{
    [DataContract]
    public class FacilityIdentifierQuery
        : IRequest<FacilityIdentifierDto>
    {
        [DataMember]
        public int FacilityId { get; private set; }

        public FacilityIdentifierQuery()
        {
        }

        public FacilityIdentifierQuery(int facilityId) : this()
        {
            FacilityId = facilityId;
        }
    }
}
