using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.UserAggregate
{
    [DataContract]
    public class AddUserCommand
        : IRequest<UserIdentifierDto>
    {
        [DataMember]
        public string FirstName { get; private set; }

        [DataMember]
        public string LastName { get; private set; }

        [DataMember]
        public string Email { get; private set; }

        [DataMember]
        public string UserName { get; private set; }

        [DataMember]
        public string Password { get; private set; }

        [DataMember]
        public string ConfirmPassword { get; private set; }

        [DataMember]
        public List<string> Roles { get; set; }

        [DataMember]
        public List<string> Facilities { get; set; }

        public AddUserCommand()
        {
        }

        public AddUserCommand(string firstName, string lastName, string email, string userName, string password, string confirmPassword, List<string> roles, List<string> facilities) : this()
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            UserName = userName;
            Password = password;
            ConfirmPassword = confirmPassword;
            Roles = roles;
            Facilities = facilities;
        }
    }
}
