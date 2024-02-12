namespace OpenRIMS.PV.Main.API.Models.Parameters
{
    public class AnalyserDatasetResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Filter analysis dataset by selected cohort group
        /// </summary>
        public int CohortGroupId { get; set; }
    }
}
