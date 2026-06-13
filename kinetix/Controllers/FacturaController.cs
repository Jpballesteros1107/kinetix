using kinetix.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace kinetix.Controllers
{
    public class FacturaController : Controller
    {
        // LISTAR
        public ActionResult Index()
        {
            List<Factura> lista = new List<Factura>();

            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM Facturas",
                cn);

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Factura f = new Factura();

                f.IdFactura = int.Parse(dr["IdFactura"].ToString());

                f.IdPedido = int.Parse(dr["IdPedido"].ToString());

                f.Fecha = DateTime.Parse(dr["Fecha"].ToString());

                f.Total = decimal.Parse(dr["Total"].ToString());

                f.MetodoPago = dr["MetodoPago"].ToString();

                f.Estado = dr["Estado"].ToString();

                lista.Add(f);
            }

            cn.Close();

            return View(lista);
        }


        // VISTA CREAR
        public ActionResult Create()
        {
            return View();
        }


        // GUARDAR
        [HttpPost]
        public ActionResult Create(Factura f)
        {
            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd = new SqlCommand(
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

            cmd.Parameters.AddWithValue("@idp", f.IdPedido);

            cmd.Parameters.AddWithValue("@fec", DateTime.Now);

            cmd.Parameters.AddWithValue("@tot", f.Total);

            cmd.Parameters.AddWithValue("@met", f.MetodoPago);

            cmd.Parameters.AddWithValue("@est", f.Estado);

            cmd.ExecuteNonQuery();

            cn.Close();

            return RedirectToAction("Index");
        }


        // VISTA EDITAR
        public ActionResult Edit(int id)
        {
            Factura f = new Factura();

            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM Facturas WHERE IdFactura=@id",
                cn);

            cmd.Parameters.AddWithValue("@id", id);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                f.IdFactura = int.Parse(dr["IdFactura"].ToString());

                f.IdPedido = int.Parse(dr["IdPedido"].ToString());

                f.Fecha = DateTime.Parse(dr["Fecha"].ToString());

                f.Total = decimal.Parse(dr["Total"].ToString());

                f.MetodoPago = dr["MetodoPago"].ToString();

                f.Estado = dr["Estado"].ToString();
            }

            cn.Close();

            return View(f);
        }


        // ACTUALIZAR
        [HttpPost]
        public ActionResult Edit(Factura f)
        {
            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd = new SqlCommand(
                @"UPDATE Facturas
                SET IdPedido=@idp,
                    Total=@tot,
                    MetodoPago=@met,
                    Estado=@est
                WHERE IdFactura=@id",
                cn);

            cmd.Parameters.AddWithValue("@idp", f.IdPedido);

            cmd.Parameters.AddWithValue("@tot", f.Total);

            cmd.Parameters.AddWithValue("@met", f.MetodoPago);

            cmd.Parameters.AddWithValue("@est", f.Estado);

            cmd.Parameters.AddWithValue("@id", f.IdFactura);

            cmd.ExecuteNonQuery();

            cn.Close();

            return RedirectToAction("Index");
        }


        // ELIMINAR
        public ActionResult Delete(int id)
        {
            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd = new SqlCommand(
                "DELETE FROM Facturas WHERE IdFactura=@id",
                cn);

            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();

            cn.Close();

            return RedirectToAction("Index");
        }
    }
}