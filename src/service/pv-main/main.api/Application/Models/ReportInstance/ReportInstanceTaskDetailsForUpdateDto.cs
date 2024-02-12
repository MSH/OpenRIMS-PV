namespace OpenRIMS.PV.Main.API.Models
{
    public class ReportInstanceTaskDetailsForUpdateDto
    {
        /// <summary>
        /// The source of that data quality task
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// A detailed description of the task requirement
        /// </summary>
        public string Description { get; set; }
    }
}
