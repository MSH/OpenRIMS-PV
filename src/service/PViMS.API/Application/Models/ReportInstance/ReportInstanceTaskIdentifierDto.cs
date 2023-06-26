using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A report instance task representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class ReportInstanceTaskIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the report instance task
        /// </summary>
        [DataMember]
        public int Id { get; set; }
    }
}
