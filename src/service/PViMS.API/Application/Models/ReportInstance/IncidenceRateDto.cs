using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A dto representing the incidence rate for a medication (exposed or non-exposed)
    /// </summary>
    [DataContract()]
    public class IncidenceRateDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The number of cases for the population set
        /// </summary>
        [DataMember]
        public int Cases { get; set; }

        /// <summary>
        /// The number of non cases for the population set
        /// </summary>
        [DataMember]
        public int NonCases { get; set; }

        /// <summary>
        /// The total population for the population set
        /// </summary>
        [DataMember]
        public decimal Population { get; set; }

        /// <summary>
        /// The incidence rate for the population set
        /// </summary>
        [DataMember]
        public decimal IncidenceRate { get; set; }
    }
}
