using MediatR;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.ConceptAggregate
{
    [DataContract]
    public class ChangeProductDetailsCommand
        : IRequest<bool>
    {
        [DataMember]
        public int ProductId { get; private set; }

        [DataMember]
        public string ConceptName { get; private set; }

        [DataMember]
        public string ProductName { get; private set; }

        [DataMember]
        public string Manufacturer { get; private set; }

        [DataMember]
        public string Description { get; private set; }

        [DataMember]
        public bool Active { get; private set; }

        public ChangeProductDetailsCommand()
        {
        }

        public ChangeProductDetailsCommand(int productId, string conceptName, string productName, string manufacturer, string description, bool active): this()
        {
            ProductId = productId;
            ConceptName = conceptName;
            ProductName = productName;
            Manufacturer = manufacturer;
            Description = description;
            Active = active;
        }
    }
}