using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A meta widget representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class MetaWidgetDetailDto : MetaWidgetIdentifierDto
    {
        /// <summary>
        /// The definition of the widget
        /// </summary>
        [DataMember]
        public string WidgetDefinition { get; set; }

        /// <summary>
        /// Widget content
        /// </summary>
        [DataMember]
        public string Content { get; set; }

        /// <summary>
        /// The type of widget
        /// </summary>
        [DataMember]
        public string WidgetType { get; set; }

        /// <summary>
        /// The location of the widget
        /// </summary>
        [DataMember]
        public string WidgetLocation { get; set; }

        /// <summary>
        /// The status of the widget
        /// </summary>
        [DataMember]
        public string WidgetStatus { get; set; }

        /// <summary>
        /// Widget icon
        /// </summary>
        [DataMember]
        public string Icon { get; set; }

        /// <summary>
        /// A list of widgets associated to the page
        /// </summary>
        [DataMember]
        public ICollection<WidgetistItemDto> ContentItems { get; set; } = new List<WidgetistItemDto>();

    }
}
