using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PVIMS.Services
{
    public class CustomAttributeService : ICustomAttributeService
    {
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeConfigRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly IUnitOfWorkInt _unitOfWork;

        public CustomAttributeService(
            IUnitOfWorkInt unitOfWork,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeConfigRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _customAttributeConfigRepository = customAttributeConfigRepository ?? throw new ArgumentNullException(nameof(customAttributeConfigRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
        }

        /// <summary>
        /// Returns entities that have custom attributes
        /// </summary>
        /// <returns></returns>
        public IList<string> ListExtendableEntities()
        {
            var entityAssembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(n => n.GetName().Name == "PVIMS.Core");

            if (entityAssembly == null) return new List<string>();

            return entityAssembly
                .GetTypes()
                .Where(t => typeof(IExtendable).IsAssignableFrom(t))
                .Select(tn => tn.Name)
                .ToList();
        }

        public IList<CustomAttributeConfigListItem> ListCustomAttributes(string entityName)
        {
            var customAttributes = _customAttributeConfigRepository.List();

            IList<CustomAttributeConfigListItem> attributesOfEntity =
                (from c in customAttributes
                 where c.ExtendableTypeName == entityName
                 select new CustomAttributeConfigListItem
                 {
                     CustomAttributeConfigId = c.Id,
                     EntityName = entityName,
                     Category = c.Category,
                     AttributeName = c.AttributeKey,
                     AttributeTypeName = c.CustomAttributeType == CustomAttributeType.Numeric ? "Numeric" : c.CustomAttributeType == CustomAttributeType.String ? "Text" : c.CustomAttributeType == CustomAttributeType.DateTime ? "Date" : c.CustomAttributeType == CustomAttributeType.Selection ? "Selection" : "",
                     Required = c.IsRequired,
                     StringMaxLength = c.CustomAttributeType == CustomAttributeType.String ? c.StringMaxLength : null,
                     NumericMinValue = c.CustomAttributeType == CustomAttributeType.Numeric ? c.NumericMinValue : null,
                     NumericMaxValue = c.CustomAttributeType == CustomAttributeType.Numeric ? c.NumericMaxValue : null,
                     FutureDateOnly = c.CustomAttributeType == CustomAttributeType.DateTime ? (bool?)c.FutureDateOnly : null,
                     PastDateOnly = c.CustomAttributeType == CustomAttributeType.DateTime ? (bool?)c.PastDateOnly : null,
                     Searchable = c.IsSearchable,
                 }).ToList();

            return attributesOfEntity;
        }

        public async Task AddCustomAttributeAsync(CustomAttributeConfigDetail customAttribute)
        {
            var newCustomAttribute = new CustomAttributeConfiguration()
            {
                ExtendableTypeName = customAttribute.EntityName,
                Category = customAttribute.Category,
                AttributeKey = customAttribute.AttributeName,
                AttributeDetail = customAttribute.AttributeDetail,
                CustomAttributeType = customAttribute.CustomAttributeType,
                IsRequired = customAttribute.Required,
                IsSearchable = customAttribute.Searchable
            };

            switch (newCustomAttribute.CustomAttributeType)
            {
                case CustomAttributeType.Numeric:
                    if (customAttribute.NumericMinValue.HasValue)
                    {
                        newCustomAttribute.NumericMinValue = customAttribute.NumericMinValue.Value;
                    }

                    if (customAttribute.NumericMaxValue.HasValue)
                    {
                        newCustomAttribute.NumericMaxValue = customAttribute.NumericMaxValue.Value;
                    }
                    break;
                case CustomAttributeType.String:
                    if (customAttribute.StringMaxLength.HasValue)
                    {
                        newCustomAttribute.StringMaxLength = customAttribute.StringMaxLength.Value;
                    }
                    break;
                case CustomAttributeType.DateTime:
                    newCustomAttribute.FutureDateOnly = customAttribute.FutureDateOnly;
                    newCustomAttribute.PastDateOnly = customAttribute.PastDateOnly;
                    break;
                default:
                    break;
            }

            await _customAttributeConfigRepository.SaveAsync(newCustomAttribute);

            if(newCustomAttribute.CustomAttributeType == CustomAttributeType.Selection)
            {
                var newSelectionDataItem = new SelectionDataItem()
                {
                    AttributeKey = customAttribute.AttributeName,
                    Value = "",
                    SelectionKey = "0"
                };

                await _selectionDataItemRepository.SaveAsync(newSelectionDataItem);
            }
        }

        public async Task UpdateCustomAttributeAsync(CustomAttributeConfigDetail customAttribute)
        {
            var updateCustomAttribute = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().Single(ca => ca.ExtendableTypeName == customAttribute.EntityName && ca.AttributeKey == customAttribute.AttributeName);

            updateCustomAttribute.Category = customAttribute.Category;
            updateCustomAttribute.AttributeDetail = customAttribute.AttributeDetail;
            updateCustomAttribute.IsRequired = customAttribute.Required;
            updateCustomAttribute.IsSearchable = customAttribute.Searchable;

            switch (updateCustomAttribute.CustomAttributeType)
            {
                case CustomAttributeType.Numeric:
                    if (customAttribute.NumericMinValue.HasValue)
                    {
                        updateCustomAttribute.NumericMinValue = customAttribute.NumericMinValue.Value;
                    }

                    if (customAttribute.NumericMaxValue.HasValue)
                    {
                        updateCustomAttribute.NumericMaxValue = customAttribute.NumericMaxValue.Value;
                    }
                    break;
                case CustomAttributeType.String:
                    if (customAttribute.StringMaxLength.HasValue)
                    {
                        updateCustomAttribute.StringMaxLength = customAttribute.StringMaxLength.Value;
                    }
                    break;
                case CustomAttributeType.DateTime:
                    updateCustomAttribute.FutureDateOnly = customAttribute.FutureDateOnly;
                    updateCustomAttribute.PastDateOnly = customAttribute.PastDateOnly;
                    break;
                default:
                    break;
            }

            _customAttributeConfigRepository.Update(updateCustomAttribute);
            await _unitOfWork.CompleteAsync();
        }

        public IList<SelectionDataItemDetail> ListSelectionDataItems(string attributeName)
        {
            var referenceData = _selectionDataItemRepository.Queryable()
                .Where(di => di.AttributeKey == attributeName)
                .ToList();

            IList<SelectionDataItemDetail> selectionDataItems =
                (from item in referenceData
                 select new SelectionDataItemDetail
                 {
                     SelectionDataItemId = item.Id,
                     AttributeKey = item.AttributeKey,
                     SelectionKey = item.SelectionKey,
                     DataItemValue = item.Value
                 }).ToList();

            return selectionDataItems;
        }

        public async Task AddSelectionDataItemAsync(SelectionDataItemDetail selectionDataItem)
        {
            var newSelectionDataItem = new SelectionDataItem()
            {
                AttributeKey = selectionDataItem.AttributeKey,
                Value = selectionDataItem.DataItemValue,
                SelectionKey = selectionDataItem.SelectionKey
            };

            await _selectionDataItemRepository.SaveAsync(newSelectionDataItem);
        }

        public async Task<string> GetCustomAttributeValueAsync(string extendableTypeName, string attributeKey, IExtendable extended)
        {
            var configuration = await _customAttributeConfigRepository.GetAsync(c => c.ExtendableTypeName == extendableTypeName && c.AttributeKey == attributeKey);
            if (configuration == null) return "";

            DateTime dttemp;
            var val = extended.GetAttributeValue(configuration.AttributeKey);
            if(val == null) { return ""; };

            switch (configuration.CustomAttributeType)
            {
                case CustomAttributeType.FirstClassProperty:
                case CustomAttributeType.None:
                    return string.Empty;

                case CustomAttributeType.Numeric:
                case CustomAttributeType.String:
                    return val.ToString();

                case CustomAttributeType.Selection:
                    var selection = _selectionDataItemRepository.Get(s => s.AttributeKey == configuration.AttributeKey && s.SelectionKey == val.ToString());
                    return selection?.Value;

                case CustomAttributeType.DateTime:
                    return DateTime.TryParse(val.ToString(), out dttemp) ? Convert.ToDateTime(val) > DateTime.MinValue ? Convert.ToDateTime(val).ToString("yyyy-MM-dd") : string.Empty : string.Empty;

                default:
                    return string.Empty;
            }
        }
    }
}
