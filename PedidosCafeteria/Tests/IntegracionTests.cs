using PedidosCafeteria.Application;
using PedidosCafeteria.Domain;
using PedidosCafeteria.Infrastructure;
using Xunit;

namespace PedidosCafeteria.Tests;

public class IntegracionTests
{
    [Fact]
    public void FlujoCompleto_CrearPedidoPagarYCompletar_DebeEjecutarseCorrectamente()
    {
        // Arrange - Configurar servicios
        var repoProductos = new RepositorioProductosMemoria();
        var repoPedidos = new RepositorioPedidosMemoria();
        var servicioInventario = new ServicioInventario(repoProductos);
        var calculador = new CalculadorPrecios();
        var servicio = new ServicioCafeteria(repoProductos, repoPedidos, servicioInventario, calculador);

        // Agregar productos
        var cafe = new Bebida("BEB001", "Café", 3500, 10, TipoBebida.Cafe, true);
        var empanada = new Comida("COM001", "Empanada", 3200, 5, TipoComida.Snack, true, TimeSpan.FromMinutes(2));
        repoProductos.Agregar(cafe);
        repoProductos.Agregar(empanada);

        // Act - Flujo completo
        // 1. Estudiante crea pedido
        var pedidoId = servicio.CrearPedido("EST001");

        // 2. Agregar productos
        servicio.AgregarProductoAPedido(pedidoId, "BEB001", 2); // 2 cafés
        servicio.AgregarProductoAPedido(pedidoId, "COM001", 1); // 1 empanada

        // 3. Procesar pago con bono (10% descuento)
        var pago = new PagoBono("BONO123", "EST001");
        var pagoExitoso = servicio.ProcesarPago(pedidoId, pago);

        // 4. Operador gestiona el pedido
        servicio.IniciarPreparacionPedido(pedidoId);
        servicio.MarcarPedidoListo(pedidoId);
        servicio.CompletarPedido(pedidoId);

        // Assert - Verificar todo el flujo
        Assert.True(pagoExitoso);

        var pedidoFinal = servicio.ObtenerPedido(pedidoId);
        Assert.Equal(EstadoPedido.Completado, pedidoFinal.Estado);
        Assert.Equal(2, pedidoFinal.Items.Count);

        // Verificar cálculos
        var expectedTotal = (3500m * 1.19m * 2) + (3200m * 1.19m * 1); // Con IVA
        Assert.Equal(expectedTotal, pedidoFinal.Total);

        // Verificar stock actualizado
        Assert.Equal(8, cafe.Stock); // 10 - 2
        Assert.Equal(4, empanada.Stock); // 5 - 1

        // Verificar que el pedido aparece en consultas
        var pedidosCompletados = servicio.ObtenerPedidosPorEstado(EstadoPedido.Completado);
        Assert.Contains(pedidoFinal, pedidosCompletados);

        var pedidosEstudiante = servicio.ObtenerPedidosEstudiante("EST001");
        Assert.Contains(pedidoFinal, pedidosEstudiante);
    }
}