using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A representation of a widget sub item content
    /// </summary>
    public class WidgetSubItemForUpdateDto 
    {
        /// <summary>
        /// The title of the widget
        /// </summary>
        [Required] 
        public string Title { get; set; }

        /// <summary>
        /// The sub-title of the widget
        /// </summary>
        public string SubTitle { get; set; }

        /// <summary>
        /// The page the widget should point to
        /// </summary>
        [Required]
        public string ContentPage { get; set; }
    }
}
