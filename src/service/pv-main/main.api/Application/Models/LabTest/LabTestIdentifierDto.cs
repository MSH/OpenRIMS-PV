using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A lab test representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class LabTestIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the lab test
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the lab test
        /// </summary>
        [DataMember]
        public string LabTestName { get; set; }

        /// <summary>
        /// Is this lab test currently active
        /// </summary>
        [DataMember]
        public string Active { get; set; }
    }
}
