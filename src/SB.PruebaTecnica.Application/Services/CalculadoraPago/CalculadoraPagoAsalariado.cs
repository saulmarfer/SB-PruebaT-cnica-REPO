using SB.PruebaTecnica.Domain.Entities;
using SB.PruebaTecnica.Domain.Enums;

namespace SB.PruebaTecnica.Application.Services.CalculadoraPago
{
    /// <summary>
    /// Pago semanal = SalarioSemanal
    /// </summary>
    public class CalculadoraPagoAsalariado : ICalculadoraPagoStrategy
    {
        public TipoEmpleado TipoSoportado => TipoEmpleado.Asalariado;

        public decimal Calcular(Empleado empleado)
        {
            var e = (EmpleadoAsalariado)empleado;
            return e.SalarioSemanal;
        }

        public string ExplicarCalculo(Empleado empleado)
        {
            var e = (EmpleadoAsalariado)empleado;
            return $"Pago semanal = SalarioSemanal ({e.SalarioSemanal:C})";
        }
    }
}
