using MediatR;
using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.CohortGroupAggregate
{
    [DataContract]
    public class ChangeCohortGroupDetailsCommand
        : IRequest<bool>
    {
        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public string CohortName { get; private set; }

        [DataMember]
        public string CohortCode { get; private set; }

        [DataMember]
        public DateTime StartDate { get; private set; }

        [DataMember]
        public DateTime? FinishDate { get; private set; }

        [DataMember]
        public string ConditionName { get; private set; }

        public ChangeCohortGroupDetailsCommand()
        {
        }

        public ChangeCohortGroupDetailsCommand(int id, string cohortName, string cohortCode, DateTime startDate, DateTime? finishDate, string conditionName) : this()
        {
            Id = id;
            CohortName = cohortName;
            CohortCode = cohortCode;
            StartDate = startDate;
            FinishDate = finishDate;
            ConditionName = conditionName;
        }
    }
}
