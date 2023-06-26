using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
	public class TerminologyMedDra : EntityBase
	{
		public TerminologyMedDra()
		{
            Children = new HashSet<TerminologyMedDra>();
            ConditionMedDras = new HashSet<ConditionMedDra>();
            PatientClinicalEvents = new HashSet<PatientClinicalEvent>();
            PatientConditions = new HashSet<PatientCondition>();
            ReportInstances = new HashSet<ReportInstance>();
            Scales = new HashSet<MedDRAScale>();

            Common = false;
		}

        public string MedDraTerm { get; set; }
        public string MedDraCode { get; set; }
        public string MedDraTermType { get; set; }
        public int? ParentId { get; set; }
        public string MedDraVersion { get; set; }
        public bool Common { get; set; }

        public virtual TerminologyMedDra Parent { get; set; }

		public virtual ICollection<TerminologyMedDra> Children { get; set; }
        public virtual ICollection<ConditionMedDra> ConditionMedDras { get; set; }
        public virtual ICollection<PatientClinicalEvent> PatientClinicalEvents { get; set; }
        public virtual ICollection<PatientCondition> PatientConditions { get; set; }
        public virtual ICollection<ReportInstance> ReportInstances { get; set; }
        public virtual ICollection<MedDRAScale> Scales { get; set; }

        public string DisplayName
        {
            get
            {
                return MedDraTerm;
            }
        }
	}
}