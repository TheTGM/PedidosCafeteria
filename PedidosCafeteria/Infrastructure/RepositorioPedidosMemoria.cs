using PedidosCafeteria.Domain;
using PedidosCafeteria.Domain.Contratos;

namespace PedidosCafeteria.Infrastructure;

public class RepositorioPedidosMemoria : IRepositorioPedidos
{
    private readonly Dictionary<string, Pedido> _pedidos = new();

    public void Guardar(Pedido pedido)
    {
        if (pedido == null) throw new ArgumentNullException(nameof(pedido));
        _pedidos[pedido.Id] = pedido;
    }

    public void Actualizar(Pedido pedido)
    {
        if (pedido == null) throw new ArgumentNullException(nameof(pedido));

        if (!_pedidos.ContainsKey(pedido.Id))
            throw new InvalidOperationException($"Pedido con ID '{pedido.Id}' no existe");

        _pedidos[pedido.Id] = pedido;
    }

    public Pedido? ObtenerPorId(string id)
    {
        return _pedidos.TryGetValue(id, out var pedido) ? pedido : null;
    }

    public IEnumerable<Pedido> ListarPorEstudiante(string estudianteId)
    {
        return _pedidos.Values
            .Where(p => p.EstudianteId == estudianteId)
            .OrderByDescending(p => p.FechaCreacion)
            .ToList();
    }

    public IEnumerable<Pedido> ListarPorEstado(EstadoPedido estado)
    {
        return _pedidos.Values
            .Where(p => p.Estado == estado)
            .OrderBy(p => p.FechaCreacion)
            .ToList();
    }

    public IEnumerable<Pedido> ListarPorFecha(DateTime fecha)
    {
        return _pedidos.Values
            .Where(p => p.FechaCreacion.Date == fecha.Date)
            .OrderBy(p => p.FechaCreacion)
            .ToList();
    }

    public IEnumerable<Pedido> ListarCompletadosEnPeriodo(DateTime inicio, DateTime fin)
    {
        return _pedidos.Values
            .Where(p => p.Estado == EstadoPedido.Completado &&
                       p.FechaCreacion.Date >= inicio.Date &&
                       p.FechaCreacion.Date <= fin.Date)
            .OrderBy(p => p.FechaCreacion)
            .ToList();
    }
}