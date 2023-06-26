using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A dto representing the list of patients contributing to the reporting period
    /// </summary>
    [DataContract()]
    public class AnalyserPatientDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The full name of the patient
        /// </summary>
        [DataMember]
        public string PatientName { get; set; }

        /// <summary>
        /// The medication that the patient was on
        /// </summary>
        [DataMember]
        public string Medication { get; set; }

        /// <summary>
        /// The start date of the contribution
        /// </summary>
        [DataMember]
        public string StartDate { get; set; }

        /// <summary>
        /// The finish date of the contribution
        /// </summary>
        [DataMember]
        public string FinishDate { get; set; }

        /// <summary>
        /// The number of days contributed by the patient to the analysis period
        /// </summary>
        [DataMember]
        public int DaysContributed { get; set; }

        /// <summary>
        /// Did the patient experience this adverse drug reaction
        /// </summary>
        [DataMember]
        public string ExperiencedReaction { get; set; }

        /// <summary>
        /// Any risk factors included as part of the criteria
        /// </summary>
        [DataMember]
        public string RiskFactor { get; set; }

        /// <summary>
        /// Risk factor stratification
        /// </summary>
        [DataMember]
        public string RiskFactorOption { get; set; }

        /// <summary>
        /// Does the patient meet this risk factor criteria
        /// </summary>
        [DataMember]
        public string FactorMet { get; set; }
    }
}
