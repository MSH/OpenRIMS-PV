using OpenRIMS.PV.Main.Core.SeedWork;
using System;

namespace OpenRIMS.PV.Main.Core.Aggregates.UserAggregate
{
    public class RefreshToken : Entity<int>
    {
        public RefreshToken() : base() { /* Required by EF */ }

        public RefreshToken(string token, DateTime expires, string remoteIpAddress) : this()
        {
            Token = token;
            Expires = expires;
            RemoteIpAddress = remoteIpAddress;
        }

        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public string RemoteIpAddress { get; set; }
        public int UserId { get; set; }

        public User User { get; set; }

        public bool Active => DateTime.UtcNow <= Expires;
    }
}
