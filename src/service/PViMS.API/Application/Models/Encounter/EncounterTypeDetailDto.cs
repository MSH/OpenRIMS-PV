using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// An encounter type representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class EncounterTypeDetailDto : EncounterTypeIdentifierDto
    {
        /// <summary>
        /// The name of the work plan
        /// </summary>
        [DataMember]
        public string WorkPlanName { get; set; }

        /// <summary>
        /// A detailed definition of the encounter type
        /// </summary>
        [DataMember]
        public string Help { get; set; }
    }
}
