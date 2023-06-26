using MediatR;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.UserAggregate
{
    [DataContract]
    public class ChangeUserFacilitiesCommand
        : IRequest<bool>
    {
        [DataMember]
        public int UserId { get; private set; }

        [DataMember]
        public List<string> Facilities { get; set; }

        public ChangeUserFacilitiesCommand()
        {
        }

        public ChangeUserFacilitiesCommand(int userId, List<string> facilities) : this()
        {
            UserId = userId;
            Facilities = facilities;
        }
    }
}
