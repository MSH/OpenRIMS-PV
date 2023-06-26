using System;
using System.Collections.Generic;
using System.Linq;

using PVIMS.Core.ValueTypes;

namespace PVIMS.Core.Entities
{
    public class MetaPage : EntityBase
    {
        public MetaPage()
        {
            IsSystem = false;
            IsVisible = true;

            Widgets = new HashSet<MetaWidget>();
        }

        public Guid MetaPageGuid { get; set; }
        public string PageName { get; set; }
        public string PageDefinition { get; set; }
        public string MetaDefinition { get; set; }
        public string Breadcrumb { get; set; }
        public bool IsSystem { get; set; }
        public bool IsVisible { get; set; }

        public virtual ICollection<MetaWidget> Widgets { get; set; }

        public bool IsWidgetUnique(MetaWidgetLocation location)
        {
            if (Widgets.Count == 0)
                { return true; }
            else
                { return Widgets.Any(w => w.WidgetLocation == location); };
        }
    }
}
