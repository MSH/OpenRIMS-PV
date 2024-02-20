namespace OpenRIMS.PV.Main.API.Models
{
    public class MetaFormCategoryForUpdateDto
    {
        /// <summary>
        /// The unique id of the meta table
        /// </summary>
        public int MetaTableId { get; set; }

        /// <summary>
        /// The name of the category
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Additional help for the caegory
        /// </summary>
        public string Help { get; set; }

        /// <summary>
        /// Icon associated to the category
        /// </summary>
        public string Icon { get; set; }
    }
}