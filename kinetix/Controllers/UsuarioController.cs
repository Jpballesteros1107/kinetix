using kinetix.Models;
using System;
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
            List<Usuario> lista =
                ObtenerUsuarios(
                    @"SELECT * FROM Usuarios
                    WHERE Estado = 'Activo'");

            return View(lista);
        }


        // =========================
        // BUSCAR
        // =========================
        public ActionResult Buscar(string texto)
        {
            string consulta =
                @"SELECT * FROM Usuarios
                WHERE Nombre LIKE @txt
                OR Correo LIKE @txt";

            List<Usuario> lista =
                new List<Usuario>();

            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        consulta,
                        cn);

                cmd.Parameters.AddWithValue(
                    "@txt",
                    "%" + texto + "%");

                SqlDataReader dr =
                    cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(MapearUsuario(dr));
                }
            }

            return View("Index", lista);
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
        public ActionResult Create(Usuario u)
        {
            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
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

                cmd.Parameters.AddWithValue(
                    "@r",
                    string.IsNullOrEmpty(u.Rol)
                    ? "Cliente"
                    : u.Rol);

                cmd.Parameters.AddWithValue(
                    "@e",
                    "Activo");

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }


        // =========================
        // VISTA EDITAR
        // =========================
        public ActionResult Edit(int id)
        {
            Usuario u =
                new Usuario();

            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        @"SELECT *
                        FROM Usuarios
                        WHERE IdUsuario=@id",
                        cn);

                cmd.Parameters.AddWithValue(
                    "@id",
                    id);

                SqlDataReader dr =
                    cmd.ExecuteReader();

                if (dr.Read())
                {
                    u = MapearUsuario(dr);

                    u.Password =
                        dr["Password"].ToString();
                }
            }

            return View(u);
        }

        // ACTUALIZAR
        [HttpPost]
        public ActionResult Edit(Usuario u)
        {
            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
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
                cmd.Parameters.AddWithValue("@r", u.Rol);
                cmd.Parameters.AddWithValue("@e", u.Estado);
                cmd.Parameters.AddWithValue("@id", u.IdUsuario);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // ELIMINAR
        public ActionResult Delete(int id)
        {
            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        @"UPDATE Usuarios
                          SET Estado = 'Inactivo'
                          WHERE IdUsuario = @id",
                        cn);

                cmd.Parameters.AddWithValue(
                    "@id",
                    id);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

       // METODO LISTAR
        private List<Usuario> ObtenerUsuarios(
            string consulta)
        {
            List<Usuario> lista =
                new List<Usuario>();

            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        consulta,
                        cn);

                SqlDataReader dr =
                    cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(
                        MapearUsuario(dr));
                }
            }

            return lista;
        }

        // MAPEAR USUARIO
        private Usuario MapearUsuario(
            SqlDataReader dr)
        {
            return new Usuario
            {
                IdUsuario =
                    Convert.ToInt32(
                        dr["IdUsuario"]),

                Nombre =
                    dr["Nombre"].ToString(),

                Correo =
                    dr["Correo"].ToString(),

                Telefono =
                    dr["Telefono"].ToString(),

                Rol =
                    dr["Rol"].ToString(),

                Estado =
                    dr["Estado"].ToString()
            };
        }

        public ActionResult Inactivos()
        {
            List<Usuario> lista =
                ObtenerUsuarios(
                    @"SELECT *
                      FROM Usuarios
                      WHERE Estado = 'Inactivo'");

            return View(lista);
        }

        public ActionResult Restaurar(int id)
        {
            using (SqlConnection cn =
                Conexion.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        @"UPDATE Usuarios
                          SET Estado='Activo'
                          WHERE IdUsuario=@id",
                        cn);

                cmd.Parameters.AddWithValue(
                    "@id",
                    id);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Inactivos");
        }
    }
}