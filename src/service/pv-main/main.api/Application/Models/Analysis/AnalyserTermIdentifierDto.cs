using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A dto representing the meddra terms available for analysis over an analysis period
    /// </summary>
    [DataContract()]
    public class AnalyserTermIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique identifer for the term
        /// </summary>
        [DataMember]
        public int TerminologyMeddraId { get; set; }

        /// <summary>
        /// The name of the term
        /// </summary>
        [DataMember]
        public string MeddraTerm { get; set; }
    }
}
