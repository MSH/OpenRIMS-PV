namespace OpenRIMS.PV.Main.API.Models.Parameters
{
    public class ConfigResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like lab results returned in  
        /// Default order attribute is Id  
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "Id";
    }
}
