using MediatR;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.CustomAttributeAggregate
{
    [DataContract]
    public class DeleteSelectionValueCommand
        : IRequest<bool>
    {
        [DataMember]
        public int CustomAttributeId { get; private set; }

        [DataMember]
        public string Key { get; private set; }

        public DeleteSelectionValueCommand()
        {
        }

        public DeleteSelectionValueCommand(int customAttributeId, string key): this()
        {
            CustomAttributeId = customAttributeId;
            Key = key;
        }
    }
}
