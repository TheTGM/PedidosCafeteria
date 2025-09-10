using PedidosCafeteria.Domain;
using PedidosCafeteria.Domain.Contratos;
namespace PedidosCafeteria.Application;

public class CalculadorPrecios : ICalculadorPrecios
{
    public decimal CalcularSubtotal(IEnumerable<ItemPedido> items)
    {
        return items?.Sum(item => item.Subtotal) ?? 0;
    }

    public decimal CalcularIVA(decimal subtotal)
    {
        // IVA ya está incluido en el precio final de cada producto
        // Esta implementación es para casos donde se necesite calcular el IVA por separado
        return subtotal * 0.19m / 1.19m; // Extrae el IVA del subtotal
    }

    public decimal CalcularTotal(IEnumerable<ItemPedido> items)
    {
        // El total ya incluye IVA porque cada producto tiene PrecioFinal con IVA
        return CalcularSubtotal(items);
    }

    public decimal AplicarDescuento(decimal monto, decimal porcentajeDescuento)
    {
        if (porcentajeDescuento < 0 || porcentajeDescuento > 100)
            throw new ArgumentException("Porcentaje de descuento debe estar entre 0 y 100");

        return monto * (1 - porcentajeDescuento / 100);
    }
}