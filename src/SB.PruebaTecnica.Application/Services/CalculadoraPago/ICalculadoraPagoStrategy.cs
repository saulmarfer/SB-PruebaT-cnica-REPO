using SB.PruebaTecnica.Domain.Entities;
using SB.PruebaTecnica.Domain.Enums;

namespace SB.PruebaTecnica.Application.Services.CalculadoraPago
{
    /// <summary>
    /// Patrón Strategy: cada tipo de empleado tiene su propio algoritmo de cálculo
    /// de pago semanal. Esto permite agregar nuevos tipos de empleado sin modificar
    /// código existente (principio Abierto/Cerrado - OCP).
    /// </summary>
    public interface ICalculadoraPagoStrategy
    {
        TipoEmpleado TipoSoportado { get; }
        decimal Calcular(Empleado empleado);
        string ExplicarCalculo(Empleado empleado);
    }
}
