using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A report instance medication representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class ReportInstanceMedicationIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the medication
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The unique id of the report instance
        /// </summary>
        [DataMember]
        public int ReportInstanceId { get; set; }

        /// <summary>
        /// The globally unique identifier for the medication
        /// </summary>
        [DataMember]
        public Guid ReportInstanceMedicationGuid { get; set; }
    }
}
