using OpenRIMS.PV.Main.API.Models.MetaForm;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A meta form representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class MetaFormExpandedDto : MetaFormDetailDto
    {
        /// <summary>
        /// A list of selection values associated to the attribute (if applicable)
        /// </summary>
        [DataMember]
        public ICollection<MetaFormCategoryDto> Categories { get; set; } = new List<MetaFormCategoryDto>();
    }
}
