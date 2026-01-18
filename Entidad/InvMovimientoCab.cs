namespace Andloe.Entidad
{
    public class InvMovimientoCab
    {
        public long InvMovId { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } = "";       // 'ENTRADA','SALIDA','AJUSTE','TRANSFERENCIA', etc.
        public string? Origen { get; set; }          // 'VENTA','AJUSTE','COMPRA', etc.
        public long? OrigenId { get; set; }
        public int? AlmacenIdOrigen { get; set; }
        public int? AlmacenIdDestino { get; set; }
        public string Usuario { get; set; } = "";
        public string? Observacion { get; set; }
        public string Estado { get; set; } = "";     // 'APLICADO','PENDIENTE', etc.
    }

    public class InvMovimientoLin
    {
        public long InvMovId { get; set; }
        public int Linea { get; set; }
        public string ProductoCodigo { get; set; } = "";
        public decimal Cantidad { get; set; }        // 18,4
        public decimal CostoUnitario { get; set; }   // 18,6

        public string? Usuario { get; set; }

        public DateTime? FechaCreacion { get; set; }
        public string? UsuarioCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string? UsuarioModificacion { get; set; }
    }
}
