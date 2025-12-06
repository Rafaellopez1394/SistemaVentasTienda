// CapaDatos/CD_TipoPrecio.cs
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_TipoPrecio
    {
        private static CD_TipoPrecio _instancia = null;
        private CD_TipoPrecio() { }

        public static CD_TipoPrecio Instancia
        {
            get { return _instancia ?? (_instancia = new CD_TipoPrecio()); }
        }

        public List<TipoPrecio> ObtenerTiposPrecio()
        {
            var lista = new List<TipoPrecio>();

            using (SqlConnection con = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT TipoPrecioID, Clave, Nombre, Cargo, Activo 
                    FROM TipoPrecio 
                    WHERE Activo = 1 
                    ORDER BY Nombre";

                SqlCommand cmd = new SqlCommand(query, con);

                try
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new TipoPrecio
                            {
                                TipoPrecioID = Convert.ToInt32(dr["TipoPrecioID"]),
                                Clave = dr["Clave"].ToString(),
                                Nombre = dr["Nombre"].ToString(),
                                Cargo = Convert.ToDecimal(dr["Cargo"]),
                                Activo = Convert.ToBoolean(dr["Activo"])
                            });
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    lista = new List<TipoPrecio>();
                }
            }

            return lista;
        }
    }
}