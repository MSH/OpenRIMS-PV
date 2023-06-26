using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
	public class PatientStatus : EntityBase
	{
		public PatientStatus()
		{
			PatientStatusHistories = new HashSet<PatientStatusHistory>();
		}

		public string Description { get; set; }

		public virtual ICollection<PatientStatusHistory> PatientStatusHistories { get; set; }
	}
}