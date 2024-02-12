using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
	public class Priority : EntityBase
	{
		public Priority()
		{
			Encounters = new HashSet<Encounter>();
		}
		
		public string Description { get; set; }

		public virtual ICollection<Encounter> Encounters { get; set; }
	}
}