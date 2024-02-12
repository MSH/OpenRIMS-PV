namespace OpenRIMS.PV.Main.Core.Entities
{
	public class PatientLanguage : EntityBase
	{
		public bool Preferred { get; set; }
		public int LanguageId { get; set; }
		public int PatientId { get; set; }

		public virtual Language Language { get; set; }
		public virtual Patient Patient { get; set; }
	}
}