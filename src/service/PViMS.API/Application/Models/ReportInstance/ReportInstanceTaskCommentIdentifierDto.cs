using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A report instance task comment representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class ReportInstanceTaskCommentIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the report instance task comment
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The comment that is added to the task
        /// </summary>
        [DataMember]
        public string Comment { get; set; }

    }
}
