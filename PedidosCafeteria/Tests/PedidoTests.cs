using PedidosCafeteria.Domain;
using Xunit;

namespace PedidosCafeteria.Tests;

public class PedidoTests
{
    [Fact]
    public void Pedido_CreacionValida_DebeInicializarCorrectamente()
    {
        // Arrange
        var estudianteId = "EST001";

        // Act
        var pedido = new Pedido(estudianteId);

        // Assert
        Assert.NotNull(pedido.Id);
        Assert.NotEmpty(pedido.Id);
        Assert.Equal(estudianteId, pedido.EstudianteId);
        Assert.Equal(EstadoPedido.Pendiente, pedido.Estado);
        Assert.Empty(pedido.Items);
        Assert.Equal(0, pedido.Total);
        Assert.Null(pedido.FechaPago);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Pedido_EstudianteIdInvalido_DebeLanzarExcepcion(string estudianteId)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Pedido(estudianteId));
    }

    [Fact]
    public void AgregarItem_ProductoValido_DebeAgregarCorrectamente()
    {
        // Arrange
        var pedido = new Pedido("EST001");
        var producto = new Bebida("BEB001", "Café", 3500, 10, TipoBebida.Cafe, true);
        var cantidad = 2;

        // Act
        pedido.AgregarItem(producto, cantidad);

        // Assert
        Assert.Single(pedido.Items);
        var item = pedido.Items.First();
        Assert.Equal(producto, item.Producto);
        Assert.Equal(cantidad, item.Cantidad);
        Assert.Equal(producto.PrecioFinal * cantidad, pedido.Total);
    }

    [Fact]
    public void AgregarItem_ProductoExistente_DebeActualizarCantidad()
    {
        // Arrange
        var pedido = new Pedido("EST001");
        var producto = new Bebida("BEB001", "Café", 3500, 10, TipoBebida.Cafe, true);

        // Act
        pedido.AgregarItem(producto, 2);
        pedido.AgregarItem(producto, 1); // Agregar más del mismo producto

        // Assert
        Assert.Single(pedido.Items); // Solo un item
        var item = pedido.Items.First();
        Assert.Equal(3, item.Cantidad); // Cantidad sumada
        Assert.Equal(producto.PrecioFinal * 3, pedido.Total);
    }

    [Fact]
    public void AgregarItem_PedidoConfirmado_DebeLanzarExcepcion()
    {
        // Arrange
        var pedido = new Pedido("EST001");
        var producto = new Bebida("BEB001", "Café", 3500, 10, TipoBebida.Cafe, true);
        var pago = new PagoEfectivo(10000);

        pedido.AgregarItem(producto, 1);
        pedido.ConfirmarPago(pago);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            pedido.AgregarItem(producto, 1));
        Assert.Contains("No se puede modificar pedido confirmado", exception.Message);
    }

    [Fact]
    public void ConfirmarPago_PedidoValido_DebeCambiarEstado()
    {
        // Arrange
        var pedido = new Pedido("EST001");
        var producto = new Bebida("BEB001", "Café", 3500, 10, TipoBebida.Cafe, true);
        var pago = new PagoEfectivo(10000);

        pedido.AgregarItem(producto, 1);

        // Act
        pedido.ConfirmarPago(pago);

        // Assert
        Assert.Equal(EstadoPedido.PagoProcesado, pedido.Estado);
        Assert.Equal(pago, pedido.MetodoPago);
        Assert.NotNull(pedido.FechaPago);
    }

    [Fact]
    public void ConfirmarPago_PedidoVacio_DebeLanzarExcepcion()
    {
        // Arrange
        var pedido = new Pedido("EST001");
        var pago = new PagoEfectivo(10000);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            pedido.ConfirmarPago(pago));
        Assert.Contains("Pedido vacío", exception.Message);
    }
}