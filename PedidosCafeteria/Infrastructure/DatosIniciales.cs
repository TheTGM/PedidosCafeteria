using PedidosCafeteria.Domain;
using PedidosCafeteria.Domain.Contratos;

namespace PedidosCafeteria.Infrastructure;

public static class DatosIniciales
{
    public static List<Producto> ObtenerProductosIniciales()
    {
        return new List<Producto>
        {
            // Bebidas Calientes
            new Bebida("BEB001", "Café Americano", 3500, 50, TipoBebida.Cafe, true, "Café negro tradicional"),
            new Bebida("BEB002", "Café con Leche", 4000, 40, TipoBebida.Cafe, true, "Café con leche caliente"),
            new Bebida("BEB003", "Cappuccino", 4500, 30, TipoBebida.Cafe, true, "Café con leche espumada"),
            new Bebida("BEB004", "Chocolate Caliente", 4200, 25, TipoBebida.Te, true, "Chocolate con leche caliente"),
            new Bebida("BEB005", "Té Verde", 2800, 35, TipoBebida.Te, true, "Té verde natural"),
            
            // Bebidas Frías
            new Bebida("BEB006", "Jugo Natural", 3800, 20, TipoBebida.Jugo, false, "Jugo de frutas natural"),
            new Bebida("BEB007", "Gaseosa", 2500, 60, TipoBebida.Gaseosa, false, "Bebida gaseosa 350ml"),
            new Bebida("BEB008", "Agua", 2000, 100, TipoBebida.Agua, false, "Agua purificada 500ml"),
            new Bebida("BEB009", "Café Frío", 4800, 15, TipoBebida.Cafe, false, "Café frío con hielo"),
            
            // Comidas Rápidas
            new Comida("COM001", "Sandwich de Pollo", 8500, 20, TipoComida.Sandwich, true, TimeSpan.FromMinutes(5), "Sandwich con pollo y verduras"),
            new Comida("COM002", "Sandwich de Jamón", 7500, 25, TipoComida.Sandwich, true, TimeSpan.FromMinutes(3), "Sandwich de jamón y queso"),
            new Comida("COM003", "Ensalada César", 9200, 15, TipoComida.Ensalada, true, TimeSpan.FromMinutes(8), "Ensalada con pollo y aderezo césar"),
            new Comida("COM004", "Ensalada de Frutas", 6800, 10, TipoComida.Ensalada, true, TimeSpan.FromMinutes(5), "Mix de frutas frescas"),
            
            // Snacks
            new Comida("SNK001", "Empanada", 3200, 40, TipoComida.Snack, true, TimeSpan.FromMinutes(2), "Empanada de carne o pollo"),
            new Comida("SNK002", "Croissant", 4500, 20, TipoComida.Snack, true, TimeSpan.FromMinutes(3), "Croissant relleno"),
            new Comida("SNK003", "Muffin", 3800, 25, TipoComida.Snack, false, TimeSpan.Zero, "Muffin de arándanos"),
            new Comida("SNK004", "Galletas", 2200, 50, TipoComida.Snack, false, TimeSpan.Zero, "Paquete de galletas"),
            
            // Postres
            new Comida("PST001", "Brownie", 4200, 15, TipoComida.Postre, false, TimeSpan.Zero, "Brownie de chocolate"),
            new Comida("PST002", "Torta del Día", 5500, 8, TipoComida.Postre, false, TimeSpan.Zero, "Porción de torta especial"),
            
            // Platos del Día
            new Comida("PLT001", "Almuerzo Ejecutivo", 12500, 10, TipoComida.Plato, true, TimeSpan.FromMinutes(12), "Plato principal + bebida"),
        };
    }

    public static void CargarDatos(IRepositorioProductos repositorio)
    {
        var productos = ObtenerProductosIniciales();

        foreach (var producto in productos)
        {
            repositorio.Agregar(producto);
        }
    }
}