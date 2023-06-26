namespace PVIMS.API.Models.Parameters
{
    public class PatientMedicationReportResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Provide the ability to filter by medication name using a partial or full search term
        /// </summary>
        public string SearchTerm { get; set; } = "";
    }
}
