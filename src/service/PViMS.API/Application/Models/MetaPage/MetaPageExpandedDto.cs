using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A meta page representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class MetaPageExpandedDto : MetaPageDetailDto
    {
        /// <summary>
        /// A list of widgets associated to the page that are not published
        /// </summary>
        [DataMember]
        public ICollection<MetaWidgetIdentifierDto> UnpublishedWidgets { get; set; } = new List<MetaWidgetIdentifierDto>();

        /// <summary>
        /// A list of widgets associated to the page
        /// </summary>
        [DataMember]
        public ICollection<MetaWidgetDetailDto> Widgets { get; set; } = new List<MetaWidgetDetailDto>();
    }
}
