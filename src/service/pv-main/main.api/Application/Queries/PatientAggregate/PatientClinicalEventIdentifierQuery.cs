using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class PatientClinicalEventIdentifierQuery
        : IRequest<PatientClinicalEventIdentifierDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int PatientClinicalEventId { get; private set; }

        public PatientClinicalEventIdentifierQuery()
        {
        }

        public PatientClinicalEventIdentifierQuery(int patientId, int patientClinicalEventId) : this()
        {
            PatientId = patientId;
            PatientClinicalEventId = patientClinicalEventId;
        }
    }
}
