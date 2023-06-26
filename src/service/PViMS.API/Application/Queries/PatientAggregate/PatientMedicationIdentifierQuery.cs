using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class PatientMedicationIdentifierQuery
        : IRequest<PatientMedicationIdentifierDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int PatientMedicationId { get; private set; }

        public PatientMedicationIdentifierQuery()
        {
        }

        public PatientMedicationIdentifierQuery(int patientId, int patientMedicationId) : this()
        {
            PatientId = patientId;
            PatientMedicationId = patientMedicationId;
        }
    }
}
