using MediatR;
using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.CustomAttributes;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.CustomAttributeAggregate
{
    [DataContract]
    public class AddCustomAttributeCommand
        : IRequest<CustomAttributeIdentifierDto>
    {
        [DataMember]
        public string ExtendableTypeName { get; private set; }

        [DataMember]
        public CustomAttributeType CustomAttributeType { get; private set; }

        [DataMember]
        public string Category { get; private set; }

        [DataMember]
        public string AttributeKey { get; private set; }

        [DataMember]
        public string AttributeDetail { get; private set; }

        [DataMember]
        public bool IsRequired { get; private set; }

        [DataMember]
        public int? MaxLength { get; private set; }

        [DataMember]
        public int? MinValue { get; private set; }

        [DataMember]
        public int? MaxValue { get; private set; }

        [DataMember]
        public bool? FutureDateOnly { get; private set; }

        [DataMember]
        public bool? PastDateOnly { get; private set; }

        [DataMember]
        public bool IsSearchable { get; private set; }

        public AddCustomAttributeCommand()
        {
        }

        public AddCustomAttributeCommand(string extendableTypeName, CustomAttributeType customAttributeType, string category, string attributeKey, string attributeDetail, bool isRequired, int? maxLength, int? minValue, int? maxValue, bool? futureDateOnly, bool? pastDateOnly, bool isSearchable): this()
        {
            ExtendableTypeName = extendableTypeName;
            CustomAttributeType = customAttributeType;
            Category = category;
            AttributeKey = attributeKey;
            AttributeDetail = attributeDetail;
            IsRequired = isRequired;
            MaxLength = maxLength;
            MinValue = minValue;
            MaxValue = maxValue;
            FutureDateOnly = futureDateOnly;
            PastDateOnly = pastDateOnly;
            IsSearchable = isSearchable;
        }
    }
}