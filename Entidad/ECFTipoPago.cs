namespace Andloe.Entidad
{
    public class ECFTipoPago
    {
        public int TipoPagoECFId { get; set; }
        public string CodigoDGII { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public bool Activo { get; set; }

        public override string ToString() => $"{CodigoDGII} - {Descripcion}";
    }
}