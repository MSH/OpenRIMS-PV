using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A meta form representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class MetaFormDetailDto : MetaFormIdentifierDto
    {
        /// <summary>
        /// Is this a system form
        /// </summary>
        [DataMember]
        public string System { get; set; }

        /// <summary>
        /// The name of the cohort group that the meta form belongs to
        /// </summary>
        [DataMember]
        public string CohortGroup { get; set; }

        /// <summary>
        /// The action (shortened) name of the meta form
        /// </summary>
        [DataMember]
        public string ActionName { get; set; }

        /// <summary>
        /// Current version number of the form
        /// </summary>
        [DataMember]
        public string Version { get; set; }
    }
}
