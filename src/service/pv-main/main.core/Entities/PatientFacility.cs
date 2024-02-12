using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using System;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public class PatientFacility : EntityBase
	{
        public DateTime EnrolledDate { get; set; }
        public int FacilityId { get; set; }
        public int PatientId { get; set; }
        public bool Archived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        public string ArchivedReason { get; set; }
        public int? AuditUserId { get; set; }

        public virtual Facility Facility { get; set; }
		public virtual Patient Patient { get; set; }
        public virtual User AuditUser { get; set; }
	}
}