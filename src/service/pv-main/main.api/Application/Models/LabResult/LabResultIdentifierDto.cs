using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A lab result representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class LabResultIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the lab test
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the lab result
        /// </summary>
        [DataMember]
        public string LabResultName { get; set; }

        /// <summary>
        /// Is this lab result currently active
        /// </summary>
        [DataMember]
        public string Active { get; set; }
    }
}
