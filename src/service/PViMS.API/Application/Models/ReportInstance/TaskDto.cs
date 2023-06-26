using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A representation of a task that can be allocated to a report instance
    /// </summary>
    [DataContract()]
    public class TaskDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique identifier of the task
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The data source necessitating the task
        /// </summary>
        [DataMember]
        public string Source { get; set; }

        /// <summary>
        /// A detailed description of the task
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// The type of task that has been allocated
        /// </summary>
        [DataMember]
        public string TaskType { get; set; }

        /// <summary>
        /// The current status of the task
        /// </summary>
        [DataMember]
        public string TaskStatus { get; set; }

        /// <summary>
        /// The age of the task in days
        /// </summary>
        [DataMember]
        public string TaskAge { get; set; }

        /// <summary>
        /// Details of the task creation
        /// </summary>
        [DataMember]
        public string CreatedDetail { get; set; }

        /// <summary>
        /// Details of the last update to the task
        /// </summary>
        [DataMember]
        public string UpdatedDetail { get; set; }

        /// <summary>
        /// All events that have taken place against this report instance
        /// </summary>
        [DataMember]
        public ICollection<TaskCommentDto> Comments { get; set; } = new List<TaskCommentDto>();
    }
}
