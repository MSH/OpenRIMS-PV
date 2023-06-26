using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
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
