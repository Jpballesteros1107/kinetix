using kinetix.Models;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace kinetix.Controllers
{
    public class RegistroController : Controller
    {
        // =========================
        // VISTA
        // =========================
        public ActionResult Index()
        {
            return View();
        }


        // =========================
        // REGISTRAR
        // =========================
        [HttpPost]
        public ActionResult Index(Usuario u)
        {
            // VALIDAR CAMPOS
            if (
                string.IsNullOrEmpty(u.Nombre) ||
                string.IsNullOrEmpty(u.Correo) ||
                string.IsNullOrEmpty(u.Telefono) ||
                string.IsNullOrEmpty(u.Password)
               )
            {
                ViewBag.Error =
                    "Todos los campos son obligatorios";

                return View();
            }

            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                // =========================
                // VALIDAR CORREO
                // =========================

                SqlCommand validar =
                    new SqlCommand(
                        @"SELECT COUNT(*)
                        FROM Usuarios
                        WHERE Correo=@c",
                        cn);

                validar.Parameters.AddWithValue(
                    "@c",
                    u.Correo);

                int existe =
                    int.Parse(
                        validar.ExecuteScalar().ToString());

                if (existe > 0)
                {
                    ViewBag.Error =
                        "El correo ya está registrado";

                    return View();
                }

                // =========================
                // INSERTAR USUARIO
                // =========================

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
            }

            TempData["Success"] =
                "Registro exitoso";

            return RedirectToAction(
                "Index",
                "Login");
        }
    }
}