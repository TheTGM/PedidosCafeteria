namespace PedidosCafeteria.Domain;

public class ItemPedido
{
    public Producto Producto { get; }
    public int Cantidad { get; private set; }
    public decimal PrecioUnitario { get; }
    public decimal Subtotal => PrecioUnitario * Cantidad;

    public ItemPedido(Producto producto, int cantidad)
    {
        if (producto == null) throw new ArgumentNullException(nameof(producto));
        if (cantidad <= 0) throw new ArgumentException("Cantidad debe ser positiva", nameof(cantidad));

        Producto = producto;
        Cantidad = cantidad;
        PrecioUnitario = producto.PrecioFinal; // Precio al momento de agregar
    }

    public void ActualizarCantidad(int nuevaCantidad)
    {
        if (nuevaCantidad <= 0) throw new ArgumentException("Cantidad debe ser positiva");
        Cantidad = nuevaCantidad;
    }
}
