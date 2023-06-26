using System;
using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Utilities;

namespace PVIMS.Core.Entities
{
    public class PatientCondition : EntityBase, IExtendable
    {
        public PatientCondition()
        {
            PatientConditionGuid = Guid.NewGuid();
        }

        public void ChangeConditionDetails(DateTime onsetDate, DateTime? outcomeDate, Outcome outcome, TreatmentOutcome treatmentOutcome, string caseNumber, string comments)
        {
            OnsetDate = onsetDate;
            OutcomeDate = outcomeDate;
            Outcome = outcome;
            TreatmentOutcome = treatmentOutcome;
            CaseNumber = caseNumber;
            Comments = comments;
        }

        public DateTime OnsetDate { get; set; }
        public DateTime? OutcomeDate { get; set; }
        public string Comments { get; set; }
        public int? ConditionId { get; set; }
        public int PatientId { get; set; }
        public Guid PatientConditionGuid { get; set; }
        public int? TerminologyMedDraId { get; set; }
        public bool Archived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        public string ArchivedReason { get; set; }
        public int? AuditUserId { get; set; }
        public int? OutcomeId { get; set; }
        public int? TreatmentOutcomeId { get; set; }
        public string ConditionSource { get; set; }
        public string CaseNumber { get; set; }

        public virtual TerminologyMedDra TerminologyMedDra { get; set; }
        public virtual Outcome Outcome { get; set; }
        public virtual TreatmentOutcome TreatmentOutcome { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual User AuditUser { get; set; }
        public virtual Condition Condition { get; set; }

        private CustomAttributeSet customAttributes = new CustomAttributeSet();

        CustomAttributeSet IExtendable.CustomAttributes
        {
            get { return customAttributes; }
        }

        public string CustomAttributesXmlSerialised
        {

            get { return SerialisationHelper.SerialiseToXmlString(customAttributes); }
            private set
            {
                if (string.IsNullOrEmpty(value))
                {
                    customAttributes = new CustomAttributeSet();
                }
                else
                {
                    customAttributes = SerialisationHelper.DeserialiseFromXmlString<CustomAttributeSet>(value);
                }
            }
        }

        void IExtendable.SetAttributeValue<T>(string attributeKey, T attributeValue, string updatedByUser)
        {
            customAttributes.SetAttributeValue(attributeKey, attributeValue, updatedByUser);
        }

        object IExtendable.GetAttributeValue(string attributeKey)
        {
            return customAttributes.GetAttributeValue(attributeKey);
        }

        public void ValidateAndSetAttributeValue<T>(CustomAttributeDetail attributeDetail, T attributeValue, string updatedByUser)
        {
            customAttributes.ValidateAndSetAttributeValue(attributeDetail, attributeValue, updatedByUser);
        }

        public DateTime GetUpdatedDate(string attributeKey)
        {
            return customAttributes.GetUpdatedDate(attributeKey);
        }

        public string GetUpdatedByUser(string attributeKey)
        {
            return customAttributes.GetUpdatedByUser(attributeKey);
        }
    }
}