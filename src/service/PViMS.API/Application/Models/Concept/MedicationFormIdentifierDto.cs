using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A medication form representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class MedicationFormIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the medication form
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the medication form
        /// </summary>
        [DataMember]
        public string FormName { get; set; }
    }
}
