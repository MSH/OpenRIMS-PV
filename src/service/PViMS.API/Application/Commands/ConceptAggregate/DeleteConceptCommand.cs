using MediatR;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.ConceptAggregate
{
    [DataContract]
    public class DeleteConceptCommand
        : IRequest<bool>
    {
        [DataMember]
        public int Id { get; private set; }

        public DeleteConceptCommand()
        {
        }

        public DeleteConceptCommand(int id) : this()
        {
            Id = id;
        }
    }
}
