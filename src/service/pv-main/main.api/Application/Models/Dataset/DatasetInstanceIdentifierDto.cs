using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A dataset instance representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class DatasetInstanceIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the dataset instance
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The uniqe identifier for the dataset instance
        /// </summary>
        [DataMember]
        public string DatasetInstanceGuid { get; set; }
    }
}
