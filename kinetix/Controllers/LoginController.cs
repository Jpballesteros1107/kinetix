using kinetix.Models;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Web.Security;

namespace kinetix.Controllers
{
    public class LoginController : Controller
    {
        // LOGIN VIEW
        public ActionResult Index()
        {
            return View();
        }

        // LOGIN
        [HttpPost]
        public ActionResult Index(string correo,
                                  string password)
        {
            SqlConnection cn =
                Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd =
                new SqlCommand(
                    @"SELECT * FROM Usuarios
                    WHERE Correo=@c
                    AND Password=@p",
                    cn);

            cmd.Parameters.AddWithValue("@c", correo);

            cmd.Parameters.AddWithValue("@p", password);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                Session["Usuario"] =
                    dr["Nombre"].ToString();

                Session["Rol"] =
                    dr["Rol"].ToString();

                return RedirectToAction(
                    "Index",
                    "Home");
            }

            ViewBag.Error =
                "Correo o contraseña incorrectos";

            cn.Close();

            return View();
        }

        // LOGOUT
        public ActionResult Logout()
        {
            Session.Clear();

            FormsAuthentication.SignOut();

            return RedirectToAction(
                "Index");
        }
    }
}