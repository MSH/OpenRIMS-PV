using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A meta dependency representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class MetaDependencyDetailDto : MetaDependencyIdentifierDto
    {
        /// <summary>
        /// The name of the parent column
        /// </summary>
        [DataMember]
        public string ParentColumnName { get; set; }

        /// <summary>
        /// The name of the reference column
        /// </summary>
        [DataMember]
        public string ReferenceColumnName { get; set; }

        /// <summary>
        /// The name of the parent table
        /// </summary>
        [DataMember]
        public string ParentTableName { get; set; }

        /// <summary>
        /// The name of the reference table
        /// </summary>
        [DataMember]
        public string ReferenceTableName { get; set; }
    }
}
