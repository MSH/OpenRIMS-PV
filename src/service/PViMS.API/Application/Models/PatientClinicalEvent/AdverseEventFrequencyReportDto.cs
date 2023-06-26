using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A dto representing the output for an adverse event report
    /// </summary>
    [DataContract()]
    public class AdverseEventFrequencyReportDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The reporting period
        /// </summary>
        [DataMember]
        public string PeriodDisplay { get; set; }

        /// <summary>
        /// Meddra term
        /// </summary>
        [DataMember]
        public string SystemOrganClass { get; set; }

        /// <summary>
        /// The total number of patients for grade 1
        /// </summary>
        [DataMember]
        public int Grade1Count { get; set; }

        /// <summary>
        /// The total number of patients for grade 2
        /// </summary>
        [DataMember]
        public int Grade2Count { get; set; }

        /// <summary>
        /// The total number of patients for grade 3
        /// </summary>
        [DataMember]
        public int Grade3Count { get; set; }

        /// <summary>
        /// The total number of patients for grade 4
        /// </summary>
        [DataMember]
        public int Grade4Count { get; set; }


        /// <summary>
        /// The total number of patients for grade 5
        /// </summary>
        [DataMember]
        public int Grade5Count { get; set; }

        /// <summary>
        /// The total number of patients for grade unknown
        /// </summary>
        [DataMember]
        public int GradeUnknownCount { get; set; }

    }

}
