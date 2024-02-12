using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A meddra term representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class MeddraTermIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the meddra term
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the term
        /// </summary>
        [DataMember]
        public string MedDraTerm { get; set; }

        /// <summary>
        /// The code of the term
        /// </summary>
        [DataMember]
        public string MedDraCode { get; set; }
    }
}
