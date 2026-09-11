namespace SB.PruebaTecnica.Infrastructure.Settings
{
    public class ArchivoDatosSettings
    {
        public const string SeccionConfiguracion = "ArchivoDatos";

        public string Carpeta { get; set; } = "Data";
        public string ArchivoEntidadesGubernamentales { get; set; } = "entidades-gubernamentales.txt";
    }
}
