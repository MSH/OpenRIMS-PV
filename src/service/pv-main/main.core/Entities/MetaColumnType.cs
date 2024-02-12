using System;
using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public class MetaColumnType : EntityBase
    {
        public MetaColumnType()
        {
            MetaColumns = new HashSet<MetaColumn>();
        }

        public Guid MetaColumnTypeGuid { get; set; }
        public string Description { get; set; }

        public virtual ICollection<MetaColumn> MetaColumns { get; set; }
    }
}
