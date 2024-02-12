using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A patient medication representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class PatientMedicationDetailDto : PatientMedicationIdentifierDto
    {
        /// <summary>
        /// The source description of the medication
        /// </summary>
        [DataMember]
        public string SourceDescription { get; set; }

        /// <summary>
        /// The unqiue identifier of the medication concept
        /// </summary>
        [DataMember]
        public long ConceptId { get; set; }

        /// <summary>
        /// The unqiue identifier of the medication product
        /// </summary>
        [DataMember]
        public long? ProductId { get; set; }

        /// <summary>
        /// The display name for the medication (based on concept and product)
        /// </summary>
        [DataMember]
        public string Medication { get; set; }

        /// <summary>
        /// The dose of the medication
        /// </summary>
        [DataMember]
        public string Dose { get; set; }

        /// <summary>
        /// The dose unit of the medication
        /// </summary>
        [DataMember]
        public string DoseUnit { get; set; }

        /// <summary>
        /// The dosing frequency of the medication
        /// </summary>
        [DataMember]
        public string DoseFrequency { get; set; }

        /// <summary>
        /// The start date of the medication
        /// </summary>
        [DataMember]
        public string StartDate { get; set; }

        /// <summary>
        /// The end date of the medication
        /// </summary>
        [DataMember]
        public string EndDate { get; set; }

        /// <summary>
        /// The indication type of the medication (Custom Attribute)
        /// </summary>
        [DataMember]
        public string IndicationType { get; set; }

        /// <summary>
        /// The reason why the patient stopped the medication (Custom Attribute)
        /// </summary>
        [DataMember]
        public string ReasonForStopping { get; set; }

        /// <summary>
        /// Clinician action taken with regard to medicine if related to AE (Custom Attribute)
        /// </summary>
        [DataMember]
        public string ClinicianAction { get; set; }

        /// <summary>
        /// Effect OF Dechallenge (D) and Rechallenge (R) (Custom Attribute)
        /// </summary>
        [DataMember]
        public string ChallengeEffect { get; set; }

        /// <summary>
        /// A list of custom attributes associated to the patient medication
        /// </summary>
        [DataMember]
        public ICollection<AttributeValueDto> MedicationAttributes { get; set; } = new List<AttributeValueDto>();
    }
}
