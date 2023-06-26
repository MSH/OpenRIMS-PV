using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;
using System.Threading.Tasks;

namespace PVIMS.Core.Services
{
    public interface IInfrastructureService
    {
        bool HasAssociatedData(DatasetElement element);
        DatasetElement GetTerminologyMedDra();
        Config GetOrCreateConfig(ConfigType configType);
        Task SetConfigValueAsync(ConfigType configType, string configValue);
    }
}
