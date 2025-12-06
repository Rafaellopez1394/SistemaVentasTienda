using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Sucursal
    {
        public static CD_Sucursal _instancia = null;

        private CD_Sucursal()
        {

        }

        public static CD_Sucursal Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new CD_Sucursal();
                }
                return _instancia;
            }
        }

        public List<Sucursal> ObtenerSucursales()
        {
            List<Sucursal> rptListaUsuario = new List<Sucursal>();
            using (SqlConnection oConexion = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("usp_ObtenerSucursal", oConexion);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    oConexion.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        rptListaUsuario.Add(new Sucursal()
                        {
                            SucursalID = Convert.ToInt32(dr["SucursalID"].ToString()),
                            Nombre = dr["Nombre"].ToString(),
                            RFC = dr["RFC"].ToString(),
                            Direccion = dr["Direccion"].ToString(),
                            Telefono = dr["Telefono"].ToString(),
                            Activo = Convert.ToBoolean(dr["Activo"].ToString())

                        });
                    }
                    dr.Close();

                    return rptListaUsuario;

                }
                catch (Exception ex)
                {
                    rptListaUsuario = null;
                    return rptListaUsuario;
                }
            }
        }

        public bool RegistrarSucursal(Sucursal oSucursal)
        {
            bool respuesta = true;
            using (SqlConnection oConexion = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_RegistrarSucursal", oConexion);
                    cmd.Parameters.AddWithValue("Nombre", oSucursal.Nombre);
                    cmd.Parameters.AddWithValue("RFC", oSucursal.RFC);
                    cmd.Parameters.AddWithValue("Direccion", oSucursal.Direccion);
                    cmd.Parameters.AddWithValue("Telefono", oSucursal.Telefono);
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;

                    oConexion.Open();

                    cmd.ExecuteNonQuery();

                    respuesta = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);

                }
                catch (Exception ex)
                {
                    respuesta = false;
                }

            }

            return respuesta;

        }


        public bool ModificarSucursal(Sucursal oSucursal)
        {
            bool respuesta = true;
            using (SqlConnection oConexion = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_ModificarSucursal", oConexion);
                    cmd.Parameters.AddWithValue("SucursalID", oSucursal.SucursalID);
                    cmd.Parameters.AddWithValue("Nombre", oSucursal.Nombre);
                    cmd.Parameters.AddWithValue("RFC", oSucursal.RFC);
                    cmd.Parameters.AddWithValue("Direccion", oSucursal.Direccion);
                    cmd.Parameters.AddWithValue("Telefono", oSucursal.Telefono);
                    cmd.Parameters.AddWithValue("Activo", oSucursal.Activo);
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;

                    oConexion.Open();

                    cmd.ExecuteNonQuery();

                    respuesta = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);

                }
                catch (Exception ex)
                {
                    respuesta = false;
                }

            }

            return respuesta;

        }

        public bool EliminarSucursal(int SucursalID)
        {
            bool respuesta = true;
            using (SqlConnection oConexion = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_EliminarSucursal", oConexion);
                    cmd.Parameters.AddWithValue("SucursalID", SucursalID);
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oConexion.Open();

                    cmd.ExecuteNonQuery();

                    respuesta = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);

                }
                catch (Exception ex)
                {
                    respuesta = false;
                }

            }

            return respuesta;

        }
    }
}
