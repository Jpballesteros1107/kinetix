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
                    LEFT JOIN Conductores c
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

                    int idUsuario =
                        Convert.ToInt32(
                            Session["IdUsuario"]);

                    p.IdConductor =
                        Convert.ToInt32(
                            dr["IdConductor"]);

                    p.NombreUsuario =
                        dr["NombreUsuario"].ToString();

                    p.NombreConductor =
                        dr["NombreConductor"] == DBNull.Value
                        ? "Sin asignar"
                        : dr["NombreConductor"].ToString();

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
            if (Session["IdUsuario"] == null)
            {
                return RedirectToAction(
                    "Index",
                    "Login");
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

                int idUsuario =
                    Convert.ToInt32(
                        Session["IdUsuario"]);

                decimal valor = 15000;

                SqlCommand cmd =
                    new SqlCommand(
                    @"
                    INSERT INTO Pedidos
                    (
                        IdUsuario,
                        Origen,
                        Destino,
                        Estado,
                        Valor,
                        Fecha
                    )
                    VALUES
                    (
                        @idu,
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
                    idUsuario);

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

            return RedirectToAction("MisViajes");
        }

        // CAMBIAR ESTADO
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
                    int idUsuario =
                    Convert.ToInt32(
                        Session["IdUsuario"]);

                    SqlCommand conductorCmd =
                        new SqlCommand(
                        @"SELECT IdConductor
                          FROM Conductores
                          WHERE IdUsuario=@idu",
                        cn);

                    conductorCmd.Parameters
                        .AddWithValue(
                            "@idu",
                            idUsuario);

                    int idConductor =
                        Convert.ToInt32(
                            conductorCmd.ExecuteScalar());

                    SqlCommand asignarCmd =
                        new SqlCommand(
                        @"
                        UPDATE Pedidos
                        SET
                            Estado='Aceptado',
                            IdConductor=@idc
                        WHERE IdPedido=@id
                        ",
                        cn);

                    asignarCmd.Parameters
                        .AddWithValue(
                            "@idc",
                            idConductor);

                    asignarCmd.Parameters
                        .AddWithValue(
                            "@id",
                            id);

                    asignarCmd.ExecuteNonQuery();
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

        public ActionResult MisViajes()
        {
            if (Session["Rol"] == null ||
                Session["Rol"].ToString() != "Conductor")
            {
                return RedirectToAction(
                    "Index",
                    "Login");
            }

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
                ISNULL(c.Nombre,'Sin asignar')
                    AS NombreConductor
            FROM Pedidos p
            INNER JOIN Usuarios u
                ON p.IdUsuario = u.IdUsuario
            LEFT JOIN Conductores c
                ON p.IdConductor = c.IdConductor
            WHERE

                (
                    p.Estado = 'Pendiente'
                    AND p.IdConductor IS NULL
                )

                OR

                (
                    p.IdConductor =
                    (
                        SELECT IdConductor
                        FROM Conductores
                        WHERE IdUsuario = @idu
                    )
                )

            ORDER BY p.IdPedido DESC
            ",
                    cn);

                cmd.Parameters.AddWithValue(
                    "@idu",
                    Session["IdUsuario"]);

                SqlDataReader dr =
                    cmd.ExecuteReader();

                while (dr.Read())
                {
                    Pedido p =
                        new Pedido();

                    p.IdPedido =
                        Convert.ToInt32(
                            dr["IdPedido"]);

                    p.IdUsuario =
                        Convert.ToInt32(
                            dr["IdUsuario"]);

                    p.IdConductor =
                        dr["IdConductor"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(
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
    }
}