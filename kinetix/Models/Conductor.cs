using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kinetix.Models
{
    public class Conductor
    {
        public int IdConductor { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Licencia { get; set; }
        public string Telefono { get; set; }
        public string Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}