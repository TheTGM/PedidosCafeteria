namespace PedidosCafeteria.Domain;

public abstract class Producto
{
    private int _stock;
    private int _stockReservado;

    public string Id { get; }
    public string Nombre { get; private set; }
    public string Descripcion { get; private set; }
    public decimal PrecioBase { get; private set; }
    public bool Activo { get; private set; }

    // Encapsulamiento: Solo lectura del stock disponible
    public int Stock => _stock;
    public int StockDisponible => _stock - _stockReservado;

    // Abstracción: cada tipo de producto calcula su precio final diferente
    public abstract decimal PrecioFinal { get; }

    protected Producto(string id, string nombre, decimal precioBase, int stockInicial, string descripcion = "")
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("ID requerido", nameof(id));
        if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre requerido", nameof(nombre));
        if (precioBase <= 0) throw new ArgumentException("Precio debe ser positivo", nameof(precioBase));
        if (stockInicial < 0) throw new ArgumentException("Stock no puede ser negativo", nameof(stockInicial));

        Id = id;
        Nombre = nombre;
        PrecioBase = precioBase;
        _stock = stockInicial;
        Descripcion = descripcion;
        Activo = true;
    }

    // Encapsulamiento: Métodos controlados para modificar stock
    public void ReservarStock(int cantidad)
    {
        if (cantidad <= 0) throw new ArgumentException("Cantidad debe ser positiva");
        if (cantidad > StockDisponible) throw new InvalidOperationException("Stock insuficiente para reservar");

        _stockReservado += cantidad;
    }

    public void LiberarReserva(int cantidad)
    {
        if (cantidad <= 0) throw new ArgumentException("Cantidad debe ser positiva");
        if (cantidad > _stockReservado) throw new InvalidOperationException("No se puede liberar más de lo reservado");

        _stockReservado -= cantidad;
    }

    public void ConfirmarVenta(int cantidad)
    {
        if (cantidad <= 0) throw new ArgumentException("Cantidad debe ser positiva");
        if (cantidad > _stockReservado) throw new InvalidOperationException("Debe reservar antes de confirmar venta");

        _stock -= cantidad;
        _stockReservado -= cantidad;
    }

    public void ActualizarStock(int nuevoStock)
    {
        if (nuevoStock < 0) throw new ArgumentException("Stock no puede ser negativo");
        if (nuevoStock < _stockReservado) throw new InvalidOperationException("Stock no puede ser menor que reservaciones");

        _stock = nuevoStock;
    }

    public void ActualizarPrecio(decimal nuevoPrecio)
    {
        if (nuevoPrecio <= 0) throw new ArgumentException("Precio debe ser positivo");
        PrecioBase = nuevoPrecio;
    }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;
}