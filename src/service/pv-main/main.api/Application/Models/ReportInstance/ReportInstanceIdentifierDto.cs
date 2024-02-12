using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A report instance representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class ReportInstanceIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the report instance
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The globally unique identifier for the report instance
        /// </summary>
        [DataMember]
        public Guid ReportInstanceGuid { get; set; }

        /// <summary>
        /// The globally unique identifier for the source context
        /// </summary>
        [DataMember]
        public Guid ContextGuid { get; set; }

        /// <summary>
        /// The report identifier
        /// </summary>
        [DataMember]
        public string Identifier { get; set; }
    }
}
