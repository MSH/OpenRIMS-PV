using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A cohort group representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class CohortGroupIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the cohort group
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the cohort
        /// </summary>
        [DataMember]
        public string CohortName { get; set; }

        /// <summary>
        /// The code of the cohort
        /// </summary>
        [DataMember]
        public string CohortCode { get; set; }

        /// <summary>
        /// The start date of the cohort
        /// </summary>
        [DataMember]
        public string StartDate { get; set; }

    }
}
