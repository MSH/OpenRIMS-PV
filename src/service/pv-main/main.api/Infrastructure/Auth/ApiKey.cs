using System;
using System.Collections.Generic;

namespace OpenRIMS.PV.Main.API.Infrastructure.Auth
{
    public class ApiKey
    {
        public ApiKey(int id, string owner, string key, IReadOnlyCollection<string> roles)
        {
            Id = id;
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Roles = roles ?? throw new ArgumentNullException(nameof(roles));
        }

        public int Id { get; }
        public string Owner { get; }
        public string Key { get; }
        public IReadOnlyCollection<string> Roles { get; }
    }
}
