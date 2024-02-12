using OpenRIMS.PV.Main.Core.Models;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// An encounter representation DTO - EXPANDED DETAILS
    /// </summary>
    [DataContract()]
    public class EncounterExpandedDto : EncounterDetailDto
    {
        /// <summary>
        /// List of conditions associated to the patient
        /// </summary>
        [DataMember]
        public ICollection<PatientConditionDetailDto> PatientConditions { get; set; } = new List<PatientConditionDetailDto>();

        /// <summary>
        /// List of clinical events associated to the patient
        /// </summary>
        [DataMember]
        public ICollection<PatientClinicalEventDetailDto> PatientClinicalEvents { get; set; } = new List<PatientClinicalEventDetailDto>();

        /// <summary>
        /// List of medications associated to the patient
        /// </summary>
        [DataMember]
        public ICollection<PatientMedicationDetailDto> PatientMedications { get; set; } = new List<PatientMedicationDetailDto>();

        /// <summary>
        /// List of lab tests associated to the patient
        /// </summary>
        [DataMember]
        public ICollection<PatientLabTestDetailDto> PatientLabTests { get; set; } = new List<PatientLabTestDetailDto>();

        /// <summary>
        /// List of condition groups the patient belongs to
        /// </summary>
        [DataMember]
        public ICollection<PatientConditionGroupDto> ConditionGroups { get; set; } = new List<PatientConditionGroupDto>();

        /// <summary>
        /// Patient weight history
        /// </summary>
        [DataMember]
        public SeriesValueList[] WeightSeries { get; set; }
    }
}
