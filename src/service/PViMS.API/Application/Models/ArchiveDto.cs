using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class ArchiveDto
    {
        /// <summary>
        /// The reason for archiving the resource
        /// </summary>
        public string Reason { get; set; }
    }
}
