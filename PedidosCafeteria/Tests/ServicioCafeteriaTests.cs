using PedidosCafeteria.Application;
using PedidosCafeteria.Domain;
using PedidosCafeteria.Domain.Excepciones;
using PedidosCafeteria.Infrastructure;
using Xunit;

namespace PedidosCafeteria.Tests;

public class ServicioCafeteriaTests
{
    private readonly ServicioCafeteria _servicio;
    private readonly RepositorioProductosMemoria _repositorioProductos;
    private readonly RepositorioPedidosMemoria _repositorioPedidos;

    public ServicioCafeteriaTests()
    {
        _repositorioProductos = new RepositorioProductosMemoria();
        _repositorioPedidos = new RepositorioPedidosMemoria();
        var servicioInventario = new ServicioInventario(_repositorioProductos);
        var calculadorPrecios = new CalculadorPrecios();

        _servicio = new ServicioCafeteria(
            _repositorioProductos,
            _repositorioPedidos,
            servicioInventario,
            calculadorPrecios);

        // Agregar productos de prueba
        var cafe = new Bebida("BEB001", "Café Americano", 3500, 10, TipoBebida.Cafe, true);
        var empanada = new Comida("COM001", "Empanada", 3200, 5, TipoComida.Snack, true, TimeSpan.FromMinutes(2));

        _repositorioProductos.Agregar(cafe);
        _repositorioProductos.Agregar(empanada);
    }

    [Fact]
    public void CrearPedido_EstudianteValido_DebeRetornarIdPedido()
    {
        // Arrange
        var estudianteId = "EST001";

        // Act
        var pedidoId = _servicio.CrearPedido(estudianteId);

        // Assert
        Assert.NotNull(pedidoId);
        Assert.NotEmpty(pedidoId);

        var pedido = _servicio.ObtenerPedido(pedidoId);
        Assert.NotNull(pedido);
        Assert.Equal(estudianteId, pedido.EstudianteId);
        Assert.Equal(EstadoPedido.Pendiente, pedido.Estado);
    }

    [Fact]
    public void AgregarProductoAPedido_ProductoValido_DebeAgregarCorrectamente()
    {
        // Arrange
        var pedidoId = _servicio.CrearPedido("EST001");
        var productoId = "BEB001";
        var cantidad = 2;

        // Act
        _servicio.AgregarProductoAPedido(pedidoId, productoId, cantidad);

        // Assert
        var pedido = _servicio.ObtenerPedido(pedidoId);
        Assert.Single(pedido.Items);
        Assert.Equal(cantidad, pedido.Items.First().Cantidad);
        Assert.Equal(3500m * 1.19m * cantidad, pedido.Total);
    }

    [Fact]
    public void AgregarProductoAPedido_StockInsuficiente_DebeLanzarExcepcion()
    {
        // Arrange
        var pedidoId = _servicio.CrearPedido("EST001");
        var productoId = "COM001"; // Empanada con stock 5
        var cantidad = 10; // Más del stock disponible

        // Act & Assert
        Assert.Throws<StockInsuficienteException>(() =>
            _servicio.AgregarProductoAPedido(pedidoId, productoId, cantidad));
    }

    [Fact]
    public void ProcesarPago_PagoValido_DebeConfirmarPedido()
    {
        // Arrange
        var pedidoId = _servicio.CrearPedido("EST001");
        _servicio.AgregarProductoAPedido(pedidoId, "BEB001", 1);
        var pago = new PagoEfectivo(10000m);

        // Act
        var resultado = _servicio.ProcesarPago(pedidoId, pago);

        // Assert
        Assert.True(resultado);
        var pedido = _servicio.ObtenerPedido(pedidoId);
        Assert.Equal(EstadoPedido.PagoProcesado, pedido.Estado);
        Assert.Equal(pago, pedido.MetodoPago);
    }

    [Fact]
    public void ProcesarPago_ActualizaInventario_DebeReducirStock()
    {
        // Arrange
        var pedidoId = _servicio.CrearPedido("EST001");
        _servicio.AgregarProductoAPedido(pedidoId, "BEB001", 2);
        var pago = new PagoEfectivo(10000m);

        var stockInicial = _repositorioProductos.ObtenerPorId("BEB001").Stock;

        // Act
        _servicio.ProcesarPago(pedidoId, pago);

        // Assert
        var stockFinal = _repositorioProductos.ObtenerPorId("BEB001").Stock;
        Assert.Equal(stockInicial - 2, stockFinal);
    }

    [Fact]
    public void ObtenerProductosDisponibles_DebeRetornarSoloActivosConStock()
    {
        // Act
        var productos = _servicio.ObtenerProductosDisponibles().ToList();

        // Assert
        Assert.Equal(2, productos.Count); // Solo los 2 productos agregados en el constructor
        Assert.All(productos, p => Assert.True(p.Activo));
        Assert.All(productos, p => Assert.True(p.StockDisponible > 0));
    }
}
