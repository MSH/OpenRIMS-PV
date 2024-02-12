using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A meta table representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class MetaTableDetailDto : MetaTableIdentifierDto
    {
        /// <summary>
        /// The name of the table
        /// </summary>
        [DataMember]
        public string TableName { get; set; }

        /// <summary>
        /// A friendly name for the table
        /// </summary>
        [DataMember]
        public string FriendlyName { get; set; }

        /// <summary>
        /// A friendly description for the table
        /// </summary>
        [DataMember]
        public string FriendlyDescription { get; set; }

        /// <summary>
        /// The type of table
        /// </summary>
        [DataMember]
        public string TableType { get; set; }
    }
}
