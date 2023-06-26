using PVIMS.Core.Models;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A dto representing meddra terms available for analysis over an analysis period - WITH RESULTS
    /// </summary>
    [DataContract()]
    public class AnalyserTermDetailDto : AnalyserTermIdentifierDto
    {
        /// <summary>
        /// A list of analysis results per medication for the associated term
        /// </summary>
        [DataMember]
        public ICollection<AnalyserResultDto> Results { get; set; } = new List<AnalyserResultDto>();

        /// <summary>
        /// Unadjusted relative risk results ready for chart representation
        /// </summary>
        [DataMember]
        public SeriesValueList[] RelativeRiskSeries { get; set; }

        /// <summary>
        /// Exposed case results ready for chart representation
        /// </summary>
        [DataMember]
        public SeriesValueList[] ExposedCaseSeries { get; set; }
    }
}
