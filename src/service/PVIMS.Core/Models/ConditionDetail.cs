using PVIMS.Core.CustomAttributes;
using System;

namespace PVIMS.Core.Models
{
    public class ConditionDetail: ExtendableDetail
    {
        public int? MeddraTermId { get; set; }

        public string ConditionSource { get; set; }

        public DateTime OnsetDate { get; set; }

        public DateTime? OutcomeDate { get; set; }

        public string TreatmentOutcome { get; set; }

        public string CaseNumber { get; set; }

        public string Comments { get; set; }
    }
}
