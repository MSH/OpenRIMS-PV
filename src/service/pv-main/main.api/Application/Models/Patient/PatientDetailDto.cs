using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A patient representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class PatientDetailDto : PatientIdentifierDto
    {
        /// <summary>
        /// The first name of the patient
        /// </summary>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// The middle name of the patient
        /// </summary>
        [DataMember]
        public string MiddleName { get; set; }

        /// <summary>
        /// The last name of the patient
        /// </summary>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// The date of birth of the patient
        /// </summary>
        [DataMember]
        public string DateOfBirth { get; set; }

        /// <summary>
        /// The age of the patient
        /// </summary>
        [DataMember]
        public int Age { get; set; }

        /// <summary>
        /// The age group of the patient
        /// </summary>
        [DataMember]
        public string AgeGroup { get; set; }

        /// <summary>
        /// Details of the household creation
        /// </summary>
        [DataMember]
        public string CreatedDetail { get; set; }

        /// <summary>
        /// Details of the last update to the household
        /// </summary>
        [DataMember]
        public string UpdatedDetail { get; set; }

        /// <summary>
        /// The latest encounter date of the patient
        /// </summary>
        [DataMember]
        public string LatestEncounterDate { get; set; }

        /// <summary>
        /// The current status of the patient
        /// </summary>
        [DataMember]
        public string CurrentStatus { get; set; }

        /// <summary>
        /// The medical record number of the patient
        /// </summary>
        [DataMember]
        public string MedicalRecordNumber { get; set; }

        /// <summary>
        /// Primary condition group case numbers for the patient
        /// </summary>
        [DataMember]
        public List<string> CaseNumber { get; set; } = new List<string>();

        /// <summary>
        /// Free format notes for the patient
        /// </summary>
        [DataMember]
        public string Notes { get; set; }

        /// <summary>
        /// A list of custom attributes associated to the patient
        /// </summary>
        [DataMember]
        public ICollection<AttributeValueDto> PatientAttributes { get; set; } = new List<AttributeValueDto>();
    }
}
