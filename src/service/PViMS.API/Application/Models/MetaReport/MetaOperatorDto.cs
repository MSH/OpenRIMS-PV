using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A meta operator representation DTO
    /// </summary>
    [DataContract()]
    public class MetaOperatorDto
    {
        /// <summary>
        /// The name of the operator
        /// </summary>
        [DataMember]
        public string OperatorName { get; set; }

        /// <summary>
        /// The operator value
        /// </summary>
        [DataMember]
        public string OperatorValue { get; set; }
    }
}
