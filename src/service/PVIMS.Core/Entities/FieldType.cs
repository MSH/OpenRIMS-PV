using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
	public class FieldType : EntityBase
	{
		public FieldType()
		{
			Fields = new HashSet<Field>();
		}

		public string Description { get; set; }

		public virtual ICollection<Field> Fields { get; set; }
	}
}