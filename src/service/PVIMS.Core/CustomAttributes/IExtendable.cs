using PVIMS.Core.Entities;
using System;

namespace PVIMS.Core.CustomAttributes
{
    public interface IExtendable
    {
        CustomAttributeSet CustomAttributes { get; }
        string CustomAttributesXmlSerialised { get; }
        void SetAttributeValue<T>(string attributeKey, T attributeValue, string updatedByUser);
        void ValidateAndSetAttributeValue<T>(CustomAttributeDetail attributeDetail, T attributeValue, string updatedByUser);
        object GetAttributeValue(string attributeKey);
        DateTime GetUpdatedDate(string attributeKey);
        string GetUpdatedByUser(string attributeKey);
    }
}
