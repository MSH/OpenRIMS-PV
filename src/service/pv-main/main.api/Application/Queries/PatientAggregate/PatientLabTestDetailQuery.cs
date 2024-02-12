using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class PatientLabTestDetailQuery
        : IRequest<PatientLabTestDetailDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public int PatientLabTestId { get; private set; }

        public PatientLabTestDetailQuery()
        {
        }

        public PatientLabTestDetailQuery(int patientId, int patientLabTestId) : this()
        {
            PatientId = patientId;
            PatientLabTestId = patientLabTestId;
        }
    }
}