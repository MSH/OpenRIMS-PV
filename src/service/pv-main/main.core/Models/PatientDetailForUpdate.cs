using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using OpenRIMS.PV.Main.Core.CustomAttributes;

namespace OpenRIMS.PV.Main.Core.Models
{
    public class PatientDetailForUpdate : ExtendableDetail
    {
        public PatientDetailForUpdate()
        {
            Conditions = new List<ConditionDetail>();
            LabTests = new List<LabTestDetail>();
            Medications = new List<MedicationDetail>();
            ClinicalEvents = new List<ClinicalEventDetail>();
            Attachments = new List<AttachmentDetail>();
        }

        [StringLength(30)]
        public string FirstName { get; set; }

        [StringLength(30)]
        public string Surname { get; set; }

        [StringLength(30)]
        public string MiddleName { get; set; }

        public DateTime? DateOfBirth { get; set; }

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
