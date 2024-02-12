namespace OpenRIMS.PV.Main.API.Infrastructure.Helpers
{
    public record Provider(string Name, string Assembly)
    {
        public static readonly Provider MySQL = new(nameof(MySQL), "Main.Infrastructure.MySQL");
        public static readonly Provider SQL = new(nameof(SQL), "Main.Infrastructure.SQL");
    }
}
