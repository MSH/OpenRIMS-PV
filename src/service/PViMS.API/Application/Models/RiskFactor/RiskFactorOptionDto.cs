using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// The stratification options associated to a risk factor
    /// </summary>
    [DataContract()]
    public class RiskFactorOptionDto
    {
        /// <summary>
        /// The name of the risk factor option
        /// </summary>
        [DataMember]
        public string OptionName { get; set; }

        /// <summary>
        /// The criteria for determining if the risk factor option is met
        /// </summary>
        [DataMember]
        public string Criteria { get; set; }

        /// <summary>
        /// The name of the risk factor option as it is displayed to the user
        /// </summary>
        [DataMember]
        public string Display { get; set; }
    }
}
