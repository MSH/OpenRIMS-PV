using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class PatientMedicationDetailQuery
        : IRequest<PatientMedicationDetailDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int PatientMedicationId { get; private set; }

        public PatientMedicationDetailQuery()
        {
        }

        public PatientMedicationDetailQuery(int patientId, int patientMedicationId) : this()
        {
            PatientId = patientId;
            PatientMedicationId = patientMedicationId;
        }
    }
}
