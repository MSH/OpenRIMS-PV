using OpenRIMS.PV.Main.Core.CustomAttributes;
using OpenRIMS.PV.Main.Core.SeedWork;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public class CustomAttributeConfiguration : Entity<int>
    {
        public string ExtendableTypeName { get; set; }
        public CustomAttributeType CustomAttributeType { get; set; }
        public string Category { get; set; }
        public string AttributeKey { get; set; }
        public string AttributeDetail { get; set; }
        public bool IsRequired { get; set; }
        public int? StringMaxLength { get; set; }
        public int? NumericMinValue { get; set; }
        public int? NumericMaxValue { get; set; }
        public bool FutureDateOnly { get; set; }
        public bool PastDateOnly { get; set; }
        public bool IsSearchable { get; set; }
    }
}
