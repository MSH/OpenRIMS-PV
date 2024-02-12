using OpenRIMS.PV.Main.Core.Aggregates.DatasetAggregate;
using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public class ContextType : EntityBase
    {
        public ContextType()
        {
            Datasets = new HashSet<Dataset>();
        }

        public string Description { get; set; }

        public virtual ICollection<Dataset> Datasets { get; set; }
    }
}
