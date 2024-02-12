using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using System;

namespace OpenRIMS.PV.Main.Core.Entities
{
	public class Attachment : AuditedEntityBase
	{
        protected Attachment()
        {
        }

        public Attachment(byte[] content, string description, string fileName, long size, AttachmentType attachmentType, Encounter encounter, Patient patient, ActivityExecutionStatusEvent activityExecutionStatusEvent)
        {
            Archived = false;

            Content = content;
            Description = description;
            FileName = fileName;
            Size = size;
            AttachmentType = attachmentType;
            AttachmentTypeId = attachmentType.Id;

            Encounter = encounter;
            EncounterId = encounter?.Id;

            Patient = patient;
            PatientId = patient?.Id;

            ActivityExecutionStatusEvent = activityExecutionStatusEvent;
            ActivityExecutionStatusEventId = activityExecutionStatusEvent?.Id;
        }

        public void ArchiveAttachment(User user, string reason)
        {
            Archived = true;
            ArchivedDate = DateTime.Now;
            ArchivedReason = reason;
            AuditUser = user;
            AuditUserId = user.Id;
        }

        public byte[] Content { get; private set; }
        public string Description { get; private set; }
        public string FileName { get; private set; }
        public long Size { get; private set; }

        public int AttachmentTypeId { get; private set; }
        public virtual AttachmentType AttachmentType { get; private set; }

        public int? EncounterId { get; private set; }
        public virtual Encounter Encounter { get; private set; }

        public int? PatientId { get; private set; }
        public virtual Patient Patient { get; private set; }

        public bool Archived { get; private set; }
        public DateTime? ArchivedDate { get; private set; }
        public string ArchivedReason { get; private set; }

        public int? AuditUserId { get; private set; }
        public virtual User AuditUser { get; private set; }

        public int? ActivityExecutionStatusEventId { get; private set; }
        public virtual ActivityExecutionStatusEvent ActivityExecutionStatusEvent { get; private set; }
    }
}