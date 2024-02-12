using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A dto representing the output for an adverse event report
    /// </summary>
    [DataContract()]
    public class AdverseEventReportDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The adverse event being reported on
        /// </summary>
        [DataMember]
        public string AdverseEvent { get; set; }

        /// <summary>
        /// Stratification criteria
        /// </summary>
        [DataMember]
        public string StratificationCriteria { get; set; }

        /// <summary>
        /// The total number of patients meeting the criteria
        /// </summary>
        [DataMember]
        public int PatientCount { get; set; }
    }

}
