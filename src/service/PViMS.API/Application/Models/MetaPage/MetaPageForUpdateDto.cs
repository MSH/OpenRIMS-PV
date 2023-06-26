using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Models.ValueTypes;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class MetaPageForUpdateDto
    {
        /// <summary>
        /// The name of the meta page
        /// </summary>
        [Required]
        [StringLength(50)]
        public string PageName { get; set; }

        /// <summary>
        /// What the purpose of the page is
        /// </summary>
        [StringLength(250)]
        public string PageDefinition { get; set; }

        /// <summary>
        /// Page configuration details. Currently not in use
        /// </summary>
        public string MetaDefinition { get; set; }

        /// <summary>
        /// Page breadcrumb. Currently not in use
        /// </summary>
        [StringLength(250)]
        public string Breadcrumb { get; set; }

        /// <summary>
        /// Is this page visible to the menu
        /// </summary>
        [Required]
        [ValidEnumValue]
        public YesNoValueType Visible { get; set; }
    }
}
