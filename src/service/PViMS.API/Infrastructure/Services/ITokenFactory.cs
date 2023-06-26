namespace PVIMS.API.Infrastructure.Services
{
    public interface ITokenFactory
    {
        string GenerateToken(int size = 32);
    }
}
