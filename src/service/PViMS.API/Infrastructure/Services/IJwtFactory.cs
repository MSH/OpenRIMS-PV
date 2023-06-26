using PVIMS.API.Infrastructure.Auth;
using PVIMS.Core.Aggregates.UserAggregate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PVIMS.API.Infrastructure.Services
{
    public interface IJwtFactory
    {
        Task<AccessToken> GenerateEncodedToken(User user, IList<string> roles);
    }
}
