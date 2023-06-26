using PVIMS.API.Models.MetaForm;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A meta form representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class MetaFormDetailDto : MetaFormIdentifierDto
    {
        /// <summary>
        /// Is this a system form
        /// </summary>
        [DataMember]
        public string System { get; set; }

        /// <summary>
        /// Current version number of the form
        /// </summary>
        [DataMember]
        public string Version { get; set; }

        /// <summary>
        /// A list of selection values associated to the attribute (if applicable)
        /// </summary>
        [DataMember]
        public ICollection<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    }
}
