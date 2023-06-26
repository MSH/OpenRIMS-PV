using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class ConfigForUpdateDto
    {
        /// <summary>
        /// The value of the configuration
        /// </summary>
        [StringLength(100)]
        public string ConfigValue { get; set; }
    }
}
