using System;
using OpenRIMS.PV.Main.Core.ValueTypes;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public class MetaWidget : EntityBase
    {
        public MetaWidget()
        {
            MetaWidgetGuid = Guid.NewGuid();

            WidgetStatus = MetaWidgetStatus.Unpublished;
            WidgetLocation = MetaWidgetLocation.Unassigned;
        }

        public Guid MetaWidgetGuid { get; set; }
        public string WidgetName { get; set; }
        public string WidgetDefinition { get; set; }
        public string Content { get; set; }
        public int WidgetTypeId { get; set; }
        public int MetaPageId { get; set; }
        public MetaWidgetLocation WidgetLocation { get; set; }
        public MetaWidgetStatus WidgetStatus { get; set; }
        public string Icon { get; set; }

        public virtual MetaWidgetType WidgetType { get; set; }
        public virtual MetaPage MetaPage { get; set; }
    }
}
