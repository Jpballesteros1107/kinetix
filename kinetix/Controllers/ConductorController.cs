using kinetix.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace kinetix.Controllers
{
    public class ConductorController : Controller
    {
        // =========================
        // LISTAR
        // =========================
        public ActionResult Index(string buscar = "")
        {
            List<Conductor> lista =
                new List<Conductor>();

            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        @"SELECT *
                          FROM Conductores
                          WHERE Nombre LIKE @txt
                          OR Licencia LIKE @txt
                          OR Telefono LIKE @txt
                          ORDER BY IdConductor DESC",
                        cn);

                cmd.Parameters.AddWithValue(
                    "@txt",
                    "%" + buscar + "%");

                SqlDataReader dr =
                    cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new Conductor
                    {
                        IdConductor =
                            Convert.ToInt32(
                                dr["IdConductor"]),

                        Nombre =
                            dr["Nombre"].ToString(),

                        Licencia =
                            dr["Licencia"].ToString(),

                        Telefono =
                            dr["Telefono"].ToString(),

                        Estado =
                            dr["Estado"].ToString()
                    });
                }
            }

            return View(lista);
        }

        // =========================
        // VISTA CREAR
        // =========================
        public ActionResult Create()
        {
            return View();
        }

        // =========================
        // GUARDAR
        // =========================
        [HttpPost]
        public ActionResult Create(Conductor c)
        {
            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        @"INSERT INTO Conductores
                        (
                            Nombre,
                            Licencia,
                            Telefono,
                            Estado
                        )
                        VALUES
                        (
                            @nom,
                            @lic,
                            @tel,
                            'Disponible'
                        )",
                        cn);

                cmd.Parameters.AddWithValue(
                    "@nom",
                    c.Nombre);

                cmd.Parameters.AddWithValue(
                    "@lic",
                    c.Licencia);

                cmd.Parameters.AddWithValue(
                    "@tel",
                    c.Telefono);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // =========================
        // VISTA EDITAR
        // =========================
        public ActionResult Edit(int id)
        {
            Conductor c =
                new Conductor();

            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        @"SELECT *
                          FROM Conductores
                          WHERE IdConductor=@id",
                        cn);

                cmd.Parameters.AddWithValue(
                    "@id",
                    id);

                SqlDataReader dr =
                    cmd.ExecuteReader();

                if (dr.Read())
                {
                    c.IdConductor =
                        Convert.ToInt32(
                            dr["IdConductor"]);

                    c.Nombre =
                        dr["Nombre"].ToString();

                    c.Licencia =
                        dr["Licencia"].ToString();

                    c.Telefono =
                        dr["Telefono"].ToString();

                    c.Estado =
                        dr["Estado"].ToString();
                }
            }

            return View(c);
        }

        // =========================
        // ACTUALIZAR
        // =========================
        [HttpPost]
        public ActionResult Edit(Conductor c)
        {
            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        @"UPDATE Conductores
                          SET Nombre=@nom,
                              Licencia=@lic,
                              Telefono=@tel,
                              Estado=@est
                          WHERE IdConductor=@id",
                        cn);

                cmd.Parameters.AddWithValue(
                    "@nom",
                    c.Nombre);

                cmd.Parameters.AddWithValue(
                    "@lic",
                    c.Licencia);

                cmd.Parameters.AddWithValue(
                    "@tel",
                    c.Telefono);

                cmd.Parameters.AddWithValue(
                    "@est",
                    c.Estado);

                cmd.Parameters.AddWithValue(
                    "@id",
                    c.IdConductor);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // =========================
        // ELIMINAR
        // =========================
        public ActionResult Delete(int id)
        {
            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        @"DELETE FROM Conductores
                          WHERE IdConductor=@id",
                        cn);

                cmd.Parameters.AddWithValue(
                    "@id",
                    id);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Solicitar(Conductor c)
        {
            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        @"INSERT INTO Conductores
                (
                    IdUsuario,
                    Nombre,
                    Licencia,
                    Telefono,
                    Estado,
                    FechaRegistro
                )
                VALUES
                (
                    @idu,
                    @nom,
                    @lic,
                    @tel,
                    'Pendiente',
                    GETDATE()
                )",
                        cn);

                cmd.Parameters.AddWithValue(
                    "@idu",
                    Session["IdUsuario"]);

                cmd.Parameters.AddWithValue(
                    "@nom",
                    c.Nombre);

                cmd.Parameters.AddWithValue(
                    "@lic",
                    c.Licencia);

                cmd.Parameters.AddWithValue(
                    "@tel",
                    c.Telefono);

                cmd.ExecuteNonQuery();
            }

            TempData["Mensaje"] =
                "Solicitud enviada correctamente";

            return RedirectToAction(
                "DashboardCliente",
                "Home");
        }

        public ActionResult Aprobar(int id)
        {
            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                // =========================
                // OBTENER SOLICITUD
                // =========================

                SqlCommand obtener =
                    new SqlCommand(
                        @"SELECT *
                  FROM SolicitudesConductores
                  WHERE IdSolicitud=@id",
                        cn);

                obtener.Parameters.AddWithValue(
                    "@id",
                    id);

                SqlDataReader dr =
                    obtener.ExecuteReader();

                SolicitudConductor s =
                    new SolicitudConductor();

                if (dr.Read())
                {
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
                }

                dr.Close();

                // =========================
                // INSERTAR EN CONDUCTORES
                // =========================

                SqlCommand insertar =
                    new SqlCommand(
                        @"INSERT INTO Conductores
                (
                    Nombre,
                    Licencia,
                    Telefono,
                    Estado
                )
                VALUES
                (
                    @nom,
                    @lic,
                    @tel,
                    'Disponible'
                )",
                        cn);

                insertar.Parameters.AddWithValue(
                    "@nom",
                    s.Nombre);

                insertar.Parameters.AddWithValue(
                    "@lic",
                    s.Licencia);

                insertar.Parameters.AddWithValue(
                    "@tel",
                    s.Telefono);

                insertar.ExecuteNonQuery();

                // =========================
                // ACTUALIZAR SOLICITUD
                // =========================

                SqlCommand actualizar =
                    new SqlCommand(
                        @"UPDATE SolicitudesConductores
                  SET Estado='Aprobado'
                  WHERE IdSolicitud=@id",
                        cn);

                actualizar.Parameters.AddWithValue(
                    "@id",
                    id);

                actualizar.ExecuteNonQuery();
            }

            return RedirectToAction(
                "DashboardAdmin",
                "Home");
        }

        public ActionResult Rechazar(int id)
        {
            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        @"UPDATE SolicitudesConductores
                  SET Estado='Rechazado'
                  WHERE IdSolicitud=@id",
                        cn);

                cmd.Parameters.AddWithValue(
                    "@id",
                    id);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction(
                "DashboardAdmin",
                "Home");
        }
    }
}