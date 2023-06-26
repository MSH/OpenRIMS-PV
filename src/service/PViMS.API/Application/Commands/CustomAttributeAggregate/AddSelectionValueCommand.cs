using MediatR;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.CustomAttributeAggregate
{
    [DataContract]
    public class AddSelectionValueCommand
        : IRequest<bool>
    {
        [DataMember]
        public string AttributeKey { get; private set; }

        [DataMember]
        public string SelectionKey { get; private set; }

        [DataMember]
        public string DataItemValue { get; private set; }

        public AddSelectionValueCommand()
        {
        }

        public AddSelectionValueCommand(string attributeKey, string selectionKey, string dataItemValue): this()
        {
            AttributeKey = attributeKey;
            SelectionKey = selectionKey;
            DataItemValue = dataItemValue;
        }
    }
}