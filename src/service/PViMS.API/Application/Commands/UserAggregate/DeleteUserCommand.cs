using MediatR;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.UserAggregate
{
    [DataContract]
    public class DeleteUserCommand
        : IRequest<bool>
    {
        [DataMember]
        public int UserId { get; private set; }

        public DeleteUserCommand()
        {
        }

        public DeleteUserCommand(int userId) : this()
        {
            UserId = userId;
        }
    }
}
