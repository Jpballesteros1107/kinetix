using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kinetix.Models
{
    public class Factura
    {
        public int IdFactura { get; set; }
        public int IdPedido { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; }
        public string Estado { get; set; }
    }
}