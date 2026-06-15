using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kinetix.Models
{
    public class Dashboard
    {
        public int TotalUsuarios { get; set; }
        public int TotalConductores { get; set; }
        public int TotalVehiculos { get; set; }
        public int TotalPedidos { get; set; }
        public int TotalFacturas { get; set; }
        public int ViajesFinalizados { get; set; }
        public int ViajesCancelados { get; set; }
        public decimal Ganancias { get; set; }
        public List<SolicitudConductor> Solicitudes { get; set; }
    }
}