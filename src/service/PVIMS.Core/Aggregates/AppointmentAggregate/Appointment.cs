using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.Exceptions;
using System;

namespace PVIMS.Core.Entities
{
    public class Appointment : AuditedEntityBase
    {
        public Appointment(int patientId, DateTime appointmentDate, string reason)
        {
            PatientId = patientId;
            AppointmentDate = appointmentDate;
            Reason = reason;
            Dna = false;
            Cancelled = false;
        }

        protected Appointment() { }

        public DateTime AppointmentDate { get; private set; }
        public string Reason { get; private set; }
        public bool Dna { get; private set; }
        public bool Cancelled { get; private set; }
        public string CancellationReason { get; private set; }
        public int PatientId { get; private set; }
        public bool Archived { get; private set; }
        public DateTime? ArchivedDate { get; private set; }
        public string ArchivedReason { get; private set; }

        public int? AuditUserId { get; private set; }
        public virtual User AuditUser { get; private set; }

        public void Archive(User user, string reason)
        {
            Archived = true;
            ArchivedDate = DateTime.Now;
            ArchivedReason = reason;
            AuditUser = user;
            AuditUserId = user.Id;
        }

        public void ChangeDetails(DateTime appointmentDate, string reason, bool cancelled, string cancellationReason)
        {
            AppointmentDate = appointmentDate;
            Reason = reason;
            Cancelled = cancelled;
            CancellationReason = cancellationReason;
        }

        public void MarkAsDNA()
        {
            if(Dna)
            {
                throw new DomainException($"Appointment {Id} already marked as DNA");
            }
            Dna = true;
        }
    }
}
