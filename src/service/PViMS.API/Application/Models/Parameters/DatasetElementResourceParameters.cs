namespace PVIMS.API.Models.Parameters
{
    public class DatasetElementResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like dataset elements returned in  
        /// Default order attribute is Id  
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "Id";

        /// <summary>
        /// Provide the ability to filter dataset elements by the dataset they are allocated too
        /// </summary>
        public string DatasetName { get; set; } = "";

        /// <summary>
        /// Provide the ability to filter dataset elements using a partial or full search term
        /// </summary>
        public string ElementName { get; set; } = "";
    }
}
