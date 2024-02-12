using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRIMS.PV.Main.Core.Aggregates.DatasetAggregate
{
    public class Dataset : AuditedEntityBase
	{
        public string DatasetName { get; private set; }
        public bool Active { get; private set; }
        public string InitialiseProcess { get; private set; }
        public string RulesProcess { get; private set; }
        public string Help { get; private set; }
        public string Uid { get; private set; }
        public bool IsSystem { get; private set; }

        public int ContextTypeId { get; private set; }
        public virtual ContextType ContextType { get; private set; }

        public int? EncounterTypeWorkPlanId { get; private set; }
        public virtual EncounterTypeWorkPlan EncounterTypeWorkPlan { get; private set; }

        public int? DatasetXmlId { get; private set; }
        public virtual DatasetXml DatasetXml { get; private set; }

        private List<DatasetCategory> _datasetCategories;
        public IEnumerable<DatasetCategory> DatasetCategories => _datasetCategories.AsReadOnly();

        private List<DatasetInstance> _datasetInstances;
        public IEnumerable<DatasetInstance> DatasetInstances => _datasetInstances.AsReadOnly();

        private List<DatasetRule> _datasetRules;
        public IEnumerable<DatasetRule> DatasetRules => _datasetRules.AsReadOnly();

        private List<WorkPlan> _workPlans;
        public IEnumerable<WorkPlan> WorkPlans => _workPlans.AsReadOnly();

        protected Dataset()
        {
            _datasetCategories = new List<DatasetCategory>();
            _datasetInstances = new List<DatasetInstance>();
            _datasetRules = new List<DatasetRule>();
            _workPlans = new List<WorkPlan>();
        }

        public Dataset(string datasetName, ContextType contextType, string initialiseProcess, string rulesProcess, string help, string uid)
        {
            _datasetCategories = new List<DatasetCategory>();
            _datasetInstances = new List<DatasetInstance>();
            _datasetRules = new List<DatasetRule>();
            _workPlans = new List<WorkPlan>();

            DatasetName = datasetName;
            InitialiseProcess = initialiseProcess;
            RulesProcess = rulesProcess;
            Help = help;
            Uid = uid;
            ContextTypeId = contextType.Id;
            ContextType = contextType;

            Created = DateTime.Now;
            Active = true;
            IsSystem = false;
        }

        public void ChangeDatasetDetails(string datasetName, string help)
        {
            DatasetName = datasetName;
            Help = help;
        }

        public DatasetInstance CreateInstance(int contextId, string tag, EncounterTypeWorkPlan encounterTypeWorkPlan, DatasetInstance spontaneousReport, PatientClinicalEvent activeReport)
        {
            var instance = new DatasetInstance(this, contextId, tag, encounterTypeWorkPlan);

            foreach(DatasetCategory category in DatasetCategories)
            {
                foreach(DatasetCategoryElement element in category.DatasetCategoryElements)
                {
                    if(element.DatasetElement.Field.FieldType.Description == "Table")
                    {
                        instance.AddInstanceValue(element.DatasetElement, "<<Table>>");
                    }
                }
            }

            if(spontaneousReport != null)
            {
                instance.InitialiseValuesForSpontaneousDataset(tag, spontaneousReport);
            }
            if (activeReport != null)
            {
                instance.InitialiseValuesForActiveDataset(tag, activeReport);
            }

            return instance;
        }

        public DatasetRule GetOrCreateRule(DatasetRuleType ruleType)
        {
            var rule = DatasetRules.SingleOrDefault(dr => dr.RuleType == ruleType);
            if (rule == null)
            {
                rule = new DatasetRule()
                {
                    Dataset = this,
                    RuleActive = false,
                    RuleType = ruleType
                };
                _datasetRules.Add(rule);
            }

            return rule;
        }
    }
}