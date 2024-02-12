using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
	public class WorkPlanCareEvent : EntityBase
	{
		public WorkPlanCareEvent()
		{
			WorkPlanCareEventDatasetCategories = new HashSet<WorkPlanCareEventDatasetCategory>();
		}

		public short Order { get; set; }
		public bool Active { get; set; }
		public int CareEventId { get; set; }
		public int WorkPlanId { get; set; }

		public virtual CareEvent CareEvent { get; set; }
		public virtual WorkPlan WorkPlan { get; set; }

		public virtual ICollection<WorkPlanCareEventDatasetCategory> WorkPlanCareEventDatasetCategories { get; set; }
	}
}