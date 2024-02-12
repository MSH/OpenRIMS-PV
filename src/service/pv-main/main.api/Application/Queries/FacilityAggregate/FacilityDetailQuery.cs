using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.FacilityAggregate
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
