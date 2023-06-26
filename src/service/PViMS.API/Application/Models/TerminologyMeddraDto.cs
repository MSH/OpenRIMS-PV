using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A dto representing a MedDRA terminology value
    /// </summary>
    [DataContract()]
    public class TerminologyMedDraDto
    {
        /// <summary>
        /// The meddra term
        /// </summary>
        [DataMember]
        public string MedDraTerm { get; set; }

        /// <summary>
        /// The meddra code
        /// </summary>
        [DataMember]
        public string MedDraCode { get; set; }

        /// <summary>
        /// The type of meddra term
        /// </summary>
        [DataMember]
        public string MedDraTermType { get; set; }
    }
}
