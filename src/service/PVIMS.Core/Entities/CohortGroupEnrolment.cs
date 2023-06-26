using PVIMS.Core.Aggregates.UserAggregate;
using System;

namespace PVIMS.Core.Entities
{
    public partial class CohortGroupEnrolment : EntityBase
    {
        public DateTime EnroledDate { get; set; }
        public int CohortGroupId { get; set; }
        public int PatientId { get; set; }
        public DateTime? DeenroledDate { get; set; }
        public bool Archived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        public string ArchivedReason { get; set; }
        public int? AuditUserId { get; set; }

        public virtual Patient Patient { get; set; }
        public virtual User AuditUser { get; set; }
        public virtual CohortGroup CohortGroup { get; set; }
    }
}
