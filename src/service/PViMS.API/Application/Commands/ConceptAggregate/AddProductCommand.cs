using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.ConceptAggregate
{
    [DataContract]
    public class AddProductCommand
        : IRequest<ProductIdentifierDto>
    {
        [DataMember]
        public string ConceptName { get; set; }

        [DataMember]
        public string ProductName { get; private set; }

        [DataMember]
        public string Manufacturer { get; private set; }

        [DataMember]
        public string Description { get; private set; }

        public AddProductCommand()
        {
        }

        public AddProductCommand(string conceptName, string productName, string manufacturer, string description): this()
        {
            ConceptName = conceptName;
            ProductName = productName;
            Manufacturer = manufacturer;
            Description = description;
        }
    }
}
