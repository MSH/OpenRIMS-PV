using OpenRIMS.PV.Main.API.Infrastructure.Attributes;
using OpenRIMS.PV.Main.API.Models.ValueTypes;
using System;

namespace OpenRIMS.PV.Main.API.Models
{
    public class AppointmentForUpdateDto
    {
        /// <summary>
        /// The date of the appointment
        /// </summary>
        public DateTime AppointmentDate { get; set; }

        /// <summary>
        /// The reason for the appointment
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Has the appointment been cancelled
        /// </summary>
        [ValidEnumValue]
        public YesNoValueType Cancelled { get; set; }

        /// <summary>
        /// The reason for the cancellation if it has been cancelled
        /// </summary>
        public string CancellationReason { get; set; }
    }
}
