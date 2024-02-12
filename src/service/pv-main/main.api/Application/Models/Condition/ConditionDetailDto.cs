using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    public class ConditionDetailDto : ConditionIdentifierDto
    {
        /// <summary>
        /// Is this a chronic condition
        /// </summary>
        [DataMember]
        public string Chronic { get; set; }

        /// <summary>
        /// A list of lab tests associated to the condition group
        /// </summary>
        [DataMember]
        public ICollection<ConditionLabTestDto> ConditionLabTests { get; set; } = new List<ConditionLabTestDto>();

        /// <summary>
        /// A list of MedDRA terms associated to the condition group
        /// </summary>
        [DataMember]
        public ICollection<ConditionMeddraDto> ConditionMedDras { get; set; } = new List<ConditionMeddraDto>();

        /// <summary>
        /// A list of medications associated to the condition group
        /// </summary>
        [DataMember]
        public ICollection<ConditionMedicationDto> ConditionMedications { get; set; } = new List<ConditionMedicationDto>();

        /// <summary>
        /// A list of cohort groups associated to the condition group
        /// </summary>
        [DataMember]
        public ICollection<CohortGroupIdentifierDto> CohortGroups { get; set; } = new List<CohortGroupIdentifierDto>();
    }
}
