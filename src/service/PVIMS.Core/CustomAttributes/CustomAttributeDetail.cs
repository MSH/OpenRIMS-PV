using System;

namespace PVIMS.Core.CustomAttributes
{
    public class CustomAttributeDetail
    {
        public int Id { get; set; }
        public CustomAttributeType Type { get; set; }
        public string Category { get; set; }
        public string AttributeKey { get; set; }
        public bool IsRequired { get; set; }
        public int? StringMaxLength { get; set; }
        public int? NumericMinValue { get; set; }
        public int? NumericMaxValue { get; set; }
        public bool FutureDateOnly { get; set; }
        public bool PastDateOnly { get; set; }
        public bool IsSearchable { get; set; }
        public object Value { get; set; }
        //public List<SelectListItem> RefData { get; set; }

        public string TransformValueToString()
        {
            DateTime parsedDate;

            if (DateTime.TryParse(Value.ToString(), out parsedDate))
            {
                if (parsedDate > DateTime.MinValue)
                {
                    return parsedDate.ToString("yyyy-MM-dd");
                }
                return string.Empty;
            }
            return Value.ToString();
        }
    }
}