using MediatR;
using PVIMS.API.Models;
using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.CohortGroupAggregate
{
    [DataContract]
    public class AddCohortGroupCommand
        : IRequest<CohortGroupDetailDto>
    {
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

        public AddCohortGroupCommand()
        {
        }

        public AddCohortGroupCommand(string cohortName, string cohortCode, DateTime startDate, DateTime? finishDate, string conditionName) : this()
        {
            CohortName = cohortName;
            CohortCode = cohortCode;
            StartDate = startDate;
            FinishDate = finishDate;
            ConditionName = conditionName;
        }
    }
}
