using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.SeedWork;
using System;
using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
	public class Encounter : AuditedEntityBase, IHasCustomAttributes
	{
        protected Encounter()
        { 
        }

		public Encounter(Patient patient)
		{
			Attachments = new HashSet<Attachment>();
			PatientClinicalEvents = new HashSet<PatientClinicalEvent>();
            EncounterGuid = Guid.NewGuid();
            Patient = patient;
		}

        public DateTime EncounterDate { get; set; }
        public string Notes { get; set; }
        public Guid EncounterGuid { get; set; }
        public bool Discharged { get; set; }
        public string CustomAttributesXmlSerialised { get; set; }
        public int EncounterTypeId { get; set; }
        public int PatientId { get; set; }
        public int PriorityId { get; set; }
        public bool Archived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        public string ArchivedReason { get; set; }
        public int? AuditUserId { get; set; }

        public virtual User AuditUser { get; set; }
        public virtual EncounterType EncounterType { get; set; }
		public virtual Patient Patient { get; set; }
		public virtual Priority Priority { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<PatientClinicalEvent> PatientClinicalEvents { get; set; }
	}
}