using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenRIMS.PV.Main.Core.CustomAttributes;
using CustomAttributeConfiguration = OpenRIMS.PV.Main.Core.Entities.CustomAttributeConfiguration;
using ICustomAttributeConfigRepository = OpenRIMS.PV.Main.Core.Repositories.ICustomAttributeConfigRepository;
using ISelectionDataRepository = OpenRIMS.PV.Main.Core.Repositories.ISelectionDataRepository;
using ITypeExtensionHandler = OpenRIMS.PV.Main.Core.Services.ITypeExtensionHandler;
using SelectionDataItem = OpenRIMS.PV.Main.Core.Entities.SelectionDataItem;

namespace OpenRIMS.PV.Main.Services
{
    public class TypeExtensionHandler : ITypeExtensionHandler
    {
        private readonly ICustomAttributeConfigRepository attributeConfigRepository;
        private readonly ISelectionDataRepository selectionDataRepository;
        private ICollection<SelectionDataItem> allSelectionDataItems = new Collection<SelectionDataItem>();

        public TypeExtensionHandler(ICustomAttributeConfigRepository attributeConfigRepository,
            ISelectionDataRepository selectionDataRepository)
        {
            this.attributeConfigRepository = attributeConfigRepository;
            this.selectionDataRepository = selectionDataRepository;
        }

        /// <summary>
        /// Returns a list of unpopulated Custom Attributes for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<CustomAttributeDetail> BuildModelExtension<T>() where T : IExtendable
        {
            var attributeConfigs = attributeConfigRepository.RetrieveAttributeConfigurationsForType(typeof(T).Name);

            var modelExtension = new List<CustomAttributeDetail>();

            if (attributeConfigs == null || !attributeConfigs.Any())
                return modelExtension;

            foreach (var customAttributeConfig in attributeConfigs)
            {
                var attributeDetail = new CustomAttributeDetail()
                {
                    Id = customAttributeConfig.Id,
                    Type = customAttributeConfig.CustomAttributeType,
                    Category = customAttributeConfig.Category,
                    AttributeKey = customAttributeConfig.AttributeKey,
                    IsRequired = customAttributeConfig.IsRequired,
                    StringMaxLength = customAttributeConfig.StringMaxLength,
                    NumericMinValue = customAttributeConfig.NumericMinValue,
                    NumericMaxValue = customAttributeConfig.NumericMaxValue,
                    FutureDateOnly = customAttributeConfig.FutureDateOnly,
                    PastDateOnly = customAttributeConfig.PastDateOnly,
                    IsSearchable = customAttributeConfig.IsSearchable,
                    Value = GetAttributeValue(null, customAttributeConfig)
                };

                //if (customAttributeConfig.CustomAttributeType == CustomAttributeType.Selection)
                //{
                //    var refData = RetrieveSelectionDataForAttribute(customAttributeConfig.AttributeKey);

                //    if (refData != null && refData.Any())
                //        attributeDetail.RefData = refData.Select(s =>
                //                new SelectListItem
                //                {
                //                    Value = s.SelectionKey.ToString(),
                //                    Text = s.Value,
                //                    Selected = (attributeDetail.Value != null && s.Id == Convert.ToInt32(attributeDetail.Value))
                //                }).ToList();
                //}

                modelExtension.Add(attributeDetail);
            }

            return modelExtension;
        }

        /// <summary>
        /// Returns list of Custom Attributes for the specified extendable object with values prepopulated
        /// </summary>
        /// <param name="extendableObject">The extendable object.</param>
        /// <returns></returns>
        public List<CustomAttributeDetail> BuildModelExtension(IExtendable extendableObject)
        {
            var modelExtension = new List<CustomAttributeDetail>();

            var attributeConfigs = attributeConfigRepository.RetrieveAttributeConfigurationsForType(extendableObject.GetType().Name);

            if (attributeConfigs == null || !attributeConfigs.Any())
                attributeConfigs = attributeConfigRepository.RetrieveAttributeConfigurationsForType(extendableObject.GetType().BaseType.Name);

            if (attributeConfigs == null || !attributeConfigs.Any())
                return modelExtension;

            foreach (var customAttributeConfig in attributeConfigs)
            {
                var attributeDetail = new CustomAttributeDetail()
                {
                    Id = customAttributeConfig.Id,
                    Type = customAttributeConfig.CustomAttributeType,
                    Category = customAttributeConfig.Category,
                    AttributeKey = customAttributeConfig.AttributeKey,
                    IsRequired = customAttributeConfig.IsRequired,
                    StringMaxLength = customAttributeConfig.StringMaxLength,
                    NumericMinValue = customAttributeConfig.NumericMinValue,
                    NumericMaxValue = customAttributeConfig.NumericMaxValue,
                    FutureDateOnly = customAttributeConfig.FutureDateOnly,
                    PastDateOnly = customAttributeConfig.PastDateOnly,
                    IsSearchable = customAttributeConfig.IsSearchable,
                    Value = GetAttributeValue(extendableObject, customAttributeConfig)
                };

                //if (customAttributeConfig.CustomAttributeType == CustomAttributeType.Selection)
                //{
                //    var refData = RetrieveSelectionDataForAttribute(customAttributeConfig.AttributeKey);

                //    if (refData != null && refData.Any())
                //        attributeDetail.RefData = refData.Select(s =>
                //                new SelectListItem
                //                {
                //                    Value = s.SelectionKey.ToString(),
                //                    Text = s.Value,
                //                    Selected = (attributeDetail.Value != null && s.Id == Convert.ToInt32(attributeDetail.Value))
                //                }).ToList();
                //}

                modelExtension.Add(attributeDetail);
            }

            return modelExtension;
        }

