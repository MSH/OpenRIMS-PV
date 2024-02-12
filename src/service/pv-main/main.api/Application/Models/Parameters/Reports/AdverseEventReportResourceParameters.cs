using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.Core.ValueTypes;

namespace OpenRIMS.PV.Main.API.Models.Parameters
{
    public class AdverseEventReportResourceParameters : BaseReportResourceParameters
    {
        /// <summary>
        /// Filter reports by criteria
        /// </summary>
        [ValidEnumValue]
        public AdverseEventStratifyCriteria AdverseEventStratifyCriteria { get; set; } = AdverseEventStratifyCriteria.AgeGroup;

        /// <summary>
        /// Filter reports by age group
        /// </summary>
        [ValidEnumValue]
        public AgeGroupCriteria AgeGroupCriteria { get; set; } = AgeGroupCriteria.FifetyFiveToSixtyFour;

        /// <summary>
        /// Filter reports by gender custom attribute
        /// </summary>
        public string GenderId { get; set; } = "";

        /// <summary>
        /// Filter reports by regimen custom attribute
        /// </summary>
        public string RegimenId { get; set; } = "";

        /// <summary>
        /// Filter reports by organisation unit
        /// </summary>
        public int OrganisationUnitId { get; set; } = 0;

        /// <summary>
        /// Filter reports by outcome custom attribute
        /// </summary>
        public string OutcomeId { get; set; } = "";

        /// <summary>
        /// Filter reports by is serious custom attribute
        /// </summary>
        public string IsSeriousId { get; set; } = "";

        /// <summary>
        /// Filter reports by seriousness custom attribute
        /// </summary>
        public string SeriousnessId { get; set; } = "";

        /// <summary>
        /// Filter reports by classification custom attribute
        /// </summary>
        public string ClassificationId { get; set; } = "";
    }
}
