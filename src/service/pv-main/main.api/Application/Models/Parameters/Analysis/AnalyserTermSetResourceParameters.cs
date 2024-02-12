using System;
using System.Collections.Generic;

namespace OpenRIMS.PV.Main.API.Models.Parameters
{
    public class AnalyserTermSetResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Filter analysis by selected condition
        /// </summary>
        public int ConditionId { get; set; }

        /// <summary>
        /// Filter analysis by selected cohort group
        /// </summary>
        public int CohortGroupId { get; set; }

        /// <summary>
        /// Filter by range
        /// </summary>
        public DateTime SearchFrom { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Filter by range
        /// </summary>
        public DateTime SearchTo { get; set; } = DateTime.MaxValue;

        /// <summary>
        /// Include risk factor options into analysis
        /// </summary>
        public List<string> RiskFactorOptionNames { get; set; }
    }
}
