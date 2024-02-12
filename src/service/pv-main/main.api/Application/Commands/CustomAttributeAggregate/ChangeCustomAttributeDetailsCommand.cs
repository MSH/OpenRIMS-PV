using MediatR;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.CustomAttributeAggregate
{
    [DataContract]
    public class ChangeCustomAttributeDetailsCommand
        : IRequest<bool>
    {
        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public string ExtendableTypeName { get; private set; }

        [DataMember]
        public string AttributeKey { get; private set; }

        [DataMember]
        public string Category { get; private set; }

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

        public ChangeCustomAttributeDetailsCommand()
        {
        }

        public ChangeCustomAttributeDetailsCommand(int id, string extendableTypeName, string category, string attributeKey, string attributeDetail, bool isRequired, int? maxLength, int? minValue, int? maxValue, bool? futureDateOnly, bool? pastDateOnly, bool isSearchable): this()
        {
            Id = id;
            ExtendableTypeName = extendableTypeName;
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
