using PedidosCafeteria.Domain;
using Xunit;

namespace PedidosCafeteria.Tests;

public class ProductoTests
{
    [Fact]
    public void Bebida_CreacionValida_DebeCalcularPrecioConIVA()
    {
        // Arrange
        var precioBase = 3500m;
        var expectedPrecioFinal = precioBase * 1.19m; // IVA 19%

        // Act
        var bebida = new Bebida("BEB001", "Café Americano", precioBase, 50,
                               TipoBebida.Cafe, true, "Café negro tradicional");

        // Assert
        Assert.Equal(expectedPrecioFinal, bebida.PrecioFinal);
        Assert.Equal("BEB001", bebida.Id);
        Assert.Equal("Café Americano", bebida.Nombre);
        Assert.Equal(50, bebida.Stock);
        Assert.True(bebida.Activo);
    }

    [Fact]
    public void Comida_CreacionValida_DebeCalcularPrecioConIVA()
    {
        // Arrange
        var precioBase = 8500m;
        var expectedPrecioFinal = precioBase * 1.19m;
        var tiempoPrep = TimeSpan.FromMinutes(5);

        // Act
        var comida = new Comida("COM001", "Sandwich Pollo", precioBase, 20,
                               TipoComida.Sandwich, true, tiempoPrep, "Sandwich con pollo");

        // Assert
        Assert.Equal(expectedPrecioFinal, comida.PrecioFinal);
        Assert.Equal(TipoComida.Sandwich, comida.Tipo);
        Assert.True(comida.RequierePreparacion);
        Assert.Equal(tiempoPrep, comida.TiempoPreparacion);
    }

    [Theory]
    [InlineData("", "Nombre", 1000, 10)] // ID vacío
    [InlineData("BEB001", "", 1000, 10)] // Nombre vacío
    [InlineData("BEB001", "Café", 0, 10)] // Precio cero
    [InlineData("BEB001", "Café", -100, 10)] // Precio negativo
    [InlineData("BEB001", "Café", 1000, -5)] // Stock negativo
    public void Producto_DatosInvalidos_DebeLanzarExcepcion(string id, string nombre, decimal precio, int stock)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Bebida(id, nombre, precio, stock, TipoBebida.Cafe, true));
    }

    [Fact]
    public void ReservarStock_CantidadValida_DebeReducirStockDisponible()
    {
        // Arrange
        var producto = new Bebida("BEB001", "Café", 3500, 10, TipoBebida.Cafe, true);
        var cantidadReservar = 3;

        // Act
        producto.ReservarStock(cantidadReservar);

        // Assert
        Assert.Equal(10, producto.Stock); // Stock total no cambia
        Assert.Equal(7, producto.StockDisponible); // Stock disponible se reduce
    }

    [Fact]
    public void ReservarStock_CantidadExcesiva_DebeLanzarExcepcion()
    {
        // Arrange
        var producto = new Bebida("BEB001", "Café", 3500, 5, TipoBebida.Cafe, true);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            producto.ReservarStock(10));
        Assert.Contains("Stock insuficiente", exception.Message);
    }

    [Fact]
    public void ConfirmarVenta_ConReservaPrevia_DebeActualizarStockCorrectamente()
    {
        // Arrange
        var producto = new Bebida("BEB001", "Café", 3500, 10, TipoBebida.Cafe, true);
        producto.ReservarStock(3);

        // Act
        producto.ConfirmarVenta(3);

        // Assert
        Assert.Equal(7, producto.Stock);
        Assert.Equal(7, producto.StockDisponible);
    }

    [Fact]
    public void LiberarReserva_ConReservaPrevia_DebeRestaurarStockDisponible()
    {
        // Arrange
        var producto = new Bebida("BEB001", "Café", 3500, 10, TipoBebida.Cafe, true);
        producto.ReservarStock(3);

        // Act
        producto.LiberarReserva(3);

        // Assert
        Assert.Equal(10, producto.Stock);
        Assert.Equal(10, producto.StockDisponible);
    }
}