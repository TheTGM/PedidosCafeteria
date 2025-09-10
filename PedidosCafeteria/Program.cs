// PedidosCafeteria/Program.cs
using PedidosCafeteria.Application;
using PedidosCafeteria.Domain;
using PedidosCafeteria.Infrastructure;
using PedidosCafeteria.Menus;
using System.Text;

namespace PedidosCafeteria;

class Program
{
    private static ServicioCafeteria _servicioCafeteria = null!;
    private static GeneradorReportes _generadorReportes = null!;

    static void Main(string[] args)
    {
        try
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Console.WriteLine("🚀 Iniciando Sistema de Cafetería Universitaria...");

            // Configurar dependencias
            ConfigurarServicios();

            // Cargar datos iniciales
            CargarDatosIniciales();

            Console.WriteLine("✅ Sistema iniciado correctamente\n");

            // Mostrar menú principal
            MostrarMenuPrincipal();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error crítico: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine("\nPresione cualquier tecla para salir...");
            Console.ReadKey();
        }
    }

    static void ConfigurarServicios()
    {
        // Crear repositorios (en memoria)
        var repositorioProductos = new RepositorioProductosMemoria();
        var repositorioPedidos = new RepositorioPedidosMemoria();

        // Crear servicios de aplicación
        var calculadorPrecios = new CalculadorPrecios();
        var servicioInventario = new ServicioInventario(repositorioProductos);

        _servicioCafeteria = new ServicioCafeteria(
            repositorioProductos,
            repositorioPedidos,
            servicioInventario,
            calculadorPrecios);

        _generadorReportes = new GeneradorReportes(repositorioPedidos, repositorioProductos);
    }

    static void CargarDatosIniciales()
    {
        // Cargar productos iniciales
        var productos = ObtenerProductosIniciales();

        foreach (var producto in productos)
        {
            try
            {
                _servicioCafeteria.RegistrarProducto(producto);
            }
            catch
            {
                // Ignorar errores de productos duplicados
            }
        }

        Console.WriteLine($"📦 Cargados {productos.Count} productos iniciales");
    }

    static List<Producto> ObtenerProductosIniciales()
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

    static void MostrarMenuPrincipal()
    {
        bool continuar = true;

        while (continuar)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("☕ SISTEMA DE CAFETERÍA UNIVERSITARIA");
                Console.WriteLine("=".PadRight(50, '='));
                Console.WriteLine();
                Console.WriteLine("Seleccione su perfil de usuario:");
                Console.WriteLine();
                Console.WriteLine("👨‍🎓 1. Estudiante");
                Console.WriteLine("👩‍🍳 2. Operador de Cafetería");
                Console.WriteLine("👨‍💼 3. Administrador");
                Console.WriteLine("📊 4. Ver Estado del Sistema");
                Console.WriteLine("ℹ️ 5. Acerca del Sistema");
                Console.WriteLine("❌ 0. Salir");
                Console.WriteLine();

                Console.Write("Opción: ");
                var opcion = Console.ReadLine()?.Trim();

                switch (opcion)
                {
                    case "1":
                        MenuEstudiante();
                        break;
                    case "2":
                        MenuOperador();
                        break;
                    case "3":
                        MenuAdministrador();
                        break;
                    case "4":
                        MostrarEstadoSistema();
                        break;
                    case "5":
                        MostrarAcercaDel();
                        break;
                    case "0":
                        continuar = false;
                        MostrarMensajeDespedida();
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("⚠️ Opción no válida. Intente nuevamente.");
                        Console.ResetColor();
                        Console.ReadKey();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("Presione cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }
    }

    static void MenuEstudiante()
    {
        Console.Write("Ingrese su ID de estudiante: ");
        var estudianteId = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(estudianteId))
        {
            Console.WriteLine("❌ ID de estudiante requerido");
            Console.ReadKey();
            return;
        }

        var menu = new MenuEstudiante(_servicioCafeteria, estudianteId);
        menu.Mostrar();
    }

    static void MenuOperador()
    {
        Console.WriteLine("🔑 Acceso de Operador");
        Console.Write("Ingrese clave de operador (demo: 'operador123'): ");
        var clave = Console.ReadLine();

        if (clave != "operador123")
        {
            Console.WriteLine("❌ Clave incorrecta");
            Console.ReadKey();
            return;
        }

        var menu = new MenuOperador(_servicioCafeteria);
        menu.Mostrar();
    }

    static void MenuAdministrador()
    {
        Console.WriteLine("🔑 Acceso de Administrador");
        Console.Write("Ingrese clave de administrador (demo: 'admin123'): ");
        var clave = Console.ReadLine();

        if (clave != "admin123")
        {
            Console.WriteLine("❌ Clave incorrecta");
            Console.ReadKey();
            return;
        }

        var menu = new MenuAdministrador(_servicioCafeteria, _generadorReportes);
        menu.Mostrar();
    }

    static void MostrarEstadoSistema()
    {
        Console.Clear();
        Console.WriteLine("📊 ESTADO DEL SISTEMA");
        Console.WriteLine("=".PadRight(50, '='));

        try
        {
            // Productos disponibles
            var productos = _servicioCafeteria.ObtenerProductosDisponibles().ToList();
            Console.WriteLine($"🛍️ Productos disponibles: {productos.Count}");

            // Stock total
            var stockTotal = productos.Sum(p => p.Stock);
            Console.WriteLine($"📦 Stock total: {stockTotal} unidades");

            // Productos sin stock
            var sinStock = productos.Count(p => p.Stock == 0);
            if (sinStock > 0)
            {
                Console.WriteLine($"⚠️ Productos sin stock: {sinStock}");
            }

            // Valor del inventario
            var valorInventario = productos.Sum(p => p.Stock * p.PrecioBase);
            Console.WriteLine($"💰 Valor del inventario: ${valorInventario:F0}");

            // Pedidos por estado
            var estadosPedidos = new[]
            {
                EstadoPedido.Pendiente,
                EstadoPedido.PagoProcesado,
                EstadoPedido.EnPreparacion,
                EstadoPedido.ListoParaRecoger,
                EstadoPedido.Completado
            };

            Console.WriteLine("\n📋 PEDIDOS POR ESTADO:");
            foreach (var estado in estadosPedidos)
            {
                var cantidad = _servicioCafeteria.ObtenerPedidosPorEstado(estado).Count();
                if (cantidad > 0)
                {
                    var emoji = estado switch
                    {
                        EstadoPedido.Pendiente => "⏳",
                        EstadoPedido.PagoProcesado => "💳",
                        EstadoPedido.EnPreparacion => "👨‍🍳",
                        EstadoPedido.ListoParaRecoger => "✅",
                        EstadoPedido.Completado => "📦",
                        _ => "•"
                    };
                    Console.WriteLine($"  {emoji} {estado}: {cantidad}");
                }
            }

            // Ingresos del día
            var ingresoHoy = _generadorReportes.CalcularIngresosDelDia(DateTime.Today);
            Console.WriteLine($"\n💰 Ingresos de hoy: ${ingresoHoy:F0}");

            // Productos más vendidos hoy
            Console.WriteLine("\n🏆 TOP 3 PRODUCTOS DE HOY:");
            var topHoy = _generadorReportes.ObtenerProductosMasVendidosPorPeriodo(DateTime.Today, DateTime.Today, 3);
            foreach (var producto in topHoy)
            {
                Console.WriteLine($"  {producto.Ranking}. {producto.NombreProducto}: {producto.CantidadVendida} vendidos");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error obteniendo estado: {ex.Message}");
        }

        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    static void MostrarAcercaDel()
    {
        Console.Clear();
        Console.WriteLine("ℹ️  ACERCA DEL SISTEMA");
        Console.WriteLine("=".PadRight(50, '='));
        Console.WriteLine();
        Console.WriteLine("🏫 Sistema de Cafetería Universitaria");
        Console.WriteLine("📅 Versión: 1.0.0");
        Console.WriteLine("🛠️ Tecnología: .NET 8 - C#");
        Console.WriteLine();
        Console.WriteLine("👨‍💻 Desarrollado aplicando los 4 pilares de POO:");
        Console.WriteLine("   ✅ Encapsulamiento - Estado protegido");
        Console.WriteLine("   ✅ Herencia - Jerarquías de clases");
        Console.WriteLine("   ✅ Polimorfismo - Interfaces dinámicas");
        Console.WriteLine("   ✅ Abstracción - Servicios simplificados");
        Console.WriteLine();
        Console.WriteLine("🎯 Funcionalidades:");
        Console.WriteLine("   • Gestión de pedidos estudiantiles");
        Console.WriteLine("   • Control de inventario en tiempo real");
        Console.WriteLine("   • Múltiples métodos de pago");
        Console.WriteLine("   • Reportes de ventas y análisis");
        Console.WriteLine("   • Estados de preparación y entrega");
        Console.WriteLine();
        Console.WriteLine("🔧 Credenciales de Demo:");
        Console.WriteLine("   👩‍🍳 Operador: operador123");
        Console.WriteLine("   👨‍💼 Administrador: admin123");
        Console.WriteLine();
        Console.WriteLine("📚 Proyecto académico - Programación Orientada a Objetos");

        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    static void MostrarMensajeDespedida()
    {
        Console.Clear();
        Console.WriteLine("👋 GRACIAS POR USAR EL SISTEMA DE CAFETERÍA UNIVERSITARIA");
        Console.WriteLine();
        Console.WriteLine("Sistema desarrollado aplicando los 4 pilares de POO:");
        Console.WriteLine("✅ Encapsulamiento - Estado protegido en entidades");
        Console.WriteLine("✅ Herencia - Jerarquías de Producto y MetodoPago");
        Console.WriteLine("✅ Polimorfismo - Interfaces y comportamientos dinámicos");
        Console.WriteLine("✅ Abstracción - Servicios y contratos claros");
        Console.WriteLine();
        Console.WriteLine("🎓 Proyecto académico exitoso!");
        Console.WriteLine("💻 Tecnología: .NET 8 + C#");
        Console.WriteLine("🏗️ Arquitectura: Clean Architecture con capas");
        Console.WriteLine();
        Console.WriteLine("¡Hasta la próxima! ☕");
        Console.WriteLine();
        Console.WriteLine("Presione cualquier tecla para salir...");
        Console.ReadKey();
    }
}