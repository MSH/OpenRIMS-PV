using MediatR;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.CohortGroupAggregate
{
    [DataContract]
    public class DeleteCohortGroupCommand
        : IRequest<bool>
    {
        [DataMember]
        public int Id { get; private set; }

        public DeleteCohortGroupCommand()
        {
        }

        public DeleteCohortGroupCommand(int id) : this()
        {
            Id = id;
        }
    }
}
