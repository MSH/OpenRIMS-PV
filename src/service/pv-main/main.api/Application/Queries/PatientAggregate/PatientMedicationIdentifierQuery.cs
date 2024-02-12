using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.PatientAggregate
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
