namespace OpenRIMS.PV.Main.Core.Entities
{
	public class FieldValue : EntityBase
	{
		public FieldValue()
		{
			Default = true;
			Other = false;
			Unknown = false;
		}

		public string Value { get; set; }
		public bool Default { get; set; }
		public bool Other { get; set; }
		public bool Unknown { get; set; }
		public int FieldId { get; set; }

		public virtual Field Field { get; set; }
	}
}