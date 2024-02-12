using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A risk factor representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class RiskFactorIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the risk factor
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the risk factor
        /// </summary>
        [DataMember]
        public string FactorName { get; set; }

        /// <summary>
        /// Is this risk factor currently active
        /// </summary>
        [DataMember]
        public string Active { get; set; }
    }
}
