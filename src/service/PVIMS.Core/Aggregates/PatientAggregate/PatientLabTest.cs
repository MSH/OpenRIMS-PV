using System;
using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Utilities;

namespace PVIMS.Core.Entities
{
    public class PatientLabTest : EntityBase, IExtendable
	{
        protected PatientLabTest()
        {
        }

        public PatientLabTest(DateTime testDate, string testResult, LabTest labTest, LabTestUnit testUnit, string labValue, string referenceLower, string referenceUpper, string labTestSource)
        {
            PatientLabTestGuid = Guid.NewGuid();
            Archived = false;

            TestDate = testDate;
            TestResult = testResult;
            LabTestId = labTest.Id;
            LabTest = labTest;

            if (testUnit != null)
            {
                TestUnitId = testUnit.Id;
                TestUnit = testUnit;
            }

            LabValue = labValue;
            ReferenceLower = referenceLower;
            ReferenceUpper = referenceUpper;
            LabTestSource = labTestSource;
        }

        public void ChangeDetails(DateTime testDate, string testResult, LabTestUnit testUnit, string labValue, string referenceLower, string referenceUpper)
        {
            TestDate = testDate;
            TestResult = testResult;

            if (testUnit != null)
            {
                TestUnitId = testUnit.Id;
                TestUnit = testUnit;
            }

            LabValue = labValue;
            ReferenceLower = referenceLower;
            ReferenceUpper = referenceUpper;
        }

        public void Archive(User user, string reason)
        {
            Archived = true;
            ArchivedDate = DateTime.Now;
            ArchivedReason = reason;
            AuditUser = user;
            AuditUserId = user.Id;
        }

        public DateTime TestDate { get; private set; }
        public string TestResult { get; private set; }

        public int LabTestId { get; private set; }
        public virtual LabTest LabTest { get; private set; }

        public int? TestUnitId { get; private set; }    
        public virtual LabTestUnit TestUnit { get; private set; }

        public string LabValue { get; private set; }
        public string ReferenceLower { get; private set; }
        public string ReferenceUpper { get; private set; }

        public string LabTestSource { get; private set; }

        public int PatientId { get; private set; }
        public virtual Patient Patient { get; private set; }

        public Guid PatientLabTestGuid { get; private set; }

        public bool Archived { get; private set; }
        public DateTime? ArchivedDate { get; private set; }
        public string ArchivedReason { get; private set; }
        
        public int? AuditUserId { get; private set; }
        public virtual User AuditUser { get; private set; }

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