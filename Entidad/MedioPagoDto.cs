using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andloe.Entidad
{
    public class MedioPagoDto
    {
        public int MedioPagoId { get; set; }
        public string Nombre { get; set; } = "";
    }

    public class PosFormaPagoUiDto
    {
        public string Clave { get; set; } = "";
        public string FormaPagoCodigo { get; set; } = "";
        public string Nombre { get; set; } = "";
    }


}

