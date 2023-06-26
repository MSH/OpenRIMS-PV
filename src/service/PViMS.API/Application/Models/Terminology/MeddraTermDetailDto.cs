using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A meddra term representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class MeddraTermDetailDto : MeddraTermIdentifierDto
    {
        /// <summary>
        /// The parent MedDra term
        /// </summary>
        [DataMember]
        public string ParentMedDraTerm { get; set; }

        /// <summary>
        /// The level of the MedDra term
        /// </summary>
        [DataMember]
        public string MedDraTermType { get; set; }

        /// <summary>
        /// The version number of this MedDra term
        /// </summary>
        [DataMember]
        public string MedDraVersion { get; set; }

        /// <summary>
        /// A list of custom attributes associated to the patient
        /// </summary>
        [DataMember]
        public ICollection<MeddraTermIdentifierDto> Children { get; set; } = new List<MeddraTermIdentifierDto>();
    }
}
