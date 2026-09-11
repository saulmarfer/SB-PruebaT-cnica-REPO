using SB.PruebaTecnica.Domain.Entities;
using SB.PruebaTecnica.Domain.Enums;

namespace SB.PruebaTecnica.Application.Services.CalculadoraPago
{
    /// <summary>
    /// Si horasTrabajadas &lt;= 40: pago = sueldoPorHora * horasTrabajadas
    /// Si horasTrabajadas &gt; 40:  pago = (sueldoPorHora * 40) + (sueldoPorHora * 1.5 * (horasTrabajadas - 40))
    /// </summary>
    public class CalculadoraPagoPorHoras : ICalculadoraPagoStrategy
    {
        private const int HorasRegularesMaximas = 40;
        private const decimal FactorHoraExtra = 1.5m;

        public TipoEmpleado TipoSoportado => TipoEmpleado.PorHoras;

        public decimal Calcular(Empleado empleado)
        {
            var e = (EmpleadoPorHoras)empleado;

            if (e.HorasTrabajadas <= HorasRegularesMaximas)
            {
                return e.SueldoPorHora * e.HorasTrabajadas;
            }

            var horasExtra = e.HorasTrabajadas - HorasRegularesMaximas;
            var pagoRegular = e.SueldoPorHora * HorasRegularesMaximas;
            var pagoExtra = e.SueldoPorHora * FactorHoraExtra * horasExtra;

            return pagoRegular + pagoExtra;
        }

        public string ExplicarCalculo(Empleado empleado)
        {
            var e = (EmpleadoPorHoras)empleado;
            return e.HorasTrabajadas <= HorasRegularesMaximas
                ? $"Pago semanal = SueldoPorHora ({e.SueldoPorHora:C}) x HorasTrabajadas ({e.HorasTrabajadas})"
                : $"Pago semanal = (SueldoPorHora x 40) + (SueldoPorHora x 1.5 x (HorasTrabajadas - 40))";
        }
    }
}
