using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Models.Patient
{
    /// <summary>
    /// A dto representing the output of a patient search query
    /// </summary>
    [DataContract()]
    public class PatientSearchDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique id of the patient
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
        /// The currrent facility of the patient
        /// </summary>
        [DataMember]
        public string FacilityName { get; set; }

        /// <summary>
        /// The date of birth of the patient
        /// </summary>
        [DataMember]
        public string DateOfBirth { get; set; }

        /// <summary>
        /// The total number of patients for grade 3
        /// </summary>
        [DataMember]
        public string Age { get; set; }

        /// <summary>
        /// The total number of patients for grade 4
        /// </summary>
        [DataMember]
        public string LatestEncounterDate { get; set; }
    }
}
