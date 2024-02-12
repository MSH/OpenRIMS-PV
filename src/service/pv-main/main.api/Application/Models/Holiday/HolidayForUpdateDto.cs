using System;
using System.ComponentModel.DataAnnotations;

namespace OpenRIMS.PV.Main.API.Models
{
    public class HolidayForUpdateDto
    {
        /// <summary>
        /// The date of the holiday
        /// </summary>
        public DateTime HolidayDate { get; set; }

        /// <summary>
        /// The description of the holiday
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Description { get; set; }
    }
}
