namespace PedidosCafeteria.Domain.Modelos;

public class ReporteVentas
{
    public DateTime Fecha { get; set; }
    public DateTime? FechaFin { get; set; } // Para reportes de período
    public int TotalPedidos { get; set; }
    public decimal IngresoTotal { get; set; }
    public decimal PromedioVentaPorPedido => TotalPedidos > 0 ? IngresoTotal / TotalPedidos : 0;
    public List<ItemReporte> ProductosVendidos { get; set; } = new();
    public List<MetodoPagoReporte> MetodosPago { get; set; } = new();

    public override string ToString()
    {
        var periodo = FechaFin.HasValue
            ? $"Período: {Fecha:yyyy-MM-dd} a {FechaFin:yyyy-MM-dd}"
            : $"Fecha: {Fecha:yyyy-MM-dd}";

        return $"{periodo}\n" +
               $"Total Pedidos: {TotalPedidos}\n" +
               $"Ingresos: ${IngresoTotal:F2}\n" +
               $"Promedio por Pedido: ${PromedioVentaPorPedido:F2}";
    }
}

public class ItemReporte
{
    public string ProductoId { get; set; } = string.Empty;
    public string NombreProducto { get; set; } = string.Empty;
    public int CantidadVendida { get; set; }
    public decimal IngresoTotal { get; set; }
}

public class MetodoPagoReporte
{
    public string TipoMetodo { get; set; } = string.Empty;
    public int CantidadTransacciones { get; set; }
    public decimal MontoTotal { get; set; }
}

public class ProductoMasVendido
{
    public string ProductoId { get; set; } = string.Empty;
    public string NombreProducto { get; set; } = string.Empty;
    public int CantidadVendida { get; set; }
    public decimal IngresoGenerado { get; set; }
    public int Ranking { get; set; }

    public override string ToString()
    {
        return $"{Ranking}. {NombreProducto} - Vendidos: {CantidadVendida}, Ingresos: ${IngresoGenerado:F2}";
    }
}