        /// <summary>
        /// Updates the extendable object with values from the custom attribute collection.
        /// </summary>
        /// <param name="extendableToUpdate">The extendable to update.</param>
        /// <param name="customAttributeDetails">The custom attribute details.</param>
        /// <returns>The updated Extendable object</returns>
        /// <exception cref="CustomAttributeException">Unknown AttributeType for AttributeKey: {0}</exception>
        public IExtendable UpdateExtendable(IExtendable extendableToUpdate, IEnumerable<CustomAttributeDetail> customAttributeDetails, string updatedByUser)
        {
            if (customAttributeDetails == null || !customAttributeDetails.Any())
                return extendableToUpdate;

            foreach (var customAttribute in customAttributeDetails)
            {
                try
                {
                    switch (customAttribute.Type)
                    {
                        case CustomAttributeType.Numeric:
                            extendableToUpdate.SetAttributeValue<Decimal>(customAttribute.AttributeKey, customAttribute.Value == null ? 0 : Convert.ToDecimal(customAttribute.Value), updatedByUser);
                            break;
                        case CustomAttributeType.String:
                            extendableToUpdate.SetAttributeValue<String>(customAttribute.AttributeKey, customAttribute.Value == null ? string.Empty : customAttribute.Value.ToString(), updatedByUser);
                            break;
                        case CustomAttributeType.Selection:
                            extendableToUpdate.SetAttributeValue<Int32>(customAttribute.AttributeKey, customAttribute.Value == null ? 0 : Convert.ToInt32(customAttribute.Value), updatedByUser);
                            break;
                        case CustomAttributeType.DateTime:
                            extendableToUpdate.SetAttributeValue<DateTime>(customAttribute.AttributeKey, customAttribute.Value == null || String.IsNullOrEmpty(customAttribute.Value.ToString()) ? DateTime.MinValue : Convert.ToDateTime(customAttribute.Value), updatedByUser);
                            break;
                        default:
                            throw new CustomAttributeException("Unknown AttributeType for AttributeKey: {0}", customAttribute.AttributeKey);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error setting attribute value for {customAttribute.AttributeKey} with error {ex.Message}");
                }
            }

            return extendableToUpdate;
        }

        /// <summary>
        /// Updates the extendable object with values from the custom attribute collection.
        /// </summary>
        /// <param name="extendableToUpdate">The extendable to update.</param>
        /// <param name="customAttributeDetails">The custom attribute details.</param>
        /// <returns>The updated Extendable object</returns>
        /// <exception cref="CustomAttributeException">Unknown AttributeType for AttributeKey: {0}</exception>
        public IExtendable ValidateAndUpdateExtendable(IExtendable extendableToUpdate, IEnumerable<CustomAttributeDetail> customAttributeDetails, string updatedByUser)
        {
            if (customAttributeDetails == null || !customAttributeDetails.Any())
                return extendableToUpdate;

            foreach (var customAttribute in customAttributeDetails)
            {
                try
                {
                    switch (customAttribute.Type)
                    {
                        case CustomAttributeType.Numeric:
                            extendableToUpdate.ValidateAndSetAttributeValue<Decimal>(customAttribute, customAttribute.Value == null ? 0 : Convert.ToDecimal(customAttribute.Value), updatedByUser);
                            break;
                        case CustomAttributeType.String:
                            extendableToUpdate.ValidateAndSetAttributeValue<String>(customAttribute, customAttribute.Value == null ? string.Empty : customAttribute.Value.ToString(), updatedByUser);
                            break;
                        case CustomAttributeType.Selection:
                            extendableToUpdate.ValidateAndSetAttributeValue<Int32>(customAttribute, customAttribute.Value == null ? 0 : Convert.ToInt32(customAttribute.Value), updatedByUser);
                            break;
                        case CustomAttributeType.DateTime:
                            extendableToUpdate.ValidateAndSetAttributeValue<DateTime>(customAttribute, customAttribute.Value == null || String.IsNullOrEmpty(customAttribute.Value.ToString()) ? DateTime.MinValue : Convert.ToDateTime(customAttribute.Value), updatedByUser);
                            break;
                        default:
                            throw new CustomAttributeException("Unknown AttributeType for AttributeKey: {0}", customAttribute.AttributeKey);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error setting attribute value for {customAttribute.AttributeKey} with error {ex.Message}");
                }
            }

            return extendableToUpdate;
        }

        private object GetAttributeValue(IExtendable extendable, CustomAttributeConfiguration config)
        {
            switch (config.CustomAttributeType)
            {
                case CustomAttributeType.Numeric:
                    return (extendable == null || extendable.GetAttributeValue(config.AttributeKey) == null)
                        ? default(decimal)
                        : extendable.GetAttributeValue(config.AttributeKey);
                case CustomAttributeType.String:
                    return (extendable == null || extendable.GetAttributeValue(config.AttributeKey) == null)
                        ? " "// Not a big fan of this but Razor does not recognise empty strings and ends up not rendering a control for the attribute.
                        : extendable.GetAttributeValue(config.AttributeKey);
                case CustomAttributeType.Selection:
                    return (extendable == null || extendable.GetAttributeValue(config.AttributeKey) == null)
                        ? default(int)
                        : extendable.GetAttributeValue(config.AttributeKey);
                case CustomAttributeType.DateTime:
                    return (extendable == null || extendable.GetAttributeValue(config.AttributeKey) == null)
                        ? default(DateTime)
                        : extendable.GetAttributeValue(config.AttributeKey);
                default:
                    throw new CustomAttributeException("Unknown AttributeType for AttributeKey: {0}", config.AttributeKey);
            }
        }
    }
}
