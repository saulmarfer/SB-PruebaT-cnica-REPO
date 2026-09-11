using SB.PruebaTecnica.Domain.Entities;
using SB.PruebaTecnica.Domain.Enums;

namespace SB.PruebaTecnica.Application.Services.CalculadoraPago
{
    /// <summary>
    /// Pago semanal = (VentasBrutas * TarifaComision) + SalarioBase + (SalarioBase * 0.10)
    /// </summary>
    public class CalculadoraPagoAsalariadoPorComision : ICalculadoraPagoStrategy
    {
        private const decimal FactorBonoSalarioBase = 0.10m;

        public TipoEmpleado TipoSoportado => TipoEmpleado.AsalariadoPorComision;

        public decimal Calcular(Empleado empleado)
        {
            var e = (EmpleadoAsalariadoPorComision)empleado;
            var comision = e.VentasBrutas * e.TarifaComision;
            var bono = e.SalarioBase * FactorBonoSalarioBase;

            return comision + e.SalarioBase + bono;
        }

        public string ExplicarCalculo(Empleado empleado)
        {
            var e = (EmpleadoAsalariadoPorComision)empleado;
            return $"Pago semanal = (VentasBrutas x TarifaComision) + SalarioBase + (SalarioBase x 0.10) " +
                   $"= ({e.VentasBrutas:C} x {e.TarifaComision:P}) + {e.SalarioBase:C} + ({e.SalarioBase:C} x 10%)";
        }
    }
}
