using OpenRIMS.PV.Main.API.Infrastructure.Auth;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Infrastructure.Services
{
    public interface IJwtFactory
    {
        Task<AccessToken> GenerateEncodedToken(User user, IList<string> roles);
    }
}
