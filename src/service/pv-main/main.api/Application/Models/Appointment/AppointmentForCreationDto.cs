using System;

namespace OpenRIMS.PV.Main.API.Models
{
    public class AppointmentForCreationDto
    {
        /// <summary>
        /// The date of the appointment
        /// </summary>
        public DateTime AppointmentDate { get; set; }

        /// <summary>
        /// The reason for the appointment
        /// </summary>
        public string Reason { get; set; }
    }
}
