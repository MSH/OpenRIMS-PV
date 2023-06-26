using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A meta table representation DTO - EXPANDED
    /// </summary>
    [DataContract()]
    public class MetaTableExpandedDto : MetaTableDetailDto
    {
        /// <summary>
        /// The columns associated to this meta table
        /// </summary>
        [DataMember]
        public ICollection<MetaColumnExpandedDto> Columns { get; set; } = new List<MetaColumnExpandedDto>();
    }
}
