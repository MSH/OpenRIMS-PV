using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A representation of a widget list item content
    /// </summary>
    public class WidgetListItemForUpdateDto
    {
        /// <summary>
        /// The title of the widget
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Widget content
        /// </summary>
        [Required]
        public string Content { get; set; }
    }
}
