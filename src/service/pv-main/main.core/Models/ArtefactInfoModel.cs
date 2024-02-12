using System;

namespace OpenRIMS.PV.Main.Core.Models
{
    public class ArtefactInfoModel
    {
        public string FileName { get; set; }
        public string RelativePath { get; set; }
        public string Path { get; set; }
        public string MimeType { get; set; }

        public string FullPath
        {
            get { return $"{Path}{FileName}"; }
        }
    }
}
