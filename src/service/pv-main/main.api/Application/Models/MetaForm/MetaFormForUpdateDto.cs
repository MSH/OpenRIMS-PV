namespace OpenRIMS.PV.Main.API.Models
{
    public class MetaFormForUpdateDto
    {
        /// <summary>
        /// The unique id of the cohort group
        /// </summary>
        public int CohortGroupId { get; set; }

        /// <summary>
        /// The name of the form
        /// </summary>
        public string FormName { get; set; }

        /// <summary>
        /// The action name of the form
        /// </summary>
        public string ActionName { get; set; }
    }
}
