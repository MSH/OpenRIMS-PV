using PVIMS.Core.Aggregates.DatasetAggregate;
using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
	public class WorkPlan : EntityBase
	{
		public WorkPlan()
		{
			EncounterTypeWorkPlans = new HashSet<EncounterTypeWorkPlan>();
			WorkPlanCareEvents = new HashSet<WorkPlanCareEvent>();
		}

		public string Description { get; set; }
		public int? DatasetId { get; set; }

		public virtual Dataset Dataset { get; set; }

		public virtual ICollection<EncounterTypeWorkPlan> EncounterTypeWorkPlans { get; set; }
		public virtual ICollection<WorkPlanCareEvent> WorkPlanCareEvents { get; set; }
	}
}