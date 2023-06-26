using PVIMS.Core.ValueTypes;

namespace PVIMS.Core.Entities
{
    public class Config : AuditedEntityBase
    {
        public string ConfigValue { get; set; }

        public virtual ConfigType ConfigType { get; set; }
    }
}