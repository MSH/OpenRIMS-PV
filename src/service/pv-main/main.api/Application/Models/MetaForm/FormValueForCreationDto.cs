namespace OpenRIMS.PV.Main.API.Models
{
    public class FormValueForCreationDto
    {
        public FormValueForCreationDto[] FormControlArray { get; set; }
        public string FormControlKey { get; set; }
        public string FormControlValue { get; set; }
    }
}