using System.Collections.Generic;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Core.Entities
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