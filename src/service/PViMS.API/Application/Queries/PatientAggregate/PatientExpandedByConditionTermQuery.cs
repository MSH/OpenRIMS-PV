using MediatR;
using PVIMS.API.Models;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Queries.PatientAggregate
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
