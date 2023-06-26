using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A representation of the list of medications associated to a condition group
    /// </summary>
    [DataContract()]
    public class ConditionMedicationDto
    {
        /// <summary>
        /// The unique id of the condition medication
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The unique id of the product id
        /// </summary>
        [DataMember]
        public int ProductId { get; set; }

        /// <summary>
        /// The name of the medication
        /// </summary>
        [DataMember]
        public string MedicationName { get; set; }
    }
}
