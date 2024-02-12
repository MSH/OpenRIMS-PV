using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.API.Models.ValueTypes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpenRIMS.PV.Main.API.Models
{
    public class ConditionForUpdateDto
    {
        /// <summary>
        /// The name of the condition group
        /// </summary>
        [Required]
        [StringLength(50)]
        public string ConditionName { get; set; }

        /// <summary>
        /// Is the condition group for a chronic condition
        /// </summary>
        [Required]
        [ValidEnumValue]
        public YesNoValueType Chronic { get; set; }

        /// <summary>
        /// Is the condition group active
        /// </summary>
        [Required]
        [ValidEnumValue]
        public YesNoValueType Active { get; set; }

        /// <summary>
        /// List of lab tests unique identifiers
        /// </summary>
        [Required]
        public List<int> ConditionLabTests { get; set; }

        /// <summary>
        /// List of meddra unique identifiers
        /// </summary>
        [Required]
        public List<int> ConditionMedDras { get; set; }

        /// <summary>
        /// List of product identifiers
        /// </summary>
        [Required]
        public List<int> ConditionMedications { get; set; }

    }
}
