namespace PedidosCafeteria.Domain.Contratos;

public interface IRepositorioPedidos
{
    void Guardar(Pedido pedido);
    void Actualizar(Pedido pedido);
    Pedido? ObtenerPorId(string id);
    IEnumerable<Pedido> ListarPorEstudiante(string estudianteId);
    IEnumerable<Pedido> ListarPorEstado(EstadoPedido estado);
    IEnumerable<Pedido> ListarPorFecha(DateTime fecha);
    IEnumerable<Pedido> ListarCompletadosEnPeriodo(DateTime inicio, DateTime fin);
}