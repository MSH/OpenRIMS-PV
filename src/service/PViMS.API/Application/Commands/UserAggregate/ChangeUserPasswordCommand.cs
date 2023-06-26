using MediatR;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.UserAggregate
{
    [DataContract]
    public class ChangeUserPasswordCommand
        : IRequest<bool>
    {
        [DataMember]
        public int UserId { get; private set; }

        [DataMember]
        public string Password { get; private set; }

        public ChangeUserPasswordCommand()
        {
        }

        public ChangeUserPasswordCommand(int userId, string password) : this()
        {
            UserId = userId;
            Password = password;
        }
    }
}
