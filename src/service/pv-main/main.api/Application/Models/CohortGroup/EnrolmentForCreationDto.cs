using System;

namespace OpenRIMS.PV.Main.API.Models
{
    public class EnrolmentForCreationDto
    {
        /// <summary>
        /// The unique id of the cohort group
        /// </summary>
        public long CohortGroupId { get; set; }

        /// <summary>
        /// The date of the enrolment
        /// </summary>
        public DateTime EnrolmentDate { get; set; }
    }
}
