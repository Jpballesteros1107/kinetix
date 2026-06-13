using kinetix.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace kinetix.Controllers
{
    public class VehiculoController : Controller
    {
        // LISTAR
        public ActionResult Index()
        {
            List<Vehiculo> lista = new List<Vehiculo>();

            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd = new SqlCommand(
                @"SELECT
                    v.*,
                    c.Nombre AS NombreConductor
                  FROM Vehiculos v
                  INNER JOIN Conductores c
                  ON v.IdConductor = c.IdConductor",
                cn);

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Vehiculo v = new Vehiculo();

                v.IdVehiculo =
                    int.Parse(dr["IdVehiculo"].ToString());

                v.Placa = dr["Placa"].ToString();

                v.Modelo = dr["Modelo"].ToString();

                v.Marca = dr["Marca"].ToString();

                v.IdConductor =
                    int.Parse(dr["IdConductor"].ToString());

                v.NombreConductor =
                    dr["NombreConductor"].ToString();

                lista.Add(v);
            }

            cn.Close();

            return View(lista);
        }


        // VISTA CREAR
        public ActionResult Create()
        {
            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM Conductores",
                cn);

            SqlDataReader dr = cmd.ExecuteReader();

            List<SelectListItem> conductores =
                new List<SelectListItem>();

            while (dr.Read())
            {
                conductores.Add(new SelectListItem
                {
                    Text = dr["Nombre"].ToString(),

                    Value = dr["IdConductor"].ToString()
                });
            }

            cn.Close();

            ViewBag.Conductores = conductores;

            return View();
        }


        // GUARDAR
        [HttpPost]
        public ActionResult Create(Vehiculo v)
        {
            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd = new SqlCommand(
                @"INSERT INTO Vehiculos
                (
                    Placa,
                    Modelo,
                    Marca,
                    IdConductor
                )
                VALUES
                (
                    @pla,
                    @mod,
                    @mar,
                    @idc
                )",
                cn);

            cmd.Parameters.AddWithValue("@pla", v.Placa);

            cmd.Parameters.AddWithValue("@mod", v.Modelo);

            cmd.Parameters.AddWithValue("@mar", v.Marca);

            cmd.Parameters.AddWithValue("@idc", v.IdConductor);

            cmd.ExecuteNonQuery();

            cn.Close();

            return RedirectToAction("Index");
        }


        // VISTA EDITAR
        public ActionResult Edit(int id)
        {
            Vehiculo v = new Vehiculo();

            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM Vehiculos WHERE IdVehiculo=@id",
                cn);

            cmd.Parameters.AddWithValue("@id", id);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                v.IdVehiculo =
                    int.Parse(dr["IdVehiculo"].ToString());

                v.Placa = dr["Placa"].ToString();

                v.Modelo = dr["Modelo"].ToString();

                v.Marca = dr["Marca"].ToString();

                v.IdConductor =
                    int.Parse(dr["IdConductor"].ToString());
            }

            dr.Close();


            // CARGAR CONDUCTORES
            SqlCommand cmdConductores =
                new SqlCommand(
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

                    Value =
                        drConductores["IdConductor"].ToString()
                });
            }

            cn.Close();

            ViewBag.Conductores = conductores;

            return View(v);
        }


        // ACTUALIZAR
        [HttpPost]
        public ActionResult Edit(Vehiculo v)
        {
            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd = new SqlCommand(
                @"UPDATE Vehiculos
                SET Placa=@pla,
                    Modelo=@mod,
                    Marca=@mar,
                    IdConductor=@idc
                WHERE IdVehiculo=@id",
                cn);

            cmd.Parameters.AddWithValue("@pla", v.Placa);

            cmd.Parameters.AddWithValue("@mod", v.Modelo);

            cmd.Parameters.AddWithValue("@mar", v.Marca);

            cmd.Parameters.AddWithValue("@idc", v.IdConductor);

            cmd.Parameters.AddWithValue("@id", v.IdVehiculo);

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
                "DELETE FROM Vehiculos WHERE IdVehiculo=@id",
                cn);

            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();

            cn.Close();

            return RedirectToAction("Index");
        }
    }
}