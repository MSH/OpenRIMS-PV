using PVIMS.API.Infrastructure.Attributes;
using PVIMS.Core.ValueTypes;

namespace PVIMS.API.Models.Parameters
{
    public class CausalityReportResourceParameters : BaseReportResourceParameters
    {
        /// <summary>
        /// Filter reports by facility id
        /// </summary>
        public int FacilityId { get; set; } = 0;

        /// <summary>
        /// Filter reports by criteria
        /// </summary>
        [ValidEnumValue]
        public CausalityCriteria CausalityCriteria { get; set; } = CausalityCriteria.CausalitySet;
    }
}
