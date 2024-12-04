using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Models
{
    /// <summary>
    /// An artifact representation containing detaails of the system file representing the artifact
    /// </summary>
    [DataContract()]
    public class ArtifactDto
    {
        /// <summary>
        /// The file name of the artifact
        /// </summary>
        [DataMember]
        public string FileName { get; set; }

        /// <summary>
        /// The relative path of the artifact
        /// </summary>
        [DataMember]
        public string RelativePath { get; set; }

        /// <summary>
        /// The path of the artifact
        /// </summary>
        [DataMember]
        public string Path { get; set; }

        /// <summary>
        /// The mime type of the artifact
        /// </summary>
        [DataMember]
        public string MimeType { get; set; }

        /// <summary>
        /// The full path of the artifact
        /// </summary>
        [DataMember]
        public string FullPath
        {
            get { return $"{Path}{FileName}"; }
        }
    }
}
