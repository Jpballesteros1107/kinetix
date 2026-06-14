using System;
using System.Data.SqlClient;
using System.Web.Mvc;
using kinetix.Models;

namespace kinetix.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Dashboard d =
                new Dashboard();

            if (Session["Usuario"] != null)
            {
                string rol =
                    Session["Rol"].ToString();

                if (rol == "Admin")
                {
                    return RedirectToAction(
                        "DashboardAdmin");
                }

                if (rol == "Cliente")
                {
                    return RedirectToAction(
                        "DashboardCliente");
                }

                if (rol == "Conductor")
                {
                    return RedirectToAction(
                        "DashboardConductor");
                }
            }

            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                // TOTALES

                d.TotalUsuarios =
                    EjecutarConteo(
                        "SELECT COUNT(*) FROM Usuarios",
                        cn);

                d.TotalConductores =
                    EjecutarConteo(
                        "SELECT COUNT(*) FROM Conductores",
                        cn);

                d.TotalVehiculos =
                    EjecutarConteo(
                        "SELECT COUNT(*) FROM Vehiculos",
                        cn);

                d.TotalPedidos =
                    EjecutarConteo(
                        "SELECT COUNT(*) FROM Pedidos",
                        cn);

                d.TotalFacturas =
                    EjecutarConteo(
                        "SELECT COUNT(*) FROM Facturas",
                        cn);

                // =========================
                // VIAJES FINALIZADOS
                // =========================

                d.ViajesFinalizados =
                    EjecutarConteo(
                        @"SELECT COUNT(*)
                        FROM Pedidos
                        WHERE Estado='Finalizado'",
                        cn);

                // =========================
                // VIAJES CANCELADOS
                // =========================

                d.ViajesCancelados =
                    EjecutarConteo(
                        @"SELECT COUNT(*)
                        FROM Pedidos
                        WHERE Estado='Cancelado'",
                        cn);

                // =========================
                // GANANCIAS
                // =========================

                SqlCommand ganancias =
                    new SqlCommand(
                        @"SELECT ISNULL(SUM(Total),0)
                        FROM Facturas",
                        cn);

                d.Ganancias =
                    Convert.ToDecimal(
                        ganancias.ExecuteScalar());
            }

            return View(d);
        }


        // =========================
        // METODO AUXILIAR
        // =========================
        private int EjecutarConteo(
            string consulta,
            SqlConnection cn)
        {
            SqlCommand cmd =
                new SqlCommand(
                    consulta,
                    cn);

            return Convert.ToInt32(
                cmd.ExecuteScalar());
        }


        public ActionResult About()
        {
            ViewBag.Message =
                "Acerca de Kinetix";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message =
                "Contacto Kinetix";

            return View();
        }

        public ActionResult DashboardCliente()
        {
            return View();
        }

        public ActionResult DashboardConductor()
        {
            return View();
        }

        public ActionResult DashboardAdmin()
        {
            if (Session["Usuario"] == null)
            {
                return RedirectToAction(
                    "Index",
                    "Login");
            }

            if (Session["Rol"].ToString() != "Admin")
            {
                return RedirectToAction(
                    "Index");
            }

            Dashboard d =
                new Dashboard();

            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                // =========================
                // TOTALES
                // =========================

                d.TotalUsuarios =
                    EjecutarConteo(
                        "SELECT COUNT(*) FROM Usuarios",
                        cn);

                d.TotalConductores =
                    EjecutarConteo(
                        "SELECT COUNT(*) FROM Conductores",
                        cn);

                d.TotalVehiculos =
                    EjecutarConteo(
                        "SELECT COUNT(*) FROM Vehiculos",
                        cn);

                d.TotalPedidos =
                    EjecutarConteo(
                        "SELECT COUNT(*) FROM Pedidos",
                        cn);

                d.TotalFacturas =
                    EjecutarConteo(
                        "SELECT COUNT(*) FROM Facturas",
                        cn);

                // =========================
                // VIAJES FINALIZADOS
                // =========================

                d.ViajesFinalizados =
                    EjecutarConteo(
                        @"SELECT COUNT(*)
                FROM Pedidos
                WHERE Estado='Finalizado'",
                        cn);

                // =========================
                // VIAJES CANCELADOS
                // =========================

                d.ViajesCancelados =
                    EjecutarConteo(
                        @"SELECT COUNT(*)
                FROM Pedidos
                WHERE Estado='Cancelado'",
                        cn);

                // =========================
                // GANANCIAS
                // =========================

                SqlCommand ganancias =
                    new SqlCommand(
                        @"SELECT ISNULL(SUM(Total),0)
                FROM Facturas",
                        cn);

                d.Ganancias =
                    Convert.ToDecimal(
                        ganancias.ExecuteScalar());
            }

            return View(d);
        }
    }

}