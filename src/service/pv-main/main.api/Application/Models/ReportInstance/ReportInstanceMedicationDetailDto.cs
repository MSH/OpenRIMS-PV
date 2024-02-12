using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A report instance medication representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class ReportInstanceMedicationDetailDto : ReportInstanceMedicationIdentifierDto
    {
        /// <summary>
        /// Medication identification details
        /// </summary>
        [DataMember]
        public string MedicationIdentifier { get; set; }

        /// <summary>
        /// Naranjo causality assessment for medication
        /// </summary>
        [DataMember]
        public string NaranjoCausality { get; set; }

        /// <summary>
        /// WHO causality assessment for medication
        /// </summary>
        [DataMember]
        public string WhoCausality { get; set; }

        /// <summary>
        /// The start date of the medication
        /// </summary>
        [DataMember]
        public string StartDate { get; set; }

        /// <summary>
        /// The end date of the medication
        /// </summary>
        [DataMember]
        public string EndDate { get; set; }
    }
}
