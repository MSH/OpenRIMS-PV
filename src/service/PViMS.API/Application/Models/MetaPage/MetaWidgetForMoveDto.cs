using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class MetaWidgetForMoveDto
    {
        /// <summary>
        /// The unique identifier of the destination meta page
        /// </summary>
        [Required]
        public long DestinationMetaPageId { get; set; }
    }
}
