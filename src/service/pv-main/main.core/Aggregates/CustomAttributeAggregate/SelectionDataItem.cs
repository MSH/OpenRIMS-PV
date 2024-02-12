using OpenRIMS.PV.Main.Core.SeedWork;
using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public class SelectionDataItem : Entity<int>
    {
        public SelectionDataItem()
        {
            MedDrascales = new HashSet<MedDRAScale>();
        }

        public string AttributeKey { get; set; }
        public string SelectionKey { get; set; }
        public string Value { get; set; }

        public virtual ICollection<MedDRAScale> MedDrascales { get; set; }
    }
}
