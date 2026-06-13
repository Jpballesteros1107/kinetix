using kinetix.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace kinetix.Controllers
{
    public class ConductorController : Controller
    {
        // LISTAR
        public ActionResult Index()
        {
            List<Conductor> lista = new List<Conductor>();

            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM Conductores",
                cn);

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Conductor c = new Conductor();

                c.IdConductor = int.Parse(dr["IdConductor"].ToString());

                c.Nombre = dr["Nombre"].ToString();

                c.Licencia = dr["Licencia"].ToString();

                c.Telefono = dr["Telefono"].ToString();

                lista.Add(c);
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
        public ActionResult Create(Conductor c)
        {
            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd = new SqlCommand(
                @"INSERT INTO Conductores
                (
                    Nombre,
                    Licencia,
                    Telefono
                )
                VALUES
                (
                    @nom,
                    @lic,
                    @tel
                )",
                cn);

            cmd.Parameters.AddWithValue("@nom", c.Nombre);

            cmd.Parameters.AddWithValue("@lic", c.Licencia);

            cmd.Parameters.AddWithValue("@tel", c.Telefono);

            cmd.ExecuteNonQuery();

            cn.Close();

            return RedirectToAction("Index");
        }

        // VISTA EDITAR
        public ActionResult Edit(int id)
        {
            Conductor c = new Conductor();

            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM Conductores WHERE IdConductor=@id",
                cn);

            cmd.Parameters.AddWithValue("@id", id);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                c.IdConductor = int.Parse(dr["IdConductor"].ToString());

                c.Nombre = dr["Nombre"].ToString();

                c.Licencia = dr["Licencia"].ToString();

                c.Telefono = dr["Telefono"].ToString();
            }

            cn.Close();

            return View(c);
        }

        // ACTUALIZAR
        [HttpPost]
        public ActionResult Edit(Conductor c)
        {
            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd = new SqlCommand(
                @"UPDATE Conductores
                SET Nombre=@nom,
                    Licencia=@lic,
                    Telefono=@tel
                WHERE IdConductor=@id",
                cn);

            cmd.Parameters.AddWithValue("@nom", c.Nombre);

            cmd.Parameters.AddWithValue("@lic", c.Licencia);

            cmd.Parameters.AddWithValue("@tel", c.Telefono);

            cmd.Parameters.AddWithValue("@id", c.IdConductor);

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
                "DELETE FROM Conductores WHERE IdConductor=@id",
                cn);

            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();

            cn.Close();

            return RedirectToAction("Index");
        }
    }
}