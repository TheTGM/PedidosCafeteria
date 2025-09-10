namespace PedidosCafeteria.Domain.Excepciones;

public abstract class CafeteriaException : Exception
{
    protected CafeteriaException(string mensaje) : base(mensaje) { }
    protected CafeteriaException(string mensaje, Exception innerException) : base(mensaje, innerException) { }
}

public class ProductoNoEncontradoException : CafeteriaException
{
    public string ProductoId { get; }

    public ProductoNoEncontradoException(string productoId)
        : base($"Producto con ID '{productoId}' no encontrado")
    {
        ProductoId = productoId;
    }
}

public class StockInsuficienteException : CafeteriaException
{
    public string ProductoId { get; }
    public int StockDisponible { get; }
    public int CantidadRequerida { get; }

    public StockInsuficienteException(string productoId, int stockDisponible, int cantidadRequerida)
        : base($"Stock insuficiente para producto '{productoId}'. Disponible: {stockDisponible}, Requerido: {cantidadRequerida}")
    {
        ProductoId = productoId;
        StockDisponible = stockDisponible;
        CantidadRequerida = cantidadRequerida;
    }
}

public class PedidoNoEncontradoException : CafeteriaException
{
    public string PedidoId { get; }

    public PedidoNoEncontradoException(string pedidoId)
        : base($"Pedido con ID '{pedidoId}' no encontrado")
    {
        PedidoId = pedidoId;
    }
}

public class EstadoPedidoInvalidoException : CafeteriaException
{
    public string PedidoId { get; }
    public EstadoPedido EstadoActual { get; }
    public EstadoPedido EstadoRequerido { get; }

    public EstadoPedidoInvalidoException(string pedidoId, EstadoPedido estadoActual, EstadoPedido estadoRequerido)
        : base($"Pedido '{pedidoId}' está en estado '{estadoActual}', se requiere estado '{estadoRequerido}'")
    {
        PedidoId = pedidoId;
        EstadoActual = estadoActual;
        EstadoRequerido = estadoRequerido;
    }
}

public class PagoRechazadoException : CafeteriaException
{
    public string Motivo { get; }

    public PagoRechazadoException(string motivo)
        : base($"Pago rechazado: {motivo}")
    {
        Motivo = motivo;
    }
}