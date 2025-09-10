namespace PedidosCafeteria.Domain;

public class Pedido
{
    private readonly List<ItemPedido> _items = new();
    private EstadoPedido _estado = EstadoPedido.Pendiente;
    private MetodoPago? _metodoPago;

    public string Id { get; }
    public string EstudianteId { get; }
    public DateTime FechaCreacion { get; }
    public DateTime? FechaPago { get; private set; }
    public EstadoPedido Estado => _estado;
    public MetodoPago? MetodoPago => _metodoPago;

    // Encapsulamiento: colección de solo lectura
    public IReadOnlyCollection<ItemPedido> Items => _items.AsReadOnly();

    public decimal Subtotal => _items.Sum(item => item.Subtotal);
    public decimal Total => Subtotal; // Ya incluye IVA en precio final de productos

    public Pedido(string estudianteId)
    {
        if (string.IsNullOrWhiteSpace(estudianteId))
            throw new ArgumentException("ID estudiante requerido", nameof(estudianteId));

        Id = Guid.NewGuid().ToString();
        EstudianteId = estudianteId;
        FechaCreacion = DateTime.Now;
    }

    // Encapsulamiento: control de estado para modificaciones
    public void AgregarItem(Producto producto, int cantidad)
    {
        if (_estado != EstadoPedido.Pendiente)
            throw new InvalidOperationException("No se puede modificar pedido en estado: " + _estado);

        if (producto == null) throw new ArgumentNullException(nameof(producto));
        if (!producto.Activo) throw new InvalidOperationException("Producto no disponible");

        var itemExistente = _items.FirstOrDefault(i => i.Producto.Id == producto.Id);

        if (itemExistente != null)
        {
            itemExistente.ActualizarCantidad(itemExistente.Cantidad + cantidad);
        }
        else
        {
            _items.Add(new ItemPedido(producto, cantidad));
        }
    }

    public void RemoverItem(string productoId)
    {
        if (_estado != EstadoPedido.Pendiente)
            throw new InvalidOperationException("No se puede modificar pedido en estado: " + _estado);

        var item = _items.FirstOrDefault(i => i.Producto.Id == productoId);
        if (item != null)
        {
            _items.Remove(item);
        }
    }

    public void ConfirmarPago(MetodoPago metodoPago)
    {
        if (_estado != EstadoPedido.Pendiente)
            throw new InvalidOperationException("Pedido ya fue procesado");

        if (metodoPago == null) throw new ArgumentNullException(nameof(metodoPago));
        if (_items.Count == 0) throw new InvalidOperationException("Pedido vacío");

        if (!metodoPago.Procesar(Total))
            throw new InvalidOperationException("Error procesando pago");

        _metodoPago = metodoPago;
        _estado = EstadoPedido.PagoProcesado;
        FechaPago = DateTime.Now;
    }

    public void IniciarPreparacion()
    {
        if (_estado != EstadoPedido.PagoProcesado)
            throw new InvalidOperationException("Pedido debe estar pagado");

        _estado = EstadoPedido.EnPreparacion;
    }

    public void MarcarListo()
    {
        if (_estado != EstadoPedido.EnPreparacion)
            throw new InvalidOperationException("Pedido debe estar en preparación");

        _estado = EstadoPedido.ListoParaRecoger;
    }

    public void Completar()
    {
        if (_estado != EstadoPedido.ListoParaRecoger)
            throw new InvalidOperationException("Pedido debe estar listo para recoger");

        _estado = EstadoPedido.Completado;
    }

    public void Cancelar()
    {
        if (_estado == EstadoPedido.Completado)
            throw new InvalidOperationException("No se puede cancelar pedido completado");

        _estado = EstadoPedido.Cancelado;
    }
}