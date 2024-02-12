using System;
using System.Collections.Generic;

namespace OpenRIMS.PV.Main.API.Models
{
    public class PatientForCreationDto
    {
        /// <summary>
        /// The first name of the patient
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The first name of the patient
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The first name of the patient
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// The date of birth of the patient
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// The facility that the patient is being registered against
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// The primary condition group for the patient
        /// </summary>
        public int ConditionGroupId { get; set; }

        /// <summary>
        /// The meddra term that has been associated to the primary group that the patient belongs to
        /// </summary>
        public int MeddraTermId { get; set; }

        /// <summary>
        /// The cohort group the patient should be assigned to
        /// </summary>
        public int? CohortGroupId { get; set; }

        /// <summary>
        /// The date the patient should be enroled into this cohort
        /// </summary>
        public DateTime? EnroledDate { get; set; }

        /// <summary>
        /// The start date of the primary condition
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end date of the primary condition
        /// </summary>
        public DateTime? OutcomeDate { get; set; }

        /// <summary>
        /// The case number of the primary condition
        /// </summary>
        public string CaseNumber { get; set; }

        /// <summary>
        /// Any comments associated to the primary condition
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// The type of encounter on patient registration
        /// </summary>
        public int EncounterTypeId { get; set; }

        /// <summary>
        /// The priority of the encounter
        /// </summary>
        public int PriorityId { get; set; }

        /// <summary>
        /// The date of the encounter
        /// </summary>
        public DateTime EncounterDate { get; set; }

        /// <summary>
        /// Patient custom attributes
        /// </summary>
        public ICollection<AttributeValueForPostDto> Attributes { get; set; }
    }
}