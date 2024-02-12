namespace OpenRIMS.PV.Main.Core.Entities
{
    public class DatasetCategoryElementCondition : EntityBase
    {
        public int ConditionId { get; set; }
        public int DatasetCategoryElementId { get; set; }

        public virtual DatasetCategoryElement DatasetCategoryElement { get; set; }
        public virtual Condition Condition { get; set; }
    }
}