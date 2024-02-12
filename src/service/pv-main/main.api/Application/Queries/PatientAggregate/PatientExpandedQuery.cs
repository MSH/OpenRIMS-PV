using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class PatientExpandedQuery
        : IRequest<PatientExpandedDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        public PatientExpandedQuery()
        {
        }

        public PatientExpandedQuery(int patientId) : this()
        {
            PatientId = patientId;
        }
    }
}
