using kinetix.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace kinetix.Controllers
{
    public class PedidoController : Controller
    {

        // LISTAR PEDIDOS
        public ActionResult Index()
        {
            List<Pedido> lista =
                new List<Pedido>();

            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                    @"
                    SELECT
                        p.*,
                        u.Nombre AS NombreUsuario,
                        c.Nombre AS NombreConductor
                    FROM Pedidos p
                    INNER JOIN Usuarios u
                        ON p.IdUsuario = u.IdUsuario
                    INNER JOIN Conductores c
                        ON p.IdConductor = c.IdConductor
                    ",
                    cn);

                SqlDataReader dr =
                    cmd.ExecuteReader();

                while (dr.Read())
                {
                    Pedido p = new Pedido();

                    p.IdPedido =
                        Convert.ToInt32(
                            dr["IdPedido"]);

                    p.IdUsuario =
                        Convert.ToInt32(
                            dr["IdUsuario"]);

                    p.IdConductor =
                        Convert.ToInt32(
                            dr["IdConductor"]);

                    p.NombreUsuario =
                        dr["NombreUsuario"].ToString();

                    p.NombreConductor =
                        dr["NombreConductor"].ToString();

                    p.Origen =
                        dr["Origen"].ToString();

                    p.Destino =
                        dr["Destino"].ToString();

                    p.Estado =
                        dr["Estado"].ToString();

                    p.Valor =
                        Convert.ToDecimal(
                            dr["Valor"]);

                    p.Fecha =
                        Convert.ToDateTime(
                            dr["Fecha"]);

                    lista.Add(p);
                }
            }

            return View(lista);
        }


        // =========================
        // VISTA CREAR
        // =========================
        public ActionResult Create()
        {
            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                // USUARIOS
                SqlCommand usuariosCmd =
                    new SqlCommand(
                        "SELECT * FROM Usuarios",
                        cn);

                SqlDataReader usuariosDr =
                    usuariosCmd.ExecuteReader();

                List<SelectListItem> usuarios =
                    new List<SelectListItem>();

                while (usuariosDr.Read())
                {
                    usuarios.Add(
                        new SelectListItem
                        {
                            Text =
                                usuariosDr["Nombre"]
                                .ToString(),

                            Value =
                                usuariosDr["IdUsuario"]
                                .ToString()
                        });
                }

                usuariosDr.Close();


                // CONDUCTORES DISPONIBLES
                SqlCommand conductoresCmd =
                    new SqlCommand(
                        @"SELECT *
                        FROM Conductores
                        WHERE Estado='Disponible'",
                        cn);

                SqlDataReader conductoresDr =
                    conductoresCmd.ExecuteReader();

                List<SelectListItem> conductores =
                    new List<SelectListItem>();

                while (conductoresDr.Read())
                {
                    conductores.Add(
                        new SelectListItem
                        {
                            Text =
                                conductoresDr["Nombre"]
                                .ToString(),

                            Value =
                                conductoresDr["IdConductor"]
                                .ToString()
                        });
                }

                ViewBag.Usuarios =
                    usuarios;

                ViewBag.Conductores =
                    conductores;
            }

            return View();
        }


        // =========================
        // GUARDAR PEDIDO
        // =========================
        [HttpPost]
        public ActionResult Create(Pedido p)
        {
            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                // CALCULO AUTOMATICO
                Random r = new Random();

                decimal valor =
                    r.Next(8000, 50000);

                SqlCommand cmd =
                    new SqlCommand(
                    @"
                    INSERT INTO Pedidos
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
                        'Pendiente',
                        @val,
                        @fec
                    )
                    ",
                    cn);

                cmd.Parameters.AddWithValue(
                    "@idu",
                    p.IdUsuario);

                cmd.Parameters.AddWithValue(
                    "@idc",
                    p.IdConductor);

                cmd.Parameters.AddWithValue(
                    "@ori",
                    p.Origen);

                cmd.Parameters.AddWithValue(
                    "@des",
                    p.Destino);

                cmd.Parameters.AddWithValue(
                    "@val",
                    valor);

                cmd.Parameters.AddWithValue(
                    "@fec",
                    DateTime.Now);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }


        // =========================
        // CAMBIAR ESTADO
        // =========================
        [HttpPost]
        public ActionResult CambiarEstado(
            int id,
            string estado)
        {
            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                // ACTUALIZAR ESTADO
                SqlCommand estadoCmd =
                    new SqlCommand(
                    @"
                    UPDATE Pedidos
                    SET Estado=@est
                    WHERE IdPedido=@id
                    ",
                    cn);

                estadoCmd.Parameters.AddWithValue(
                    "@est",
                    estado);

                estadoCmd.Parameters.AddWithValue(
                    "@id",
                    id);

                estadoCmd.ExecuteNonQuery();


                // CONDUCTOR OCUPADO
                if (estado == "Aceptado")
                {
                    SqlCommand ocupadoCmd =
                        new SqlCommand(
                        @"
                        UPDATE Conductores
                        SET Estado='Ocupado'
                        WHERE IdConductor=
                        (
                            SELECT IdConductor
                            FROM Pedidos
                            WHERE IdPedido=@id
                        )
                        ",
                        cn);

                    ocupadoCmd.Parameters
                        .AddWithValue(
                            "@id",
                            id);

                    ocupadoCmd.ExecuteNonQuery();
                }


                // CONDUCTOR DISPONIBLE
                if (estado == "Finalizado"
                 || estado == "Cancelado")
                {
                    SqlCommand disponibleCmd =
                        new SqlCommand(
                        @"
                        UPDATE Conductores
                        SET Estado='Disponible'
                        WHERE IdConductor=
                        (
                            SELECT IdConductor
                            FROM Pedidos
                            WHERE IdPedido=@id
                        )
                        ",
                        cn);

                    disponibleCmd.Parameters
                        .AddWithValue(
                            "@id",
                            id);

                    disponibleCmd.ExecuteNonQuery();
                }


                // FACTURA AUTOMATICA
                if (estado == "Finalizado")
                {
                    // VALIDAR SI YA EXISTE
                    SqlCommand existeCmd =
                        new SqlCommand(
                        @"
                        SELECT COUNT(*)
                        FROM Facturas
                        WHERE IdPedido=@id
                        ",
                        cn);

                    existeCmd.Parameters
                        .AddWithValue(
                            "@id",
                            id);

                    int existe =
                        Convert.ToInt32(
                            existeCmd.ExecuteScalar());

                    if (existe == 0)
                    {
                        SqlCommand totalCmd =
                            new SqlCommand(
                            @"
                            SELECT Valor
                            FROM Pedidos
                            WHERE IdPedido=@id
                            ",
                            cn);

                        totalCmd.Parameters
                            .AddWithValue(
                                "@id",
                                id);

                        decimal total =
                            Convert.ToDecimal(
                                totalCmd.ExecuteScalar());

                        SqlCommand facturaCmd =
                            new SqlCommand(
                            @"
                            INSERT INTO Facturas
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
                                'Tarjeta',
                                'Pagada'
                            )
                            ",
                            cn);

                        facturaCmd.Parameters
                            .AddWithValue(
                                "@idp",
                                id);

                        facturaCmd.Parameters
                            .AddWithValue(
                                "@fec",
                                DateTime.Now);

                        facturaCmd.Parameters
                            .AddWithValue(
                                "@tot",
                                total);

                        facturaCmd.ExecuteNonQuery();
                    }
                }
            }

            return RedirectToAction(
            "DashboardConductor",
            "Home");
        }
    }
}