using System.Collections.Generic;
using OpenRIMS.PV.Main.Core.ValueTypes;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public class Activity : EntityBase
	{
        public Activity()
        {
            ExecutionStatuses = new HashSet<ActivityExecutionStatus>();
        }

        public string QualifiedName { get; set; }
        
        public int WorkFlowId { get; set; }
        public virtual WorkFlow WorkFlow { get; set; }

        public virtual ActivityTypes ActivityType { get; set; }

        public virtual ICollection<ActivityExecutionStatus> ExecutionStatuses { get; set; }
    }
}