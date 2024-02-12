using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace OpenRIMS.PV.Main.API.Models
{
    public class PatientAttachmentForCreationDto
    {
        /// <summary>
        /// A description of the file
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The posted attachment
        /// </summary>
        [Required]
        public IFormFile Attachment { get; set; }
    }
}
