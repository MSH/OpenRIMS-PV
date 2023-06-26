using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class DatasetForUpdateDto
    {
        /// <summary>
        /// The name of the dataset
        /// </summary>
        [Required]
        [StringLength(50)]
        public string DatasetName { get; set; }

        /// <summary>
        /// A detailed definition of the dataset
        /// </summary>
        [StringLength(250)]
        public string Help { get; set; }
    }
}
