using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// An appointment representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class AppointmentIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the appointment
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The date of the appointment
        /// </summary>
        [DataMember]
        public string AppointmentDate { get; set; }
    }
}
