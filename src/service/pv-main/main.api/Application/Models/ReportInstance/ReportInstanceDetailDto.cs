using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A report instance representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class ReportInstanceDetailDto : ReportInstanceIdentifierDto
    {
        /// <summary>
        /// Details of the source clinical event as described by the patient or reporter
        /// </summary>
        [DataMember]
        public string SourceIdentifier { get; set; }

        /// <summary>
        /// Patient identifier
        /// </summary>
        [DataMember]
        public string PatientIdentifier { get; set; }

        /// <summary>
        /// The classification of the report
        /// </summary>
        [DataMember]
        public string ReportClassification { get; set; }

        /// <summary>
        /// Patient unique identifier (for active reports)
        /// </summary>
        [DataMember]
        public int PatientId { get; set; }

        /// <summary>
        /// Patient clinical event unique identifier (for active reports)
        /// </summary>
        [DataMember]
        public int PatientClinicalEventId { get; set; }

        /// <summary>
        /// The unique identifier of the activity execution status (for E2B attachment)
        /// </summary>
        [DataMember]
        public int ActivityExecutionStatusEventId { get; set; }

        /// <summary>
        /// E2B xml file unique identifier
        /// </summary>
        [DataMember]
        public int AttachmentId { get; set; }

        /// <summary>
        /// Terminology that has been set by the pv specialist
        /// </summary>
        [DataMember]
        public TerminologyMedDraDto TerminologyMedDra { get; set; }

        /// <summary>
        /// Details of the report creation
        /// </summary>
        [DataMember]
        public string CreatedDetail { get; set; }

        /// <summary>
        /// Details of the last update to the report
        /// </summary>
        [DataMember]
        public string UpdatedDetail { get; set; }

        /// <summary>
        /// The qualified name of the current activity of the report
        /// </summary>
        [DataMember]
        public string QualifiedName { get; set; }

        /// <summary>
        /// The status of the current activity of the report
        /// </summary>
        [DataMember]
        public string CurrentStatus { get; set; }

        /// <summary>
        /// The e2b dataset that has been generated for this report
        /// </summary>
        [DataMember]
        public DatasetInstanceDto E2BInstance { get; set; }

        /// <summary>
        /// The number of tasks that have been allocated to this report
        /// </summary>
        [DataMember]
        public int TaskCount { get; set; }

        /// <summary>
        /// The number of tasks that have been allocated to this report that have not been completed or cancelled
        /// </summary>
        [DataMember]
        public int ActiveTaskCount { get; set; }

        /// <summary>
        /// The dataset that has been generated for a spontaneous report
        /// </summary>
        [DataMember]
        public DatasetInstanceDto SpontaneousInstance { get; set; }

        /// <summary>
        /// Medications associated to the reporting instance
        /// </summary>
        [DataMember]
        public ICollection<ReportInstanceMedicationDetailDto> Medications { get; set; } = new List<ReportInstanceMedicationDetailDto>();
    }
}
