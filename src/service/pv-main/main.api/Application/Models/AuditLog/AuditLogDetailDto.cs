using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// An audit log representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class AuditLogDetailDto : AuditLogIdentifierDto
    {
        /// <summary>
        /// The date of the transaction
        /// </summary>
        [DataMember]
        public string ActionDate { get; set; }

        /// <summary>
        /// The details of the transaction
        /// </summary>
        [DataMember]
        public string Details { get; set; }

        /// <summary>
        /// The user who created the transaction
        /// </summary>
        [DataMember]
        public string UserFullName { get; set; }

        /// <summary>
        /// is there a log file requiring download
        /// </summary>
        [DataMember]
        public bool HasLog { get; set; }
    }
}
