namespace OpenRIMS.PV.Main.Core.Services
{
    public interface IMedDraService
    {
        string ValidateSourceData(string fileName, string subdirectory);
        string ImportSourceData(string fileName, string subdirectory);
    }
}
