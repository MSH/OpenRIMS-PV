using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.API.Models.ValueTypes;
using System.ComponentModel.DataAnnotations;

namespace OpenRIMS.PV.Main.API.Models
{
    public class DatasetCategoryForUpdateDto
    {
        /// <summary>
        /// The name of the dataset category
        /// </summary>
        [Required]
        [StringLength(50)]
        public string DatasetCategoryName { get; set; }

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
