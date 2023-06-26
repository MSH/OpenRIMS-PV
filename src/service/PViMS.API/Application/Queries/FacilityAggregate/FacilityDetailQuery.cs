using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.FacilityAggregate
{
    [DataContract]
    public class FacilityDetailQuery
        : IRequest<FacilityDetailDto>
    {
        [DataMember]
        public int FacilityId { get; private set; }

        public FacilityDetailQuery()
        {
        }

        public FacilityDetailQuery(int facilityId) : this()
        {
            FacilityId = facilityId;
        }
    }
}
