using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
	public class DatasetElementType : EntityBase
	{
		public DatasetElementType()
		{
			DatasetElements = new HashSet<DatasetElement>();
		}

		public string Description { get; set; }

		public virtual ICollection<DatasetElement> DatasetElements { get; set; }
	}
}