using kinetix.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace kinetix.Controllers
{
    public class UsuarioController : Controller
    {
        // LISTAR
        public ActionResult Index()
        {
            List<Usuario> lista = new List<Usuario>();

            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM Usuarios",
                cn);

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Usuario u = new Usuario();

                u.IdUsuario = int.Parse(dr["IdUsuario"].ToString());
                u.Nombre = dr["Nombre"].ToString();
                u.Correo = dr["Correo"].ToString();
                u.Telefono = dr["Telefono"].ToString();
                u.Rol = dr["Rol"].ToString();
                u.Estado = dr["Estado"].ToString();

                lista.Add(u);
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
        public ActionResult Create(Usuario u)
        {
            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd = new SqlCommand(
                @"INSERT INTO Usuarios
                    (
                        Nombre,
                        Correo,
                        Telefono,
                        Password,
                        Rol,
                        Estado
                    )
                    VALUES
                    (
                        @n,
                        @c,
                        @t,
                        @p,
                        @r,
                        @e
                    )",
                cn);

            cmd.Parameters.AddWithValue("@n", u.Nombre);
            cmd.Parameters.AddWithValue("@c", u.Correo);
            cmd.Parameters.AddWithValue("@t", u.Telefono);
            cmd.Parameters.AddWithValue("@p", u.Password);
            cmd.Parameters.AddWithValue("@r", u.Rol);
            cmd.Parameters.AddWithValue("@e", "Activo");

            cmd.ExecuteNonQuery();

            cn.Close();

            return RedirectToAction("Index");
        }


        // VISTA EDITAR
        public ActionResult Edit(int id)
        {
            Usuario u = new Usuario();

            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM Usuarios WHERE IdUsuario=@id",
                cn);

            cmd.Parameters.AddWithValue("@id", id);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                u.IdUsuario = int.Parse(dr["IdUsuario"].ToString());
                u.Nombre = dr["Nombre"].ToString();
                u.Correo = dr["Correo"].ToString();
                u.Telefono = dr["Telefono"].ToString();
                u.Password = dr["Password"].ToString();
            }

            cn.Close();

            return View(u);
        }


        // ACTUALIZAR
        [HttpPost]
        public ActionResult Edit(Usuario u)
        {
            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd = new SqlCommand(
                @"UPDATE Usuarios
                  SET Nombre=@n,
                      Correo=@c,
                      Telefono=@t,
                      Password=@p,
                      Rol=@r,
                      Estado=@e
                  WHERE IdUsuario=@id",
                cn);

            cmd.Parameters.AddWithValue("@n", u.Nombre);
            cmd.Parameters.AddWithValue("@c", u.Correo);
            cmd.Parameters.AddWithValue("@t", u.Telefono);
            cmd.Parameters.AddWithValue("@p", u.Password);
            cmd.Parameters.AddWithValue("@id", u.IdUsuario);
            cmd.Parameters.AddWithValue("@r", u.Rol);
            cmd.Parameters.AddWithValue("@e", u.Estado);

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
                "DELETE FROM Usuarios WHERE IdUsuario=@id",
                cn);

            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();

            cn.Close();

            return RedirectToAction("Index");
        }

        public ActionResult Buscar(string texto)
        {
            List<Usuario> lista = new List<Usuario>();

            SqlConnection cn = Conexion.ObtenerConexion();

            cn.Open();

            SqlCommand cmd = new SqlCommand(
                @"SELECT * FROM Usuarios
        WHERE Nombre LIKE @txt
        OR Correo LIKE @txt",
                cn);

            cmd.Parameters.AddWithValue(
                "@txt",
                "%" + texto + "%");

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Usuario u = new Usuario();

                u.IdUsuario =
                    int.Parse(dr["IdUsuario"].ToString());

                u.Nombre = dr["Nombre"].ToString();

                u.Correo = dr["Correo"].ToString();

                u.Telefono = dr["Telefono"].ToString();

                u.Rol = dr["Rol"].ToString();

                u.Estado = dr["Estado"].ToString();

                lista.Add(u);
            }

            cn.Close();

            return View("Index", lista);
        }
    }
}