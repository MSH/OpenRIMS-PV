namespace PVIMS.Core.Entities
{
    public class DatasetCategoryCondition : EntityBase
    {
        public int ConditionId { get; set; }
        public int DatasetCategoryId { get; set; }

        public virtual DatasetCategory DatasetCategory { get; set; }
        public virtual Condition Condition { get; set; }
    }
}