using System;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class CohortGroupForUpdateDto
    {
        /// <summary>
        /// The name of the cohort group
        /// </summary>
        public string CohortName { get; set; }

        /// <summary>
        /// The code of the cohort group
        /// </summary>
        public string CohortCode { get; set; }

        /// <summary>
        /// The start date of the cohort group
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end date of the cohort group
        /// </summary>
        public DateTime? FinishDate { get; set; }

        /// <summary>
        /// The primary condition
        /// </summary>
        public string ConditionName { get; set; }
    }
}
