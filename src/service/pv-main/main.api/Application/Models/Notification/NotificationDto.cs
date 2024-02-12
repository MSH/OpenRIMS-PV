using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A notification representation DTO
    /// </summary>
    [DataContract()]
    public class NotificationDto 
    {
        /// <summary>
        /// The summary of the notification
        /// </summary>
        [DataMember]
        public string Summary { get; set; }

        /// <summary>
        /// Additional details of the notification
        /// </summary>
        [DataMember]
        public string Detail { get; set; }

        /// <summary>
        /// The type of notification created
        /// </summary>
        [DataMember]
        public string NotificationType { get; set; }

        /// <summary>
        /// The classification of notification created
        /// </summary>
        [DataMember]
        public string NotificationClassification { get; set; }

        /// <summary>
        /// Details of the notification creation
        /// </summary>
        [DataMember]
        public string CreatedDetail { get; set; }

        /// <summary>
        /// The contextual route to access the task list
        /// </summary>
        [DataMember]
        public string ContextRoute { get; set; }
    }
}
