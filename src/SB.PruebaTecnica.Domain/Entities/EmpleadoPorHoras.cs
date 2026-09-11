using SB.PruebaTecnica.Domain.Enums;

namespace SB.PruebaTecnica.Domain.Entities
{
    /// <summary>
    /// Empleado remunerado por horas trabajadas, con recargo por horas extra (&gt; 40 h/semana).
    /// </summary>
    public class EmpleadoPorHoras : Empleado
    {
        public decimal SueldoPorHora { get; set; }
        public decimal HorasTrabajadas { get; set; }

        public override TipoEmpleado Tipo => TipoEmpleado.PorHoras;
    }
}
