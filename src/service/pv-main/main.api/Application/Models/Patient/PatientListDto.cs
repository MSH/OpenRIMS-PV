using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A list representation containing details of a patient
    /// </summary>
    [DataContract()]
    public class PatientListDto
    {
        /// <summary>
        /// Unique identifier for the patient
        /// </summary>
        [DataMember]
        public int PatientId { get; set; }

        /// <summary>
        /// The first name of the patient
        /// </summary>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// The surname of the patient
        /// </summary>
        [DataMember]
        public string Surname { get; set; }

        /// <summary>
        /// full name of the patient
        /// </summary>
        [DataMember]
        public string FullName { get; set; }

        /// <summary>
        /// The name of the facility that the patient belongs to
        /// </summary>
        [DataMember]
        public string FacilityName { get; set; }
    }
}
