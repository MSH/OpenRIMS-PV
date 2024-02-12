using MediatR;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.PatientAggregate
{
    [DataContract]
    public class ChangePatientCustomAttributesCommand
        : IRequest<bool>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public IDictionary<int, string> Attributes { get; private set; }

        public ChangePatientCustomAttributesCommand()
        {
        }

        public ChangePatientCustomAttributesCommand(int patientId, IDictionary<int, string> attributes) : this()
        {
            PatientId = patientId;
            Attributes = attributes;
        }
    }
}