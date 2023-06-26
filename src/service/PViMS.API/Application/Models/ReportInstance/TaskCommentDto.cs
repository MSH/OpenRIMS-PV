using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A comment that can be allocated by a system user to a task
    /// </summary>
    [DataContract()]
    public class TaskCommentDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique identifier of the comment
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The comment noted by the user
        /// </summary>
        [DataMember]
        public string Comment { get; set; }

        /// <summary>
        /// The initials of the user that created the comment
        /// </summary>
        [DataMember]
        public string CreatedInitials { get; set; }

        /// <summary>
        /// Details of the comment creation
        /// </summary>
        [DataMember]
        public string CreatedDetail { get; set; }

        /// <summary>
        /// Details of the last update to the comment
        /// </summary>
        [DataMember]
        public string UpdatedDetail { get; set; }
    }
}
