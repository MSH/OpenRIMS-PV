using PVIMS.API.Application.Queries.ReportInstanceAggregate;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A patient representation DTO - EXPANDED DETAILS
    /// </summary>
    [DataContract()]
    public class PatientExpandedDto : PatientDetailDto
    {
        /// <summary>
        /// A list of appointments associated to the patient
        /// </summary>
        [DataMember]
        public ICollection<AppointmentDetailDto> Appointments { get; set; } = new List<AppointmentDetailDto>();

        /// <summary>
        /// A list of attachments associated to the patient
        /// </summary>
        [DataMember]
        public ICollection<AttachmentDetailDto> Attachments { get; set; } = new List<AttachmentDetailDto>();

        /// <summary>
        /// A list of status changes associated to the patient
        /// </summary>
        [DataMember]
        public ICollection<PatientStatusDto> PatientStatusHistories { get; set; } = new List<PatientStatusDto>();

        /// <summary>
        /// A list of status changes associated to the patient
        /// </summary>
        [DataMember]
        public ICollection<EncounterDetailDto> Encounters { get; set; } = new List<EncounterDetailDto>();

        /// <summary>
        /// List of conhort groups with associated enrolment if relevant
        /// </summary>
        [DataMember]
        public ICollection<CohortGroupPatientDetailDto> CohortGroups { get; set; } = new List<CohortGroupPatientDetailDto>();

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
        /// Activity per clinical event
        /// </summary>
        [DataMember]
        public ICollection<ReportInstanceEventDto> Activity { get; set; } = new List<ReportInstanceEventDto>();
    }
}
