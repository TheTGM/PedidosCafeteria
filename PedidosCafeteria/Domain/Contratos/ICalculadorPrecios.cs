namespace PedidosCafeteria.Domain.Contratos;

public interface ICalculadorPrecios
{
    decimal CalcularSubtotal(IEnumerable<ItemPedido> items);
    decimal CalcularIVA(decimal subtotal);
    decimal CalcularTotal(IEnumerable<ItemPedido> items);
    decimal AplicarDescuento(decimal monto, decimal porcentajeDescuento);
}