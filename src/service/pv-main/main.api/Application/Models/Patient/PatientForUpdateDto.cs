using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpenRIMS.PV.Main.API.Models
{
    public class PatientForUpdateDto
    {
        /// <summary>
        /// The first name of the patient
        /// </summary>
        [Required]
        [StringLength(30)]
        public string FirstName { get; set; }

        /// <summary>
        /// The first name of the patient
        /// </summary>
        [Required]
        [StringLength(30)]
        public string LastName { get; set; }

        /// <summary>
        /// The first name of the patient
        /// </summary>
        [StringLength(30)]
        public string MiddleName { get; set; }

        /// <summary>
        /// The date of birth of the patient
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Generic patient notes
        /// </summary>
        [StringLength(1000)]
        public string Notes { get; set; }

        /// <summary>
        /// The facility that the patient is being registered against
        /// </summary>
        [Required]
        [StringLength(100)]
        public string FacilityName { get; set; }

        /// <summary>
        /// Patient custom attributes and associated values
        /// </summary>
        public IDictionary<int, string> Attributes { get; set; }
    }
}
