using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A contact detail representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class ContactIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the contact detail
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The type of contact detail
        /// </summary>
        [DataMember]
        public string ContactType { get; set; }

        /// <summary>
        /// The first name of the contact at the organisation
        /// </summary>
        [DataMember]
        public string ContactFirstName { get; set; }

        /// <summary>
        /// The last name of the contact at the organisation
        /// </summary>
        [DataMember]
        public string ContactLastName { get; set; }

    }
}
