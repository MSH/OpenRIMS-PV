using System;
using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public partial class CohortGroup : EntityBase
    {
        public CohortGroup()
        {
            CohortGroupEnrolments = new HashSet<CohortGroupEnrolment>();
            EncounterTypeWorkPlans = new HashSet<EncounterTypeWorkPlan>();
        }

        public CohortGroup(string cohortName, string cohortCode, Condition condition, DateTime startDate, DateTime? finishDate)
        {
            CohortName = cohortName;
            CohortCode = cohortCode;

            ConditionId = condition.Id;
            Condition = condition;
            
            StartDate = startDate;
            FinishDate = finishDate;

            MaxEnrolment = 0;
            MinEnrolment = 0;
            LastPatientNo = 0;
        }


        public string CohortName { get; private set; }
        public string CohortCode { get; private set; }
        public int LastPatientNo { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? FinishDate { get; private set; }
        public int MinEnrolment { get; private set; }
        public int MaxEnrolment { get; private set; }

        public int? ConditionId { get; private set; }
        public virtual Condition Condition { get; private set; }

        public virtual ICollection<CohortGroupEnrolment> CohortGroupEnrolments { get; set; }
        public virtual ICollection<EncounterTypeWorkPlan> EncounterTypeWorkPlans { get; set; }

        public string DisplayName
        {
            get
            {
                return $"{CohortName} ({CohortCode})";
            }
        }

        public void ChangeDetails(string cohortName, string cohortCode, Condition condition, DateTime startDate, DateTime? finishDate)
        {
            CohortName = cohortName;
            CohortCode = cohortCode;
            Condition = condition;
            StartDate = startDate;
            FinishDate = finishDate;
        }

    }
}
