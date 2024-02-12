using OpenRIMS.PV.Main.Core.Aggregates.DatasetAggregate;
using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
	public class EncounterTypeWorkPlan : EntityBase
	{
		public EncounterTypeWorkPlan()
		{
			DatasetInstances = new HashSet<DatasetInstance>();
			Datasets = new HashSet<Dataset>();
		}

		public int? CohortGroupId { get; set; }
		public int EncounterTypeId { get; set; }
		public int WorkPlanId { get; set; }

		public virtual CohortGroup CohortGroup { get; set; }
		public virtual EncounterType EncounterType { get; set; }
		public virtual WorkPlan WorkPlan { get; set; }

		public virtual ICollection<DatasetInstance> DatasetInstances { get; set; }
		public virtual ICollection<Dataset> Datasets { get; set; }
	}
}