using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A meta column representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class MetaColumnDetailDto : MetaColumnIdentifierDto
    {
        /// <summary>
        /// The name of the column
        /// </summary>
        [DataMember]
        public string TableName { get; set; }

        /// <summary>
        /// The name of the column
        /// </summary>
        [DataMember]
        public string ColumnName { get; set; }

        /// <summary>
        /// Is this an identity column
        /// </summary>
        [DataMember]
        public bool IsIdentity { get; set; }

        /// <summary>
        /// Is this column nullable
        /// </summary>
        [DataMember]
        public bool IsNullable { get; set; }

        /// <summary>
        /// The type of the column
        /// </summary>
        [DataMember]
        public string ColumnType { get; set; }

        /// <summary>
        /// Column range of values
        /// </summary>
        [DataMember]
        public string Range { get; set; }
    }
}
