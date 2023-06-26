using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A user representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class UserIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the user
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The username of the user
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// The first name of the user
        /// </summary>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the user
        /// </summary>
        [DataMember]
        public string LastName { get; set; }
    }
}
