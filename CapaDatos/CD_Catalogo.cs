using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Catalogos
    {
        private static CD_Catalogos _instancia;
        public static CD_Catalogos Instancia => _instancia ??= new CD_Catalogos();
        private CD_Catalogos() { }

        public List<object> ObtenerRegimenesFiscales()
        {
            var lista = new List<object>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("SELECT RegimenFiscalID, Descripcion FROM CatRegimenFiscal ORDER BY Descripcion", cnx);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new { Value = dr["RegimenFiscalID"].ToString(), Text = dr["Descripcion"].ToString() });
                    }
                }
            }
            return lista;
        }

        public List<object> ObtenerUsosCFDI()
        {
            var lista = new List<object>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("SELECT UsoCFDIID, Descripcion FROM CatUsoCFDI WHERE Estatus = 1 ORDER BY Descripcion", cnx);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new { Value = dr["UsoCFDIID"].ToString(), Text = dr["Descripcion"].ToString() });
                    }
                }
            }
            return lista;
        }

        public List<object> ObtenerTiposCredito()
        {
            var lista = new List<object>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("SELECT TipoCreditoID, Nombre + ' (' + Codigo + ')' AS Nombre FROM CatTiposCredito WHERE Estatus = 1 ORDER BY Nombre", cnx);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new { Value = dr["TipoCreditoID"].ToString(), Text = dr["Nombre"].ToString() });
                    }
                }
            }
            return lista;
        }

        public List<RegimenFiscal> ObtenerRegimenes()
        {
            var lista = new List<RegimenFiscal>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("SELECT RegimenFiscalID, Descripcion FROM CatRegimenFiscal", cnx);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new RegimenFiscal { RegimenFiscalID = dr[0].ToString(), Descripcion = dr[1].ToString() });
                    }
                }
            }
            return lista;
        }

        public List<UsoCFDI> ObtenerUsosCFDI_List()
        {
            var lista = new List<UsoCFDI>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("SELECT UsoCFDIID, Descripcion FROM CatUsoCFDI WHERE Estatus = 1", cnx);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new UsoCFDI { UsoCFDIID = dr[0].ToString(), Descripcion = dr[1].ToString() });
                    }
                }
            }
            return lista;
        }

        public List<Banco> ObtenerBancos()
        {
            var lista = new List<Banco>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("SELECT BancoID, Nombre FROM CatBancos", cnx);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Banco { BancoID = Convert.ToInt32(dr[0]), Nombre = dr[1].ToString() });
                    }
                }
            }
            return lista;
        }

        public List<TipoProveedor> ObtenerTiposProveedor()
        {
            var lista = new List<TipoProveedor>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("SELECT TipoProveedorID, Descripcion FROM CatTipoProveedor", cnx);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new TipoProveedor { TipoProveedorID = Convert.ToInt32(dr[0]), Descripcion = dr[1].ToString() });
                    }
                }
            }
            return lista;
        }
        // Método duplicado: usar CD_TipoCredito.Instancia.ObtenerTodos() en su lugar

        public List<ClaveProdServSAT> ObtenerClavesProdServ()
        {
            var lista = new List<ClaveProdServSAT>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("SELECT ClaveProdServSAT, Descripcion FROM CatClaveProdServSAT ORDER BY ClaveProdServSAT", cnx);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ClaveProdServSAT
                        {
                            ClaveProdServSATID = dr["ClaveProdServSAT"].ToString(),
                            Descripcion = dr["Descripcion"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        public List<UnidadSAT> ObtenerUnidadesSAT()
        {
            var lista = new List<UnidadSAT>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("SELECT ClaveUnidadSAT, Descripcion FROM CatUnidadSAT ORDER BY ClaveUnidadSAT", cnx);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new UnidadSAT
                        {
                            ClaveUnidadSAT = dr["ClaveUnidadSAT"].ToString(),
                            Descripcion = dr["Descripcion"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        public List<CategoriaProducto> ObtenerCategorias()
        {
            var lista = new List<CategoriaProducto>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("SELECT CategoriaID, Nombre, Descripcion FROM CatCategoriasProducto WHERE Estatus = 1 ORDER BY Nombre", cnx);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new CategoriaProducto
                        {
                            CategoriaID = Convert.ToInt32(dr["CategoriaID"]),
                            Nombre = dr["Nombre"].ToString(),
                            Descripcion = dr["Descripcion"] == DBNull.Value ? null : dr["Descripcion"].ToString()
                        });
                    }
                }
            }
            return lista;
        }
        public List<TasaIVA> ObtenerTasasIVA()
        {
            var lista = new List<TasaIVA>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("SELECT TasaIVAID, Clave, Porcentaje, Descripcion FROM CatTasaIVA ORDER BY Clave", cnx);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new TasaIVA
                        {
                            TasaIVAID = Convert.ToInt32(dr["TasaIVAID"]),
                            Clave = dr["Clave"].ToString(),
                            Porcentaje = dr["Porcentaje"] == DBNull.Value ? null : (decimal?)dr["Porcentaje"],
                            Descripcion = dr["Descripcion"].ToString()
                        });
                    }
                }
            }
            return lista;
        }
        public List<TasaIEPS> ObtenerTasasIEPS()
        {
            var lista = new List<TasaIEPS>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("SELECT TasaIEPSID, Clave, Porcentaje, Descripcion FROM CatTasaIEPS ORDER BY Clave", cnx);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new TasaIEPS
                        {
                            TasaIEPSID = Convert.ToInt32(dr["TasaIEPSID"]),
                            Clave = dr["Clave"].ToString(),
                            Porcentaje = dr["Porcentaje"] == DBNull.Value ? null : (decimal?)dr["Porcentaje"],
                            Descripcion = dr["Descripcion"].ToString()
                        });
                    }
                }
            }
            return lista;
        }
    }
}