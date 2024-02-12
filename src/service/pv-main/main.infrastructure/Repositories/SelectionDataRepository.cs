using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRIMS.PV.Main.Infrastructure.Repositories
{
    public class SelectionDataRepository : ISelectionDataRepository
    {
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;

        public SelectionDataRepository(IRepositoryInt<SelectionDataItem> selectionDataItemRepository)
        {
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
        }

        /// <summary>
        /// Retrieves the selection data for specified attribute attributeKey.
        /// </summary>
        /// <param name="attributeKey">The attribute key.</param>
        /// <returns></returns>
        public ICollection<SelectionDataItem> RetrieveSelectionDataForAttribute(string attributeKey)
        {
            return _selectionDataItemRepository.Queryable()
                .Where(di => di.AttributeKey == attributeKey)
                .ToList();
        }

        /// <summary>
        /// Retrieves all selection data.
        /// </summary>
        /// <returns></returns>
        public ICollection<SelectionDataItem> RetrieveAllSelectionData()
        {
            return _selectionDataItemRepository.List();
        }

        public void AddSelectionDataItem(SelectionDataItem newItem)
        {
            _selectionDataItemRepository.Save(newItem);
        }

        public void RemoveSelectionDataItem(long selectionDataItemId)
        {
            var deleteItem = _selectionDataItemRepository.Get(selectionDataItemId);
            _selectionDataItemRepository.Delete(deleteItem);
        }
    }
}
