using MediatR;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.UserAggregate
{
    [DataContract]
    public class ChangeUserDetailCommand
        : IRequest<bool>
    {
        [DataMember]
        public int UserId { get; private set; }

        [DataMember]
        public string FirstName { get; private set; }

        [DataMember]
        public string LastName { get; private set; }

        [DataMember]
        public string Email { get; private set; }

        [DataMember]
        public string UserName { get; private set; }

        [DataMember]
        public bool Active { get; private set; }

        [DataMember]
        public bool AllowDatasetDownload { get; private set; }

        public ChangeUserDetailCommand()
        {
        }

        public ChangeUserDetailCommand(int userId, string firstName, string lastName, string email, string userName, bool active, bool allowDatasetDownload) : this()
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            UserName = userName;
            Active = active;
            AllowDatasetDownload = allowDatasetDownload;
        }
    }
}
