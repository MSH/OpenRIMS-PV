using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A meta report representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class MetaReportIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the meta report
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The globally unique identifier for the meta report
        /// </summary>
        [DataMember]
        public Guid MetaReportGuid { get; set; }

        /// <summary>
        /// The name of the meta report
        /// </summary>
        [DataMember]
        public string ReportName { get; set; }
    }
}
