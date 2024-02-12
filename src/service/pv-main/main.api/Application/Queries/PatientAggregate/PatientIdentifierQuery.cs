using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class PatientIdentifierQuery
        : IRequest<PatientIdentifierDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        public PatientIdentifierQuery()
        {
        }

        public PatientIdentifierQuery(int patientId) : this()
        {
            PatientId = patientId;
        }
    }
}
