using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public partial class CareEvent : EntityBase
    {
        public CareEvent()
        {
            WorkPlanCareEvents = new HashSet<WorkPlanCareEvent>();
        }

        public string Description { get; set; }

        public virtual ICollection<WorkPlanCareEvent> WorkPlanCareEvents { get; set; }
    }
}
