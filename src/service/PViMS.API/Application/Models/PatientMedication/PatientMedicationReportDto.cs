using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A dto representing the output for a patient on medication report
    /// </summary>
    [DataContract()]
    public class PatientMedicationReportDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the concept
        /// </summary>
        [DataMember]
        public int ConceptId { get; set; }

        /// <summary>
        /// The name of the concept
        /// </summary>
        [DataMember]
        public string ConceptName { get; set; }

        /// <summary>
        /// The total number of patients on this medication
        /// </summary>
        [DataMember]
        public int PatientCount { get; set; }

        /// <summary>
        /// List of patients on the current medication
        /// </summary>
        [DataMember]
        public List<PatientListDto> Patients { get; set; }
    }
}
