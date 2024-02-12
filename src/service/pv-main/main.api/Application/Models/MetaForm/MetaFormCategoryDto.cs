using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A meta form category representation containing a list of categories available in a form
    /// </summary>
    [DataContract()]
    public class MetaFormCategoryDto
    {
        /// <summary>
        /// The unique id of the category
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the meta table the category is associated to
        /// </summary>
        [DataMember]
        public string MetaTableName { get; set; }

        /// <summary>
        /// The globally unique identifier for the meta form category
        /// </summary>
        [DataMember]
        public Guid MetaFormCategoryGuid { get; set; }

        /// <summary>
        /// The name of the category
        /// </summary>
        [DataMember]
        public string CategoryName { get; set; }

        /// <summary>
        /// Additional help associated to the category
        /// </summary>
        [DataMember]
        public string Help { get; set; }

        /// <summary>
        /// The icon associated to the category
        /// </summary>
        [DataMember]
        public string Icon { get; set; }

        /// <summary>
        /// A list of elements that have been associated to this category
        /// </summary>
        [DataMember]
        public ICollection<MetaFormCategoryAttributeDto> Attributes { get; set; } = new List<MetaFormCategoryAttributeDto>();
    }
}
