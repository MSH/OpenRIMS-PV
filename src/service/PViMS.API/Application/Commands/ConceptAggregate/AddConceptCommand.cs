using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.ConceptAggregate
{
    [DataContract]
    public class AddConceptCommand
        : IRequest<ConceptIdentifierDto>
    {
        [DataMember]
        public string ConceptName { get; private set; }

        [DataMember]
        public string Strength { get; private set; }

        [DataMember]
        public string MedicationForm { get; private set; }

        public AddConceptCommand()
        {
        }

        public AddConceptCommand(string conceptName, string strength, string medicationForm): this()
        {
            ConceptName = conceptName;
            Strength = strength;
            MedicationForm = medicationForm;
        }
    }
}
