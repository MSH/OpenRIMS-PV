using MediatR;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.ConceptAggregate
{
    [DataContract]
    public class ChangeConceptDetailsCommand
        : IRequest<bool>
    {
        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public string ConceptName { get; private set; }

        [DataMember]
        public string Strength { get; private set; }

        [DataMember]
        public string MedicationForm { get; private set; }

        [DataMember]
        public bool Active { get; private set; }

        public ChangeConceptDetailsCommand()
        {
        }

        public ChangeConceptDetailsCommand(int id, string conceptName, string strength, string medicationForm, bool active) : this()
        {
            Id = id;
            ConceptName = conceptName;
            Strength = strength;
            MedicationForm = medicationForm;
            Active = active;
        }
    }
}
