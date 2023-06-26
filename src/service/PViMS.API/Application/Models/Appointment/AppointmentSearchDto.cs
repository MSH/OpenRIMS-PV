using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// An appointment representation DTO - SEARCH PURPOSES
    /// </summary>
    [DataContract()]
    public class AppointmentSearchDto : AppointmentIdentifierDto
    {
        /// <summary>
        /// The unique Id of the patient
        /// </summary>
        [DataMember]
        public int PatientId { get; set; }

        /// <summary>
        /// The first name of the patient
        /// </summary>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the patient
        /// </summary>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// The current facility of the patient
        /// </summary>
        [DataMember]
        public string FacilityName { get; set; }

        /// <summary>
        /// The reason for the appointment
        /// </summary>
        [DataMember]
        public string Reason { get; set; }

        /// <summary>
        /// The current status of the appointment
        /// </summary>
        [DataMember]
        public string AppointmentStatus { get; set; }

        /// <summary>
        /// The unique Id of the encounter if an encounter has been created
        /// </summary>
        [DataMember]
        public int? EncounterId { get; set; }
    }
}
