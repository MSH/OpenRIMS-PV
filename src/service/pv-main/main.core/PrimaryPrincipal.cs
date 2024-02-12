using System.Security.Principal;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;

namespace OpenRIMS.PV.Main.Core
{
    public class PrimaryPrincipal : GenericPrincipal
    {
        public PrimaryPrincipal(User user, IIdentity identity, string[] roles)
            : base(identity, roles)
        {
            this.User = user;

        }

        public User User { get; private set; }
    }
}