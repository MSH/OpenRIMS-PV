using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.Core.ValueTypes;

namespace OpenRIMS.PV.Main.API.Models.Parameters
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
