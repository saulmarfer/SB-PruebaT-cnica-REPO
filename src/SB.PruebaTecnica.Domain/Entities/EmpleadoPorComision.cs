using SB.PruebaTecnica.Domain.Enums;

namespace SB.PruebaTecnica.Domain.Entities
{
    /// <summary>
    /// Empleado remunerado exclusivamente por comisión sobre ventas brutas.
    /// Pago semanal = VentasBrutas * TarifaComision
    /// </summary>
    public class EmpleadoPorComision : Empleado
    {
        public decimal VentasBrutas { get; set; }
        public decimal TarifaComision { get; set; }

        public override TipoEmpleado Tipo => TipoEmpleado.PorComision;
    }
}
