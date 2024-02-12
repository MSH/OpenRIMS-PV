using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A lab test unit representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class LabTestUnitIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the lab test unit
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the lab test unit
        /// </summary>
        [DataMember]
        public string LabTestUnitName { get; set; }
    }
}
