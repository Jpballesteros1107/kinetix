using kinetix.Models;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace kinetix.Controllers
{
    public class LoginController : Controller
    {
        // =========================
        // VISTA LOGIN
        // =========================
        public ActionResult Index()
        {
            // Si ya inició sesión
            if (Session["Usuario"] != null)
            {
                return RedireccionarPorRol(
                    Session["Rol"].ToString());
            }

            return View();
        }


        // =========================
        // LOGIN
        // =========================
        [HttpPost]
        public ActionResult Index(
            string correo,
            string password)
        {
            if (string.IsNullOrEmpty(correo)
                || string.IsNullOrEmpty(password))
            {
                ViewBag.Error =
                    "Completa todos los campos";

                return View();
            }

            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        @"SELECT
                            IdUsuario,
                            Nombre,
                            Rol,
                            Estado
                        FROM Usuarios
                        WHERE Correo=@c
                        AND Password=@p",
                        cn);

                cmd.Parameters.AddWithValue(
                    "@c",
                    correo);

                cmd.Parameters.AddWithValue(
                    "@p",
                    password);

                SqlDataReader dr =
                    cmd.ExecuteReader();

                if (dr.Read())
                {
                    // VALIDAR ESTADO
                    if (dr["Estado"].ToString()
                        != "Activo")
                    {
                        ViewBag.Error =
                            "Tu cuenta está inactiva";

                        return View();
                    }

                    // SESIONES
                    Session["IdUsuario"] =
                        dr["IdUsuario"].ToString();

                    Session["Usuario"] =
                        dr["Nombre"].ToString();

                    Session["Rol"] =
                        dr["Rol"].ToString();

                    string rol =
                        dr["Rol"].ToString();

                    return RedireccionarPorRol(rol);
                }
            }

            ViewBag.Error =
                "Correo o contraseña incorrectos";

            return View();
        }


        // =========================
        // LOGOUT
        // =========================
        public ActionResult Logout()
        {
            Session.Clear();

            Session.Abandon();

            return RedirectToAction(
                "Index",
                "Home");
        }


        // =========================
        // REDIRECCIÓN POR ROL
        // =========================
        private ActionResult RedireccionarPorRol(
            string rol)
        {
            switch (rol)
            {
                case "Admin":

                    return RedirectToAction(
                        "DashboardAdmin",
                        "Home");

                case "Conductor":

                    return RedirectToAction(
                        "DashboardConductor",
                        "Home");

                default:

                    return RedirectToAction(
                        "DashboardCliente",
                        "Home");
            }
        }
    }
}