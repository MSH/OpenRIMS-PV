using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// An attachment representation DTO - IDENTIFIER ONLY
    /// </summary>
    [DataContract()]
    public class AttachmentIdentifierDto : LinkedResourceBaseDto
    {
        /// <summary>
        /// The unique Id of the attachment
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The file name of the attachment
        /// </summary>
        [DataMember]
        public string FileName { get; set; }
    }
}
