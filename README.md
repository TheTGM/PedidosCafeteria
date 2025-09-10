# PedidosCafeteria
[Ver documento PDF](PedidosCafeteria/docs/SISTEMA-DE-CAFETERÍA.pdf)
# 🏫☕ Sistema de Cafetería

## Descripción

Sistema de gestión de pedidos para la cafetería de una universidad, desarrollado en **.NET 8** aplicando los **4 pilares de la Programación Orientada a Objetos (POO)**. Permite a estudiantes realizar pedidos digitales, a operadores gestionar la preparación y a administradores generar reportes de ventas.

## 🚀 Tecnologías Utilizadas

- **.NET 8** - Framework principal
- **C#** - Lenguaje de programación
- **Consola** - Interfaz de usuario
- **Memoria** - Persistencia base
- **UTF-8** - Codificación para emojis

## 📋 Requisitos del Sistema

### Software Requerido
- **.NET 8 SDK** o superior
- **Visual Studio 2022** (recomendado) o **Visual Studio Code**
- **Terminal** con soporte UTF-8 (Windows Terminal, PowerShell, Command Prompt)

### Verificar Instalación
```bash
dotnet --version
# Debe mostrar 8.0.x o superior
```

## 📦 Instalación y Configuración

### 1. Clonar o Descargar el Proyecto
```bash
# Si tienes Git
git clone https://github.com/usuario/PedidosCafeteria.git
cd PedidosCafeteria

# O descargar ZIP y extraer
```

### 2. Restaurar Dependencias
```bash
dotnet restore
```

### 3. Compilar el Proyecto
```bash
dotnet build
```

### 4. Verificar Compilación
Si la compilación es exitosa, verás:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

## ▶️ Cómo Ejecutar

### Opción 1: Desde la Terminal
```bash
# Ejecutar directamente
dotnet run --project PedidosCafeteria

# O navegar al directorio y ejecutar
cd PedidosCafeteria
dotnet run
```

### Opción 2: Desde Visual Studio
1. Abrir `PedidosCafeteria.sln`
2. Establecer `PedidosCafeteria` como proyecto de inicio
3. Presionar `F5` (con depuración) o `Ctrl+F5` (sin depuración)

### Opción 3: Ejecutable Compilado
```bash
# Compilar ejecutable
dotnet publish -c Release -o ./publish

# Ejecutar
./publish/PedidosCafeteria.exe  # Windows
./publish/PedidosCafeteria      # Linux/Mac
```

## 👥 Usuarios y Credenciales

### 🔑 Accesos del Sistema

| Tipo de Usuario | Credenciales | Descripción |
|-----------------|--------------|-------------|
| **👨‍🎓 Estudiante** | ID: `EST001`, `EST002`, etc. | Sin contraseña, solo requiere ID |
| **👩‍🍳 Operador** | Clave: `operador123` | Gestión de pedidos y inventario |
| **👨‍💼 Administrador** | Clave: `admin123` | Reportes y gestión completa |

## 🧪 Cómo Probar el Sistema

### Flujo de Prueba Completo (15 minutos)

#### 1. **Prueba como Estudiante (5 min)**
```
1. Ejecutar aplicación
2. Opción 1 → Estudiante
3. ID: EST001
4. Opción 1 → Ver productos disponibles
5. Opción 2 → Crear nuevo pedido
   - Agregar: Café Americano x2
   - Agregar: Empanada x1
6. Pagar con Bono: BONO123 (10% descuento)
7. Anotar ID del pedido generado
8. Opción 4 → Consultar estado del pedido
```

#### 2. **Prueba como Operador (5 min)**
```
1. Volver al menú principal (Opción 0)
2. Opción 2 → Operador
3. Clave: operador123
4. Opción 1 → Ver cola de pedidos pagados
5. Opción 2 → Iniciar preparación (usar ID del paso anterior)
6. Opción 3 → Marcar pedido como listo
7. Opción 4 → Completar pedido
8. Opción 6 → Ver estado del inventario
```

