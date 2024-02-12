using System;
using System.ComponentModel.DataAnnotations;

namespace OpenRIMS.PV.Main.API.Models
{
    public class EncounterForCreationDto
    {
        /// <summary>
        /// The type of encounter
        /// </summary>
        [Required]
        public int EncounterTypeId { get; set; }

        /// <summary>
        /// The priority of the encounter
        /// </summary>
        [Required]
        public int PriorityId { get; set; }

        /// <summary>
        /// The date of the encounter
        /// </summary>
        public DateTime EncounterDate { get; set; }

        /// <summary>
        /// General notes assigned to the encounter
        /// </summary>
        [StringLength(500)]
        public string Notes { get; set; }
    }
}
