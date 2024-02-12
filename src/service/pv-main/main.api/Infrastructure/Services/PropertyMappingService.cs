using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRIMS.PV.Main.API.Infrastructure.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _descriptionPropertyMapping =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "Description", new PropertyMappingValue(new List<string>() { "Description" }) },
           };

        private Dictionary<string, PropertyMappingValue> _encounterDetailPropertyMapping =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "EncounterGuid", new PropertyMappingValue(new List<string>() { "EncounterGuid" }) },
               { "EncounterDate", new PropertyMappingValue(new List<string>() { "EncounterDate" } ) }
           };

        private Dictionary<string, PropertyMappingValue> _medicationPropertyMapping =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "DrugName", new PropertyMappingValue(new List<string>() { "DrugName" }) },
           };

        private Dictionary<string, PropertyMappingValue> _orgUnitIdentifierPropertyMapping =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
               { "Name", new PropertyMappingValue(new List<string>() { "OrgUnitName" } ) },
           };

        private Dictionary<string, PropertyMappingValue> _patientDetailPropertyMapping =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "PatientGuid", new PropertyMappingValue(new List<string>() { "PatientGuid" }) },
               { "FirstName", new PropertyMappingValue(new List<string>() { "FirstName" } ) },
               { "Surname", new PropertyMappingValue(new List<string>() { "Surname" } ) }
           };

        private Dictionary<string, PropertyMappingValue> _productIdentifierPropertyMapping =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "ProductGuid", new PropertyMappingValue(new List<string>() { "ProductGuid" }) },
               { "DrugName", new PropertyMappingValue(new List<string>() { "DrugName" } ) }
           };

        private Dictionary<string, PropertyMappingValue> _productDetailPropertyMapping =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "ProductGuid", new PropertyMappingValue(new List<string>() { "ProductGuid" }) },
               { "DrugName", new PropertyMappingValue(new List<string>() { "DrugName" } ) }
           };

        private Dictionary<string, PropertyMappingValue> _reportInstanceDetailPropertyMapping =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) }
           };

        private IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            propertyMappings.Add(new PropertyMapping<EncounterDetailDto, Encounter>(_encounterDetailPropertyMapping));
            propertyMappings.Add(new PropertyMapping<FacilityTypeIdentifierDto, FacilityType>(_descriptionPropertyMapping));
            propertyMappings.Add(new PropertyMapping<LabTestIdentifierDto, LabTest>(_descriptionPropertyMapping));
            propertyMappings.Add(new PropertyMapping<LabResultIdentifierDto, LabResult>(_descriptionPropertyMapping));
            propertyMappings.Add(new PropertyMapping<LabTestUnitIdentifierDto, LabTestUnit>(_descriptionPropertyMapping));
            propertyMappings.Add(new PropertyMapping<MedicationFormIdentifierDto, MedicationForm>(_descriptionPropertyMapping));
            propertyMappings.Add(new PropertyMapping<OrgUnitIdentifierDto, OrgUnit>(_orgUnitIdentifierPropertyMapping));
            propertyMappings.Add(new PropertyMapping<PatientDetailDto, Patient>(_patientDetailPropertyMapping));
            propertyMappings.Add(new PropertyMapping<ReportInstanceDetailDto, ReportInstance>(_reportInstanceDetailPropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping
            <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance for <{typeof(TSource)},{typeof(TDestination)}");
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is separated by ",", so we split it.
            var fieldsAfterSplit = fields.Split(',');

            // run through the fields clauses
            foreach (var field in fieldsAfterSplit)
            {
                // trim
                var trimmedField = field.Trim();

                // remove everything after the first " " - if the fields 
                // are coming from an orderBy string, this part must be 
                // ignored
                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);

                // find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;

        }

    }
}
