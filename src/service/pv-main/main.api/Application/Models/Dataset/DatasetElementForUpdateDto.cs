using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.API.Models.ValueTypes;
using OpenRIMS.PV.Main.Core.ValueTypes;
using System.ComponentModel.DataAnnotations;

namespace OpenRIMS.PV.Main.API.Models
{
    public class DatasetElementForUpdateDto
    {
        /// <summary>
        /// The name of the dataset element
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ElementName { get; set; }

        /// <summary>
        /// The E2B OID for the element
        /// </summary>
        [StringLength(50)]
        public string OID { get; set; }

        /// <summary>
        /// The default value for the element
        /// </summary>
        [StringLength(150)]
        public string DefaultValue { get; set; }

        /// <summary>
        /// The type of field the element is implementing
        /// </summary>
        [ValidEnumValue]
        public YesNoValueType Mandatory { get; set; }

        /// <summary>
        /// The type of field the element is implementing
        /// </summary>
        [ValidEnumValue]
        public YesNoValueType Anonymise { get; set; }

        /// <summary>
        /// Is this a system defined variable
        /// </summary>
        [ValidEnumValue]
        public YesNoValueType System { get; set; }

        /// <summary>
        /// Single dataset rule
        /// </summary>
        [ValidEnumValue]
        public YesNoValueType SingleDatasetRule { get; set; }

        /// <summary>
        /// The type of field the element is implementing
        /// </summary>
        [ValidEnumValue]
        public FieldTypes FieldTypeName { get; set; }

        /// <summary>
        /// For string based elements, what is the maximum length of this element
        /// </summary>
        public short? MaxLength { get; set; }

        /// <summary>
        /// For numeric based elements, how many decimals should be allowed
        /// </summary>
        public short? Decimals { get; set; }

        /// <summary>
        /// For numeric based elements, the upper range for the element
        /// </summary>
        public decimal? MaxSize { get; set; }

        /// <summary>
        /// For numeric based elements, the lower range for the element
        /// </summary>
        public decimal? MinSize { get; set; }
    }
}
