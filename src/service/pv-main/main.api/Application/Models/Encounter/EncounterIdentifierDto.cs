using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// Am encounter representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class EncounterIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the encounter
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The unique id od the patient
        /// </summary>
        [DataMember]
        public int PatientId { get; set; }

        /// <summary>
        /// The globally unique identifier for the encounter
        /// </summary>
        [DataMember]
        public Guid EncounterGuid { get; set; }

        /// <summary>
        /// The date of the encounter
        /// </summary>
        [DataMember]
        public string EncounterDate { get; set; }
    }
}
