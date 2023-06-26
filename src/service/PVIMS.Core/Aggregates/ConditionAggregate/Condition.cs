using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
    public class Condition : EntityBase
	{
		public Condition()
		{
            Active = true;
            Chronic = false;

            CohortGroups = new HashSet<CohortGroup>();
            ConditionLabTests = new HashSet<ConditionLabTest>();
            ConditionMedications = new HashSet<ConditionMedication>();
            ConditionMedDras = new HashSet<ConditionMedDra>();
            DatasetCategoryConditions = new HashSet<DatasetCategoryCondition>();
            DatasetCategoryElementConditions = new HashSet<DatasetCategoryElementCondition>();
            PatientConditions = new HashSet<PatientCondition>();
		}

        public string Description { get; set; }
        public bool Chronic { get; set; }
        public bool Active { get; set; }

        public virtual ICollection<CohortGroup> CohortGroups { get; set; }
        public virtual ICollection<ConditionLabTest> ConditionLabTests { get; set; }
        public virtual ICollection<ConditionMedication> ConditionMedications { get; set; }
        public virtual ICollection<ConditionMedDra> ConditionMedDras { get; set; }
        public virtual ICollection<DatasetCategoryCondition> DatasetCategoryConditions { get; set; }
        public virtual ICollection<DatasetCategoryElementCondition> DatasetCategoryElementConditions { get; set; }
        public virtual ICollection<PatientCondition> PatientConditions { get; set; }

        public bool HasMedDra(List<TerminologyMedDra> meddras)
        {
            if (ConditionMedDras.Count == 0)
            {
                return false;
            }
            else
            {
                foreach (var cm in ConditionMedDras)
                {
                    if (meddras.Contains(cm.TerminologyMedDra)) {
                        return true;
                    }
                }
                return false;
            }
        }
	}
}