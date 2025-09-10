using PedidosCafeteria.Domain;
using Xunit;

namespace PedidosCafeteria.Tests;

public class MetodoPagoTests
{
    [Fact]
    public void PagoEfectivo_MontoSuficiente_DebeProcesarCorrectamente()
    {
        // Arrange
        var montoRecibido = 15000m;
        var montoPedido = 10000m;
        var expectedCambio = 5000m;
        var pago = new PagoEfectivo(montoRecibido);

        // Act
        var resultado = pago.Procesar(montoPedido);

        // Assert
        Assert.True(resultado);
        Assert.Equal(expectedCambio, pago.Cambio);
        Assert.NotEqual(default(DateTime), pago.FechaProcesamiento);
    }

    [Fact]
    public void PagoEfectivo_MontoInsuficiente_DebeRechazar()
    {
        // Arrange
        var montoRecibido = 5000m;
        var montoPedido = 10000m;
        var pago = new PagoEfectivo(montoRecibido);

        // Act
        var resultado = pago.Procesar(montoPedido);

        // Assert
        Assert.False(resultado);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1000)]
    public void PagoEfectivo_MontoInvalido_DebeLanzarExcepcion(decimal montoInvalido)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PagoEfectivo(montoInvalido));
    }

    [Fact]
    public void PagoTarjeta_DatosValidos_DebeProcesarCorrectamente()
    {
        // Arrange
        var numeroTarjeta = "1234567890123456";
        var titular = "Juan Pérez";
        var pago = new PagoTarjeta(numeroTarjeta, titular);

        // Act
        var resultado = pago.Procesar(10000m);

        // Assert
        Assert.True(resultado);
        Assert.Equal(numeroTarjeta, pago.NumeroTarjeta);
        Assert.Equal(titular, pago.Titular);
    }

    [Fact]
    public void PagoTarjeta_TarjetaRechazada_DebeRechazar()
    {
        // Arrange - tarjetas que empiecen con "0000" se rechazan
        var numeroTarjeta = "0000567890123456";
        var titular = "Juan Pérez";
        var pago = new PagoTarjeta(numeroTarjeta, titular);

        // Act
        var resultado = pago.Procesar(10000m);

        // Assert
        Assert.False(resultado);
    }

    [Theory]
    [InlineData("123456", "Juan Pérez")] // Número muy corto
    [InlineData("12345678901234567", "Juan Pérez")] // Número muy largo
    [InlineData("1234567890123456", "")] // Titular vacío
    [InlineData("1234567890123456", null)] // Titular nulo
    public void PagoTarjeta_DatosInvalidos_DebeLanzarExcepcion(string numero, string titular)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PagoTarjeta(numero, titular));
    }

    [Fact]
    public void PagoBono_BonoValido_DebeProcesarConDescuento()
    {
        // Arrange
        var codigoBono = "BONO123";
        var estudianteId = "EST001";
        var pago = new PagoBono(codigoBono, estudianteId);

        // Act
        var resultado = pago.Procesar(10000m);

        // Assert
        Assert.True(resultado);
        Assert.Equal(codigoBono, pago.CodigoBono);
        Assert.Equal(estudianteId, pago.EstudianteId);
    }

    [Fact]
    public void PagoBono_BonoInvalido_DebeRechazar()
    {
        // Arrange - bonos que empiecen con "INVALID" se rechazan
        var codigoBono = "INVALIDBONO";
        var estudianteId = "EST001";
        var pago = new PagoBono(codigoBono, estudianteId);

        // Act
        var resultado = pago.Procesar(10000m);

        // Assert
        Assert.False(resultado);
    }

    [Fact]
    public void MetodosPago_GenerarRecibo_DebeContenerInformacionCorrecta()
    {
        // Arrange
        var pagoEfectivo = new PagoEfectivo(15000m);
        var pagoTarjeta = new PagoTarjeta("1234567890123456", "Juan Pérez");
        var pagoBono = new PagoBono("BONO123", "EST001");

        pagoEfectivo.Procesar(10000m);
        pagoTarjeta.Procesar(10000m);
        pagoBono.Procesar(10000m);

        // Act
        var reciboEfectivo = pagoEfectivo.ObtenerRecibo();
        var reciboTarjeta = pagoTarjeta.ObtenerRecibo();
        var reciboBono = pagoBono.ObtenerRecibo();

        // Assert
        Assert.Contains("PAGO EN EFECTIVO", reciboEfectivo);
        Assert.Contains("15000", reciboEfectivo);
        Assert.Contains("5000", reciboEfectivo); // Cambio

        Assert.Contains("PAGO CON TARJETA", reciboTarjeta);
        Assert.Contains("****-****-****-3456", reciboTarjeta); // Número enmascarado
        Assert.Contains("Juan Pérez", reciboTarjeta);

        Assert.Contains("BONO ESTUDIANTIL", reciboBono);
        Assert.Contains("BONO123", reciboBono);
        Assert.Contains("10%", reciboBono);
    }
}