using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.Core.ValueTypes;

namespace OpenRIMS.PV.Main.API.Models.Parameters
{
    public class AdverseEventFrequencyReportResourceParameters : BaseReportResourceParameters
    {
        /// <summary>
        /// Filter reports by criteria
        /// </summary>
        [ValidEnumValue]
        public FrequencyCriteria FrequencyCriteria { get; set; } = FrequencyCriteria.Annual;
    }
}
