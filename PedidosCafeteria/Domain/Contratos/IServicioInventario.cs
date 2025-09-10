namespace PedidosCafeteria.Domain.Contratos;

public interface IServicioInventario
{
    bool VerificarDisponibilidad(string productoId, int cantidadRequerida);
    void ReservarStock(string productoId, int cantidad);
    void LiberarReserva(string productoId, int cantidad);
    void ConfirmarVenta(string productoId, int cantidad);
    void ReponerStock(string productoId, int cantidadAgregar);

    // Información de inventario
    int ObtenerStockDisponible(string productoId);
    IEnumerable<Producto> ObtenerProductosBajoStock(int limite = 5);
    IEnumerable<Producto> ObtenerProductosSinStock();
}