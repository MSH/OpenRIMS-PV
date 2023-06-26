using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
	public class EncounterType : EntityBase
	{
		public EncounterType() 
		{
			Encounters = new HashSet<Encounter>();
			EncounterTypeWorkPlans = new HashSet<EncounterTypeWorkPlan>();
		}

		public string Description { get; set; }
		public string Help { get; set; }
		public bool Chronic { get; set; }

		public virtual ICollection<Encounter> Encounters { get; set; }
		public virtual ICollection<EncounterTypeWorkPlan> EncounterTypeWorkPlans { get; set; }
	}
}