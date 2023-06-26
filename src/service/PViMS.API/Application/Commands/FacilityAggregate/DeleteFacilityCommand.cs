using MediatR;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.FacilityAggregate
{
    [DataContract]
    public class DeleteFacilityCommand
        : IRequest<bool>
    {
        [DataMember]
        public int Id { get; private set; }

        public DeleteFacilityCommand()
        {
        }

        public DeleteFacilityCommand(int id) : this()
        {
            Id = id;
        }
    }
}
