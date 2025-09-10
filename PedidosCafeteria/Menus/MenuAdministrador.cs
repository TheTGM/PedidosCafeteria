using PedidosCafeteria.Application;
using PedidosCafeteria.Domain;

namespace PedidosCafeteria.Menus;

public class MenuAdministrador : MenuBase
{
    private readonly ServicioCafeteria _servicioCafeteria;
    private readonly GeneradorReportes _generadorReportes;

    public MenuAdministrador(ServicioCafeteria servicioCafeteria, GeneradorReportes generadorReportes)
    {
        _servicioCafeteria = servicioCafeteria ?? throw new ArgumentNullException(nameof(servicioCafeteria));
        _generadorReportes = generadorReportes ?? throw new ArgumentNullException(nameof(generadorReportes));
    }

    public void Mostrar()
    {
        bool continuar = true;

        while (continuar)
        {
            try
            {
                MostrarTitulo("CAFETERÍA UNI - ADMINISTRADOR");

                Console.WriteLine("GESTIÓN DE PRODUCTOS");
                Console.WriteLine("1. Ver Todos los Productos");
                Console.WriteLine("2. Agregar Nuevo Producto");
                Console.WriteLine("3. Actualizar Precio");
                Console.WriteLine("4. Desactivar Producto");

                Console.WriteLine("\nREPORTES");
                Console.WriteLine("5. Reporte de Ventas del Día");
                Console.WriteLine("6. Productos Más Vendidos");
                Console.WriteLine("7. Ingresos del Día");
                Console.WriteLine("8. Reporte de Stock Bajo");

                Console.WriteLine("\n0. Volver al Menú Principal");
                Console.WriteLine();

                var opcion = LeerTexto("Seleccione una opción");

                switch (opcion)
                {
                    case "1":
                        VerTodosLosProductos();
                        break;
                    case "2":
                        AgregarNuevoProducto();
                        break;
                    case "3":
                        ActualizarPrecio();
                        break;
                    case "4":
                        DesactivarProducto();
                        break;
                    case "5":
                        ReporteVentasDelDia();
                        break;
                    case "6":
                        ProductosMasVendidos();
                        break;
                    case "7":
                        IngresosDelDia();
                        break;
                    case "8":
                        ReporteStockBajo();
                        break;
                    case "0":
                        continuar = false;
                        break;
                    default:
                        MostrarMensaje("Opción no válida", true);
                        PausarPantalla();
                        break;
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error: {ex.Message}", true);
                PausarPantalla();
            }
        }
    }

    private void VerTodosLosProductos()
    {
        MostrarTitulo("TODOS LOS PRODUCTOS");

        var productos = _servicioCafeteria.ObtenerProductosDisponibles().ToList();

        if (!productos.Any())
        {
            MostrarInfo("No hay productos registrados.");
        }
        else
        {
            foreach (var producto in productos)
            {
                MostrarProducto(producto);
                if (!producto.Activo)
                {
                    Console.WriteLine("    ⚠️ PRODUCTO DESACTIVADO");
                    Console.WriteLine();
                }
            }
        }

        PausarPantalla();
    }

    private void AgregarNuevoProducto()
    {
        MostrarTitulo("AGREGAR NUEVO PRODUCTO");

        Console.WriteLine("Tipo de producto:");
        Console.WriteLine("1. Bebida");
        Console.WriteLine("2. Comida");

        var tipo = LeerTexto("Seleccione tipo");

        var id = LeerTexto("ID del producto (ej: BEB001, COM001)");
        var nombre = LeerTexto("Nombre");
        var precio = LeerDecimal("Precio base");
        var stock = LeerEntero("Stock inicial");
        var descripcion = LeerTexto("Descripción (opcional)");

        try
        {
            Producto nuevoProducto;

            if (tipo == "1")
            {
                Console.WriteLine("Tipo de bebida:");
                Console.WriteLine("1. Café  2. Té  3. Jugo  4. Gaseosa  5. Agua");
                var tipoBebida = LeerTexto("Tipo");

                var esCaliente = LeerTexto("¿Es caliente? (s/n)").ToLower() == "s";

                TipoBebida tipoEnum = tipoBebida switch
                {
                    "1" => TipoBebida.Cafe,
                    "2" => TipoBebida.Te,
                    "3" => TipoBebida.Jugo,
                    "4" => TipoBebida.Gaseosa,
                    "5" => TipoBebida.Agua,
                    _ => TipoBebida.Agua
                };

                nuevoProducto = new Bebida(id, nombre, precio, stock, tipoEnum, esCaliente, descripcion);
            }
            else
            {
                Console.WriteLine("Tipo de comida:");
                Console.WriteLine("1. Sandwich  2. Ensalada  3. Snack  4. Postre  5. Plato");
                var tipoComida = LeerTexto("Tipo");

                var requierePreparacion = LeerTexto("¿Requiere preparación? (s/n)").ToLower() == "s";
                TimeSpan tiempoPreparacion = TimeSpan.Zero;

                if (requierePreparacion)
                {
                    var minutos = LeerEntero("Tiempo de preparación (minutos)");
                    tiempoPreparacion = TimeSpan.FromMinutes(minutos);
                }

                TipoComida tipoEnum = tipoComida switch
                {
                    "1" => TipoComida.Sandwich,
                    "2" => TipoComida.Ensalada,
                    "3" => TipoComida.Snack,
                    "4" => TipoComida.Postre,
                    "5" => TipoComida.Plato,
                    _ => TipoComida.Snack
                };

                nuevoProducto = new Comida(id, nombre, precio, stock, tipoEnum, requierePreparacion, tiempoPreparacion, descripcion);
            }

            _servicioCafeteria.RegistrarProducto(nuevoProducto);
            MostrarMensaje("Producto agregado exitosamente");
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error agregando producto: {ex.Message}", true);
        }

        PausarPantalla();
    }

    private void ActualizarPrecio()
    {
        MostrarTitulo("ACTUALIZAR PRECIO");

        var productoId = LeerTexto("ID del producto");
        var nuevoPrecio = LeerDecimal("Nuevo precio base");

        try
        {
            _servicioCafeteria.ActualizarPrecioProducto(productoId, nuevoPrecio);
            MostrarMensaje($"Precio actualizado para producto {productoId}");
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error: {ex.Message}", true);
        }

        PausarPantalla();
    }

    private void DesactivarProducto()
    {
        MostrarTitulo("DESACTIVAR PRODUCTO");

        var productoId = LeerTexto("ID del producto a desactivar");

        try
        {
            _servicioCafeteria.DesactivarProducto(productoId);
            MostrarMensaje($"Producto {productoId} desactivado");
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error: {ex.Message}", true);
        }

        PausarPantalla();
    }

    private void ReporteVentasDelDia()
    {
        MostrarTitulo("REPORTE DE VENTAS DEL DÍA");

        var fechaTexto = LeerTexto("Fecha (yyyy-mm-dd) o Enter para hoy");
        DateTime fecha = DateTime.Today;

        if (!string.IsNullOrWhiteSpace(fechaTexto))
        {
            if (!DateTime.TryParse(fechaTexto, out fecha))
            {
                MostrarMensaje("Fecha inválida, usando fecha de hoy", true);
                fecha = DateTime.Today;
            }
        }

        var reporte = _generadorReportes.GenerarReporteDiario(fecha);

        Console.WriteLine();
        Console.WriteLine("📊 REPORTE DE VENTAS");
        Console.WriteLine("=".PadRight(40, '='));
        Console.WriteLine(reporte.ToString());

        if (reporte.ProductosVendidos.Any())
        {
            Console.WriteLine("\n🛍️ PRODUCTOS VENDIDOS:");
            foreach (var item in reporte.ProductosVendidos.Take(10))
            {
                Console.WriteLine($"  • {item.NombreProducto}: {item.CantidadVendida} unidades - ${item.IngresoTotal:F0}");
            }
        }

        if (reporte.MetodosPago.Any())
        {
            Console.WriteLine("\n💳 MÉTODOS DE PAGO:");
            foreach (var metodo in reporte.MetodosPago)
            {
                Console.WriteLine($"  • {metodo.TipoMetodo}: {metodo.CantidadTransacciones} transacciones - ${metodo.MontoTotal:F0}");
            }
        }

        PausarPantalla();
    }

    private void ProductosMasVendidos()
    {
        MostrarTitulo("PRODUCTOS MÁS VENDIDOS");

        var top = LeerEntero("¿Cuántos productos mostrar? (máximo)");
        if (top <= 0) top = 10;

        var productos = _generadorReportes.ObtenerProductosMasVendidos(top).ToList();

        if (!productos.Any())
        {
            MostrarInfo("No hay datos de ventas disponibles.");
        }
        else
        {
            Console.WriteLine($"\n🏆 TOP {productos.Count} PRODUCTOS MÁS VENDIDOS (últimos 30 días):");
            Console.WriteLine("=".PadRight(60, '='));

            foreach (var producto in productos)
            {
                Console.WriteLine(producto.ToString());
            }
        }

        PausarPantalla();
    }

    private void IngresosDelDia()
    {
        MostrarTitulo("INGRESOS DEL DÍA");

        var fechaTexto = LeerTexto("Fecha (yyyy-mm-dd) o Enter para hoy");
        DateTime fecha = DateTime.Today;

        if (!string.IsNullOrWhiteSpace(fechaTexto))
        {
            DateTime.TryParse(fechaTexto, out fecha);
        }

        var ingresos = _generadorReportes.CalcularIngresosDelDia(fecha);
        var pedidos = _generadorReportes.ContarPedidosCompletados(fecha);

        Console.WriteLine();
        Console.WriteLine($"💰 INGRESOS DEL DÍA {fecha:yyyy-MM-dd}");
        Console.WriteLine("=".PadRight(40, '='));
        Console.WriteLine($"Total de ingresos: ${ingresos:F0}");
        Console.WriteLine($"Pedidos completados: {pedidos}");

        if (pedidos > 0)
        {
            Console.WriteLine($"Promedio por pedido: ${ingresos / pedidos:F0}");
        }

        PausarPantalla();
    }

    private void ReporteStockBajo()
    {
        MostrarTitulo("REPORTE DE STOCK BAJO");

        var limite = LeerEntero("Límite de stock (productos con stock menor o igual)");

        // Obtener productos con stock bajo
        var productos = _servicioCafeteria.ObtenerProductosDisponibles()
            .Where(p => p.Stock <= limite)
            .OrderBy(p => p.Stock)
            .ToList();

        Console.WriteLine();
        if (!productos.Any())
        {
            MostrarInfo($"No hay productos con stock menor o igual a {limite}.");
        }
        else
        {
            Console.WriteLine($"⚠️ PRODUCTOS CON STOCK BAJO (≤ {limite}):");
            Console.WriteLine("=".PadRight(50, '='));

            foreach (var producto in productos)
            {
                var estado = producto.Stock == 0 ? "🔴 SIN STOCK" : "🟡 STOCK BAJO";
                Console.WriteLine($"{estado} - [{producto.Id}] {producto.Nombre}: {producto.Stock} unidades");
            }
        }

        PausarPantalla();
    }
}