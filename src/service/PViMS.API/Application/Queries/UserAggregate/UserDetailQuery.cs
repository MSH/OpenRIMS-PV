using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.UserAggregate
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
