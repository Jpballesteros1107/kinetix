using System.Data.SqlClient;
using System.Web.Mvc;
using kinetix.Models;

namespace kinetix.Controllers
{
    public class ConductorRegistroController : Controller
    {
        // =========================
        // VISTA
        // =========================
        public ActionResult Index()
        {
            return View();
        }


        // =========================
        // REGISTRO CONDUCTOR
        // =========================
        [HttpPost]
        public ActionResult Index(SolicitudConductor s)
        {
            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                new SqlCommand(
                    @"INSERT INTO SolicitudesConductores
                    (
                        Nombre,
                        Correo,
                        Telefono,
                        Licencia,
                        Vehiculo,
                        Estado
                    )
                    VALUES
                    (
                        @nom,
                        @cor,
                        @tel,
                        @lic,
                        @veh,
                        'Pendiente'
                    )",
                    cn);

                cmd.Parameters.AddWithValue("@nom", s.Nombre);
                cmd.Parameters.AddWithValue("@cor", s.Correo);
                cmd.Parameters.AddWithValue("@tel", s.Telefono);
                cmd.Parameters.AddWithValue("@lic", s.Licencia);
                cmd.Parameters.AddWithValue("@veh", s.Vehiculo);

                cmd.ExecuteNonQuery();

                ViewBag.Mensaje =
                "Solicitud enviada correctamente";

                return View();
            }
        }
    }
}