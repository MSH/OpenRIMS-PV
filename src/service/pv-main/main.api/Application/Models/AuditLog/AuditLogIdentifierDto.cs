using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// An audit log representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class AuditLogIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the audit log
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The type of audit transaction
        /// </summary>
        [DataMember]
        public string AuditType { get; set; }
    }
}
