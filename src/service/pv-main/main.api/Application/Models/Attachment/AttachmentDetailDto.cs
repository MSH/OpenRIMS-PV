using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// An attachment representation DTO - FULL DETAILS
    /// </summary>
    [DataContract()]
    public class AttachmentDetailDto : AttachmentIdentifierDto
    {
        /// <summary>
        /// A description of the attachment
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// The type of the attachment
        /// </summary>
        [DataMember]
        public string AttachmentType { get; set; }

        /// <summary>
        /// Details of the household creation
        /// </summary>
        [DataMember]
        public string CreatedDetail { get; set; }

        /// <summary>
        /// Details of the last update to the household
        /// </summary>
        [DataMember]
        public string UpdatedDetail { get; set; }
    }

}
