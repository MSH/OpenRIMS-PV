namespace OpenRIMS.PV.Main.Core.Entities
{
	public class LabResult : EntityBase
	{
		public LabResult()
		{
			Active = true;
		}

		public string Description { get; set; }
		public bool Active { get; set; }
	}
}