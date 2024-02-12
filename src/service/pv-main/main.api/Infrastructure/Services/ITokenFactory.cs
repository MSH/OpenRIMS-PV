namespace OpenRIMS.PV.Main.API.Infrastructure.Services
{
    public interface ITokenFactory
    {
        string GenerateToken(int size = 32);
    }
}
