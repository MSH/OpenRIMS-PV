using PVIMS.Core.Aggregates.ConceptAggregate;
using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
	public class MedicationForm : EntityBase
	{
		public MedicationForm()
		{
			Concepts = new HashSet<Concept>();
		}

		public string Description { get; set; }

		public virtual ICollection<Concept> Concepts { get; set; }
	}
}