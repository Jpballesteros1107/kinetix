using System.Configuration;
using System.Data.SqlClient;

namespace kinetix.Models
{
    public class Conexion
    {
        public static SqlConnection ObtenerConexion()
        {
            SqlConnection cn = new SqlConnection(
                ConfigurationManager
                .ConnectionStrings["Conexion"]
                .ConnectionString);

            return cn;
        }
    }
}