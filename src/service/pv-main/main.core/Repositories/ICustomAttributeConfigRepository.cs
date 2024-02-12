using OpenRIMS.PV.Main.Core.Entities;
using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Repositories
{
    public interface ICustomAttributeConfigRepository
    {
        IList<CustomAttributeConfiguration> RetrieveAttributeConfigurationsForType(string typeName);
        IList<string> RetrieveAttributeKeysForType(string typeName);
    }
}
