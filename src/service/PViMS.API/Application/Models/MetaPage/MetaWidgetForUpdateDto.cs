using PVIMS.API.Infrastructure.Attributes;
using PVIMS.Core.ValueTypes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class MetaWidgetForUpdateDto
    {
        /// <summary>
        /// The name of the meta widget
        /// </summary>
        [Required]
        [StringLength(50)]
        public string WidgetName { get; set; }

        /// <summary>
        /// What the purpose of the widget is
        /// </summary>
        [StringLength(250)]
        public string WidgetDefinition { get; set; }

        /// <summary>
        /// The icon that is associated to the widget
        /// </summary>
        [StringLength(50)]
        public string Icon { get; set; }

        /// <summary>
        /// The location of the widget
        /// </summary>
        [Required]
        [ValidEnumValue]
        public MetaWidgetLocation WidgetLocation { get; set; }

        /// <summary>
        /// The status of the widget
        /// </summary>
        [Required]
        [ValidEnumValue]
        public MetaWidgetStatus WidgetStatus { get; set; }

        /// <summary>
        /// Text associated to the general widget type
        /// </summary>
        public string GeneralContent { get; set; }

        /// <summary>
        /// Sub item content for the sub item widget type
        /// </summary>
        public ICollection<WidgetSubItemForUpdateDto> SubItems { get; set; } = new List<WidgetSubItemForUpdateDto>();

        /// <summary>
        /// Sub item content for the sub item widget type
        /// </summary>
        public ICollection<WidgetListItemForUpdateDto> ListItems { get; set; } = new List<WidgetListItemForUpdateDto>();
    }
}
