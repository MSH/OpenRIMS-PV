using MediatR;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.CustomAttributeAggregate
{
    [DataContract]
    public class DeleteCustomAttributeCommand
        : IRequest<bool>
    {
        [DataMember]
        public int Id { get; private set; }

        public DeleteCustomAttributeCommand()
        {
        }

        public DeleteCustomAttributeCommand(int id) : this()
        {
            Id = id;
        }
    }
}
