namespace PVIMS.Core.Entities
{
	public class MedDRAGrading : EntityBase
	{
		public string Grade { get; set; }
		public string Description { get; set; }
		public int ScaleId { get; set; }

        public virtual MedDRAScale Scale { get; set; }
	}
}