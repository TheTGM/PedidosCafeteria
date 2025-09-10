namespace PedidosCafeteria.Domain;

public abstract class MetodoPago
{
    public DateTime FechaProcesamiento { get; protected set; }

    // Polimorfismo: cada método implementa su lógica de procesamiento
    public abstract bool Procesar(decimal monto);
    public abstract string ObtenerRecibo();

    // Abstracción: método común con posible override
    protected virtual decimal CalcularDescuento(decimal monto) => 0;

    protected void RegistrarProcesamiento()
    {
        FechaProcesamiento = DateTime.Now;
    }
}

public sealed class PagoEfectivo : MetodoPago
{
    public decimal MontoRecibido { get; }
    public decimal Cambio { get; private set; }

    public PagoEfectivo(decimal montoRecibido)
    {
        if (montoRecibido <= 0) throw new ArgumentException("Monto debe ser positivo");
        MontoRecibido = montoRecibido;
    }

    public override bool Procesar(decimal monto)
    {
        if (MontoRecibido < monto)
            return false;

        Cambio = MontoRecibido - monto;
        RegistrarProcesamiento();
        return true;
    }

    public override string ObtenerRecibo()
    {
        return $"PAGO EN EFECTIVO\nRecibido: ${MontoRecibido:F2}\nCambio: ${Cambio:F2}\nFecha: {FechaProcesamiento}";
    }
}

public sealed class PagoTarjeta : MetodoPago
{
    public string NumeroTarjeta { get; }
    public string Titular { get; }

    public PagoTarjeta(string numeroTarjeta, string titular)
    {
        if (string.IsNullOrWhiteSpace(numeroTarjeta)) throw new ArgumentException("Número de tarjeta requerido");
        if (numeroTarjeta.Length != 16) throw new ArgumentException("Número de tarjeta inválido");
        if (string.IsNullOrWhiteSpace(titular)) throw new ArgumentException("Titular requerido");

        NumeroTarjeta = numeroTarjeta;
        Titular = titular;
    }

    public override bool Procesar(decimal monto)
    {
        // Simulación de validación de tarjeta
        if (NumeroTarjeta.StartsWith("0000"))
            return false; // Tarjeta rechazada

        RegistrarProcesamiento();
        return true;
    }

    public override string ObtenerRecibo()
    {
        var tarjetaOculta = "****-****-****-" + NumeroTarjeta[^4..];
        return $"PAGO CON TARJETA\nTarjeta: {tarjetaOculta}\nTitular: {Titular}\nFecha: {FechaProcesamiento}";
    }
}

public sealed class PagoBono : MetodoPago
{
    public string CodigoBono { get; }
    public string EstudianteId { get; }

    public PagoBono(string codigoBono, string estudianteId)
    {
        if (string.IsNullOrWhiteSpace(codigoBono)) throw new ArgumentException("Código de bono requerido");
        if (string.IsNullOrWhiteSpace(estudianteId)) throw new ArgumentException("ID estudiante requerido");

        CodigoBono = codigoBono;
        EstudianteId = estudianteId;
    }

    // Override: los bonos tienen descuento del 10%
    protected override decimal CalcularDescuento(decimal monto) => monto * 0.10m;

    public override bool Procesar(decimal monto)
    {
        // Simulación de validación de bono
        var descuento = CalcularDescuento(monto);
        var montoFinal = monto - descuento;

        // Validar que el bono existe y tiene saldo
        if (CodigoBono.StartsWith("INVALID"))
            return false;

        RegistrarProcesamiento();
        return true;
    }

    public override string ObtenerRecibo()
    {
        return $"PAGO CON BONO ESTUDIANTIL\nCódigo: {CodigoBono}\nDescuento: 10%\nFecha: {FechaProcesamiento}";
    }
}