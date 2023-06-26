using System;
using System.Collections.Generic;
using System.Linq;
using PVIMS.Core.CustomAttributes;

namespace PVIMS.Core.Models
{
    public class EncounterDetail
    {
        public int PatientId { get; set; }

        public int EncounterTypeId { get; set; }
        public int PriorityId { get; set; }
        public DateTime EncounterDate { get; set; }
        public string Notes { get; set; }

        public List<CustomAttributeDetail> CustomAttributes { get; set; }

        public List<string> InvalidAttributes { get; set; }

        public bool IsValid()
        {
            var valid = true;

            // Ensure all required fields have a value
            foreach (var attributeDetail in CustomAttributes)
            {
                var value = attributeDetail.Value.ToString();
                if (attributeDetail.IsRequired)
                {
                    switch (attributeDetail.Type)
                    {
                        case CustomAttributeType.String:
                            if (String.IsNullOrWhiteSpace(value))
                            {
                                PublishMessage($"{attributeDetail.AttributeKey} is a required field and must be captured");
                                valid = false;
                            }
                            break;

                        case CustomAttributeType.Selection:
                            if (String.IsNullOrWhiteSpace(value) || value == "0")
                            {
                                PublishMessage($"{attributeDetail.AttributeKey} is a required field and must be captured");
                                valid = false;
                            }
                            break;

                        case CustomAttributeType.DateTime:
                            var dateTimeValue = Convert.ToDateTime(value);
                            if (String.IsNullOrWhiteSpace(value) || dateTimeValue == DateTime.MinValue)
                            {
                                PublishMessage($"{attributeDetail.AttributeKey} is a required field and must be captured");
                                valid = false;
                            }
                            break;

                        default:
                            break;
                    }
                }
            }

            return valid;
        }

        public void SetAttributeValue(string attributeKey, string attributeValue)
        {
            var attributeDetail = this.CustomAttributes.SingleOrDefault(ca => ca.AttributeKey == attributeKey);
            if(attributeDetail == null)
            {
                throw new ArgumentNullException(attributeKey);
            }

            attributeDetail.Value = attributeValue;
        }

        private void PublishMessage(string message)
        {
            if (InvalidAttributes == null)
            {
                InvalidAttributes = new List<string>();
            }
            if(!InvalidAttributes.Contains(message))
            {
                InvalidAttributes.Add(message);
            }
        }
    }

}
