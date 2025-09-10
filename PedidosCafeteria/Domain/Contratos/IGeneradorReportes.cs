using PedidosCafeteria.Domain.Modelos;
namespace PedidosCafeteria.Domain.Contratos;

public interface IGeneradorReportes
{
    ReporteVentas GenerarReporteDiario(DateTime fecha);
    ReporteVentas GenerarReportePeriodo(DateTime inicio, DateTime fin);
    IEnumerable<ProductoMasVendido> ObtenerProductosMasVendidos(int top = 10);
    IEnumerable<ProductoMasVendido> ObtenerProductosMasVendidosPorPeriodo(DateTime inicio, DateTime fin, int top = 10);
    decimal CalcularIngresosDelDia(DateTime fecha);
    decimal CalcularIngresosPeriodo(DateTime inicio, DateTime fin);
    int ContarPedidosCompletados(DateTime fecha);
}