using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A meta filter representation DTO
    /// </summary>
    [DataContract()]
    public class MetaFilterDto
    {
        /// <summary>
        /// The order of the filter
        /// </summary>
        [DataMember]
        public int Index { get; set; }

        /// <summary>
        /// The unique attribute name
        /// </summary>
        [DataMember]
        public string AttributeName { get; set; }

        /// <summary>
        /// The type of column being used for the filter
        /// </summary>
        [DataMember]
        public string ColumnType { get; set; }

        /// <summary>
        /// The operator used in the query
        /// </summary>
        [DataMember]
        public string Operator { get; set; }

        /// <summary>
        /// The relation of the filters
        /// </summary>
        [DataMember]
        public string Relation { get; set; }
    }
}
