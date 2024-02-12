using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A configuration representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class ConfigDetailDto : ConfigIdentifierDto
    {
        /// <summary>
        /// Details of the household creation
        /// </summary>
        [DataMember]
        public string CreatedDetail { get; set; }

        /// <summary>
        /// Details of the last update to the household
        /// </summary>
        [DataMember]
        public string UpdatedDetail { get; set; }
    }
}
