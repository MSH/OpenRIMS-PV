using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A representation of a widget list item content
    /// </summary>
    [DataContract()]
    public class WidgetistItemDto 
    {
        /// <summary>
        /// The title of the widget
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// The sub-title of the widget
        /// </summary>
        [DataMember]
        public string SubTitle { get; set; }

        /// <summary>
        /// The page the widget should point to
        /// </summary>
        [DataMember]
        public string ContentPage { get; set; }

        /// <summary>
        /// Widget content
        /// </summary>
        [DataMember]
        public string Content { get; set; }

        /// <summary>
        /// Widget modified date
        /// </summary>
        [DataMember]
        public string Modified { get; set; }
    }
}
