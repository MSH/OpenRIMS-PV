using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A report instance representation DTO including events and comments collections
    /// </summary>
    [DataContract()]
    public class ReportInstanceExpandedDto : ReportInstanceDetailDto
    {
        /// <summary>
        /// All events that have taken place against this report instance
        /// </summary>
        [DataMember]
        public ICollection<ActivityExecutionStatusEventDto> Events { get; set; } = new List<ActivityExecutionStatusEventDto>();

        /// <summary>
        /// All tasks that have been allocated to this report instance
        /// </summary>
        [DataMember]
        public ICollection<TaskDto> Tasks { get; set; } = new List<TaskDto>();
    }
}
