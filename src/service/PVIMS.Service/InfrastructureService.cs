using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using PVIMS.Core.ValueTypes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PVIMS.Services
{
    public class InfrastructureService : IInfrastructureService 
    {
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IRepositoryInt<DatasetInstanceValue> _instanceValueRepository;
        private readonly IRepositoryInt<Config> _configRepository;

        public InfrastructureService(IUnitOfWorkInt unitOfWork,
            IRepositoryInt<DatasetInstanceValue> instanceValueRepository,
            IRepositoryInt<Config> configRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _instanceValueRepository = instanceValueRepository ?? throw new ArgumentNullException(nameof(instanceValueRepository));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
        }

        #region "Referential Checks"

        public bool HasAssociatedData(DatasetElement element)
        {
            var hasData = false;

            hasData = (element.DatasetCategoryElements.Count > 0 || element.DatasetElementSubs.Count > 0 || _instanceValueRepository.Queryable().Any(div => div.DatasetElement.Id == element.Id));

            return hasData;
        }

        public DatasetElement GetTerminologyMedDra()
        {
            var meddraElement = _unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.ElementName == "TerminologyMedDra");
            if (meddraElement == null)
            {
                meddraElement = new DatasetElement()
                {
                    // Prepare new element
                    DatasetElementType = _unitOfWork.Repository<DatasetElementType>().Queryable().Single(x => x.Description == "Generic"),
                    Field = new Field()
                    {
                        Anonymise = false,
                        Mandatory = false,
                        FieldType = _unitOfWork.Repository<FieldType>().Queryable().Single(x => x.Description == "AlphaNumericTextbox")
                    },
                    ElementName = "TerminologyMedDra",
                    DefaultValue = string.Empty,
                    Oid = string.Empty,
                    System = true
                };
                var rule = meddraElement.GetRule(DatasetRuleType.ElementCanoOnlyLinkToSingleDataset);
                rule.RuleActive = true;

                _unitOfWork.Repository<DatasetElement>().Save(meddraElement);
            }
            return meddraElement;
        }

        public Config GetOrCreateConfig(ConfigType configType)
        {
            var config = _unitOfWork.Repository<Config>().Queryable().
                FirstOrDefault(c => c.ConfigType == configType);

            if (config == null)
            {
                config = new Config()
                {
                    // Prepare new config
                    ConfigType = configType,
                    ConfigValue = ""
                };
                _unitOfWork.Repository<Config>().Save(config);
            }
            return config;
        }

        public async Task SetConfigValueAsync(ConfigType configType, string configValue)
        {
            var config = _unitOfWork.Repository<Config>().Queryable().
                FirstOrDefault(c => c.ConfigType == configType);

            if (config == null)
            {
                config = new Config()
                {
                    // Prepare new config
                    ConfigType = configType,
                    ConfigValue = configValue
                };
                await _configRepository.SaveAsync(config);
            }
            else
            {
                config.ConfigValue = configValue;
                _configRepository.Update(config);
            }
            await _unitOfWork.CompleteAsync();
        }

        #endregion

        #region "Private"

        #endregion
    }
}
