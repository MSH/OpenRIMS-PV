using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A dataset element representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class DatasetElementIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the dataset element
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The globally unique identifier for the dataset element
        /// </summary>
        [DataMember]
        public Guid DatasetElementGuid { get; set; }

        /// <summary>
        /// The name of the dataset element
        /// </summary>
        [DataMember]
        public string ElementName { get; set; }

        /// <summary>
        /// The type of field
        /// </summary>
        [DataMember]
        public string FieldTypeName { get; set; }
    }
}
