using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Utilities;
using System;

namespace PVIMS.Core.Entities
{
    public class PatientClinicalEvent : EntityBase, IExtendable
	{
        protected PatientClinicalEvent()
        {
        }

        public PatientClinicalEvent(DateTime? onsetDate, DateTime? resolutionDate, TerminologyMedDra sourceTerminology, string sourceDescription)
        {
            PatientClinicalEventGuid = Guid.NewGuid();
            Archived = false;

            OnsetDate = onsetDate;
            ResolutionDate = resolutionDate;

            if (sourceTerminology != null)
            {
                SourceTerminologyMedDra = sourceTerminology;
                SourceTerminologyMedDraId = sourceTerminology.Id;
            }

            SourceDescription = sourceDescription;
        }

        public void ChangeDetails(DateTime? onsetDate, DateTime? resolutionDate, TerminologyMedDra sourceTerminology, string sourceDescription)
        {
            OnsetDate = onsetDate;
            ResolutionDate = resolutionDate;

            SourceTerminologyMedDra = sourceTerminology;
            SourceTerminologyMedDraId = sourceTerminology?.Id;

            SourceDescription = sourceDescription;
        }

        public void Archive(User user, string reason)
        {
            Archived = true;
            ArchivedDate = DateTime.Now;
            ArchivedReason = reason;
            AuditUser = user;
            AuditUserId = user.Id;
        }

        public DateTime? OnsetDate { get; private set; }
        public DateTime? ResolutionDate { get; private set; }

        public int? EncounterId { get; private set; }
        public virtual Encounter Encounter { get; private set; }

        public int PatientId { get; private set; }
        public virtual Patient Patient { get; private set; }

        public Guid PatientClinicalEventGuid { get; private set; }
        
        public int? SourceTerminologyMedDraId { get; private set; }
        public virtual TerminologyMedDra SourceTerminologyMedDra { get; private set; }

        public int? TerminologyMedDraId1 { get; private set; }
        public bool Archived { get; private set; }
        public DateTime? ArchivedDate { get; private set; }
        public string ArchivedReason { get; private set; }
        
        public int? AuditUserId { get; private set; }
        public virtual User AuditUser { get; private set; }

        public string SourceDescription { get; private set; }

        private CustomAttributeSet customAttributes = new CustomAttributeSet();

        public string AgeGroup
        {
            get
            {
                if (OnsetDate == null)
                {
                    return "";
                }
                if (Patient.DateOfBirth == null)
                {
                    return "";
                }

                DateTime onset = Convert.ToDateTime(OnsetDate);
                DateTime bday = Convert.ToDateTime(Patient.DateOfBirth);

                string ageGroup = "";
                if (onset <= bday.AddMonths(1)) { ageGroup = "Neonate <= 1 month"; };
                if (onset <= bday.AddMonths(48) && onset > bday.AddMonths(1)) { ageGroup = "Infant > 1 month and <= 4 years"; };
                if (onset <= bday.AddMonths(132) && onset > bday.AddMonths(48)) { ageGroup = "Child > 4 years and <= 11 years"; };
                if (onset <= bday.AddMonths(192) && onset > bday.AddMonths(132)) { ageGroup = "Adolescent > 11 years and <= 16 years"; };
                if (onset <= bday.AddMonths(828) && onset > bday.AddMonths(192)) { ageGroup = "Adult > 16 years and <= 69 years"; };
                if (onset > bday.AddMonths(828)) { ageGroup = "Elderly > 69 years"; };

                return ageGroup;
            }
        }

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