#### 3. **Prueba como Administrador (5 min)**
```
1. Volver al menú principal
2. Opción 3 → Administrador
3. Clave: admin123
4. Opción 1 → Ver todos los productos
5. Opción 5 → Reporte de ventas del día
6. Opción 6 → Productos más vendidos
7. Opción 8 → Reporte de stock bajo
```

### Casos de Prueba Específicos

#### ✅ **Caso 1: Pedido Exitoso**
```
Datos:
- Estudiante: EST001
- Productos: Café Americano x2, Sandwich Pollo x1
- Pago: Bono BONO123

Resultado Esperado:
- Pedido creado con ID único
- Descuento 10% aplicado
- Stock actualizado automáticamente
```

#### ❌ **Caso 2: Error de Stock**
```
Datos:
- Producto: Empanada (stock inicial: 40)
- Cantidad: 50

Resultado Esperado:
- Error: "Stock insuficiente"
- No se modifica el inventario
```

#### 💳 **Caso 3: Métodos de Pago**
```
Efectivo:
- Monto pedido: $10,000
- Monto recibido: $15,000
- Resultado: Cambio $5,000

Tarjeta Válida:
- Número: 1234567890123456
- Resultado: Pago aprobado

Tarjeta Inválida:
- Número: 123456
- Resultado: Error "Número inválido"
```

## 🏗️ Decisiones de Diseño

### Arquitectura General

#### **Estructura en Capas**
```
📁 PedidosCafeteria/
├── 📁 Domain/           # Entidades de negocio
├── 📁 Application/      # Servicios y lógica
├── 📁 Infrastructure/   # Repositorios y datos
├── 📁 Menus/           # Interfaces de usuario
└── 📄 Program.cs       # Punto de entrada
```

**Justificación:** Separación clara de responsabilidades siguiendo Clean Architecture, facilitando mantenimiento y testing.

#### **Patrón Repository**
- **IRepositorioProductos**, **IRepositorioPedidos**
- **Implementación en memoria** para simplicidad
- **Fácil extensión** a base de datos o JSON

**Justificación:** Abstrae la persistencia, permitiendo cambiar implementación sin afectar lógica de negocio.

### Aplicación de POO

#### **🔒 Encapsulamiento**
```csharp
public class Producto
{
    private int _stock;              // Estado protegido
    private int _stockReservado;     // No accesible directamente
    
    public int StockDisponible => _stock - _stockReservado;  // Solo lectura
    
    public void ReservarStock(int cantidad)  // Método controlado
    {
        if (cantidad > StockDisponible) 
            throw new InvalidOperationException("Stock insuficiente");
        _stockReservado += cantidad;
    }
}
```

#### **🏗️ Herencia**
```csharp
public abstract class Producto { /* Comportamiento común */ }
public sealed class Bebida : Producto { /* Específico de bebidas */ }
public sealed class Comida : Producto { /* Específico de comidas */ }

public abstract class MetodoPago { /* Contrato común */ }
public sealed class PagoEfectivo : MetodoPago { /* Lógica específica */ }
```

#### **🎭 Polimorfismo**
```csharp
// Mismo método, comportamiento diferente según implementación
MetodoPago pago = new PagoBono("BONO123");
bool resultado = pago.Procesar(total);  // Aplica descuento automáticamente

MetodoPago pago2 = new PagoEfectivo(15000);
bool resultado2 = pago2.Procesar(total);  // Valida monto recibido
```

#### **🎯 Abstracción**
```csharp
public interface IServicioCafeteria
{
    string CrearPedido(string estudianteId);           // Operación simple
    bool ProcesarPago(string pedidoId, MetodoPago pago); // Oculta complejidad
}
```

### Decisiones Técnicas

#### **Validaciones**
- **Entrada del usuario:** Números positivos, strings no vacíos
- **Reglas de negocio:** Stock no negativo, estados válidos
- **Excepciones específicas:** Mensajes claros para cada error

