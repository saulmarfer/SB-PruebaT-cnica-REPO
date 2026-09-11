using SB.PruebaTecnica.Domain.Enums;

namespace SB.PruebaTecnica.Application.DTOs
{
    public class EmpleadoRequestDto
    {
        public TipoEmpleado Tipo { get; set; }
        public string? PrimerNombre { get; set; }
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string NumeroSeguroSocial { get; set; } = string.Empty;
        public string? Departamento { get; set; }
        public bool Activo { get; set; } = true;

        // Asalariado
        public decimal? SalarioSemanal { get; set; }

        // Por horas
        public decimal? SueldoPorHora { get; set; }
        public decimal? HorasTrabajadas { get; set; }

        // Por comisión / Asalariado por comisión
        public decimal? VentasBrutas { get; set; }
        public decimal? TarifaComision { get; set; }

        // Asalariado por comisión
        public decimal? SalarioBase { get; set; }
    }

    public class EmpleadoResponseDto
    {
        public int Id { get; set; }
        public TipoEmpleado Tipo { get; set; }
        public string TipoDescripcion { get; set; } = string.Empty;
        public string? PrimerNombre { get; set; }
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string NumeroSeguroSocial { get; set; } = string.Empty;
        public string? Departamento { get; set; }
        public bool Activo { get; set; }

        public decimal? SalarioSemanal { get; set; }
        public decimal? SueldoPorHora { get; set; }
        public decimal? HorasTrabajadas { get; set; }
        public decimal? VentasBrutas { get; set; }
        public decimal? TarifaComision { get; set; }
        public decimal? SalarioBase { get; set; }

        public decimal PagoSemanal { get; set; }
    }

    public class EmpleadoFiltroDto
    {
        public string? Nombre { get; set; }
        public string? Departamento { get; set; }
        public bool? Activo { get; set; }
    }

    public class ReportePagoDto
    {
        public int EmpleadoId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string TipoDescripcion { get; set; } = string.Empty;
        public string DetalleCalculo { get; set; } = string.Empty;
        public decimal PagoSemanal { get; set; }
    }
}
