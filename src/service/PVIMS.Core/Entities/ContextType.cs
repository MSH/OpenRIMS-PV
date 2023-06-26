using PVIMS.Core.Aggregates.DatasetAggregate;
using System.Collections.Generic;

namespace PVIMS.Core.Entities
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
