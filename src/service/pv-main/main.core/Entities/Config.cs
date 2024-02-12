using OpenRIMS.PV.Main.Core.ValueTypes;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public class Config : AuditedEntityBase
    {
        public string ConfigValue { get; set; }

        public virtual ConfigType ConfigType { get; set; }
    }
}