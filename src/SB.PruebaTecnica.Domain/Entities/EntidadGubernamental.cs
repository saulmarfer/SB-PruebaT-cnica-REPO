using SB.PruebaTecnica.Domain.Common;

namespace SB.PruebaTecnica.Domain.Entities
{
    /// <summary>
    /// Representa una entidad gubernamental de la República Dominicana.
    /// Se persiste en un archivo de texto plano dentro del proyecto de Infraestructura,
    /// según lo exigido en las especificaciones técnicas.
    /// </summary>
    public class EntidadGubernamental : BaseEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string Siglas { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? SitioWeb { get; set; }
        public bool Activo { get; set; } = true;
    }
}
