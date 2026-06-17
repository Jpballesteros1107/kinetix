using kinetix.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace kinetix.Controllers
{
    public class FacturaController : Controller
    {
        // =========================
        // LISTAR
        // =========================
        public ActionResult Index(string buscar = "")
        {
            List<Factura> lista =
                new List<Factura>();

            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        @"SELECT
                            f.*,
                            u.Nombre AS Cliente
                          FROM Facturas f
                          INNER JOIN Pedidos p
                            ON f.IdPedido = p.IdPedido
                          INNER JOIN Usuarios u
                            ON p.IdUsuario = u.IdUsuario
                          WHERE
                            u.Nombre LIKE @txt
                            OR f.MetodoPago LIKE @txt
                            OR f.Estado LIKE @txt
                          ORDER BY f.IdFactura DESC",
                        cn);

                cmd.Parameters.AddWithValue(
                    "@txt",
                    "%" + buscar + "%");

                SqlDataReader dr =
                    cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new Factura
                    {
                        IdFactura =
                            Convert.ToInt32(
                                dr["IdFactura"]),

                        IdPedido =
                            Convert.ToInt32(
                                dr["IdPedido"]),

                        Fecha =
                            Convert.ToDateTime(
                                dr["Fecha"]),

                        Total =
                            Convert.ToDecimal(
                                dr["Total"]),

                        MetodoPago =
                            dr["MetodoPago"].ToString(),

                        Estado =
                            dr["Estado"].ToString(),

                        Cliente =
                            dr["Cliente"].ToString()
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
        public ActionResult Create(Factura f)
        {
            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        @"INSERT INTO Facturas
                        (
                            IdPedido,
                            Fecha,
                            Total,
                            MetodoPago,
                            Estado
                        )
                        VALUES
                        (
                            @idp,
                            @fec,
                            @tot,
                            @met,
                            @est
                        )",
                        cn);

                cmd.Parameters.AddWithValue(
                    "@idp",
                    f.IdPedido);

                cmd.Parameters.AddWithValue(
                    "@fec",
                    DateTime.Now);

                cmd.Parameters.AddWithValue(
                    "@tot",
                    f.Total);

                cmd.Parameters.AddWithValue(
                    "@met",
                    f.MetodoPago);

                cmd.Parameters.AddWithValue(
                    "@est",
                    f.Estado);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }


        // =========================
        // VISTA EDITAR
        // =========================
        public ActionResult Edit(int id)
        {
            Factura f =
                new Factura();

            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        @"SELECT *
                          FROM Facturas
                          WHERE IdFactura=@id",
                        cn);

                cmd.Parameters.AddWithValue(
                    "@id",
                    id);

                SqlDataReader dr =
                    cmd.ExecuteReader();

                if (dr.Read())
                {
                    f.IdFactura =
                        Convert.ToInt32(
                            dr["IdFactura"]);

                    f.IdPedido =
                        Convert.ToInt32(
                            dr["IdPedido"]);

                    f.Fecha =
                        Convert.ToDateTime(
                            dr["Fecha"]);

                    f.Total =
                        Convert.ToDecimal(
                            dr["Total"]);

                    f.MetodoPago =
                        dr["MetodoPago"].ToString();

                    f.Estado =
                        dr["Estado"].ToString();
                }
            }

            return View(f);
        }


        // =========================
        // ACTUALIZAR
        // =========================
        [HttpPost]
        public ActionResult Edit(Factura f)
        {
            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        @"UPDATE Facturas
                          SET
                              Total=@tot,
                              MetodoPago=@met,
                              Estado=@est
                          WHERE IdFactura=@id",
                        cn);

                cmd.Parameters.AddWithValue(
                    "@tot",
                    f.Total);

                cmd.Parameters.AddWithValue(
                    "@met",
                    f.MetodoPago);

                cmd.Parameters.AddWithValue(
                    "@est",
                    f.Estado);

                cmd.Parameters.AddWithValue(
                    "@id",
                    f.IdFactura);

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
                        @"DELETE FROM Facturas
                          WHERE IdFactura=@id",
                        cn);

                cmd.Parameters.AddWithValue(
                    "@id",
                    id);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        /* Mis facturas */
        public ActionResult MisFacturas()
        {
            if (Session["Rol"] == null ||
                Session["Rol"].ToString() != "Cliente")
            {
                return RedirectToAction(
                    "Index",
                    "Login");
            }

            List<Factura> lista =
                new List<Factura>();

            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                    @"
                    SELECT
                        f.*,
                        u.Nombre AS Cliente
                    FROM Facturas f

                    INNER JOIN Pedidos p
                        ON f.IdPedido = p.IdPedido

                    INNER JOIN Usuarios u
                        ON p.IdUsuario = u.IdUsuario

                    WHERE p.IdUsuario = @idu

                    ORDER BY f.Fecha DESC
                    ",
                    cn);

                cmd.Parameters.AddWithValue(
                    "@idu",
                    Session["IdUsuario"]);

                SqlDataReader dr =
                    cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new Factura
                    {
                        IdFactura =
                            Convert.ToInt32(
                                dr["IdFactura"]),

                        IdPedido =
                            Convert.ToInt32(
                                dr["IdPedido"]),

                        Fecha =
                            Convert.ToDateTime(
                                dr["Fecha"]),

                        Total =
                            Convert.ToDecimal(
                                dr["Total"]),

                        MetodoPago =
                            dr["MetodoPago"].ToString(),

                        Estado =
                            dr["Estado"].ToString(),

                        Cliente =
                            dr["Cliente"].ToString()
                    });
                }
            }

            ViewBag.TotalPagado =
                lista.Sum(x => x.Total);

            return View(lista);
        }

    }
}