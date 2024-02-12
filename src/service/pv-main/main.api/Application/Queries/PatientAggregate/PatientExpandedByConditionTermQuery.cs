using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Queries.PatientAggregate
{
    [DataContract]
    public class PatientExpandedByConditionTermQuery
        : IRequest<PatientExpandedDto>
    {
        [DataMember]
        public string CaseNumber { get; private set; }

        public PatientExpandedByConditionTermQuery()
        {
        }

        public PatientExpandedByConditionTermQuery(string caseNumber) : this()
        {
            CaseNumber = caseNumber;
        }
    }
}
