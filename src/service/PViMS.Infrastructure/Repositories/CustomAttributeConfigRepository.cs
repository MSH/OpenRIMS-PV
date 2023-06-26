using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using PVIMS.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PVIMS.Infrastructure.Repositories
{
    public class CustomAttributeConfigRepository : ICustomAttributeConfigRepository
    {
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeConfigurationRepository;

        public CustomAttributeConfigRepository(IRepositoryInt<CustomAttributeConfiguration> customAttributeConfigurationRepository)
        {
            _customAttributeConfigurationRepository = customAttributeConfigurationRepository ?? throw new ArgumentNullException(nameof(customAttributeConfigurationRepository));
        }

        /// <summary>
        /// Retrieves the attribute configurations for the specified type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public IList<CustomAttributeConfiguration> RetrieveAttributeConfigurationsForType(string typeName)
        {
            return _customAttributeConfigurationRepository.Queryable()
                .Where(ca => ca.ExtendableTypeName == typeName)
                .ToList();
        }

        /// <summary>
        /// Retrieves the attribute keys for the specified type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public IList<string> RetrieveAttributeKeysForType(string typeName)
        {
            return _customAttributeConfigurationRepository.Queryable()
                .Where(ca => ca.ExtendableTypeName == typeName)
                .Select(c => c.AttributeKey)
                .ToList();
        }
    }
}
