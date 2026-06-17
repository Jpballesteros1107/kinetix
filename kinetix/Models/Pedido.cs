using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kinetix.Models
{
    public class Pedido
    {
        public int IdPedido { get; set; }
        public int IdUsuario { get; set; }
        public int? IdConductor { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreConductor { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public string Estado { get; set; }
        public decimal Valor { get; set; }
        public DateTime Fecha { get; set; }
    }
}