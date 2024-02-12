using System;
using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public class MetaWidgetType : EntityBase
    {
        public MetaWidgetType()
        {
            MetaWidgets = new HashSet<MetaWidget>();
        }

        public Guid MetaWidgetTypeGuid { get; set; }
        public string Description { get; set; }

        public virtual ICollection<MetaWidget> MetaWidgets { get; set; }
    }
}
