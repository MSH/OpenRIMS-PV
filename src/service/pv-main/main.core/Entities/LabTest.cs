using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
	public class LabTest : EntityBase
	{
		public LabTest()
		{
			Active = true;

			ConditionLabTests = new HashSet<ConditionLabTest>();
			PatientLabTests = new HashSet<PatientLabTest>();
		}

		public bool Active { get; set; }
		public string Description { get; set; }

		public virtual ICollection<ConditionLabTest> ConditionLabTests { get; set; }
		public virtual ICollection<PatientLabTest> PatientLabTests { get; set; }
	}
}