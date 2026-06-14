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
            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        @"SELECT *
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
                    Session["Usuario"] =
                        dr["Nombre"].ToString();

                    Session["Rol"] =
                        dr["Rol"].ToString();

                    Session["IdUsuario"] =
                        dr["IdUsuario"].ToString();

                    string rol =
                        dr["Rol"].ToString();

                    // REDIRECCIÓN
                    if (rol == "Admin")
                    {
                        return RedirectToAction(
                            "DashboardAdmin",
                            "Home");
                    }

                    if (rol == "Conductor")
                    {
                        return RedirectToAction(
                            "DashboardConductor",
                            "Home");
                    }

                    return RedirectToAction(
                        "DashboardCliente",
                        "Home");
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

            return RedirectToAction(
                "Index",
                "Home");
        }
    }
}