using SB.PruebaTecnica.Domain.Enums;

namespace SB.PruebaTecnica.Domain.Entities
{
    /// <summary>
    /// Empleado con salario base más comisión sobre ventas, más un 10% adicional
    /// sobre el salario base.
    /// Pago semanal = (VentasBrutas * TarifaComision) + SalarioBase + (SalarioBase * 0.10)
    /// </summary>
    public class EmpleadoAsalariadoPorComision : Empleado
    {
        public decimal VentasBrutas { get; set; }
        public decimal TarifaComision { get; set; }
        public decimal SalarioBase { get; set; }

        public override TipoEmpleado Tipo => TipoEmpleado.AsalariadoPorComision;
    }
}
