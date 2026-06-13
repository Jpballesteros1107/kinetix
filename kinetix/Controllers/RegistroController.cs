using kinetix.Models;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace kinetix.Controllers
{
    public class RegistroController : Controller
    {
        // VIEW
        public ActionResult Index()
        {
            return View();
        }

        // REGISTRAR
        [HttpPost]
        public ActionResult Index(Usuario u)
        {
            SqlConnection cn =
                Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd =
                new SqlCommand(
                    @"INSERT INTO Usuarios
                    (
                        Nombre,
                        Correo,
                        Telefono,
                        Password,
                        Rol,
                        Estado
                    )
                    VALUES
                    (
                        @n,
                        @c,
                        @t,
                        @p,
                        'Cliente',
                        'Activo'
                    )",
                    cn);

            cmd.Parameters.AddWithValue(
                "@n",
                u.Nombre);

            cmd.Parameters.AddWithValue(
                "@c",
                u.Correo);

            cmd.Parameters.AddWithValue(
                "@t",
                u.Telefono);

            cmd.Parameters.AddWithValue(
                "@p",
                u.Password);

            cmd.ExecuteNonQuery();

            cn.Close();

            return RedirectToAction(
                "Index",
                "Login");
        }
    }
}