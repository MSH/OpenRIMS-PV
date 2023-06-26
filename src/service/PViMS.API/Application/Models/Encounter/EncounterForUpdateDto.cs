using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.API.Models
{
    public class EncounterForUpdateDto
    {
        /// <summary>
        /// Generic patient notes
        /// </summary>
        [StringLength(1000)]
        public string Notes { get; set; }

        /// <summary>
        /// Encounter elements and associated values
        /// </summary>
        public IDictionary<int, string> Elements { get; set; }
    }
}
