using kinetix.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace kinetix.Controllers
{
    public class HomeController : Controller
    {
        // =========================
        // LANDING PAGE
        // =========================
        public ActionResult Index()
        {
            if (Session["Usuario"] != null)
            {
                string rol =
                    Session["Rol"].ToString();

                switch (rol)
                {
                    case "Admin":
                        return RedirectToAction(
                            "DashboardAdmin");

                    case "Cliente":
                        return RedirectToAction(
                            "DashboardCliente");

                    case "Conductor":
                        return RedirectToAction(
                            "DashboardConductor");
                }
            }

            Dashboard d =
                ObtenerDashboard();

            return View(d);
        }

        //DASHBOARD ADMIN
        public ActionResult DashboardAdmin()
        {
            if (!ValidarRol("Admin"))
            {
                return RedirectToAction(
                    "Index",
                    "Login");
            }

            Dashboard d =
                ObtenerDashboard();

            d.Solicitudes =
                new List<SolicitudConductor>();

            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmdSolicitudes =
                    new SqlCommand(
                        @"SELECT *
                  FROM SolicitudesConductores
                  WHERE Estado='Pendiente'",
                        cn);

                SqlDataReader dr =
                    cmdSolicitudes.ExecuteReader();

                while (dr.Read())
                {
                    SolicitudConductor s =
                        new SolicitudConductor();

                    s.IdSolicitud =
                        Convert.ToInt32(
                            dr["IdSolicitud"]);

                    s.Nombre =
                        dr["Nombre"].ToString();

                    s.Correo =
                        dr["Correo"].ToString();

                    s.Telefono =
                        dr["Telefono"].ToString();

                    s.Licencia =
                        dr["Licencia"].ToString();

                    s.Vehiculo =
                        dr["Vehiculo"].ToString();

                    s.Estado =
                        dr["Estado"].ToString();

                    d.Solicitudes.Add(s);
                }

                dr.Close();
            }

            return View(d);
        }


        // =========================
        // DASHBOARD CLIENTE
        // =========================
        public ActionResult DashboardCliente()
        {
            if (!ValidarRol("Cliente"))
            {
                return RedirectToAction(
                    "Index",
                    "Login");
            }

            return View();
        }


        // =========================
        // DASHBOARD CONDUCTOR
        // =========================
        public ActionResult DashboardConductor()
        {
            if (!ValidarRol("Conductor"))
            {
                return RedirectToAction(
                    "Index",
                    "Login");
            }

            return View();
        }


        // =========================
        // ABOUT
        // =========================
        public ActionResult About()
        {
            ViewBag.Message =
                "Acerca de Kinetix";

            return View();
        }


        // =========================
        // CONTACT
        // =========================
        public ActionResult Contact()
        {
            ViewBag.Message =
                "Contacto Kinetix";

            return View();
        }


        // =========================
        // OBTENER ESTADISTICAS
        // =========================
        private Dashboard ObtenerDashboard()
        {
            Dashboard d =
                new Dashboard();

            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

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

                d.ViajesFinalizados =
                    EjecutarConteo(
                        @"SELECT COUNT(*)
                          FROM Pedidos
                          WHERE Estado='Finalizado'",
                        cn);

                d.ViajesCancelados =
                    EjecutarConteo(
                        @"SELECT COUNT(*)
                          FROM Pedidos
                          WHERE Estado='Cancelado'",
                        cn);

                SqlCommand ganancias =
                    new SqlCommand(
                        @"SELECT ISNULL(SUM(Total),0)
                          FROM Facturas",
                        cn);

                d.Ganancias =
                    Convert.ToDecimal(
                        ganancias.ExecuteScalar());
            }

            return d;
        }


        // =========================
        // EJECUTAR COUNT
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


        // =========================
        // VALIDAR ROL
        // =========================
        private bool ValidarRol(string rol)
        {
            if (Session["Usuario"] == null)
            {
                return false;
            }

            return Session["Rol"].ToString() == rol;
        }
    }
}