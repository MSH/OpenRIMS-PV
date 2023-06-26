using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A meta page representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class MetaPageDetailDto : MetaPageIdentifierDto
    {
        /// <summary>
        /// The definition of the page
        /// </summary>
        [DataMember]
        public string PageDefinition { get; set; }

        /// <summary>
        /// The meta definition of the page
        /// </summary>
        [DataMember]
        public string MetaDefinition { get; set; }

        /// <summary>
        /// The page breadcrumb
        /// </summary>
        [DataMember]
        public string Breadcrumb { get; set; }

        /// <summary>
        /// Is this a system page
        /// </summary>
        [DataMember]
        public string System { get; set; }

        /// <summary>
        /// Is this page visible
        /// </summary>
        [DataMember]
        public string Visible { get; set; }
    }
}
