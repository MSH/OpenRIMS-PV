using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A meta report representation DTO - EXPANDED
    /// </summary>
    [DataContract()]
    public class MetaReportExpandedDto : MetaReportDetailDto
    {
        /// <summary>
        /// The associated core entity for the report
        /// </summary>
        [DataMember]
        public MetaTableExpandedDto CoreMetaTable { get; set; }

        /// <summary>
        /// The attributes associated to the report
        /// </summary>
        [DataMember]
        public ICollection<MetaAttributeDto> Attributes { get; set; } = new List<MetaAttributeDto>();

        /// <summary>
        /// The filters associated to the report
        /// </summary>
        [DataMember]
        public ICollection<MetaFilterDto> Filters { get; set; } = new List<MetaFilterDto>();
    }
}
