using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Models.ValueTypes;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class DatasetCategoryElementForUpdateDto
    {
        /// <summary>
        /// The unique identifier of the element being linked
        /// </summary>
        [Required]
        public long DatasetElementId { get; set; }

        /// <summary>
        /// The friendly name of the dataset category
        /// </summary>
        [StringLength(150)]
        public string FriendlyName { get; set; }

        /// <summary>
        /// Additional help for the dataset category
        /// </summary>
        [StringLength(350)]
        public string Help { get; set; }

        /// <summary>
        /// Is this a category used for acute data collection
        /// </summary>
        [ValidEnumValue]
        public YesNoValueType Acute { get; set; }

        /// <summary>
        /// Is this a category used for chronic data collection
        /// </summary>
        [ValidEnumValue]
        public YesNoValueType Chronic { get; set; }
    }
}
