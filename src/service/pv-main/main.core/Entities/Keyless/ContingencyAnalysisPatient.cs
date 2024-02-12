namespace OpenRIMS.PV.Main.Core.Entities.Keyless
{
    public class ContingencyAnalysisPatient
    {
        public string PatientName { get; set; }
        public string StartDate { get; set; }
        public string FinishDate { get; set; }
        public string Drug { get; set; }
        public int DaysContributed { get; set; }
        public int ADR { get; set; }
        public string RiskFactor { get; set; }
        public string RiskFactorOption { get; set; }
        public string FactorMet { get; set; }
    }
}
