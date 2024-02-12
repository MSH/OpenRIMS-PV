using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// An outcome representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class OutcomeIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the outcome
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the outcome
        /// </summary>
        [DataMember]
        public string OutcomeName { get; set; }
    }
}
