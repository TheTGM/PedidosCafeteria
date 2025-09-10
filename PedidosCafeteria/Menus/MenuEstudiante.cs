using PedidosCafeteria.Application;
using PedidosCafeteria.Domain;

namespace PedidosCafeteria.Menus;

public class MenuEstudiante : MenuBase
{
    private readonly ServicioCafeteria _servicioCafeteria;
    private string _estudianteId;

    public MenuEstudiante(ServicioCafeteria servicioCafeteria, string estudianteId)
    {
        _servicioCafeteria = servicioCafeteria ?? throw new ArgumentNullException(nameof(servicioCafeteria));
        _estudianteId = estudianteId ?? throw new ArgumentNullException(nameof(estudianteId));
    }

    public void Mostrar()
    {
        bool continuar = true;

        while (continuar)
        {
            try
            {
                MostrarTitulo($"CAFETERÍA UNI - ESTUDIANTE: {_estudianteId}");

                Console.WriteLine("🛒 GESTIÓN DE PEDIDOS");
                Console.WriteLine("1. Ver Productos Disponibles");
                Console.WriteLine("2. Crear Nuevo Pedido");
                Console.WriteLine("3. Ver Mis Pedidos");
                Console.WriteLine("4. Consultar Estado de Pedido");
                Console.WriteLine();
                Console.WriteLine("0. Volver al Menú Principal");
                Console.WriteLine();

                var opcion = LeerTexto("Seleccione una opción");

                switch (opcion)
                {
                    case "1":
                        VerProductosDisponibles();
                        break;
                    case "2":
                        CrearNuevoPedido();
                        break;
                    case "3":
                        VerMisPedidos();
                        break;
                    case "4":
                        ConsultarEstadoPedido();
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

    private void VerProductosDisponibles()
    {
        MostrarTitulo("PRODUCTOS DISPONIBLES");

        var productos = _servicioCafeteria.ObtenerProductosDisponibles().ToList();

        if (!productos.Any())
        {
            MostrarInfo("No hay productos disponibles en este momento.");
        }
        else
        {
            MostrarListaProductos(productos, "MENÚ DE HOY");
            MostrarEstadisticasRapidas(productos);
        }

        PausarPantalla();
    }

    private void CrearNuevoPedido()
    {
        MostrarTitulo("CREAR NUEVO PEDIDO");

        try
        {
            // Crear el pedido
            var pedidoId = _servicioCafeteria.CrearPedido(_estudianteId);
            MostrarMensaje($"Pedido creado con ID: {pedidoId}");

            bool agregarMasItems = true;
            bool pedidoTieneItems = false;

            while (agregarMasItems)
            {
                Console.WriteLine("\n¿Qué desea agregar al pedido?");

                var productos = _servicioCafeteria.ObtenerProductosDisponibles().ToList();
                if (!productos.Any())
                {
                    MostrarInfo("No hay productos disponibles.");
                    break;
                }

                // Mostrar productos numerados
                Console.WriteLine("\n📋 PRODUCTOS DISPONIBLES:");
                for (int i = 0; i < productos.Count; i++)
                {
                    var p = productos[i];
                    var estado = p.StockDisponible == 0 ? " ❌ SIN STOCK" :
                                p.StockDisponible <= 5 ? $" ⚠️ QUEDAN {p.StockDisponible}" :
                                $" ✅ Stock: {p.StockDisponible}";

                    Console.WriteLine($"{i + 1}. {p.Nombre} - ${p.PrecioFinal:F0}{estado}");
                    if (!string.IsNullOrEmpty(p.Descripcion))
                        Console.WriteLine($"   📝 {p.Descripcion}");
                }

                Console.WriteLine($"{productos.Count + 1}. Ver carrito actual");
                Console.WriteLine("0. Continuar al pago");

                var seleccion = LeerEntero("Seleccione opción");

                if (seleccion == 0)
                {
                    // Continuar al pago
                    break;
                }
                else if (seleccion == productos.Count + 1)
                {
                    // Ver carrito
                    MostrarCarritoActual(pedidoId);
                    continue;
                }
                else if (seleccion >= 1 && seleccion <= productos.Count)
                {
                    var productoSeleccionado = productos[seleccion - 1];

                    if (productoSeleccionado.StockDisponible == 0)
                    {
                        MostrarMensaje("Producto sin stock disponible", true);
                        continue;
                    }

                    var cantidad = LeerEntero($"Cantidad de {productoSeleccionado.Nombre}");

                    if (cantidad <= 0)
                    {
                        MostrarMensaje("La cantidad debe ser mayor a 0", true);
                        continue;
                    }

                    try
                    {
                        _servicioCafeteria.AgregarProductoAPedido(pedidoId, productoSeleccionado.Id, cantidad);
                        pedidoTieneItems = true;

                        var subtotal = cantidad * productoSeleccionado.PrecioFinal;
                        MostrarMensaje($"✅ Agregado: {productoSeleccionado.Nombre} x{cantidad} - ${subtotal:F0}");

                        // Mostrar total actualizado
                        var pedido = _servicioCafeteria.ObtenerPedido(pedidoId);
                        if (pedido != null)
                        {
                            Console.WriteLine($"💰 Total del pedido: ${pedido.Total:F0}");
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

                Console.WriteLine("\n¿Desea agregar otro producto? (s/n)");
                var continuar = Console.ReadLine()?.ToLower();
                agregarMasItems = continuar == "s" || continuar == "si" || continuar == "sí";
            }

            // Procesar pago si el pedido tiene items
            var pedidoFinal = _servicioCafeteria.ObtenerPedido(pedidoId);
            if (pedidoFinal?.Items.Any() == true)
            {
                ProcesarPagoPedido(pedidoId, pedidoFinal.Total);
            }
            else
            {
                _servicioCafeteria.CancelarPedido(pedidoId);
                MostrarInfo("Pedido cancelado por estar vacío.");
            }
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error creando pedido: {ex.Message}", true);
        }

        PausarPantalla();
    }

    private void MostrarCarritoActual(string pedidoId)
    {
        var pedido = _servicioCafeteria.ObtenerPedido(pedidoId);
        if (pedido == null)
        {
            MostrarMensaje("No se pudo obtener el pedido", true);
            return;
        }

        Console.WriteLine("\n🛒 CARRITO ACTUAL:");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

        if (!pedido.Items.Any())
        {
            Console.WriteLine("   (Carrito vacío)");
        }
        else
        {
            foreach (var item in pedido.Items)
            {
                Console.WriteLine($"• {item.Cantidad}x {item.Producto.Nombre}");
                Console.WriteLine($"  ${item.PrecioUnitario:F0} c/u = ${item.Subtotal:F0}");
            }

            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine($"💰 TOTAL: ${pedido.Total:F0}");
        }

        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    }

    private void ProcesarPagoPedido(string pedidoId, decimal total)
    {
        MostrarTitulo("PROCESAR PAGO");

        Console.WriteLine($"💰 Total a pagar: ${total:F0}");
        Console.WriteLine();
        Console.WriteLine("🏦 Métodos de pago disponibles:");
        Console.WriteLine("1. 💵 Efectivo");
        Console.WriteLine("2. 💳 Tarjeta de Crédito/Débito");
        Console.WriteLine("3. 🎫 Bono Estudiantil (10% descuento)");
        Console.WriteLine("0. ❌ Cancelar pedido");

        var opcion = LeerTexto("Seleccione método de pago");

        MetodoPago? metodoPago = null;

        switch (opcion)
        {
            case "1":
                metodoPago = ProcesarPagoEfectivo(total);
                break;

            case "2":
                metodoPago = ProcesarPagoTarjeta();
                break;

            case "3":
                metodoPago = ProcesarPagoBono(total);
                break;

            case "0":
                _servicioCafeteria.CancelarPedido(pedidoId);
                MostrarInfo("Pedido cancelado.");
                return;

            default:
                MostrarMensaje("Opción no válida", true);
                return;
        }

        if (metodoPago != null)
        {
            try
            {
                if (_servicioCafeteria.ProcesarPago(pedidoId, metodoPago))
                {
                    MostrarMensaje("🎉 ¡Pago procesado exitosamente!");
                    Console.WriteLine();
                    Console.WriteLine("📄 RECIBO:");
                    Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                    Console.WriteLine(metodoPago.ObtenerRecibo());
                    Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                    MostrarInfo($"Su pedido {pedidoId} será preparado. ¡Estará listo pronto!");
                    MostrarInfo("Puede consultar el estado en la opción 4 del menú.");
                }
                else
                {
                    MostrarMensaje("❌ Pago rechazado", true);
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error procesando pago: {ex.Message}", true);
            }
        }
    }

    private MetodoPago? ProcesarPagoEfectivo(decimal total)
    {
        Console.WriteLine($"\n💵 PAGO EN EFECTIVO");
        Console.WriteLine($"Total: ${total:F0}");

        var montoRecibido = LeerDecimal("Monto recibido");

        if (montoRecibido < total)
        {
            MostrarMensaje($"Monto insuficiente. Se requieren ${total:F0}", true);
            return null;
        }

        return new PagoEfectivo(montoRecibido);
    }

    private MetodoPago? ProcesarPagoTarjeta()
    {
        Console.WriteLine($"\n💳 PAGO CON TARJETA");

        var numeroTarjeta = LeerTexto("Número de tarjeta (16 dígitos)");
        var titular = LeerTexto("Nombre del titular");

        if (numeroTarjeta.Length != 16 || !numeroTarjeta.All(char.IsDigit))
        {
            MostrarMensaje("Número de tarjeta inválido (debe tener 16 dígitos)", true);
            return null;
        }

        if (string.IsNullOrWhiteSpace(titular))
        {
            MostrarMensaje("Nombre del titular requerido", true);
            return null;
        }

        return new PagoTarjeta(numeroTarjeta, titular);
    }

    private MetodoPago? ProcesarPagoBono(decimal total)
    {
        Console.WriteLine($"\n🎫 PAGO CON BONO ESTUDIANTIL");
        Console.WriteLine($"Total original: ${total:F0}");

        var descuento = total * 0.10m;
        var totalConDescuento = total - descuento;

        Console.WriteLine($"Descuento (10%): -${descuento:F0}");
        Console.WriteLine($"Total a pagar: ${totalConDescuento:F0}");
        Console.WriteLine();

        var codigoBono = LeerTexto("Código del bono estudiantil");

        if (string.IsNullOrWhiteSpace(codigoBono))
        {
            MostrarMensaje("Código de bono requerido", true);
            return null;
        }

        // Simulación de validación
        if (codigoBono.ToUpper().StartsWith("INVALID"))
        {
            MostrarMensaje("Código de bono inválido", true);
            return null;
        }

        return new PagoBono(codigoBono, _estudianteId);
    }

    private void VerMisPedidos()
    {
        MostrarTitulo("MIS PEDIDOS");

        var pedidos = _servicioCafeteria.ObtenerPedidosEstudiante(_estudianteId).ToList();

        if (!pedidos.Any())
        {
            MostrarInfo("No tiene pedidos registrados.");
        }
        else
        {
            Console.WriteLine($"📋 Encontrados {pedidos.Count} pedido(s):");
            Console.WriteLine();

            foreach (var pedido in pedidos)
            {
                MostrarPedido(pedido);
            }
        }

        PausarPantalla();
    }

    private void ConsultarEstadoPedido()
    {
        MostrarTitulo("CONSULTAR ESTADO DE PEDIDO");

        var pedidoId = LeerTexto("Ingrese el ID del pedido");

        if (string.IsNullOrWhiteSpace(pedidoId))
        {
            MostrarMensaje("ID de pedido requerido", true);
            PausarPantalla();
            return;
        }

        var pedido = _servicioCafeteria.ObtenerPedido(pedidoId);

        if (pedido == null || pedido.EstudianteId != _estudianteId)
        {
            MostrarMensaje("Pedido no encontrado o no le pertenece", true);
        }
        else
        {
            MostrarPedido(pedido);

            // Información adicional según el estado
            switch (pedido.Estado)
            {
                case EstadoPedido.Pendiente:
                    MostrarAdvertencia("Su pedido está pendiente de pago.");
                    break;
                case EstadoPedido.PagoProcesado:
                    MostrarInfo("Su pedido está en cola para preparación.");
                    break;
                case EstadoPedido.EnPreparacion:
                    MostrarInfo("👨‍🍳 Su pedido se está preparando. ¡Ya casi está listo!");
                    break;
                case EstadoPedido.ListoParaRecoger:
                    MostrarMensaje("🎉 ¡Su pedido está listo para recoger!");
                    break;
                case EstadoPedido.Completado:
                    MostrarInfo("✅ Pedido completado. ¡Gracias por su compra!");
                    break;
                case EstadoPedido.Cancelado:
                    MostrarAdvertencia("❌ Pedido cancelado.");
                    break;
            }
        }

        PausarPantalla();
    }
}