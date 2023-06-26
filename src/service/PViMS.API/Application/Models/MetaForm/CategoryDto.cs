using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models.MetaForm
{
    /// <summary>
    /// A meta form category representation containing a list of categories available in a form
    /// </summary>
    [DataContract()]
    public class CategoryDto
    {
        /// <summary>
        /// The name of the category
        /// </summary>
        [DataMember]
        public string CategoryName { get; set; }

        /// <summary>
        /// The display type of the category
        /// </summary>
        [DataMember]
        public string DisplayType { get; set; }

        /// <summary>
        /// A list of elements that have been associated to this category
        /// </summary>
        [DataMember]
        public ICollection<ElementDto> Elements { get; set; } = new List<ElementDto>();
    }
}
