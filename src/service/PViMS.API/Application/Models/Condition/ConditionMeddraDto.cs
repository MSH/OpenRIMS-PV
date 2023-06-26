using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A representation of the list of MedDRA terms associated to a condition group
    /// </summary>
    [DataContract()]
    public class ConditionMeddraDto
    {
        /// <summary>
        /// The unique id of the condition meddra term
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The unique id of the meddra term
        /// </summary>
        [DataMember]
        public int TerminologyMedDraId { get; set; }

        /// <summary>
        /// The name of the meddra term
        /// </summary>
        [DataMember]
        public string MedDraTerm { get; set; }
    }
}
