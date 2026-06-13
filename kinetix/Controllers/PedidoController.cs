using kinetix.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace kinetix.Controllers
{
    public class PedidoController : Controller
    {
        // LISTAR
        public ActionResult Index()
        {
            List<Pedido> lista = new List<Pedido>();

            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd = new SqlCommand(
                 @"SELECT
                        p.*,
                        u.Nombre AS NombreUsuario,
                        c.Nombre AS NombreConductor
                    FROM Pedidos p
                    INNER JOIN Usuarios u
                        ON p.IdUsuario = u.IdUsuario
                    INNER JOIN Conductores c
                        ON p.IdConductor = c.IdConductor",
                cn);

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Pedido p = new Pedido();

                p.IdPedido = int.Parse(dr["IdPedido"].ToString());

                p.IdUsuario = int.Parse(dr["IdUsuario"].ToString());

                p.IdConductor = int.Parse(dr["IdConductor"].ToString());

                p.NombreUsuario = dr["NombreUsuario"].ToString();

                p.NombreConductor = dr["NombreConductor"].ToString();

                p.Origen = dr["Origen"].ToString();

                p.Destino = dr["Destino"].ToString();

                p.Estado = dr["Estado"].ToString();

                p.Valor = decimal.Parse(dr["Valor"].ToString());

                p.Fecha = DateTime.Parse(dr["Fecha"].ToString());

                lista.Add(p);
            }

            cn.Close();

            return View(lista);
        }


        // VISTA CREAR
        public ActionResult Create()
        {
            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            // USUARIOS
            SqlCommand cmdUsuarios = new SqlCommand(
                "SELECT * FROM Usuarios",
                cn);

            SqlDataReader drUsuarios = cmdUsuarios.ExecuteReader();

            List<SelectListItem> usuarios =
                new List<SelectListItem>();

            while (drUsuarios.Read())
            {
                usuarios.Add(new SelectListItem
                {
                    Text = drUsuarios["Nombre"].ToString(),

                    Value = drUsuarios["IdUsuario"].ToString()
                });
            }

            drUsuarios.Close();


            // CONDUCTORES
            SqlCommand cmdConductores = new SqlCommand(
                "SELECT * FROM Conductores",
                cn);

            SqlDataReader drConductores =
                cmdConductores.ExecuteReader();

            List<SelectListItem> conductores =
                new List<SelectListItem>();

            while (drConductores.Read())
            {
                conductores.Add(new SelectListItem
                {
                    Text = drConductores["Nombre"].ToString(),

                    Value = drConductores["IdConductor"].ToString()
                });
            }

            drConductores.Close();

            cn.Close();

            ViewBag.Usuarios = usuarios;

            ViewBag.Conductores = conductores;

            return View();
        }


        // GUARDAR
        [HttpPost]
        public ActionResult Create(Pedido p)
        {
            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd = new SqlCommand(
                @"INSERT INTO Pedidos
                (
                    IdUsuario,
                    IdConductor,
                    Origen,
                    Destino,
                    Estado,
                    Valor,
                    Fecha
                )
                VALUES
                (
                    @idu,
                    @idc,
                    @ori,
                    @des,
                    @est,
                    @val,
                    @fec
                )",
                cn);

            cmd.Parameters.AddWithValue("@idu", p.IdUsuario);

            cmd.Parameters.AddWithValue("@idc", p.IdConductor);

            cmd.Parameters.AddWithValue("@ori", p.Origen);

            cmd.Parameters.AddWithValue("@des", p.Destino);

            cmd.Parameters.AddWithValue("@est", "Pendiente");

            cmd.Parameters.AddWithValue("@val", p.Valor);

            cmd.Parameters.AddWithValue("@fec", DateTime.Now);

            cmd.ExecuteNonQuery();

            // OBTENER EL ÚLTIMO PEDIDO INSERTADO
            SqlCommand cmdUltimo = new SqlCommand(
                "SELECT MAX(IdPedido) FROM Pedidos",
                cn);

            int idPedido =
                int.Parse(cmdUltimo.ExecuteScalar().ToString());


            // CREAR FACTURA AUTOMÁTICA
            SqlCommand cmdFactura = new SqlCommand(
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

            cmdFactura.Parameters.AddWithValue("@idp", idPedido);

            cmdFactura.Parameters.AddWithValue(
                "@fec",
                DateTime.Now);

            cmdFactura.Parameters.AddWithValue(
                "@tot",
                p.Valor);

            cmdFactura.Parameters.AddWithValue(
                "@met",
                "Efectivo");

            cmdFactura.Parameters.AddWithValue(
                "@est",
                "Pendiente");

            cmdFactura.ExecuteNonQuery();

            cn.Close();

            return RedirectToAction("Index");
        }
    }
}