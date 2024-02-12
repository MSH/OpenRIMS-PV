using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A risk factor representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class RiskFactorDetailDto : RiskFactorIdentifierDto
    {
        /// <summary>
        /// The criteria for determining if the risk factor is met
        /// </summary>
        [DataMember]
        public string Criteria { get; set; }

        /// <summary>
        /// The name of the risk factor as it is displayed to the user
        /// </summary>
        [DataMember]
        public string Display { get; set; }

        /// <summary>
        /// Is this a system defined element
        /// </summary>
        [DataMember]
        public string System { get; set; }

        /// <summary>
        /// A list of selection values associated to the attribute (if applicable)
        /// </summary>
        [DataMember]
        public ICollection<RiskFactorOptionDto> Options { get; set; } = new List<RiskFactorOptionDto>();
    }
}
