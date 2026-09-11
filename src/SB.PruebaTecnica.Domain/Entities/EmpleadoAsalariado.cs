using SB.PruebaTecnica.Domain.Enums;

namespace SB.PruebaTecnica.Domain.Entities
{
    /// <summary>
    /// Empleado con salario semanal fijo.
    /// Pago semanal = SalarioSemanal
    /// </summary>
    public class EmpleadoAsalariado : Empleado
    {
        public decimal SalarioSemanal { get; set; }

        public override TipoEmpleado Tipo => TipoEmpleado.Asalariado;
    }
}
