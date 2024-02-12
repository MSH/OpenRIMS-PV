namespace OpenRIMS.PV.Main.API.Models.Parameters
{
    public class DatasetElementSubResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like dataset element subs returned in  
        /// Default order attribute is Id  
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "Id";

        /// <summary>
        /// Provide the ability to filter dataset element subs using a partial or full search term
        /// </summary>
        public string ElementName { get; set; } = "";
    }
}
