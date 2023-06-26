using System.Collections.Generic;
using System.Linq;

namespace PVIMS.Core.Entities
{
	public class Field : EntityBase
	{
		public Field()
		{
			DatasetElements = new HashSet<DatasetElement>();
			DatasetElementSubs = new HashSet<DatasetElementSub>();
			FieldValues = new HashSet<FieldValue>();
		}

		public bool Mandatory { get; set; }
		public short? MaxLength { get; set; }
		public string RegEx { get; set; }
		public short? Decimals { get; set; }
		public decimal? MaxSize { get; set; }
		public decimal? MinSize { get; set; }
		public string Calculation { get; set; }
		public byte[] Image { get; set; }
		public short? FileSize { get; set; }
		public string FileExt { get; set; }
		public bool Anonymise { get; set; }
		public int FieldTypeId { get; set; }

		public virtual FieldType FieldType { get; set; }

		public virtual ICollection<DatasetElement> DatasetElements { get; set; }
		public virtual ICollection<DatasetElementSub> DatasetElementSubs { get; set; }
		public virtual ICollection<FieldValue> FieldValues { get; set; }

        public bool HasValue(string value)
        {
            return FieldValues.Any(fv => fv.Value.Trim().ToLower() == value.Trim().ToLower());
        }

        public bool HasDefaultValue(FieldValue editValue)
        {
            return FieldValues.Any(fv => fv.Id != editValue.Id && fv.Default);
        }

        public bool HasOtherValue(FieldValue editValue)
        {
            return FieldValues.Any(fv => fv.Id != editValue.Id && fv.Other);
        }

        public bool HasUnknownValue(FieldValue editValue)
        {
            return FieldValues.Any(fv => fv.Id != editValue.Id && fv.Unknown);
        }
	}
}