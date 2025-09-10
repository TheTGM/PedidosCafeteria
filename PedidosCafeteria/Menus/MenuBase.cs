using PedidosCafeteria.Domain;

namespace PedidosCafeteria.Menus;

public abstract class MenuBase
{
    protected static void MostrarTitulo(string titulo)
    {
        Console.Clear();
        Console.WriteLine("=".PadRight(50, '='));
        Console.WriteLine($" {titulo.ToUpper()}");
        Console.WriteLine("=".PadRight(50, '='));
        Console.WriteLine();
    }

    protected static void MostrarMensaje(string mensaje, bool esError = false)
    {
        if (esError)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ {mensaje}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ {mensaje}");
        }
        Console.ResetColor();
    }

    protected static void MostrarInfo(string mensaje)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"ℹ️  {mensaje}");
        Console.ResetColor();
    }

    protected static void MostrarAdvertencia(string mensaje)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"⚠️  {mensaje}");
        Console.ResetColor();
    }

    protected static string LeerTexto(string prompt)
    {
        Console.Write($"{prompt}: ");
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    protected static int LeerEntero(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            if (int.TryParse(Console.ReadLine(), out int valor) && valor >= 0)
                return valor;

            MostrarMensaje("Por favor ingrese un número válido (mayor o igual a 0)", true);
        }
    }

    protected static decimal LeerDecimal(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal valor) && valor > 0)
                return valor;

            MostrarMensaje("Por favor ingrese un número decimal válido (mayor a 0)", true);
        }
    }

    protected static DateTime LeerFecha(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} (yyyy-mm-dd): ");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
                return DateTime.Today;

            if (DateTime.TryParse(input, out DateTime fecha))
                return fecha;

            MostrarMensaje("Formato de fecha inválido. Use yyyy-mm-dd", true);
        }
    }

    protected static bool LeerConfirmacion(string prompt)
    {
        Console.Write($"{prompt} (s/n): ");
        var respuesta = Console.ReadLine()?.Trim().ToLower();
        return respuesta == "s" || respuesta == "si" || respuesta == "sí";
    }

    protected static void PausarPantalla()
    {
        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    protected static void MostrarProducto(Producto producto)
    {
        var tipo = producto switch
        {
            Bebida b => $"Bebida ({b.Tipo})" + (b.EsCaliente ? " ☕" : " 🧊"),
            Comida c => $"Comida ({c.Tipo})" + (c.RequierePreparacion ? " 👨‍🍳" : " 🍽️"),
            _ => "Producto"
        };

        var estadoStock = producto.Stock switch
        {
            0 => "🔴 SIN STOCK",
            < 5 => "🟡 STOCK BAJO",
            _ => "🟢 DISPONIBLE"
        };

        Console.WriteLine($"[{producto.Id}] {producto.Nombre}");
        Console.WriteLine($"    Tipo: {tipo}");
        Console.WriteLine($"    Precio: ${producto.PrecioFinal:F0} (Base: ${producto.PrecioBase:F0})");
        Console.WriteLine($"    Stock: {producto.Stock} - {estadoStock}");

        if (producto.StockDisponible != producto.Stock)
        {
            Console.WriteLine($"    Stock Disponible: {producto.StockDisponible} (Reservado: {producto.Stock - producto.StockDisponible})");
        }

        if (!string.IsNullOrEmpty(producto.Descripcion))
            Console.WriteLine($"    Descripción: {producto.Descripcion}");

        if (!producto.Activo)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"    ❌ PRODUCTO DESACTIVADO");
            Console.ResetColor();
        }

        Console.WriteLine();
    }

    protected static void MostrarPedido(Pedido pedido)
    {
        var estado = pedido.Estado switch
        {
            EstadoPedido.Pendiente => "⏳ Pendiente",
            EstadoPedido.PagoProcesado => "💳 Pagado",
            EstadoPedido.EnPreparacion => "👨‍🍳 En Preparación",
            EstadoPedido.ListoParaRecoger => "✅ Listo",
            EstadoPedido.Completado => "📦 Completado",
            EstadoPedido.Cancelado => "❌ Cancelado",
            _ => pedido.Estado.ToString()
        };

        Console.WriteLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine($"📋 Pedido: {pedido.Id}");
        Console.WriteLine($"👤 Estudiante: {pedido.EstudianteId}");
        Console.WriteLine($"📅 Estado: {estado}");
        Console.WriteLine($"🕐 Fecha: {pedido.FechaCreacion:yyyy-MM-dd HH:mm}");

        if (pedido.FechaPago.HasValue)
        {
            Console.WriteLine($"💰 Pagado: {pedido.FechaPago:yyyy-MM-dd HH:mm}");
        }

        if (pedido.Items.Any())
        {
            Console.WriteLine("🛒 Items:");
            foreach (var item in pedido.Items)
            {
                Console.WriteLine($"  • {item.Cantidad}x {item.Producto.Nombre} - ${item.Subtotal:F0}");
                Console.WriteLine($"    (${item.PrecioUnitario:F0} c/u)");
            }
            Console.WriteLine($"💵 Total: ${pedido.Total:F0}");
        }
        else
        {
            Console.WriteLine("🛒 Items: (vacío)");
        }

        if (pedido.MetodoPago != null)
        {
            var tipoPago = pedido.MetodoPago.GetType().Name switch
            {
                "PagoEfectivo" => "💵 Efectivo",
                "PagoTarjeta" => "💳 Tarjeta",
                "PagoBono" => "🎫 Bono Estudiantil",
                _ => "💰 Pago"
            };
            Console.WriteLine($"💳 Método de Pago: {tipoPago}");
        }

        Console.WriteLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine();
    }

    protected static void MostrarListaProductos(IEnumerable<Producto> productos, string titulo = "PRODUCTOS")
    {
        var listaProductos = productos.ToList();

        if (!listaProductos.Any())
        {
            MostrarInfo("No hay productos disponibles.");
            return;
        }

        Console.WriteLine($"📦 {titulo}:");
        Console.WriteLine();

        var bebidas = listaProductos.OfType<Bebida>().ToList();
        var comidas = listaProductos.OfType<Comida>().ToList();

        if (bebidas.Any())
        {
            Console.WriteLine("🥤 BEBIDAS:");
            Console.WriteLine();
            foreach (var bebida in bebidas)
            {
                MostrarProducto(bebida);
            }
        }

        if (comidas.Any())
        {
            Console.WriteLine("🍽️ COMIDAS:");
            Console.WriteLine();
            foreach (var comida in comidas)
            {
                MostrarProducto(comida);
            }
        }
    }

    protected static void MostrarEstadisticasRapidas(IEnumerable<Producto> productos)
    {
        var lista = productos.ToList();
        var totalProductos = lista.Count;
        var sinStock = lista.Count(p => p.Stock == 0);
        var stockBajo = lista.Count(p => p.Stock > 0 && p.Stock <= 5);
        var valorInventario = lista.Sum(p => p.Stock * p.PrecioBase);

        Console.WriteLine("📊 ESTADÍSTICAS RÁPIDAS:");
        Console.WriteLine($"   • Total productos: {totalProductos}");
        Console.WriteLine($"   • Sin stock: {sinStock}");
        Console.WriteLine($"   • Stock bajo (≤5): {stockBajo}");
        Console.WriteLine($"   • Valor inventario: ${valorInventario:F0}");
        Console.WriteLine();
    }

    protected static Producto? SeleccionarProducto(IEnumerable<Producto> productos, string mensaje = "Seleccione un producto")
    {
        var lista = productos.ToList();

        if (!lista.Any())
        {
            MostrarInfo("No hay productos disponibles.");
            return null;
        }

        Console.WriteLine($"\n{mensaje}:");
        for (int i = 0; i < lista.Count; i++)
        {
            var producto = lista[i];
            var estado = producto.Stock == 0 ? " (SIN STOCK)" : $" (Stock: {producto.StockDisponible})";
            Console.WriteLine($"{i + 1}. {producto.Nombre} - ${producto.PrecioFinal:F0}{estado}");
        }

        while (true)
        {
            var seleccion = LeerEntero("Número del producto");

            if (seleccion >= 1 && seleccion <= lista.Count)
            {
                return lista[seleccion - 1];
            }

            MostrarMensaje($"Selección inválida. Debe ser entre 1 y {lista.Count}", true);
        }
    }

    protected static void MostrarBarra(string texto, char caracter = '=', int longitud = 50)
    {
        Console.WriteLine();
        Console.WriteLine(caracter.ToString().PadRight(longitud, caracter));
        if (!string.IsNullOrEmpty(texto))
        {
            var padding = (longitud - texto.Length) / 2;
            Console.WriteLine($"{new string(' ', Math.Max(0, padding))}{texto}");
            Console.WriteLine(caracter.ToString().PadRight(longitud, caracter));
        }
        Console.WriteLine();
    }

    protected static void LimpiarPantalla()
    {
        Console.Clear();
    }

    protected static void MostrarMenu(string titulo, params string[] opciones)
    {
        MostrarTitulo(titulo);

        for (int i = 0; i < opciones.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {opciones[i]}");
        }

        Console.WriteLine("0. Volver");
        Console.WriteLine();
    }

    protected static void MostrarResumen(string titulo, Dictionary<string, object> datos)
    {
        MostrarBarra(titulo);

        foreach (var kvp in datos)
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        }

        Console.WriteLine();
    }
}