using PedidosCafeteria.Domain;
using PedidosCafeteria.Domain.Contratos;

namespace PedidosCafeteria.Infrastructure;

public class RepositorioProductosMemoria : IRepositorioProductos
{
    private readonly Dictionary<string, Producto> _productos = new();

    public void Agregar(Producto producto)
    {
        if (producto == null) throw new ArgumentNullException(nameof(producto));

        if (_productos.ContainsKey(producto.Id))
            throw new InvalidOperationException($"Ya existe un producto con ID '{producto.Id}'");

        _productos[producto.Id] = producto;
    }

    public void Actualizar(Producto producto)
    {
        if (producto == null) throw new ArgumentNullException(nameof(producto));

        if (!_productos.ContainsKey(producto.Id))
            throw new InvalidOperationException($"Producto con ID '{producto.Id}' no existe");

        _productos[producto.Id] = producto;
    }

    public Producto? ObtenerPorId(string id)
    {
        return _productos.TryGetValue(id, out var producto) ? producto : null;
    }

    public IEnumerable<Producto> Listar()
    {
        return _productos.Values.ToList();
    }

    public IEnumerable<Producto> ListarActivos()
    {
        return _productos.Values.Where(p => p.Activo).ToList();
    }

    public IEnumerable<Producto> ListarConStock()
    {
        return _productos.Values.Where(p => p.Activo && p.StockDisponible > 0).ToList();
    }

    public bool Existe(string id)
    {
        return _productos.ContainsKey(id);
    }
}