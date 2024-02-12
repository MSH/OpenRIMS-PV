using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A treatment outcome representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class TreatmentOutcomeIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the treatment outcome
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the treatment outcome
        /// </summary>
        [DataMember]
        public string TreatmentOutcomeName { get; set; }
    }
}
