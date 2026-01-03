// CapaDatos/CD_TipoFinanciamiento.cs
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_TipoFinanciamiento
    {
        private static CD_TipoFinanciamiento _instancia = null;
        private CD_TipoFinanciamiento() { }

        public static CD_TipoFinanciamiento Instancia
        {
            get
            {
                if (_instancia == null)
                    _instancia = new CD_TipoFinanciamiento();
                return _instancia;
            }
        }

        // Método que usas en el controlador
        public List<TipoFinanciamiento> ObtenerTiposFinanciamiento()
        {
            var lista = new List<TipoFinanciamiento>();

            using (SqlConnection con = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT TipoFinanciamientoID, Clave, Nombre 
                    FROM TipoFinanciamiento 
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
                            lista.Add(new TipoFinanciamiento
                            {
                                TipoFinanciamientoID = Convert.ToInt32(dr["TipoFinanciamientoID"]),
                                Clave = dr["Clave"].ToString(),
                                Nombre = dr["Nombre"].ToString()
                            });
                        }
                    }
                }
                catch (System.Exception)
                {
                    lista = new List<TipoFinanciamiento>();
                }
            }

            return lista;
        }
    }
}