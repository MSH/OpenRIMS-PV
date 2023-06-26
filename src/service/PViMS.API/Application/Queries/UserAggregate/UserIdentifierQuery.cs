using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.UserAggregate
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
