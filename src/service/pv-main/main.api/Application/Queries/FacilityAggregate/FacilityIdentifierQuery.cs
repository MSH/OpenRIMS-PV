using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.FacilityAggregate
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
