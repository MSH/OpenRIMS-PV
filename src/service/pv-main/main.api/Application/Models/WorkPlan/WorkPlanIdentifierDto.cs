using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// A work plan representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class WorkPlanIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the work plan
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The name of the work plan
        /// </summary>
        [DataMember]
        public string WorkPlanName { get; set; }
    }
}
