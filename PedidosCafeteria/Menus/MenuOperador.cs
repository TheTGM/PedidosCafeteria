// PedidosCafeteria/Menus/MenuOperador.cs
using PedidosCafeteria.Domain;
using PedidosCafeteria.Application;

namespace PedidosCafeteria.Menus;

public class MenuOperador : MenuBase
{
    private readonly ServicioCafeteria _servicioCafeteria;

    public MenuOperador(ServicioCafeteria servicioCafeteria)
    {
        _servicioCafeteria = servicioCafeteria ?? throw new ArgumentNullException(nameof(servicioCafeteria));
    }

    public void Mostrar()
    {
        bool continuar = true;

        while (continuar)
        {
            try
            {
                MostrarTitulo("CAFETERÍA UNI - OPERADOR");

                Console.WriteLine("🍳 GESTIÓN DE PEDIDOS");
                Console.WriteLine("1. Ver Cola de Pedidos Pagados");
                Console.WriteLine("2. Iniciar Preparación de Pedido");
                Console.WriteLine("3. Marcar Pedido Como Listo");
                Console.WriteLine("4. Completar Pedido (Entregado)");
                Console.WriteLine("5. Ver Pedidos por Estado");

                Console.WriteLine("\n📦 GESTIÓN DE INVENTARIO");
                Console.WriteLine("6. Ver Estado del Inventario");
                Console.WriteLine("7. Actualizar Stock de Producto");
                Console.WriteLine("8. Productos con Stock Bajo");
                Console.WriteLine("9. Reponer Stock");

                Console.WriteLine("\n0. Volver al Menú Principal");
                Console.WriteLine();

                var opcion = LeerTexto("Seleccione una opción");

                switch (opcion)
                {
                    case "1":
                        VerColaPedidosPagados();
                        break;
                    case "2":
                        IniciarPreparacionPedido();
                        break;
                    case "3":
                        MarcarPedidoListo();
                        break;
                    case "4":
                        CompletarPedido();
                        break;
                    case "5":
                        VerPedidosPorEstado();
                        break;
                    case "6":
                        VerEstadoInventario();
                        break;
                    case "7":
                        ActualizarStockProducto();
                        break;
                    case "8":
                        VerProductosStockBajo();
                        break;
                    case "9":
                        ReponerStock();
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

    private void VerColaPedidosPagados()
    {
        MostrarTitulo("COLA DE PEDIDOS PAGADOS");

        var pedidosPagados = _servicioCafeteria.ObtenerPedidosPorEstado(EstadoPedido.PagoProcesado)
            .OrderBy(p => p.FechaPago)
            .ToList();

        if (!pedidosPagados.Any())
        {
            MostrarInfo("✅ No hay pedidos pagados pendientes de preparación.");
        }
        else
        {
            Console.WriteLine($"📋 {pedidosPagados.Count} pedido(s) en cola de preparación:");
            Console.WriteLine();

            for (int i = 0; i < pedidosPagados.Count; i++)
            {
                var pedido = pedidosPagados[i];
                var tiempoEspera = DateTime.Now - pedido.FechaPago!.Value;

                Console.WriteLine($"🎯 POSICIÓN {i + 1} EN COLA:");
                MostrarPedido(pedido);

                if (tiempoEspera.TotalMinutes > 10)
                {
                    MostrarAdvertencia($"⏰ Lleva esperando {tiempoEspera.TotalMinutes:F0} minutos");
                }
                else
                {
                    MostrarInfo($"⏰ Tiempo de espera: {tiempoEspera.TotalMinutes:F0} minutos");
                }
            }
        }

        PausarPantalla();
    }

    private void IniciarPreparacionPedido()
    {
        MostrarTitulo("INICIAR PREPARACIÓN");

        // Mostrar pedidos disponibles para preparar
        var pedidosPagados = _servicioCafeteria.ObtenerPedidosPorEstado(EstadoPedido.PagoProcesado).ToList();

        if (!pedidosPagados.Any())
        {
            MostrarInfo("No hay pedidos pagados pendientes de preparación.");
            PausarPantalla();
            return;
        }

        Console.WriteLine("📋 PEDIDOS DISPONIBLES PARA PREPARAR:");
        Console.WriteLine();

        for (int i = 0; i < pedidosPagados.Count; i++)
        {
            var pedido = pedidosPagados[i];
            Console.WriteLine($"{i + 1}. Pedido {pedido.Id} - Estudiante: {pedido.EstudianteId}");
            Console.WriteLine($"   💰 Total: ${pedido.Total:F0} - Items: {pedido.Items.Count}");

            // Mostrar items que requieren preparación
            var itemsPreparacion = pedido.Items.Where(item =>
                item.Producto is Comida comida && comida.RequierePreparacion).ToList();

            if (itemsPreparacion.Any())
            {
                Console.WriteLine($"   👨‍🍳 Requiere preparación:");
                foreach (var item in itemsPreparacion)
                {
                    var comida = (Comida)item.Producto;
                    Console.WriteLine($"     • {item.Cantidad}x {item.Producto.Nombre} ({comida.TiempoPreparacion.TotalMinutes} min)");
                }
            }
            Console.WriteLine();
        }

        var seleccion = LeerEntero("Número del pedido a preparar (0 para cancelar)");

        if (seleccion == 0) return;

        if (seleccion >= 1 && seleccion <= pedidosPagados.Count)
        {
            var pedidoSeleccionado = pedidosPagados[seleccion - 1];

            try
            {
                _servicioCafeteria.IniciarPreparacionPedido(pedidoSeleccionado.Id);
                MostrarMensaje($"✅ Pedido {pedidoSeleccionado.Id} marcado como 'En Preparación'");

                // Calcular tiempo estimado
                var tiempoEstimado = CalcularTiempoPreparacion(pedidoSeleccionado);
                if (tiempoEstimado > TimeSpan.Zero)
                {
                    MostrarInfo($"⏱ Tiempo estimado de preparación: {tiempoEstimado.TotalMinutes:F0} minutos");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error: {ex.Message}", true);
            }
        }
        else
        {
            MostrarMensaje("Selección inválida", true);
        }

        PausarPantalla();
    }

    private TimeSpan CalcularTiempoPreparacion(Pedido pedido)
    {
        var tiempoTotal = TimeSpan.Zero;

        foreach (var item in pedido.Items)
        {
            if (item.Producto is Comida comida && comida.RequierePreparacion)
            {
                // El tiempo máximo, no suma (preparación en paralelo)
                if (comida.TiempoPreparacion > tiempoTotal)
                    tiempoTotal = comida.TiempoPreparacion;
            }
        }

        return tiempoTotal;
    }

    private void MarcarPedidoListo()
    {
        MostrarTitulo("MARCAR PEDIDO LISTO");

        var pedidosEnPreparacion = _servicioCafeteria.ObtenerPedidosPorEstado(EstadoPedido.EnPreparacion).ToList();

        if (!pedidosEnPreparacion.Any())
        {
            MostrarInfo("No hay pedidos en preparación.");
            PausarPantalla();
            return;
        }

        Console.WriteLine("👨‍🍳 PEDIDOS EN PREPARACIÓN:");
        Console.WriteLine();

        for (int i = 0; i < pedidosEnPreparacion.Count; i++)
        {
            var pedido = pedidosEnPreparacion[i];
            Console.WriteLine($"{i + 1}. Pedido {pedido.Id}");
            Console.WriteLine($"   👤 Estudiante: {pedido.EstudianteId}");
            Console.WriteLine($"   📝 Items: {string.Join(", ", pedido.Items.Select(it => $"{it.Cantidad}x {it.Producto.Nombre}"))}");
            Console.WriteLine();
        }

        var seleccion = LeerEntero("Número del pedido listo (0 para cancelar)");

        if (seleccion == 0) return;

        if (seleccion >= 1 && seleccion <= pedidosEnPreparacion.Count)
        {
            var pedidoSeleccionado = pedidosEnPreparacion[seleccion - 1];

            try
            {
                _servicioCafeteria.MarcarPedidoListo(pedidoSeleccionado.Id);
                MostrarMensaje($"✅ Pedido {pedidoSeleccionado.Id} marcado como 'Listo para Recoger'");
                MostrarInfo($"🔔 Notificar al estudiante {pedidoSeleccionado.EstudianteId} que su pedido está listo");
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error: {ex.Message}", true);
            }
        }
        else
        {
            MostrarMensaje("Selección inválida", true);
        }

        PausarPantalla();
    }

    private void CompletarPedido()
    {
        MostrarTitulo("COMPLETAR PEDIDO (ENTREGADO)");

        var pedidosListos = _servicioCafeteria.ObtenerPedidosPorEstado(EstadoPedido.ListoParaRecoger).ToList();

        if (!pedidosListos.Any())
        {
            MostrarInfo("No hay pedidos listos para entregar.");
            PausarPantalla();
            return;
        }

        Console.WriteLine("✅ PEDIDOS LISTOS PARA ENTREGAR:");
        Console.WriteLine();

        for (int i = 0; i < pedidosListos.Count; i++)
        {
            var pedido = pedidosListos[i];
            Console.WriteLine($"{i + 1}. Pedido {pedido.Id}");
            Console.WriteLine($"   👤 Estudiante: {pedido.EstudianteId}");
            Console.WriteLine($"   💰 Total: ${pedido.Total:F0}");
            Console.WriteLine($"   📝 Items: {string.Join(", ", pedido.Items.Select(it => $"{it.Cantidad}x {it.Producto.Nombre}"))}");
            Console.WriteLine();
        }

        var seleccion = LeerEntero("Número del pedido entregado (0 para cancelar)");

        if (seleccion == 0) return;

        if (seleccion >= 1 && seleccion <= pedidosListos.Count)
        {
            var pedidoSeleccionado = pedidosListos[seleccion - 1];

            // Confirmar entrega
            var confirmar = LeerConfirmacion($"¿Confirma la entrega del pedido {pedidoSeleccionado.Id} al estudiante {pedidoSeleccionado.EstudianteId}?");

            if (confirmar)
            {
                try
                {
                    _servicioCafeteria.CompletarPedido(pedidoSeleccionado.Id);
                    MostrarMensaje($"🎉 Pedido {pedidoSeleccionado.Id} completado exitosamente");
                    MostrarInfo("📊 Venta registrada en el sistema");
                }
                catch (Exception ex)
                {
                    MostrarMensaje($"Error: {ex.Message}", true);
                }
            }
            else
            {
                MostrarInfo("Entrega cancelada");
            }
        }
        else
        {
            MostrarMensaje("Selección inválida", true);
        }

        PausarPantalla();
    }

    private void VerPedidosPorEstado()
    {
        MostrarTitulo("PEDIDOS POR ESTADO");

        Console.WriteLine("Seleccione el estado a consultar:");
        Console.WriteLine("1. ⏳ Pendientes de Pago");
        Console.WriteLine("2. 💳 Pagados (Cola de Preparación)");
        Console.WriteLine("3. 👨‍🍳 En Preparación");
        Console.WriteLine("4. ✅ Listos para Recoger");
        Console.WriteLine("5. 📦 Completados");
        Console.WriteLine("6. ❌ Cancelados");
        Console.WriteLine("7. 📊 Resumen de Todos los Estados");

        var opcion = LeerTexto("Opción");

        EstadoPedido? estadoSeleccionado = opcion switch
        {
            "1" => EstadoPedido.Pendiente,
            "2" => EstadoPedido.PagoProcesado,
            "3" => EstadoPedido.EnPreparacion,
            "4" => EstadoPedido.ListoParaRecoger,
            "5" => EstadoPedido.Completado,
            "6" => EstadoPedido.Cancelado,
            "7" => null, // Resumen
            _ => null
        };

        if (opcion == "7")
        {
            MostrarResumenTodosLosEstados();
        }
        else if (estadoSeleccionado.HasValue)
        {
            MostrarPedidosPorEstadoEspecifico(estadoSeleccionado.Value);
        }
        else
        {
            MostrarMensaje("Opción inválida", true);
        }

        PausarPantalla();
    }

    private void MostrarResumenTodosLosEstados()
    {
        Console.Clear();
        MostrarTitulo("RESUMEN DE TODOS LOS ESTADOS");

        var estadosPedidos = new[]
        {
            EstadoPedido.Pendiente,
            EstadoPedido.PagoProcesado,
            EstadoPedido.EnPreparacion,
            EstadoPedido.ListoParaRecoger,
            EstadoPedido.Completado,
            EstadoPedido.Cancelado
        };

        Console.WriteLine("📊 DASHBOARD DE PEDIDOS:");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

        var totalPedidos = 0;

        foreach (var estado in estadosPedidos)
        {
            var pedidos = _servicioCafeteria.ObtenerPedidosPorEstado(estado).ToList();
            var cantidad = pedidos.Count;
            totalPedidos += cantidad;

            var emoji = estado switch
            {
                EstadoPedido.Pendiente => "⏳",
                EstadoPedido.PagoProcesado => "💳",
                EstadoPedido.EnPreparacion => "👨‍🍳",
                EstadoPedido.ListoParaRecoger => "✅",
                EstadoPedido.Completado => "📦",
                EstadoPedido.Cancelado => "❌",
                _ => "•"
            };

            var color = estado switch
            {
                EstadoPedido.PagoProcesado => ConsoleColor.Yellow,
                EstadoPedido.EnPreparacion => ConsoleColor.Cyan,
                EstadoPedido.ListoParaRecoger => ConsoleColor.Green,
                EstadoPedido.Completado => ConsoleColor.Blue,
                EstadoPedido.Cancelado => ConsoleColor.Red,
                _ => ConsoleColor.White
            };

            Console.ForegroundColor = color;
            Console.WriteLine($"{emoji} {estado}: {cantidad} pedido(s)");
            Console.ResetColor();

            if (cantidad > 0 && (estado == EstadoPedido.PagoProcesado || estado == EstadoPedido.EnPreparacion))
            {
                Console.WriteLine($"   Valor total: ${pedidos.Sum(p => p.Total):F0}");
            }
        }

        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine($"📋 TOTAL DE PEDIDOS: {totalPedidos}");

        // Alertas importantes
        var pedidosEspera = _servicioCafeteria.ObtenerPedidosPorEstado(EstadoPedido.PagoProcesado).Count();
        var pedidosPreparacion = _servicioCafeteria.ObtenerPedidosPorEstado(EstadoPedido.EnPreparacion).Count();
        var pedidosListos = _servicioCafeteria.ObtenerPedidosPorEstado(EstadoPedido.ListoParaRecoger).Count();

        if (pedidosEspera > 0)
        {
            MostrarAdvertencia($"⚡ {pedidosEspera} pedido(s) esperando preparación");
        }

        if (pedidosListos > 0)
        {
            MostrarMensaje($"🔔 {pedidosListos} pedido(s) listo(s) para entregar");
        }
    }

    private void MostrarPedidosPorEstadoEspecifico(EstadoPedido estado)
    {
        var pedidos = _servicioCafeteria.ObtenerPedidosPorEstado(estado).ToList();

        Console.Clear();
        MostrarTitulo($"PEDIDOS - {estado.ToString().ToUpper()}");

        if (!pedidos.Any())
        {
            MostrarInfo($"No hay pedidos en estado '{estado}'.");
        }
        else
        {
            Console.WriteLine($"📋 {pedidos.Count} pedido(s) encontrado(s):");
            Console.WriteLine();

            foreach (var pedido in pedidos)
            {
                MostrarPedido(pedido);
            }

            // Estadísticas adicionales
            if (pedidos.Any())
            {
                var valorTotal = pedidos.Sum(p => p.Total);
                Console.WriteLine($"💰 Valor total: ${valorTotal:F0}");
                Console.WriteLine($"📊 Promedio por pedido: ${valorTotal / pedidos.Count:F0}");
            }
        }
    }

    private void VerEstadoInventario()
    {
        MostrarTitulo("ESTADO DEL INVENTARIO");

        var productos = _servicioCafeteria.ObtenerProductosDisponibles().ToList();

        if (!productos.Any())
        {
            MostrarInfo("No hay productos en el inventario.");
        }
        else
        {
            // Estadísticas generales
            var totalProductos = productos.Count;
            var sinStock = productos.Count(p => p.Stock == 0);
            var stockBajo = productos.Count(p => p.Stock > 0 && p.Stock <= 5);
            var valorInventario = productos.Sum(p => p.Stock * p.PrecioBase);

            Console.WriteLine("📊 RESUMEN DEL INVENTARIO:");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine($"📦 Total de productos: {totalProductos}");
            Console.WriteLine($"🔴 Sin stock: {sinStock}");
            Console.WriteLine($"🟡 Stock bajo (≤5): {stockBajo}");
            Console.WriteLine($"💰 Valor del inventario: ${valorInventario:F0}");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine();

            // Mostrar productos por categoría
            MostrarListaProductos(productos, "INVENTARIO COMPLETO");

            // Alertas
            if (sinStock > 0)
            {
                MostrarAdvertencia($"⚠️ {sinStock} producto(s) sin stock necesita(n) reposición urgente");
            }

            if (stockBajo > 0)
            {
                MostrarAdvertencia($"⚠️ {stockBajo} producto(s) con stock bajo");
            }
        }

        PausarPantalla();
    }

    private void ActualizarStockProducto()
    {
        MostrarTitulo("ACTUALIZAR STOCK");

        var productos = _servicioCafeteria.ObtenerProductosDisponibles().ToList();

        if (!productos.Any())
        {
            MostrarInfo("No hay productos disponibles.");
            PausarPantalla();
            return;
        }

        Console.WriteLine("📦 PRODUCTOS DISPONIBLES:");
        Console.WriteLine();

        for (int i = 0; i < productos.Count; i++)
        {
            var p = productos[i];
            var estado = p.Stock == 0 ? " 🔴" : p.Stock <= 5 ? " 🟡" : " 🟢";
            Console.WriteLine($"{i + 1}. [{p.Id}] {p.Nombre}{estado}");
            Console.WriteLine($"   Stock actual: {p.Stock} | Disponible: {p.StockDisponible}");
        }

        var seleccion = LeerEntero("Número del producto (0 para cancelar)");

        if (seleccion == 0) return;

        if (seleccion >= 1 && seleccion <= productos.Count)
        {
            var producto = productos[seleccion - 1];
            Console.WriteLine($"\n📝 Producto seleccionado: {producto.Nombre}");
            Console.WriteLine($"   Stock actual: {producto.Stock}");
            Console.WriteLine($"   Stock disponible: {producto.StockDisponible}");

            var nuevoStock = LeerEntero("Nuevo stock total");

            try
            {
                _servicioCafeteria.ActualizarStockProducto(producto.Id, nuevoStock);
                MostrarMensaje($"✅ Stock actualizado para {producto.Nombre}: {producto.Stock} → {nuevoStock}");
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error: {ex.Message}", true);
            }
        }
        else
        {
            MostrarMensaje("Selección inválida", true);
        }

        PausarPantalla();
    }

    private void VerProductosStockBajo()
    {
        MostrarTitulo("PRODUCTOS CON STOCK BAJO");

        var limite = LeerEntero("Límite de stock bajo (por defecto 5)");
        if (limite <= 0) limite = 5;

        var productos = _servicioCafeteria.ObtenerProductosDisponibles()
            .Where(p => p.Stock <= limite)
            .OrderBy(p => p.Stock)
            .ToList();

        Console.WriteLine();
        if (!productos.Any())
        {
            MostrarInfo($"✅ No hay productos con stock menor o igual a {limite}.");
        }
        else
        {
            Console.WriteLine($"⚠️ PRODUCTOS CON STOCK BAJO (≤ {limite}):");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            foreach (var producto in productos)
            {
                var estado = producto.Stock == 0 ? "🔴 SIN STOCK" : "🟡 STOCK BAJO";
                var prioridad = producto.Stock == 0 ? "URGENTE" : "ATENCIÓN";

                Console.WriteLine($"{estado} - [{producto.Id}] {producto.Nombre}");
                Console.WriteLine($"   Stock: {producto.Stock} unidades - Prioridad: {prioridad}");
                Console.WriteLine($"   Precio: ${producto.PrecioFinal:F0}");
                Console.WriteLine();
            }

            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine($"📊 Total de productos con stock bajo: {productos.Count}");

            var sinStock = productos.Count(p => p.Stock == 0);
            if (sinStock > 0)
            {
                MostrarAdvertencia($"🚨 {sinStock} producto(s) sin stock requieren reposición URGENTE");
            }
        }

        PausarPantalla();
    }

    private void ReponerStock()
    {
        MostrarTitulo("REPONER STOCK");

        // Mostrar productos con stock bajo para facilitar selección
        var productosStockBajo = _servicioCafeteria.ObtenerProductosDisponibles()
            .Where(p => p.Stock <= 10)
            .OrderBy(p => p.Stock)
            .ToList();

        if (productosStockBajo.Any())
        {
            Console.WriteLine("⚠️ PRODUCTOS CON STOCK BAJO (≤10):");
            foreach (var p in productosStockBajo)
            {
                var estado = p.Stock == 0 ? "🔴" : p.Stock <= 5 ? "🟡" : "🟠";
                Console.WriteLine($"   {estado} [{p.Id}] {p.Nombre} - Stock: {p.Stock}");
            }
            Console.WriteLine();
        }

        var productoId = LeerTexto("ID del producto a reponer");

        try
        {
            var producto = _servicioCafeteria.ObtenerProductosDisponibles()
                .FirstOrDefault(p => p.Id.Equals(productoId, StringComparison.OrdinalIgnoreCase));

            if (producto == null)
            {
                MostrarMensaje("Producto no encontrado", true);
                PausarPantalla();
                return;
            }

            Console.WriteLine($"\n📝 Producto: {producto.Nombre}");
            Console.WriteLine($"   Stock actual: {producto.Stock}");
            Console.WriteLine($"   Stock disponible: {producto.StockDisponible}");

            var cantidadAgregar = LeerEntero("Cantidad a agregar al stock");
            var nuevoStock = producto.Stock + cantidadAgregar;

            var confirmar = LeerConfirmacion($"¿Confirma reponer {cantidadAgregar} unidades? (Stock final: {nuevoStock})");

            if (confirmar)
            {
                _servicioCafeteria.ActualizarStockProducto(producto.Id, nuevoStock);
                MostrarMensaje($"✅ Stock repuesto exitosamente");
                Console.WriteLine($"   {producto.Nombre}: {producto.Stock} → {nuevoStock} (+{cantidadAgregar})");
            }
            else
            {
                MostrarInfo("Reposición cancelada");
            }
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error: {ex.Message}", true);
        }

        PausarPantalla();
    }
}