namespace PVIMS.API.Models
{
    public class SelectionDataItemForCreationDto
    {
        /// <summary>
        /// The unique name of the attribute
        /// </summary>
        public string AttributeKey { get; set; }

        /// <summary>
        /// The unique id of the selection value
        /// </summary>
        public string SelectionKey { get; set; }

        /// <summary>
        /// the value of the selection item
        /// </summary>
        public string DataItemValue { get; set; }
    }
}
