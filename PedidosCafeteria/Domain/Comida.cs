namespace PedidosCafeteria.Domain;

public sealed class Comida : Producto
{
    public TipoComida Tipo { get; }
    public bool RequierePreparacion { get; }
    public TimeSpan TiempoPreparacion { get; }

    // Herencia: implementación específica del precio final
    public override decimal PrecioFinal => PrecioBase * 1.19m; // IVA 19%

    public Comida(string id, string nombre, decimal precioBase, int stockInicial,
                  TipoComida tipo, bool requierePreparacion, TimeSpan tiempoPreparacion,
                  string descripcion = "")
        : base(id, nombre, precioBase, stockInicial, descripcion)
    {
        Tipo = tipo;
        RequierePreparacion = requierePreparacion;
        TiempoPreparacion = tiempoPreparacion;
    }
}

public enum TipoComida
{
    Sandwich,
    Ensalada,
    Snack,
    Postre,
    Plato
}