using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A patient representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class PatientIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the patient
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The globally unique identifier for the patient
        /// </summary>
        [DataMember]
        public Guid PatientGuid { get; set; }

        /// <summary>
        /// The current facility of the patient
        /// </summary>
        [DataMember]
        public string FacilityName { get; set; }

        /// <summary>
        /// The organisation unit that the facility is allocated to
        /// </summary>
        [DataMember]
        public string OrganisationUnit { get; set; }

    }
}
