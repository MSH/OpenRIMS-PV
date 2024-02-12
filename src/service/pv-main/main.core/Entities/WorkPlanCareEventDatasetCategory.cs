namespace OpenRIMS.PV.Main.Core.Entities
{
	public class WorkPlanCareEventDatasetCategory : EntityBase
	{
		public int DatasetCategoryId { get; set; }
		public int WorkPlanCareEventId { get; set; }

		public virtual DatasetCategory DatasetCategory { get; set; }
		public virtual WorkPlanCareEvent WorkPlanCareEvent { get; set; }
	}
}