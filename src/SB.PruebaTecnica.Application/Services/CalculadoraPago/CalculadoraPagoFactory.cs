using SB.PruebaTecnica.Application.Exceptions;
using SB.PruebaTecnica.Domain.Enums;

namespace SB.PruebaTecnica.Application.Services.CalculadoraPago
{
    /// <summary>
    /// Selecciona en tiempo de ejecución la estrategia de cálculo correspondiente
    /// al tipo de empleado (patrón Factory + Strategy combinados).
    /// Para soportar un nuevo tipo de empleado basta con registrar una nueva
    /// ICalculadoraPagoStrategy en el contenedor de dependencias; no se modifica
    /// esta clase (OCP).
    /// </summary>
    public class CalculadoraPagoFactory
    {
        private readonly Dictionary<TipoEmpleado, ICalculadoraPagoStrategy> _estrategias;

        public CalculadoraPagoFactory(IEnumerable<ICalculadoraPagoStrategy> estrategias)
        {
            _estrategias = estrategias.ToDictionary(e => e.TipoSoportado);
        }

        public ICalculadoraPagoStrategy Obtener(TipoEmpleado tipo)
        {
            if (!_estrategias.TryGetValue(tipo, out var estrategia))
            {
                throw new ValidationAppException($"No existe una calculadora de pago registrada para el tipo '{tipo}'.");
            }

            return estrategia;
        }
    }
}
