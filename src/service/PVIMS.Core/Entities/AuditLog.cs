using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.ValueTypes;
using System;

namespace PVIMS.Core.Entities
{
    public partial class AuditLog : EntityBase
    {
        public DateTime ActionDate { get; set; }
        public string Details { get; set; }
        public int UserId { get; set; }
        public string Log { get; set; }

        public virtual AuditType AuditType { get; set; }
        public virtual User User { get; set; }
    }
}
