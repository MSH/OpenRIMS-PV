using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
	public class TreatmentOutcome : EntityBase
	{
        public TreatmentOutcome()
        {
            PatientConditions = new HashSet<PatientCondition>();
        }

        public string Description { get; set; }

        public virtual ICollection<PatientCondition> PatientConditions { get; set; }
    }
}