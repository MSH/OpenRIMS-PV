using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A meta column representation DTO - EXPANDED
    /// </summary>
    [DataContract()]
    public class MetaColumnExpandedDto : MetaColumnDetailDto
    {
        /// <summary>
        /// The operators that are applicable to the attribute type
        /// </summary>
        [DataMember]
        public ICollection<MetaOperatorDto> Operators { get; set; } = new List<MetaOperatorDto>();
    }
}
