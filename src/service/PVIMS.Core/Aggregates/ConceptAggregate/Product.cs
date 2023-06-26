using PVIMS.Core.Entities;
using System.Collections.Generic;

namespace PVIMS.Core.Aggregates.ConceptAggregate
{
	public class Product 
		: EntityBase
	{
		public string ProductName { get; private set; }
		public string Manufacturer { get; private set; }
		public string Description { get; private set; }
		public bool Active { get; private set; }
		
		public int ConceptId { get; private set; }
		public virtual Concept Concept { get; private set; }

		private List<ConditionMedication> _conditionMedications;
		public IEnumerable<ConditionMedication> ConditionMedications => _conditionMedications.AsReadOnly();

		private List<PatientMedication> _patientMedications;
		public IEnumerable<PatientMedication> PatientMedications => _patientMedications.AsReadOnly();

		protected Product()
		{
			_conditionMedications = new List<ConditionMedication>();
			_patientMedications = new List<PatientMedication>();
		}

        public Product(string productName, string manufacturer, string description)
        {
            ProductName = productName;
            Manufacturer = manufacturer;
            Description = description;
            Active = true;
        }

		public void ChangeDetails(string productName, string manufacturer, string description)
		{
			ProductName = productName;
			Manufacturer = manufacturer;
			Description = description;
		}

		public void MarkAsActive()
		{
			Active = true;
		}

		public void MarkAsInActive()
		{
			Active = false;
		}
	}
}