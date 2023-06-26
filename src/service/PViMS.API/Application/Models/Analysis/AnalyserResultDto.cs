using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A dto representing the list of signals per medication over a reporting period
    /// </summary>
    [DataContract()]
    public class AnalyserResultDto
    {
        /// <summary>
        /// The analysis result medication
        /// </summary>
        [DataMember]
        public string Medication { get; set; }

        /// <summary>
        /// The unique identifier of the medication
        /// </summary>
        [DataMember]
        public int MedicationId { get; set; }

        /// <summary>
        /// The exposed incidence rate
        /// </summary>
        [DataMember]
        public IncidenceRateDto ExposedIncidenceRate { get; set; }

        /// <summary>
        /// The non-exposed incidence rate
        /// </summary>
        [DataMember]
        public IncidenceRateDto NonExposedIncidenceRate { get; set; }

        /// <summary>
        /// The unadjusted relative risk rate
        /// </summary>
        [DataMember]
        public decimal UnadjustedRelativeRisk { get; set; }

        /// <summary>
        /// The adjusted relative risk rate
        /// </summary>
        [DataMember]
        public decimal AdjustedRelativeRisk { get; set; }

        /// <summary>
        /// Confidence interval lower limit
        /// </summary>
        [DataMember]
        public decimal ConfidenceIntervalLow { get; set; }

        /// <summary>
        /// Confidence interval upper limit
        /// </summary>
        [DataMember]
        public decimal ConfidenceIntervalHigh { get; set; }
    }
}
