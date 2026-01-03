/*using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_ProductoSucursal
    {
        public static CD_ProductoSucursal _instancia = null;

        private CD_ProductoSucursal()
        {

        }

        public static CD_ProductoSucursal Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new CD_ProductoSucursal();
                }
                return _instancia;
            }
        }

        public List<ProductoSucursal> ObtenerProductoSucursal()
        {
            List<ProductoSucursal> rptListaProductoSucursal = new List<ProductoSucursal>();
            using (SqlConnection oConexion = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("usp_ObtenerProductoSucursal", oConexion);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    oConexion.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        rptListaProductoSucursal.Add(new ProductoSucursal()
                        {
                            ProductoSucursalID = Convert.ToInt32(dr["ProductoSucursalID"].ToString()),
                            oProducto = new Producto()
                            {
                                ProductoID = Convert.ToInt32(dr["ProductoID"].ToString()),
                                Codigo = dr["CodigoProducto"].ToString(),
                                Nombre = dr["NombreProducto"].ToString(),
                                Descripcion = dr["DescripcionProducto"].ToString(),
                            },
                            oSucursal = new Sucursal()
                            {
                                SucursalID = Convert.ToInt32(dr["SucursalID"].ToString()),
                                RFC = dr["RFC"].ToString(),
                                Nombre = dr["NombreSucursal"].ToString(),
                                Direccion = dr["DireccionSucursal"].ToString(),
                            },
                            PrecioUnidadCompra = Convert.ToDecimal(dr["PrecioUnidadCompra"].ToString(), new CultureInfo("es-PE")),
                            PrecioUnidadVenta = Convert.ToDecimal(dr["PrecioUnidadVenta"].ToString(), new CultureInfo("es-PE")),
                            Stock = Convert.ToInt32(dr["Stock"].ToString()),
                            Iniciado = Convert.ToBoolean(dr["Iniciado"].ToString())
                        });
                    }
                    dr.Close();

                    return rptListaProductoSucursal;

                }
                catch (Exception ex)
                {
                    rptListaProductoSucursal = null;
                    return rptListaProductoSucursal;
                }
            }
        }

        public bool RegistrarProductoSucursal(ProductoSucursal oProductoSucursal)
        {
            bool respuesta = true;
            using (SqlConnection oConexion = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_RegistrarProductoSucursal", oConexion);
                    cmd.Parameters.AddWithValue("ProductoID", oProductoSucursal.oProducto.ProductoID);
                    cmd.Parameters.AddWithValue("SucursalID", oProductoSucursal.oSucursal.SucursalID);
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

        public bool ModificarProductoSucursal(ProductoSucursal oProductoSucursal)
        {
            bool respuesta = true;
            using (SqlConnection oConexion = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_ModificarProductoSucursal", oConexion);
                    cmd.Parameters.AddWithValue("ProductoSucursalID", oProductoSucursal.ProductoSucursalID);
                    cmd.Parameters.AddWithValue("ProductoID", oProductoSucursal.oProducto.ProductoID);
                    cmd.Parameters.AddWithValue("SucursalID", oProductoSucursal.oSucursal.SucursalID);
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

        public bool EliminarProductoSucursal(int ProductoSucursalID)
        {
            bool respuesta = true;
            using (SqlConnection oConexion = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_EliminarProductoSucursal", oConexion);
                    cmd.Parameters.AddWithValue("ProductoSucursalID", ProductoSucursalID);
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

        public bool ControlarStock(int ProductoID, int SucursalID, int Cantidad, bool Restar)
        {
            bool respuesta = true;
            using (SqlConnection oConexion = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_ControlarStock", oConexion);
                    cmd.Parameters.AddWithValue("ProductoID", ProductoID);
                    cmd.Parameters.AddWithValue("SucursalID", SucursalID);
                    cmd.Parameters.AddWithValue("Cantidad", Cantidad);
                    cmd.Parameters.AddWithValue("Restar", Restar);
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
*/