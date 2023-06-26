using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
	public class Language : EntityBase
	{
		public Language()
		{
			PatientLanguages = new HashSet<PatientLanguage>();
		}

		public string Description { get; set; }

		public virtual ICollection<PatientLanguage> PatientLanguages { get; set; }
	}
}