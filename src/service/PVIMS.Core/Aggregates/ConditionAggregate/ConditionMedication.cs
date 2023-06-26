using PVIMS.Core.Aggregates.ConceptAggregate;

namespace PVIMS.Core.Entities
{
	public class ConditionMedication : EntityBase
	{
		public int ConditionId { get; set; }
		public int? ConceptId { get; set; }
		public int? ProductId { get; set; }

		public virtual Condition Condition { get; set; }
		public virtual Concept Concept { get; set; }
		public virtual Product Product { get; set; }
	}
}