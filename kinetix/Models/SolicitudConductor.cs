using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kinetix.Models
{
    public class SolicitudConductor
    {
        public int IdSolicitud { get; set; }

        public string Nombre { get; set; }

        public string Correo { get; set; }

        public string Telefono { get; set; }

        public string Password { get; set; }

        public string Licencia { get; set; }

        public string Vehiculo { get; set; }

        public string Estado { get; set; }
    }
}