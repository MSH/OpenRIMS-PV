namespace OpenRIMS.PV.Main.API.Models.Parameters
{
    public class MetaFormResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Specify the order you would like meta forms returned in  
        /// Default order attribute is Id  
        /// Attribute must appear in payload to be sortable
        /// </summary>
        public string OrderBy { get; set; } = "Id";
    }
}
