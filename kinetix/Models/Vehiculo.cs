using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kinetix.Models
{
    public class Vehiculo
    {
        public int IdVehiculo { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public string Marca { get; set; }
        public int IdConductor { get; set; }
        public string NombreConductor { get; set; }
    }
}