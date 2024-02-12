using MediatR;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.UserAggregate
{
    [DataContract]
    public class RemoveFacilityFromUserCommand
        : IRequest<bool>
    {
        [DataMember]
        public int UserId { get; private set; }

        [DataMember]
        public int FacilityId { get; set; }

        public RemoveFacilityFromUserCommand()
        {
        }

        public RemoveFacilityFromUserCommand(int userId, int facilityId) : this()
        {
            UserId = userId;
            FacilityId = facilityId;
        }
    }
}
