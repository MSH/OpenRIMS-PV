namespace OpenRIMS.PV.Main.API.Models.Parameters
{
    public class PatientByConditionResourceParameters : BaseResourceParameters
    {
        /// <summary>
        /// Filter patients by condition case number
        /// </summary>
        public string CaseNumber { get; set; } = "";
    }
}
