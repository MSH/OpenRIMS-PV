using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A dto representing the output for an outstanding visit report
    /// </summary>
    [DataContract()]
    public class OutstandingVisitReportDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique identifier for the patient
        /// </summary>
        [DataMember]
        public long PatientId { get; set; }

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
        /// The facility that made the appointment
        /// </summary>
        [DataMember]
        public string Facility { get; set; }

        /// <summary>
        /// The date of the appointment
        /// </summary>
        [DataMember]
        public string AppointmentDate { get; set; }
    }

}
