using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A user representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class UserDetailDto : UserIdentifierDto
    {
        /// <summary>
        /// The email address of the user
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// Is this user allowed to download ther analytical dataset
        /// </summary>
        [DataMember]
        public string AllowDatasetDownload { get; set; }

        /// <summary>
        /// Is this user currently active
        /// </summary>
        [DataMember]
        public string Active { get; set; }

        /// <summary>
        /// The roles the user has been allocated to
        /// </summary>
        [DataMember]
        public string[] Roles { get; set; }

        /// <summary>
        /// The facilities the user has been allocated to
        /// </summary>
        [DataMember]
        public ICollection<UserFacilityDto> Facilities { get; set; } = new List<UserFacilityDto>();
    }
}
