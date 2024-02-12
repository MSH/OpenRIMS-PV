using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.UserAggregate
{
    [DataContract]
    public class UserIdentifierQuery
        : IRequest<UserIdentifierDto>
    {
        [DataMember]
        public int UserId { get; private set; }

        public UserIdentifierQuery()
        {
        }

        public UserIdentifierQuery(int userId) : this()
        {
            UserId = userId;
        }
    }
}
