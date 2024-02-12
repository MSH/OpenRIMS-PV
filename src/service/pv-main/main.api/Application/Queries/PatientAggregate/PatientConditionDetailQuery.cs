using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class PatientConditionDetailQuery
        : IRequest<PatientConditionDetailDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int PatientConditionId { get; private set; }

        public PatientConditionDetailQuery()
        {
        }

        public PatientConditionDetailQuery(int patientId, int patientConditionId) : this()
        {
            PatientId = patientId;
            PatientConditionId = patientConditionId;
        }
    }
}
