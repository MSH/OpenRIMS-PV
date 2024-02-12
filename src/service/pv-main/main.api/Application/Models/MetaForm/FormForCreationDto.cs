using System.ComponentModel.DataAnnotations;

namespace OpenRIMS.PV.Main.API.Models
{
    public class FormForCreationDto
    {
        /// <summary>
        /// The unique identifier of the form
        /// </summary>
        [Required]
        [StringLength(20)]
        public string FormIdentifier { get; set; }

        /// <summary>
        /// The unique identifier of the patient
        /// </summary>
        [Required]
        [StringLength(100)]
        public string PatientIdentifier { get; set; }

        /// <summary>
        /// The type of form that is being submitted
        /// </summary>
        [Required]
        [StringLength(10)]
        public string FormType { get; set; }

        /// <summary>
        /// Binary form of the first form attachment
        /// </summary>
        public string Attachment { get; set; }
        public bool HasAttachment { get; set; }

        /// <summary>
        /// Binary form of the second form attachment
        /// </summary>
        public string Attachment_2 { get; set; }
        public bool HasSecondAttachment { get; set; }

        /// <summary>
        /// Details of the form that have been submitted
        /// </summary>
        public FormValueForCreationDto[] FormValues { get; set; }
    }
}