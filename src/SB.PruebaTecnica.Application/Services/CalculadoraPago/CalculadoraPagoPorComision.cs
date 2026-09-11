using SB.PruebaTecnica.Domain.Entities;
using SB.PruebaTecnica.Domain.Enums;

namespace SB.PruebaTecnica.Application.Services.CalculadoraPago
{
    /// <summary>
    /// Pago semanal = VentasBrutas * TarifaComision
    /// </summary>
    public class CalculadoraPagoPorComision : ICalculadoraPagoStrategy
    {
        public TipoEmpleado TipoSoportado => TipoEmpleado.PorComision;

        public decimal Calcular(Empleado empleado)
        {
            var e = (EmpleadoPorComision)empleado;
            return e.VentasBrutas * e.TarifaComision;
        }

        public string ExplicarCalculo(Empleado empleado)
        {
            var e = (EmpleadoPorComision)empleado;
            return $"Pago semanal = VentasBrutas ({e.VentasBrutas:C}) x TarifaComision ({e.TarifaComision:P})";
        }
    }
}
