using SB.PruebaTecnica.Application.Services.CalculadoraPago;
using SB.PruebaTecnica.Domain.Entities;
using Xunit;

namespace SB.PruebaTecnica.Tests
{
    public class CalculadoraPagoTests
    {
        [Fact]
        public void CalculadoraPagoAsalariado_DevuelveSalarioSemanal()
        {
            // Arrange
            var empleado = new EmpleadoAsalariado { SalarioSemanal = 15000m };
            var calculadora = new CalculadoraPagoAsalariado();

            // Act
            var pago = calculadora.Calcular(empleado);

            // Assert
            Assert.Equal(15000m, pago);
        }

        [Fact]
        public void CalculadoraPagoPorHoras_SinHorasExtra_CalculaCorrectamente()
        {
            // Arrange: 35 horas a razón de RD$200/hora, sin horas extra.
            var empleado = new EmpleadoPorHoras { SueldoPorHora = 200m, HorasTrabajadas = 35m };
            var calculadora = new CalculadoraPagoPorHoras();

            // Act
            var pago = calculadora.Calcular(empleado);

            // Assert
            Assert.Equal(7000m, pago);
        }

        [Fact]
        public void CalculadoraPagoPorHoras_ConHorasExtra_AplicaFactor1_5()
        {
            // Arrange: 45 horas -> 40 regulares + 5 extra a 1.5x, a RD$200/hora.
            // Esperado = (200*40) + (200*1.5*5) = 8000 + 1500 = 9500
            var empleado = new EmpleadoPorHoras { SueldoPorHora = 200m, HorasTrabajadas = 45m };
            var calculadora = new CalculadoraPagoPorHoras();

            // Act
            var pago = calculadora.Calcular(empleado);

            // Assert
            Assert.Equal(9500m, pago);
        }

        [Fact]
        public void CalculadoraPagoPorComision_CalculaCorrectamente()
        {
            // Arrange: ventas 100,000 al 5% de comisión.
            var empleado = new EmpleadoPorComision { VentasBrutas = 100000m, TarifaComision = 0.05m };
            var calculadora = new CalculadoraPagoPorComision();

            // Act
            var pago = calculadora.Calcular(empleado);

            // Assert
            Assert.Equal(5000m, pago);
        }

        [Fact]
        public void CalculadoraPagoAsalariadoPorComision_CalculaCorrectamente()
        {
            // Arrange: ventas 50,000 al 4%, salario base 10,000.
            // comision = 2000; bono = 10000*0.10 = 1000
            // pago = 2000 + 10000 + 1000 = 13000
            var empleado = new EmpleadoAsalariadoPorComision
            {
                VentasBrutas = 50000m,
                TarifaComision = 0.04m,
                SalarioBase = 10000m
            };
            var calculadora = new CalculadoraPagoAsalariadoPorComision();

            // Act
            var pago = calculadora.Calcular(empleado);

            // Assert
            Assert.Equal(13000m, pago);
        }

        [Fact]
        public void CalculadoraPagoFactory_DevuelveEstrategiaCorrectaSegunTipo()
        {
            // Arrange
            var estrategias = new List<ICalculadoraPagoStrategy>
            {
                new CalculadoraPagoAsalariado(),
                new CalculadoraPagoPorHoras(),
                new CalculadoraPagoPorComision(),
                new CalculadoraPagoAsalariadoPorComision()
            };
            var factory = new CalculadoraPagoFactory(estrategias);
            var empleado = new EmpleadoPorComision { VentasBrutas = 1000m, TarifaComision = 0.1m };

            // Act
            var estrategia = factory.Obtener(empleado.Tipo);

            // Assert
            Assert.IsType<CalculadoraPagoPorComision>(estrategia);
        }
    }
}
