namespace PedidosCafeteria.Domain.Contratos;

public interface IRepositorioProductos
{
    void Agregar(Producto producto);
    void Actualizar(Producto producto);
    Producto? ObtenerPorId(string id);
    IEnumerable<Producto> Listar();
    IEnumerable<Producto> ListarActivos();
    IEnumerable<Producto> ListarConStock();
    bool Existe(string id);
}