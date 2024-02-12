namespace OpenRIMS.PV.Main.Core.Entities
{
    public class ConditionLabTest : EntityBase
    {
        public int ConditionId { get; set; }
        public int LabTestId { get; set; }

        public virtual Condition Condition { get; set; }
        public virtual LabTest LabTest { get; set; }
    }
}