using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.SeedWork;
using System.Collections.Generic;
using System.Linq;

namespace OpenRIMS.PV.Main.Core.Aggregates.ConceptAggregate
{
	public class Concept 
		: EntityBase, IAggregateRoot
	{
		public string ConceptName { get; private set; }
		public string Strength { get; private set; }
		public bool Active { get; private set; }
		
		public int MedicationFormId { get; private set; }
		public virtual MedicationForm MedicationForm { get; private set; }

		private List<ConceptIngredient> _conceptIngredients;
		public IEnumerable<ConceptIngredient> ConceptIngredients => _conceptIngredients.AsReadOnly();

		private List<ConditionMedication> _conditionMedications;
		public IEnumerable<ConditionMedication> ConditionMedications => _conditionMedications.AsReadOnly();

		private List<PatientMedication> _patientMedications;
		public IEnumerable<PatientMedication> PatientMedications => _patientMedications.AsReadOnly();

		private List<Product> _products;
		public IEnumerable<Product> Products => _products.AsReadOnly();

		protected Concept()
		{
			_conceptIngredients = new List<ConceptIngredient>();
			_conditionMedications = new List<ConditionMedication>();
			_patientMedications = new List<PatientMedication>();
			_products = new List<Product>();
		}

        public Concept(string conceptName, string strength, MedicationForm medicationForm)
        {
            ConceptName = conceptName;
            Strength = strength;
            MedicationForm = medicationForm;
			MedicationFormId = medicationForm.Id;
			Active = true;
		}

		public Product AddProduct(string productName, string manufacturer, string description)
		{
			var product = new Product(productName, manufacturer, description);
			_products.Add(product);
			return product;
		}

		public void ChangeDetails(string conceptName, string strength, MedicationForm medicationForm)
		{
			ConceptName = conceptName;
			Strength = strength;
			MedicationForm = medicationForm;
			MedicationFormId = medicationForm.Id;
		}

		public void MarkAsActive()
		{
			Active = true;
		}

		public void MarkAsInActive()
		{
			Active = false;
		}

		public void ChangeProductDetails(int productId, string productName, string manufacturer, string description)
		{
			var product = _products.SingleOrDefault(p => p.Id == productId);
			if (product == null)
			{
				throw new KeyNotFoundException($"Unable to locate product {productId}");
			}

			product.ChangeDetails(productName, manufacturer, description);
		}

		public void MarkProductAsActive(int productId)
		{
			var product = _products.SingleOrDefault(p => p.Id == productId);
			if (product == null)
			{
				throw new KeyNotFoundException($"Unable to locate product {productId}");
			}

			product.MarkAsActive();
		}

		public void MarkProductAsInActive(int productId)
		{
			var product = _products.SingleOrDefault(p => p.Id == productId);
			if (product == null)
			{
				throw new KeyNotFoundException($"Unable to locate product {productId}");
			}

			product.MarkAsInActive();
		}
	}
}