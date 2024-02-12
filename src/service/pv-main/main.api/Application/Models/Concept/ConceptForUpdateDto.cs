using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.API.Models.ValueTypes;

namespace OpenRIMS.PV.Main.API.Models
{
    public class ConceptForUpdateDto
    {
        /// <summary>
        /// The name of the concept
        /// </summary>
        public string ConceptName { get; set; }

        /// <summary>
        /// The strength of the concept
        /// </summary>
        public string Strength { get; set; }

        /// <summary>
        /// The form of the concept
        /// </summary>
        public string MedicationForm { get; set; }

        /// <summary>
        /// Is this concept currently active
        /// </summary>
        [ValidEnumValue]
        public YesNoValueType Active { get; set; }
    }
}
