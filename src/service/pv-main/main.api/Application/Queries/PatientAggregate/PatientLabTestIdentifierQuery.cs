using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class PatientLabTestIdentifierQuery
        : IRequest<PatientLabTestIdentifierDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int PatientLabTestId { get; private set; }

        public PatientLabTestIdentifierQuery()
        {
        }

        public PatientLabTestIdentifierQuery(int patientId, int patientLabTestId) : this()
        {
            PatientId = patientId;
            PatientLabTestId = patientLabTestId;
        }
    }
}