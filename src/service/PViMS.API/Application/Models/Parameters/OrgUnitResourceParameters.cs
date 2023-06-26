namespace PVIMS.API.Models.Parameters
{
    public class OrgUnitResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like organisation units returned in  
        /// Default order attribute is Id  
        /// Other valid options are Name
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "OrgUnitName";
    }
}
