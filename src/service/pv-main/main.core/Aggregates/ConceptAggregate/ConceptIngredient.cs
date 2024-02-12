using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Core.Aggregates.ConceptAggregate
{
	public class ConceptIngredient 
		: EntityBase
	{
		public string Ingredient { get; private set; }
		public string Strength { get; private set; }
		public bool Active { get; private set; }
		
		public int ConceptId { get; set; }
		public virtual Concept Concept { get; set; }

		protected ConceptIngredient()
		{
		}

        public ConceptIngredient(string ingredient, string strength)
        {
            Ingredient = ingredient;
            Strength = strength;
            Active = true;
        }
    }
}