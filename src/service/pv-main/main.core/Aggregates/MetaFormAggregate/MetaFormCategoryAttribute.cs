using System;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Core.Aggregates.MetaFormAggregate
{
    public class MetaFormCategoryAttribute : EntityBase
    {
        protected MetaFormCategoryAttribute()
        {
        }

        public MetaFormCategoryAttribute(CustomAttributeConfiguration customAttributeConfiguration, string label, string help)
        {
            MetaFormCategoryAttributeGuid = Guid.NewGuid();

            CustomAttributeConfigurationId = customAttributeConfiguration.Id;
            CustomAttributeConfiguration = customAttributeConfiguration;
            Label = label;
            Help = help;
        }

        public int MetaFormCategoryId { get; private set; }
        public virtual MetaFormCategory MetaFormCategory { get; private set; }

        public Guid MetaFormCategoryAttributeGuid { get; private set; }
        
        public int CustomAttributeConfigurationId { get; private set; }
        public virtual CustomAttributeConfiguration CustomAttributeConfiguration { get; private set; }

        public string Label { get; private set; }
        public string Help { get; private set; }
    }
}