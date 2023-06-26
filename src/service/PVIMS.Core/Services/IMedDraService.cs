namespace PVIMS.Core.Services
{
    public interface IMedDraService
    {
        string ValidateSourceData(string fileName, string subdirectory);
        string ImportSourceData(string fileName, string subdirectory);
    }
}
