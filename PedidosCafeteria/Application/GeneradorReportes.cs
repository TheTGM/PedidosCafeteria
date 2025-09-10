using PedidosCafeteria.Domain;
using PedidosCafeteria.Domain.Contratos;
using PedidosCafeteria.Domain.Modelos;
namespace PedidosCafeteria.Application;

public class GeneradorReportes : IGeneradorReportes
{
    private readonly IRepositorioPedidos _repositorioPedidos;
    private readonly IRepositorioProductos _repositorioProductos;

    public GeneradorReportes(IRepositorioPedidos repositorioPedidos, IRepositorioProductos repositorioProductos)
    {
        _repositorioPedidos = repositorioPedidos ?? throw new ArgumentNullException(nameof(repositorioPedidos));
        _repositorioProductos = repositorioProductos ?? throw new ArgumentNullException(nameof(repositorioProductos));
    }

    public ReporteVentas GenerarReporteDiario(DateTime fecha)
    {
        var pedidosDelDia = _repositorioPedidos.ListarPorFecha(fecha)
            .Where(p => p.Estado == EstadoPedido.Completado)
            .ToList();

        return GenerarReporteBase(pedidosDelDia, fecha);
    }

    public ReporteVentas GenerarReportePeriodo(DateTime inicio, DateTime fin)
    {
        var pedidosDelPeriodo = _repositorioPedidos.ListarCompletadosEnPeriodo(inicio, fin)
            .ToList();

        var reporte = GenerarReporteBase(pedidosDelPeriodo, inicio);
        reporte.FechaFin = fin;
        return reporte;
    }

    private ReporteVentas GenerarReporteBase(List<Pedido> pedidos, DateTime fecha)
    {
        var reporte = new ReporteVentas
        {
            Fecha = fecha,
            TotalPedidos = pedidos.Count,
            IngresoTotal = pedidos.Sum(p => p.Total)
        };

        // Agrupar productos vendidos
        var productosVendidos = pedidos
            .SelectMany(p => p.Items)
            .GroupBy(i => i.Producto.Id)
            .Select(g => new ItemReporte
            {
                ProductoId = g.Key,
                NombreProducto = g.First().Producto.Nombre,
                CantidadVendida = g.Sum(i => i.Cantidad),
                IngresoTotal = g.Sum(i => i.Subtotal)
            })
            .OrderByDescending(i => i.CantidadVendida)
            .ToList();

        reporte.ProductosVendidos = productosVendidos;

        // Agrupar métodos de pago
        var metodosPago = pedidos
            .Where(p => p.MetodoPago != null)
            .GroupBy(p => p.MetodoPago!.GetType().Name)
            .Select(g => new MetodoPagoReporte
            {
                TipoMetodo = g.Key,
                CantidadTransacciones = g.Count(),
                MontoTotal = g.Sum(p => p.Total)
            })
            .ToList();

        reporte.MetodosPago = metodosPago;

        return reporte;
    }

    public IEnumerable<ProductoMasVendido> ObtenerProductosMasVendidos(int top = 10)
    {
        var hoy = DateTime.Today;
        var hace30Dias = hoy.AddDays(-30);

        return ObtenerProductosMasVendidosPorPeriodo(hace30Dias, hoy, top);
    }

    public IEnumerable<ProductoMasVendido> ObtenerProductosMasVendidosPorPeriodo(DateTime inicio, DateTime fin, int top = 10)
    {
        var pedidos = _repositorioPedidos.ListarCompletadosEnPeriodo(inicio, fin);

        var productosVendidos = pedidos
            .SelectMany(p => p.Items)
            .GroupBy(i => i.Producto.Id)
            .Select(g => new ProductoMasVendido
            {
                ProductoId = g.Key,
                NombreProducto = g.First().Producto.Nombre,
                CantidadVendida = g.Sum(i => i.Cantidad),
                IngresoGenerado = g.Sum(i => i.Subtotal)
            })
            .OrderByDescending(p => p.CantidadVendida)
            .Take(top)
            .ToList();

        // Asignar ranking
        for (int i = 0; i < productosVendidos.Count; i++)
        {
            productosVendidos[i].Ranking = i + 1;
        }

        return productosVendidos;
    }

    public decimal CalcularIngresosDelDia(DateTime fecha)
    {
        return _repositorioPedidos.ListarPorFecha(fecha)
            .Where(p => p.Estado == EstadoPedido.Completado)
            .Sum(p => p.Total);
    }

    public decimal CalcularIngresosPeriodo(DateTime inicio, DateTime fin)
    {
        return _repositorioPedidos.ListarCompletadosEnPeriodo(inicio, fin)
            .Sum(p => p.Total);
    }

    public int ContarPedidosCompletados(DateTime fecha)
    {
        return _repositorioPedidos.ListarPorFecha(fecha)
            .Count(p => p.Estado == EstadoPedido.Completado);
    }
}