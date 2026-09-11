namespace SB.PruebaTecnica.Domain.Common
{
    /// <summary>
    /// Clase base para todas las entidades del dominio.
    /// </summary>
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaModificacion { get; set; }
    }
}
