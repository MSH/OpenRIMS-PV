using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
	public class Outcome : EntityBase
	{
        public Outcome()
        {
            PatientConditions = new HashSet<PatientCondition>();
        }

        public string Description { get; set; }

        public virtual ICollection<PatientCondition> PatientConditions { get; set; }
    }
}