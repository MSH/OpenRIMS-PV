using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;
using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public class ActivityExecutionStatus : EntityBase
	{
        public ActivityExecutionStatus()
        {
            ExecutionEvents = new HashSet<ActivityExecutionStatusEvent>();
            ActivityInstances = new HashSet<ActivityInstance>();
        }

        public string Description { get; set; }
        public int ActivityId { get; set; }
        public string FriendlyDescription { get; set; }

        public virtual Activity Activity { get; set; }

        public virtual ICollection<ActivityExecutionStatusEvent> ExecutionEvents { get; set; }
        public virtual ICollection<ActivityInstance> ActivityInstances { get; set; }
    }
}