using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A selection data item representation containing a possible set of values associated to a selection  
    /// custom attribute type
    /// </summary>
    [DataContract()]
    public class SelectionDataItemDto
    {
        /// <summary>
        /// The key for the selection value
        /// </summary>
        [DataMember]
        public string SelectionKey { get; set; }

        /// <summary>
        /// The selection value associated to the key
        /// </summary>
        [DataMember]
        public string Value { get; set; }
    }
}
