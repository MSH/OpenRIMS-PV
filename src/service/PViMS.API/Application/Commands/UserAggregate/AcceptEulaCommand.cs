using MediatR;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.UserAggregate
{
    [DataContract]
    public class AcceptEulaCommand
        : IRequest<bool>
    {
        [DataMember]
        public int UserId { get; private set; }

        public AcceptEulaCommand()
        {
        }

        public AcceptEulaCommand(int userId) : this()
        {
            UserId = userId;
        }
    }
}
