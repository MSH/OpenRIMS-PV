using System.ComponentModel.DataAnnotations;

namespace OpenRIMS.PV.Main.API.Models
{
    public class ArchiveDto
    {
        /// <summary>
        /// The reason for archiving the resource
        /// </summary>
        public string Reason { get; set; }
    }
}
