using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.ValueTypes;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.Core.Services
{
    public interface IInfrastructureService
    {
        bool HasAssociatedData(DatasetElement element);
        DatasetElement GetTerminologyMedDra();
        Config GetOrCreateConfig(ConfigType configType);
        Task SetConfigValueAsync(ConfigType configType, string configValue);
    }
}
