using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
	public class FacilityType : EntityBase
	{
		public FacilityType()
		{
			Facilities = new HashSet<Facility>();
		}

		public string Description { get; set; }

		public virtual ICollection<Facility> Facilities { get; set; }
	}
}