namespace PVIMS.API.Models.Parameters
{
    public class LabTestUnitResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like lab test units returned in  
        /// Default order attribute is Description  
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "Description";

        /// <summary>
        /// Provide the ability to filter lab test units using a partial or full search term
        /// </summary>
        public string SearchTerm { get; set; } = "";
    }
}
