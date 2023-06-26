using System;
using System.Collections.Generic;
using System.Linq;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Exceptions;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Core.Aggregates.DatasetAggregate
{
    public class DatasetInstance : AuditedEntityBase
    {
        public int ContextId { get; private set; }
        public string Tag { get; private set; }
        public Guid DatasetInstanceGuid { get; private set; }
        public DatasetInstanceStatus Status { get; private set; }

        public int DatasetId { get; private set; }
        public virtual Dataset Dataset { get; private set; }

        public int? EncounterTypeWorkPlanId { get; private set; }
        public virtual EncounterTypeWorkPlan EncounterTypeWorkPlan { get; private set; }

        private List<DatasetInstanceValue> _datasetInstanceValues;
        public IEnumerable<DatasetInstanceValue> DatasetInstanceValues => _datasetInstanceValues.AsReadOnly();

        protected DatasetInstance()
        {
            _datasetInstanceValues = new List<DatasetInstanceValue>();
        }

        public DatasetInstance(Dataset dataset, int contextId, string tag, EncounterTypeWorkPlan encounterTypeWorkPlan)
        {
            _datasetInstanceValues = new List<DatasetInstanceValue>();

            DatasetInstanceGuid = Guid.NewGuid();
            Status = DatasetInstanceStatus.INCOMPLETE;

            Dataset = dataset;
            DatasetId = dataset.Id;

            ContextId = contextId;
            Tag = tag;
            EncounterTypeWorkPlanId = encounterTypeWorkPlan?.Id;
            EncounterTypeWorkPlan = encounterTypeWorkPlan;
        }

        public void ChangeStatusToComplete()
        {
            Status = DatasetInstanceStatus.COMPLETE;
        }

        public void InitialiseValuesForSpontaneousDataset(string tag, DatasetInstance spontaneousReport)
        {
            foreach (DatasetCategory dc in Dataset.DatasetCategories)
            {
                foreach (DatasetCategoryElement dce in dc.DatasetCategoryElements)
                {
                    // Default using default value
                    if (dce.DatasetElement.DefaultValue != null && dce.DatasetElement.DefaultValue != "")
                    {
                        SetInstanceValue(dce.DatasetElement, dce.DatasetElement.DefaultValue);
                    }
                    else
                    {
                        MapValuesUsingInstance(dce, tag, spontaneousReport);
                    }
                }
            }

        }

        public void InitialiseValuesForActiveDataset(string tag, PatientClinicalEvent activeReport)
        {
            foreach (DatasetCategory dc in Dataset.DatasetCategories)
            {
                foreach (DatasetCategoryElement dce in dc.DatasetCategoryElements)
                {
                    // Default using default value
                    if (!String.IsNullOrWhiteSpace(dce.DatasetElement.DefaultValue))
                    {
                        SetInstanceValue(dce.DatasetElement, dce.DatasetElement.DefaultValue);
                    }
                    else
                    {
                        MapValuesUsingEvent(dce, tag, activeReport);
                    }
                }
            }

        }

        private void MapValuesUsingInstance(DatasetCategoryElement dce, string tag, DatasetInstance sourceInstance)
        {
            var mapping = dce.DestinationMappings.SingleOrDefault(dm => dm.Tag == tag);
            if(mapping != null)
            {
                if(dce.DatasetElement.Field.FieldType.Description != "Table")
                {
                    var sourceValue = mapping.SourceElement != null ? sourceInstance.GetInstanceValue(mapping.SourceElement.DatasetElement.ElementName) : "";
                    if (!String.IsNullOrWhiteSpace(sourceValue))
                    {
                        var formattedValue = TranslateSourceValueForElement(mapping, sourceValue);

                        if (!String.IsNullOrWhiteSpace(formattedValue))
                        {
                            SetInstanceValue(dce.DatasetElement, formattedValue);
                        }
                    }
                }
                else
                {
                    // we need to process mapping using sub elements
                    var sourceContexts = sourceInstance.GetInstanceSubValuesContext(mapping.SourceElement.DatasetElement.ElementName);
                    foreach (Guid sourceContext in sourceContexts)
                    {
                        var newContext = Guid.NewGuid();
                        var subItemValues = sourceInstance.GetInstanceSubValues(mapping.SourceElement.DatasetElement.ElementName, sourceContext);
                        foreach(DatasetMappingSub subMapping in mapping.SubMappings)
                        {
                            var sourceSubValue = subMapping.SourceElement != null ? subItemValues.SingleOrDefault(siv => siv.DatasetElementSub.Id == subMapping.SourceElement.Id) : null;
                            if (sourceSubValue != null)
                            {
                                var formattedValue = TranslateSourceValueForSubElement(subMapping, sourceSubValue.InstanceValue);

                                if (!String.IsNullOrWhiteSpace(formattedValue))
                                {
                                    SetInstanceSubValue(subMapping.DestinationElement, formattedValue, newContext);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void MapValuesUsingEvent(DatasetCategoryElement dce, string tag, PatientClinicalEvent clinicalEvent)
        {
            IExtendable ptExtended = clinicalEvent.Patient;
            IExtendable ceExtended = clinicalEvent;

            if (dce.DestinationMappings.Where(dm => dm.Tag == tag).Count() > 0)
            {
                // Get the value to be translated
                var mapping = dce.DestinationMappings.Single(dm => dm.Tag == tag);
                string sourceValue = string.Empty;
                object objectValue;

                if (mapping.MappingType == MappingType.AttributeToElement || mapping.MappingType == MappingType.AttributeToValue)
                {
                    if (!String.IsNullOrWhiteSpace(mapping.PropertyPath) && !String.IsNullOrWhiteSpace(mapping.Property))
                    {
                        switch (mapping.PropertyPath)
                        {
                            case "Patient":
                                objectValue = ptExtended.GetAttributeValue(mapping.Property);
                                sourceValue = objectValue != null ? objectValue.ToString() : "";
                                break;

                            case "PatientClinicalEvent":
                                objectValue = ceExtended.GetAttributeValue(mapping.Property);
                                sourceValue = objectValue != null ? objectValue.ToString() : "";
                                break;

                            default:
                                break;
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrWhiteSpace(mapping.Property))
                        {
                            objectValue = ceExtended.GetAttributeValue(mapping.Property);
                            sourceValue = objectValue != null ? objectValue.ToString() : "";
                        }
                    }
                }
                else
                {
                    Object src = clinicalEvent;
                    if (mapping.MappingType == MappingType.FirstClassToElement || mapping.MappingType == MappingType.FirstClassToValue)
                    {
                        if (!String.IsNullOrWhiteSpace(mapping.PropertyPath))
                        {
                            switch (mapping.PropertyPath)
                            {
                                case "Patient":
                                    src = clinicalEvent.Patient;
                                    break;

                                default:
                                    break;
                            }
                        }
                        objectValue = src.GetType().GetProperty(mapping.Property).GetValue(src, null);
                        sourceValue = objectValue != null ? objectValue.ToString() : "";
                    }
                }

                // Translate the value
                if (!String.IsNullOrWhiteSpace(sourceValue))
                {
                    var formattedValue = TranslateSourceValueForElement(mapping, sourceValue);
                        
                    if (!String.IsNullOrWhiteSpace(formattedValue))
                    {
                        SetInstanceValue(dce.DatasetElement, formattedValue);
                    }
                }
            }
        }

        private string TranslateSourceValueForElement(DatasetMapping mapping, string sourceValue)
        {
            var formattedValue = sourceValue.Trim();
            switch (mapping.MappingType)
            {
                case MappingType.FirstClassToElement:
                case MappingType.AttributeToElement:
                    // Length restriction for alpha numerics
                    if (mapping.DestinationElement.DatasetElement.Field.FieldType.Description == "AlphaNumericTextbox")
                    {
                        var len = mapping.DestinationElement.DatasetElement.Field.MaxLength != null ? Convert.ToInt32(mapping.DestinationElement.DatasetElement.Field.MaxLength) : 0;
                        formattedValue = len > 0 ? formattedValue.Length > len ? formattedValue.Substring(0, len - 1) : formattedValue : formattedValue;
                    }
                    if (mapping.DestinationElement.DatasetElement.Field.FieldType.Description == "Date")
                    {
                        DateTime tempdt;
                        if (DateTime.TryParse(formattedValue, out tempdt))
                        {
                            if (!String.IsNullOrWhiteSpace(mapping.MappingOption))
                            {
                                formattedValue = Convert.ToDateTime(formattedValue) == DateTime.MinValue ? "" : Convert.ToDateTime(formattedValue).ToString(mapping.MappingOption);
                            }
                        }
                        else
                        {
                            formattedValue = "";
                        }
                    }
                    if (mapping.DestinationElement.DatasetElement.Field.FieldType.Description == "NumericTextbox" && mapping.DestinationElement.DatasetElement.Field.Decimals == 0)
                    {
                        Int32 tempi;
                        formattedValue = Int32.TryParse(formattedValue, out tempi) ? Convert.ToInt32(formattedValue).ToString() : "";
                    }
                    if (mapping.DestinationElement.DatasetElement.Field.FieldType.Description == "NumericTextbox" && mapping.DestinationElement.DatasetElement.Field.Decimals > 0)
                    {
                        Decimal tempd;
                        formattedValue = Decimal.TryParse(formattedValue, out tempd) ? Convert.ToDecimal(formattedValue).ToString() : "";
                    }
                    break;

                case MappingType.ValueToValue:
                case MappingType.AttributeToValue:
                case MappingType.FirstClassToValue:
                    // Map value to value
                    var mappedValue = mapping.DatasetMappingValues.SingleOrDefault(mv => mv.SourceValue == sourceValue && mv.Active == true);
                    if (mappedValue != null)
                    {
                        formattedValue = mappedValue.DestinationValue;
                    }
                    else
                    {
                        formattedValue = string.Empty; // didnt find a mapping
                    }
                    break;

                case MappingType.ElementToElement:
                    // Length restriction for alpha numerics
                    if (mapping.DestinationElement.DatasetElement.Field.FieldType.Description == "AlphaNumericTextbox")
                    {
                        var len = mapping.DestinationElement.DatasetElement.Field.MaxLength != null ? Convert.ToInt32(mapping.DestinationElement.DatasetElement.Field.MaxLength) : 0;
                        formattedValue = len > 0 ? formattedValue.Length > len ? formattedValue.Substring(0, len - 1) : formattedValue : formattedValue;
                    }
                    if (mapping.DestinationElement.DatasetElement.Field.FieldType.Description == "Date")
                    {
                        DateTime tempdt;
                        if (DateTime.TryParse(formattedValue, out tempdt))
                        {
                            if (!String.IsNullOrWhiteSpace(mapping.MappingOption))
                            {
                                formattedValue = Convert.ToDateTime(formattedValue).ToString(mapping.MappingOption);
                            }
                        }
                        else
                        {
                            formattedValue = "";
                        }
                    }
                    if (mapping.DestinationElement.DatasetElement.Field.FieldType.Description == "NumericTextbox" && mapping.DestinationElement.DatasetElement.Field.Decimals == 0)
                    {
                        Int32 tempi;
                        formattedValue = Int32.TryParse(formattedValue, out tempi) ? Convert.ToInt32(formattedValue).ToString() : "";
                    }
                    if (mapping.DestinationElement.DatasetElement.Field.FieldType.Description == "NumericTextbox" && mapping.DestinationElement.DatasetElement.Field.Decimals > 0)
                    {
                        Decimal tempd;
                        formattedValue = Decimal.TryParse(formattedValue, out tempd) ? Convert.ToDecimal(formattedValue).ToString() : "";
                    }
                    break;

                default:
                    break;
            }

            return formattedValue;
        }

        private string TranslateSourceValueForSubElement(DatasetMappingSub subMapping, string sourceValue)
        {
            var formattedValue = sourceValue.Trim();
            switch (subMapping.MappingType)
            {
                case MappingType.FirstClassToElement:
                case MappingType.AttributeToElement:
                    // Length restriction for alpha numerics
                    if (subMapping.DestinationElement.Field.FieldType.Description == "AlphaNumericTextbox")
                    {
                        var len = subMapping.DestinationElement.Field.MaxLength != null ? Convert.ToInt32(subMapping.DestinationElement.Field.MaxLength) : 0;
                        formattedValue = len > 0 ? formattedValue.Length > len ? formattedValue.Substring(0, len - 1) : formattedValue : formattedValue;
                    }
                    if (subMapping.DestinationElement.Field.FieldType.Description == "Date")
                    {
                        DateTime tempdt;
                        if (DateTime.TryParse(formattedValue, out tempdt))
                        {
                            if (!String.IsNullOrWhiteSpace(subMapping.MappingOption))
                            {
                                formattedValue = Convert.ToDateTime(formattedValue).ToString(subMapping.MappingOption);
                            }
                        }
                        else
                        {
                            formattedValue = "";
                        }
                    }
                    if (subMapping.DestinationElement.Field.FieldType.Description == "NumericTextbox" && subMapping.DestinationElement.Field.Decimals == 0)
                    {
                        Int32 tempi;
                        formattedValue = Int32.TryParse(formattedValue, out tempi) ? Convert.ToInt32(formattedValue).ToString() : "";
                    }
                    if (subMapping.DestinationElement.Field.FieldType.Description == "NumericTextbox" && subMapping.DestinationElement.Field.Decimals > 0)
                    {
                        Decimal tempd;
                        formattedValue = Decimal.TryParse(formattedValue, out tempd) ? Convert.ToDecimal(formattedValue).ToString() : "";
                    }
                    break;

                case MappingType.ValueToValue:
                case MappingType.AttributeToValue:
                case MappingType.FirstClassToValue:
                    // Map value to value
                    var mappedValue = subMapping.DatasetMappingValues.SingleOrDefault(mv => mv.SourceValue == sourceValue && mv.Active == true);
                    if (mappedValue != null)
                    {
                        formattedValue = mappedValue.DestinationValue;
                    }
                    else
                    {
                        formattedValue = string.Empty; // didnt find a mapping
                    }
                    break;

                case MappingType.ElementToElement:
                    // Length restriction for alpha numerics
                    if (subMapping.DestinationElement.Field.FieldType.Description == "AlphaNumericTextbox")
                    {
                        var len = subMapping.DestinationElement.Field.MaxLength != null ? Convert.ToInt32(subMapping.DestinationElement.Field.MaxLength) : 0;
                        formattedValue = len > 0 ? formattedValue.Length > len ? formattedValue.Substring(0, len - 1) : formattedValue : formattedValue;
                    }
                    if (subMapping.DestinationElement.Field.FieldType.Description == "Date")
                    {
                        DateTime tempdt;
                        if (DateTime.TryParse(formattedValue, out tempdt))
                        {
                            if (!String.IsNullOrWhiteSpace(subMapping.MappingOption))
                            {
                                formattedValue = Convert.ToDateTime(formattedValue).ToString(subMapping.MappingOption);
                            }
                        }
                        else
                        {
                            formattedValue = "";
                        }
                    }
                    if (subMapping.DestinationElement.Field.FieldType.Description == "NumericTextbox" && subMapping.DestinationElement.Field.Decimals == 0)
                    {
                        Int32 tempi;
                        formattedValue = Int32.TryParse(formattedValue, out tempi) ? Convert.ToInt32(formattedValue).ToString() : "";
                    }
                    if (subMapping.DestinationElement.Field.FieldType.Description == "NumericTextbox" && subMapping.DestinationElement.Field.Decimals > 0)
                    {
                        Decimal tempd;
                        formattedValue = Decimal.TryParse(formattedValue, out tempd) ? Convert.ToDecimal(formattedValue).ToString() : "";
                    }
                    break;

                default:
                    break;
            }

            return formattedValue;
        }

        public DatasetInstanceValue AddInstanceValue(DatasetElement datasetElement, string instanceValue)
        {
            var datasetInstanceValue = new DatasetInstanceValue(datasetElement, this, instanceValue);

            _datasetInstanceValues.Add(datasetInstanceValue);

            return datasetInstanceValue;
        }

        public string GetInstanceValue(string datasetElementName)
        {
            if (String.IsNullOrWhiteSpace(datasetElementName)) { return string.Empty; };

            // Temporarily handle calculations here
            if (datasetElementName == "BMI")
            {
                var tempWeight = this.GetInstanceValue("Weight (kg)");
                var tempHeight = this.GetInstanceValue("Height (cm)");

                if (String.IsNullOrWhiteSpace(tempWeight) || String.IsNullOrWhiteSpace(tempHeight)) { return "Unable to calculate"; };
                double BMI = Math.Round((Convert.ToDouble(tempWeight) / ((Convert.ToDouble(tempHeight) / 100.0) * (Convert.ToDouble(tempHeight) / 100.0))), 2);
                return BMI.ToString();
            }

            var instanceValue = DatasetInstanceValues
                .SingleOrDefault(div => div.DatasetElement.ElementName == datasetElementName);

            return instanceValue?.InstanceValue;
        }

        public DatasetInstanceValue SetInstanceValue(DatasetElement datasetElement, string instanceValue)
        {
            datasetElement.Validate(instanceValue);
            var datasetInstanceValue = DatasetInstanceValues.SingleOrDefault(div => div.DatasetElement.Id == datasetElement.Id);

            if (datasetInstanceValue == null)
            {
                if (!string.IsNullOrWhiteSpace(instanceValue))
                {
                    datasetInstanceValue = AddInstanceValue(datasetElement, instanceValue);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(instanceValue))
                {
                    _datasetInstanceValues.Remove(datasetInstanceValue);
                    datasetInstanceValue = null;
                }
                else
                {
                    datasetInstanceValue.InstanceValue = instanceValue;
                }
            }

            return datasetInstanceValue;
        }

        public DatasetInstanceSubValue SetInstanceSubValue(DatasetElementSub datasetElementSub, string instanceSubValue, Guid contextValue = default(Guid))
        {
            datasetElementSub.Validate(instanceSubValue);
            var datasetInstanceValue = DatasetInstanceValues.SingleOrDefault(div => div.DatasetElement.Id == datasetElementSub.DatasetElement.Id);
            if (datasetInstanceValue == null)
            {
                if (datasetElementSub.DatasetElement.Field.FieldType.Id == (int)FieldTypes.Table)
                {
                    datasetInstanceValue = SetInstanceValue(datasetElementSub.DatasetElement, "<<TABLE>>");
                }
                else
                {
                    throw new Exception("Cannot set an instance sub value without setting the parent instance value");
                }
            }

            var datasetInstanceSubValue = datasetInstanceValue.DatasetInstanceSubValues.SingleOrDefault(disv => disv.DatasetElementSub.Id == datasetElementSub.Id && disv.ContextValue == contextValue);
            if (datasetInstanceSubValue == null)
            {
                if (!string.IsNullOrWhiteSpace(instanceSubValue))
                {
                    datasetInstanceSubValue = datasetInstanceValue.AddDatasetInstanceSubValue(datasetElementSub, instanceSubValue, contextValue);
                }
                else 
                {
                    if (datasetElementSub.Field.Mandatory)
                    {
                        throw new DatasetFieldSetException(datasetElementSub.Id.ToString(), string.Format("{0} is required", datasetElementSub.ElementName));
                    }
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(instanceSubValue))
                {
                    if (datasetElementSub.Field.Mandatory)
                    {
                        throw new DatasetFieldSetException(datasetElementSub.Id.ToString(), string.Format("{0} is required", datasetElementSub.ElementName));
                    }

                    datasetInstanceValue.DatasetInstanceSubValues.Remove(datasetInstanceSubValue);
                    datasetInstanceSubValue = null;
                }
                else
                {
                    datasetInstanceSubValue.InstanceValue = instanceSubValue;
                }
            }

            return datasetInstanceSubValue;
        }

        public Guid[] GetInstanceSubValuesContext(string datasetElementName)
        {
            var datasetInstanceValue = DatasetInstanceValues.SingleOrDefault(div => div.DatasetElement.ElementName == datasetElementName);

            if (datasetInstanceValue != null && datasetInstanceValue.DatasetInstanceSubValues.Any())
            {
                return datasetInstanceValue.DatasetInstanceSubValues.Select(disv => disv.ContextValue).Distinct().ToArray();
            }

            return new Guid[0];
        }

        public Guid? GetContextForInstanceSubValue(DatasetElement datasetElement, DatasetElementSub datasetSubElement, string instanceValue)
        {
            var datasetInstanceValue = DatasetInstanceValues.SingleOrDefault(div => div.DatasetElement.Id == datasetElement.Id);

            if (datasetInstanceValue != null && datasetInstanceValue.DatasetInstanceSubValues.Any())
            {
                return datasetInstanceValue.DatasetInstanceSubValues.Where(disv => disv.DatasetElementSub.Id == datasetSubElement.Id && disv.InstanceValue.Trim() == instanceValue.Trim()).Select(disv => disv.ContextValue).First();
            }

            return null;
        }

        public DatasetInstanceSubValue[] GetInstanceSubValues(string datasetElementName, Guid context)
        {
            var datasetInstanceValue = DatasetInstanceValues.SingleOrDefault(div => div.DatasetElement.ElementName == datasetElementName);

            if (datasetInstanceValue != null && datasetInstanceValue.DatasetInstanceSubValues.Any())
            {
                return datasetInstanceValue.DatasetInstanceSubValues.Where(disv => disv.ContextValue == context).ToArray();
            }

            return new DatasetInstanceSubValue[0];
        }
    }
}