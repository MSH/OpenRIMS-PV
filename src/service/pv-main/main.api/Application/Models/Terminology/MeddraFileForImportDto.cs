using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace OpenRIMS.PV.Main.API.Models
{
    public class MeddraFileForImportDto
    {
        /// <summary>
        /// The source file for import
        /// </summary>
        [Required]
        public IFormFile Source { get; set; }
    }
}
