using PVIMS.API.Infrastructure.Attributes;
using PVIMS.Core.ValueTypes;

namespace PVIMS.API.Models.Parameters
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
