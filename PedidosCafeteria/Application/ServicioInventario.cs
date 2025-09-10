using PedidosCafeteria.Domain;
using PedidosCafeteria.Domain.Contratos;
using PedidosCafeteria.Domain.Excepciones;
namespace PedidosCafeteria.Application;

public class ServicioInventario : IServicioInventario
{
    private readonly IRepositorioProductos _repositorioProductos;

    public ServicioInventario(IRepositorioProductos repositorioProductos)
    {
        _repositorioProductos = repositorioProductos ?? throw new ArgumentNullException(nameof(repositorioProductos));
    }

    public bool VerificarDisponibilidad(string productoId, int cantidadRequerida)
    {
        var producto = _repositorioProductos.ObtenerPorId(productoId);
        if (producto == null || !producto.Activo)
            return false;

        return producto.StockDisponible >= cantidadRequerida;
    }

    public void ReservarStock(string productoId, int cantidad)
    {
        var producto = _repositorioProductos.ObtenerPorId(productoId)
            ?? throw new ProductoNoEncontradoException(productoId);

        if (!producto.Activo)
            throw new InvalidOperationException($"Producto '{productoId}' no está activo");

        try
        {
            producto.ReservarStock(cantidad);
            _repositorioProductos.Actualizar(producto);
        }
        catch (InvalidOperationException)
        {
            throw new StockInsuficienteException(productoId, producto.StockDisponible, cantidad);
        }
    }

    public void LiberarReserva(string productoId, int cantidad)
    {
        var producto = _repositorioProductos.ObtenerPorId(productoId)
            ?? throw new ProductoNoEncontradoException(productoId);

        producto.LiberarReserva(cantidad);
        _repositorioProductos.Actualizar(producto);
    }

    public void ConfirmarVenta(string productoId, int cantidad)
    {
        var producto = _repositorioProductos.ObtenerPorId(productoId)
            ?? throw new ProductoNoEncontradoException(productoId);

        producto.ConfirmarVenta(cantidad);
        _repositorioProductos.Actualizar(producto);
    }

    public void ReponerStock(string productoId, int cantidadAgregar)
    {
        var producto = _repositorioProductos.ObtenerPorId(productoId)
            ?? throw new ProductoNoEncontradoException(productoId);

        var nuevoStock = producto.Stock + cantidadAgregar;
        producto.ActualizarStock(nuevoStock);
        _repositorioProductos.Actualizar(producto);
    }

    public int ObtenerStockDisponible(string productoId)
    {
        var producto = _repositorioProductos.ObtenerPorId(productoId)
            ?? throw new ProductoNoEncontradoException(productoId);

        return producto.StockDisponible;
    }

    public IEnumerable<Producto> ObtenerProductosBajoStock(int limite = 5)
    {
        return _repositorioProductos.ListarActivos()
            .Where(p => p.Stock <= limite)
            .OrderBy(p => p.Stock);
    }

    public IEnumerable<Producto> ObtenerProductosSinStock()
    {
        return _repositorioProductos.ListarActivos()
            .Where(p => p.Stock == 0);
    }
}