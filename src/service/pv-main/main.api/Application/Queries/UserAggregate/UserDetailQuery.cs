using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.UserAggregate
{
    [DataContract]
    public class UserDetailQuery
        : IRequest<UserDetailDto>
    {
        [DataMember]
        public int UserId { get; private set; }

        public UserDetailQuery()
        {
        }

        public UserDetailQuery(int userId) : this()
        {
            UserId = userId;
        }
    }
}
