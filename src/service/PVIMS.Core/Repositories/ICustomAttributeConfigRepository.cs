using PVIMS.Core.Entities;
using System.Collections.Generic;

namespace PVIMS.Core.Repositories
{
    public interface ICustomAttributeConfigRepository
    {
        IList<CustomAttributeConfiguration> RetrieveAttributeConfigurationsForType(string typeName);
        IList<string> RetrieveAttributeKeysForType(string typeName);
    }
}
