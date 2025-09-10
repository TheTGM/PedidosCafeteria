namespace PedidosCafeteria.Domain;

public sealed class Bebida : Producto
{
    public TipoBebida Tipo { get; }
    public bool EsCaliente { get; }

    // Herencia: implementación específica del precio final
    public override decimal PrecioFinal => PrecioBase * 1.19m; // IVA 19%

    public Bebida(string id, string nombre, decimal precioBase, int stockInicial,
                  TipoBebida tipo, bool esCaliente, string descripcion = "")
        : base(id, nombre, precioBase, stockInicial, descripcion)
    {
        Tipo = tipo;
        EsCaliente = esCaliente;
    }
}

public enum TipoBebida
{
    Cafe,
    Te,
    Jugo,
    Gaseosa,
    Agua
}