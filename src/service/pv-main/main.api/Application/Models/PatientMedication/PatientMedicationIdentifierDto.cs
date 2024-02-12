using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A patient medication representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class PatientMedicationIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the patient medication
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The globally unique identifier for the patient medication
        /// </summary>
        [DataMember]
        public Guid PatientMedicationGuid { get; set; }
    }
}
