using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A configuration representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class ConfigIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the configuration
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The type of configuration
        /// </summary>
        [DataMember]
        public string ConfigType { get; set; }

        /// <summary>
        /// The value of the configuration
        /// </summary>
        [DataMember]
        public string ConfigValue { get; set; }
    }
}
