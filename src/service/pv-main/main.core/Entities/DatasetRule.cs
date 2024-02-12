using OpenRIMS.PV.Main.Core.Aggregates.DatasetAggregate;
using OpenRIMS.PV.Main.Core.ValueTypes;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public partial class DatasetRule : EntityBase
    {
        public DatasetRule()
        {
            RuleActive = true;
        }

        public DatasetRuleType RuleType { get; set; }
        public bool RuleActive { get; set; }
        public int? DatasetId { get; set; }
        public int? DatasetElementId { get; set; }

        public virtual Dataset Dataset { get; set; }
        public virtual DatasetElement DatasetElement { get; set; }
    }
}
