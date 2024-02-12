using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Models.Encounter
{
    /// <summary>
    /// A dto representing the output of an encounter search query
    /// </summary>
    [DataContract()]
    public class SearchEncounterDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique id of the encounter
        /// </summary>
        [DataMember]
        public int EncounterId { get; set; }

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
        /// The type of encounter
        /// </summary>
        [DataMember]
        public string EncounterType { get; set; }

        /// <summary>
        /// The date of the encounter
        /// </summary>
        [DataMember]
        public string EncounterDate { get; set; }
    }
}
