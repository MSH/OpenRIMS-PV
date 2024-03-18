using System;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Core.Aggregates.MetaFormAggregate
{
    public class MetaFormCategoryAttribute : EntityBase
    {
        protected MetaFormCategoryAttribute()
        {
        }

        public MetaFormCategoryAttribute(string attributeName, CustomAttributeConfiguration customAttributeConfiguration, string label, string help)
        {
            MetaFormCategoryAttributeGuid = Guid.NewGuid();

            AttributeName = attributeName;

            if(CustomAttributeConfiguration!= null)
            {
                CustomAttributeConfigurationId = customAttributeConfiguration.Id;
                CustomAttributeConfiguration = customAttributeConfiguration;
            }
            Label = label;
            Help = help;
        }

        public int MetaFormCategoryId { get; private set; }
        public virtual MetaFormCategory MetaFormCategory { get; private set; }

        public Guid MetaFormCategoryAttributeGuid { get; private set; }

        public string AttributeName { get; private set; }

        public int? CustomAttributeConfigurationId { get; private set; }
        public virtual CustomAttributeConfiguration CustomAttributeConfiguration { get; private set; }

        public string Label { get; private set; }
        public string Help { get; private set; }

        public int FormAttributeTypeId { get; private set; }
        public bool? IsRequired { get; private set; }
        public int? StringMaxLength { get; private set; }
        public int? NumericMinValue { get; private set; }
        public int? NumericMaxValue { get; private set; }
        public bool? FutureDateOnly { get; private set; }
        public bool? PastDateOnly { get; private set; }
        public string SelectionDataItem { get; private set; }

        public void ChangeDetails(string label, string help)
        {
            Label = label;
            Help = help;
        }
    }
}