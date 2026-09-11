namespace SB.PruebaTecnica.Infrastructure.Settings
{
    public class JwtSettings
    {
        public const string SeccionConfiguracion = "Jwt";

        public string Secreto { get; set; } = string.Empty;
        public string Emisor { get; set; } = string.Empty;
        public string Audiencia { get; set; } = string.Empty;
        public int MinutosExpiracion { get; set; } = 60;
    }
}
