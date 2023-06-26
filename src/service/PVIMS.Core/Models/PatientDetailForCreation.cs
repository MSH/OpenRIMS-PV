using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PVIMS.Core.CustomAttributes;

namespace PVIMS.Core.Models
{
    public class PatientDetailForCreation : ExtendableDetail
    {
        public PatientDetailForCreation()
        {
            Conditions = new List<ConditionDetail>();
            LabTests = new List<LabTestDetail>();
            Medications = new List<MedicationDetail>();
            ClinicalEvents = new List<ClinicalEventDetail>();
            Attachments = new List<AttachmentDetail>();
        }

        public PatientDetailForCreation(string firstName, string surname, string middleName, string currentFacilityName, string notes, DateTime? dateOfBirth, int? cohortGroupId, DateTime? enroledDate, int encounterTypeId, int priorityId, DateTime encounterDate)
        {
            FirstName = firstName;
            Surname = surname;
            MiddleName = middleName;
            CurrentFacilityName = currentFacilityName;
            Notes = notes;
            DateOfBirth = dateOfBirth;
            CohortGroupId = cohortGroupId;
            EnroledDate = enroledDate;
            EncounterTypeId = encounterTypeId;
            PriorityId = priorityId;
            EncounterDate = encounterDate;

            Conditions = new List<ConditionDetail>();
            LabTests = new List<LabTestDetail>();
            Medications = new List<MedicationDetail>();
            ClinicalEvents = new List<ClinicalEventDetail>();
            Attachments = new List<AttachmentDetail>();
        }

        [Required]
        [StringLength(30)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(30)]
        public string Surname { get; set; }

        [StringLength(30)]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(100)]
        public string CurrentFacilityName { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public int? CohortGroupId { get; set; }
        public DateTime? EnroledDate { get; set; }

        public int EncounterTypeId { get; set; }
        public int PriorityId { get; set; }
        public DateTime EncounterDate { get; set; }

        public List<ConditionDetail> Conditions { get; set; }
        public List<LabTestDetail> LabTests { get; set; }
        public List<MedicationDetail> Medications { get; set; }
        public List<ClinicalEventDetail> ClinicalEvents { get; set; }
        public List<AttachmentDetail> Attachments { get; set; }
    }
}
