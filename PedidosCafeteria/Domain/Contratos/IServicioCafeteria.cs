namespace PedidosCafeteria.Domain.Contratos;

public interface IServicioCafeteria
{
    // Gestión de productos
    void RegistrarProducto(Producto producto);
    void ActualizarPrecioProducto(string productoId, decimal nuevoPrecio);
    void ActualizarStockProducto(string productoId, int nuevoStock);
    void DesactivarProducto(string productoId);
    IEnumerable<Producto> ObtenerProductosDisponibles();

    // Gestión de pedidos
    string CrearPedido(string estudianteId);
    void AgregarProductoAPedido(string pedidoId, string productoId, int cantidad);
    void RemoverProductoDePedido(string pedidoId, string productoId);
    bool ProcesarPago(string pedidoId, MetodoPago metodoPago);
    void CancelarPedido(string pedidoId);

    // Estados de pedido
    void IniciarPreparacionPedido(string pedidoId);
    void MarcarPedidoListo(string pedidoId);
    void CompletarPedido(string pedidoId);

    // Consultas
    Pedido? ObtenerPedido(string pedidoId);
    IEnumerable<Pedido> ObtenerPedidosPorEstado(EstadoPedido estado);
    IEnumerable<Pedido> ObtenerPedidosEstudiante(string estudianteId);
}