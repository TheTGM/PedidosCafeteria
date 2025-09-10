using PedidosCafeteria.Domain;
using PedidosCafeteria.Domain.Contratos;
using PedidosCafeteria.Domain.Excepciones;
namespace PedidosCafeteria.Application;

public class ServicioCafeteria : IServicioCafeteria
{
    private readonly IRepositorioProductos _repositorioProductos;
    private readonly IRepositorioPedidos _repositorioPedidos;
    private readonly IServicioInventario _servicioInventario;
    private readonly ICalculadorPrecios _calculadorPrecios;

    public ServicioCafeteria(
        IRepositorioProductos repositorioProductos,
        IRepositorioPedidos repositorioPedidos,
        IServicioInventario servicioInventario,
        ICalculadorPrecios calculadorPrecios)
    {
        _repositorioProductos = repositorioProductos ?? throw new ArgumentNullException(nameof(repositorioProductos));
        _repositorioPedidos = repositorioPedidos ?? throw new ArgumentNullException(nameof(repositorioPedidos));
        _servicioInventario = servicioInventario ?? throw new ArgumentNullException(nameof(servicioInventario));
        _calculadorPrecios = calculadorPrecios ?? throw new ArgumentNullException(nameof(calculadorPrecios));
    }

    // Gestión de productos
    public void RegistrarProducto(Producto producto)
    {
        if (producto == null) throw new ArgumentNullException(nameof(producto));

        if (_repositorioProductos.Existe(producto.Id))
            throw new InvalidOperationException($"Ya existe un producto con ID '{producto.Id}'");

        _repositorioProductos.Agregar(producto);
    }

    public void ActualizarPrecioProducto(string productoId, decimal nuevoPrecio)
    {
        var producto = _repositorioProductos.ObtenerPorId(productoId)
            ?? throw new ProductoNoEncontradoException(productoId);

        producto.ActualizarPrecio(nuevoPrecio);
        _repositorioProductos.Actualizar(producto);
    }

    public void ActualizarStockProducto(string productoId, int nuevoStock)
    {
        var producto = _repositorioProductos.ObtenerPorId(productoId)
            ?? throw new ProductoNoEncontradoException(productoId);

        producto.ActualizarStock(nuevoStock);
        _repositorioProductos.Actualizar(producto);
    }

    public void DesactivarProducto(string productoId)
    {
        var producto = _repositorioProductos.ObtenerPorId(productoId)
            ?? throw new ProductoNoEncontradoException(productoId);

        producto.Desactivar();
        _repositorioProductos.Actualizar(producto);
    }

    public IEnumerable<Producto> ObtenerProductosDisponibles()
    {
        return _repositorioProductos.ListarConStock();
    }

    // Gestión de pedidos
    public string CrearPedido(string estudianteId)
    {
        if (string.IsNullOrWhiteSpace(estudianteId))
            throw new ArgumentException("ID de estudiante requerido", nameof(estudianteId));

        var pedido = new Pedido(estudianteId);
        _repositorioPedidos.Guardar(pedido);

        return pedido.Id;
    }

    public void AgregarProductoAPedido(string pedidoId, string productoId, int cantidad)
    {
        var pedido = _repositorioPedidos.ObtenerPorId(pedidoId)
            ?? throw new PedidoNoEncontradoException(pedidoId);

        var producto = _repositorioProductos.ObtenerPorId(productoId)
            ?? throw new ProductoNoEncontradoException(productoId);

        // Verificar disponibilidad de stock
        if (!_servicioInventario.VerificarDisponibilidad(productoId, cantidad))
            throw new StockInsuficienteException(productoId, producto.StockDisponible, cantidad);

        // Reservar stock temporalmente
        _servicioInventario.ReservarStock(productoId, cantidad);

        try
        {
            pedido.AgregarItem(producto, cantidad);
            _repositorioPedidos.Actualizar(pedido);
        }
        catch
        {
            // Si falla agregar el item, liberar la reserva
            _servicioInventario.LiberarReserva(productoId, cantidad);
            throw;
        }
    }

    public void RemoverProductoDePedido(string pedidoId, string productoId)
    {
        var pedido = _repositorioPedidos.ObtenerPorId(pedidoId)
            ?? throw new PedidoNoEncontradoException(pedidoId);

        // Obtener cantidad para liberar reserva
        var item = pedido.Items.FirstOrDefault(i => i.Producto.Id == productoId);
        if (item != null)
        {
            _servicioInventario.LiberarReserva(productoId, item.Cantidad);
            pedido.RemoverItem(productoId);
            _repositorioPedidos.Actualizar(pedido);
        }
    }

    public bool ProcesarPago(string pedidoId, MetodoPago metodoPago)
    {
        var pedido = _repositorioPedidos.ObtenerPorId(pedidoId)
            ?? throw new PedidoNoEncontradoException(pedidoId);

        if (pedido.Estado != EstadoPedido.Pendiente)
            throw new EstadoPedidoInvalidoException(pedidoId, pedido.Estado, EstadoPedido.Pendiente);

        try
        {
            pedido.ConfirmarPago(metodoPago);

            // Confirmar venta en inventario (convierte reserva en venta)
            foreach (var item in pedido.Items)
            {
                _servicioInventario.ConfirmarVenta(item.Producto.Id, item.Cantidad);
            }

            _repositorioPedidos.Actualizar(pedido);
            return true;
        }
        catch (InvalidOperationException ex)
        {
            throw new PagoRechazadoException(ex.Message);
        }
    }

    public void CancelarPedido(string pedidoId)
    {
        var pedido = _repositorioPedidos.ObtenerPorId(pedidoId)
            ?? throw new PedidoNoEncontradoException(pedidoId);

        // Liberar todas las reservas
        foreach (var item in pedido.Items)
        {
            _servicioInventario.LiberarReserva(item.Producto.Id, item.Cantidad);
        }

        pedido.Cancelar();
        _repositorioPedidos.Actualizar(pedido);
    }

    // Estados de pedido
    public void IniciarPreparacionPedido(string pedidoId)
    {
        var pedido = _repositorioPedidos.ObtenerPorId(pedidoId)
            ?? throw new PedidoNoEncontradoException(pedidoId);

        pedido.IniciarPreparacion();
        _repositorioPedidos.Actualizar(pedido);
    }

    public void MarcarPedidoListo(string pedidoId)
    {
        var pedido = _repositorioPedidos.ObtenerPorId(pedidoId)
            ?? throw new PedidoNoEncontradoException(pedidoId);

        pedido.MarcarListo();
        _repositorioPedidos.Actualizar(pedido);
    }

    public void CompletarPedido(string pedidoId)
    {
        var pedido = _repositorioPedidos.ObtenerPorId(pedidoId)
            ?? throw new PedidoNoEncontradoException(pedidoId);

        pedido.Completar();
        _repositorioPedidos.Actualizar(pedido);
    }

    // Consultas
    public Pedido? ObtenerPedido(string pedidoId)
    {
        return _repositorioPedidos.ObtenerPorId(pedidoId);
    }

    public IEnumerable<Pedido> ObtenerPedidosPorEstado(EstadoPedido estado)
    {
        return _repositorioPedidos.ListarPorEstado(estado);
    }

    public IEnumerable<Pedido> ObtenerPedidosEstudiante(string estudianteId)
    {
        return _repositorioPedidos.ListarPorEstudiante(estudianteId);
    }
}