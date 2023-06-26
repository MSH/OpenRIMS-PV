using PVIMS.Core.Aggregates.DatasetAggregate;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Core.Entities
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
