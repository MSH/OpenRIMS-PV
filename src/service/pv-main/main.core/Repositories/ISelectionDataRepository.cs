using OpenRIMS.PV.Main.Core.Entities;
using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Repositories
{
    public interface ISelectionDataRepository
    {
        /// <summary>
        /// Retrieves the selection data for the specified attribute key.
        /// </summary>
        /// <param name="attributeKey">The attribute key.</param>
        /// <returns></returns>
        ICollection<SelectionDataItem> RetrieveSelectionDataForAttribute(string attributeKey);

        /// <summary>
        /// Retrieves all selection data.
        /// </summary>
        /// <returns></returns>
        ICollection<SelectionDataItem> RetrieveAllSelectionData();
    }
}