#### **Interfaz de Usuario**
- **Emojis contextuales:** Mejor experiencia visual
- **Colores diferenciados:** Verde (éxito), Rojo (error), Amarillo (advertencia)
- **Menús numerados:** Navegación intuitiva
- **Mensajes claros:** Información suficiente sin saturar

#### **Datos Iniciales**
- **21 productos precargados:** Variedad realista para pruebas
- **Stock diverso:** Algunos con stock bajo para probar alertas
- **Precios variados:** Rango de $2,000 a $12,500

## 📊 Funcionalidades Implementadas

### 👨‍🎓 **Módulo Estudiante**
- ✅ Visualización de productos con stock
- ✅ Creación de pedidos múltiples productos
- ✅ Carrito interactivo con totales
- ✅ Múltiples métodos de pago
- ✅ Seguimiento de estados en tiempo real
- ✅ Historial personal de pedidos

### 👩‍🍳 **Módulo Operador**
- ✅ Cola de preparación ordenada
- ✅ Gestión de estados de pedidos
- ✅ Control de inventario en tiempo real
- ✅ Alertas de stock bajo
- ✅ Reposición de productos
- ✅ Dashboard de operaciones

### 👨‍💼 **Módulo Administrador**
- ✅ CRUD completo de productos
- ✅ Reportes de ventas diarias
- ✅ Ranking de productos más vendidos
- ✅ Análisis de métodos de pago
- ✅ Control de precios
- ✅ Estadísticas del sistema

## 📈 Datos de Prueba Incluidos

### Productos (21 total)
```
Bebidas (9):
├── Calientes: Café Americano, Cappuccino, Chocolate, Té Verde
└── Frías: Jugo Natural, Gaseosa, Agua, Café Frío

Comidas (12):
├── Sandwiches: Pollo, Jamón
├── Ensaladas: César, Frutas  
├── Snacks: Empanada, Croissant, Muffin, Galletas
├── Postres: Brownie, Torta del Día
└── Platos: Almuerzo Ejecutivo
```

### Métodos de Pago
```
💵 Efectivo: Cualquier monto ≥ total
💳 Tarjeta: 16 dígitos (ej: 1234567890123456)
🎫 Bono: BONO123, BONO456, BONO789 (10% descuento)
```

## 📝 Notas de Desarrollo

### Limitaciones Conocidas
- **Persistencia en memoria:** Los datos se pierden al cerrar
- **Sin concurrencia:** Diseñado para un usuario a la vez
- **Autenticación simple:** Credenciales de demostración
- **Interfaz de consola:** Sin GUI gráfica

## 🎓 Valor Académico

Este proyecto demuestra:
- **POO aplicada:** Los 4 pilares implementados correctamente
- **Clean Architecture:** Separación de responsabilidades
- **Principios SOLID:** Código mantenible y extensible
- **Patrones de Diseño:** Repository, Strategy, Template Method
- **Manejo de errores:** Excepciones específicas y validaciones
- **Experiencia de usuario:** Interfaz intuitiva y funcional

## 🖼️ Imagenes del sistema
<img width="1108" height="619" alt="image" src="https://github.com/user-attachments/assets/1e8dfbbd-98d7-44e1-ba07-b42f04a97a3c" />
<img width="1113" height="623" alt="image" src="https://github.com/user-attachments/assets/c1468872-5827-49cb-a16d-b8d1ae681640" />
<img width="1110" height="618" alt="image" src="https://github.com/user-attachments/assets/4a6067b0-df4f-45c2-94a7-e037a560e609" />
<img width="1108" height="624" alt="image" src="https://github.com/user-attachments/assets/43aae3d6-5d09-4688-854b-6b5f1dc97027" />
<img width="1104" height="617" alt="image" src="https://github.com/user-attachments/assets/bba35cbd-4deb-445f-b712-f609992a3780" />
<img width="1110" height="624" alt="image" src="https://github.com/user-attachments/assets/02ec12e7-6d14-4a5a-97dc-8ad9f367f42f" />



## 👨‍💻 Desarrollo

**Autor:** [Mateo Bolivar Arroyave]  
**Universidad:** [ITM]  
