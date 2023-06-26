using PVIMS.Core.Exceptions;
using PVIMS.Core.ValueTypes;
using PVIMS.Core.Extensions;
using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
    public class DatasetElementSub : EntityBase
	{
        public DatasetElementSub()
		{
            System = false;

            DatasetInstanceSubValues = new HashSet<DatasetInstanceSubValue>();
            DatasetMappingSubDestinationElements = new HashSet<DatasetMappingSub>();
            DatasetMappingSubSourceElements = new HashSet<DatasetMappingSub>();
            DatasetXmlNodes = new HashSet<DatasetXmlNode>();
        }

        public string ElementName { get; set; }
        public short FieldOrder { get; set; }
        public int DatasetElementId { get; set; }
        public int FieldId { get; set; }
        public string Oid { get; set; }
        public string DefaultValue { get; set; }
        public bool System { get; set; }
        public string FriendlyName { get; set; }
        public string Help { get; set; }

		public virtual DatasetElement DatasetElement { get; set; }
        public virtual Field Field { get; set; }

        public virtual ICollection<DatasetInstanceSubValue> DatasetInstanceSubValues { get; set; }
        public virtual ICollection<DatasetMappingSub> DatasetMappingSubDestinationElements { get; set; }
        public virtual ICollection<DatasetMappingSub> DatasetMappingSubSourceElements { get; set; }
        public virtual ICollection<DatasetXmlNode> DatasetXmlNodes { get; set; }

        public void Validate(string instanceSubValue)
        {
            if (string.IsNullOrWhiteSpace(instanceSubValue))
            {
                if (Field.Mandatory)
                    throw new DatasetFieldSetException(ElementName, string.Format("{0} is required.", ElementName));
                else
                    return;
            }

            var fieldType = (FieldTypes)Field.FieldType.Id;

            switch (fieldType)
            {
                case FieldTypes.AlphaNumericTextbox:
                    if (Field.MaxLength.HasValue)
                    {
                        if (instanceSubValue != null && instanceSubValue.Length > Field.MaxLength.Value)
                            throw new DatasetFieldSetException(ElementName, string.Format("{0} may not contain more than {1} characters.", ElementName, Field.MaxLength.Value));
                    }
                    break;
                case FieldTypes.NumericTextbox:
                    if (!instanceSubValue.IsNumeric())
                    {
                        throw new DatasetFieldSetException(ElementName, string.Format("{0} must be a numeric value.", ElementName));
                    }

                    var decimalValue = decimal.Parse(instanceSubValue);


                    if (Field.MinSize.HasValue)
                    {
                        if (decimalValue < Field.MinSize.Value)
                            throw new DatasetFieldSetException(ElementName, string.Format("{0} may not be less than {1}.", ElementName, Field.MinSize.Value));
                    }

                    if (Field.MaxSize.HasValue)
                    {
                        if (decimalValue > Field.MaxSize.Value)
                            throw new DatasetFieldSetException(ElementName, string.Format("{0} may not be more than {1}.", ElementName, Field.MaxSize.Value));
                    }
                    break;
                case FieldTypes.Date:
                    break;
                default:
                    break;
            }
        }
    }
}