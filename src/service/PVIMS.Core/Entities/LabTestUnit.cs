using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
	public class LabTestUnit : EntityBase
	{
        public LabTestUnit()
        {
            PatientLabTests = new HashSet<PatientLabTest>();
        }

        public string Description { get; set; }

        public virtual ICollection<PatientLabTest> PatientLabTests { get; set; }
    }
}