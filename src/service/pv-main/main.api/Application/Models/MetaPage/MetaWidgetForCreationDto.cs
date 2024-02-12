using System.ComponentModel.DataAnnotations;

namespace OpenRIMS.PV.Main.API.Models
{
    public class MetaWidgetForCreationDto
    {
        /// <summary>
        /// The name of the meta widget
        /// </summary>
        [Required]
        [StringLength(50)]
        public string WidgetName { get; set; }

        /// <summary>
        /// The type of widget that is being created
        /// </summary>
        [Required]
        [StringLength(50)]
        public string WidgetType { get; set; }

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
    }
}
