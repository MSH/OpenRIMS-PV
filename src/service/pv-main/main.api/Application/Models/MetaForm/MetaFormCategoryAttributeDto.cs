using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A meta form element representation containing a list of elements available in a form category
    /// </summary>
    [DataContract()]
    public class MetaFormCategoryAttributeDto
    {
        /// <summary>
        /// The unique id of the category attribute
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The globally unique identifier for the meta form category attribute
        /// </summary>
        [DataMember]
        public Guid MetaFormCategoryAttributeGuid { get; set; }

        /// <summary>
        /// The unique id of the custom attribute
        /// </summary>
        [DataMember]
        public string AttributeId { get; set; }

        /// <summary>
        /// The name of the custom attribute
        /// </summary>
        [DataMember]
        public string AttributeName { get; set; }

        /// <summary>
        /// Form label associated to the category attribute
        /// </summary>
        [DataMember]
        public string Label { get; set; }

        /// <summary>
        /// Additional help associated to the category attribute
        /// </summary>
        [DataMember]
        public string Help { get; set; }
    }
}
