using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_MapeoIVA
    {
        private static CD_MapeoIVA _instancia;
        public static CD_MapeoIVA Instancia => _instancia ??= new CD_MapeoIVA();
        private CD_MapeoIVA() { }

        /// <summary>
        /// Obtiene el mapeo de IVA para una tasa específica
        /// </summary>
        public MapeoIVA ObtenerPorTasa(decimal tasaIVA, bool exento = false)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT MapeoIVAID, TasaIVA, Exento, CuentaDeudora, CuentaAcreedora, Descripcion, Activo, FechaAlta
                    FROM MapeoContableIVA
                    WHERE TasaIVA = @TasaIVA AND Exento = @Exento AND Activo = 1";

                SqlCommand cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@TasaIVA", tasaIVA);
                cmd.Parameters.AddWithValue("@Exento", exento);

                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new MapeoIVA
                        {
                            MapeoIVAID = Convert.ToInt32(dr["MapeoIVAID"]),
                            TasaIVA = Convert.ToDecimal(dr["TasaIVA"]),
                            Exento = Convert.ToBoolean(dr["Exento"]),
                            CuentaDeudora = Convert.ToInt32(dr["CuentaDeudora"]),
                            CuentaAcreedora = Convert.ToInt32(dr["CuentaAcreedora"]),
                            Descripcion = dr["Descripcion"]?.ToString(),
                            Activo = Convert.ToBoolean(dr["Activo"]),
                            FechaAlta = Convert.ToDateTime(dr["FechaAlta"])
                        };
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Obtiene todos los mapeos de IVA activos
        /// </summary>
        public List<MapeoIVA> ObtenerTodos()
        {
            var lista = new List<MapeoIVA>();

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT MapeoIVAID, TasaIVA, Exento, CuentaDeudora, CuentaAcreedora, Descripcion, Activo, FechaAlta
                    FROM MapeoContableIVA
                    WHERE Activo = 1
                    ORDER BY TasaIVA ASC";

                SqlCommand cmd = new SqlCommand(query, cnx);
                cnx.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new MapeoIVA
                        {
                            MapeoIVAID = Convert.ToInt32(dr["MapeoIVAID"]),
                            TasaIVA = Convert.ToDecimal(dr["TasaIVA"]),
                            Exento = Convert.ToBoolean(dr["Exento"]),
                            CuentaDeudora = Convert.ToInt32(dr["CuentaDeudora"]),
                            CuentaAcreedora = Convert.ToInt32(dr["CuentaAcreedora"]),
                            Descripcion = dr["Descripcion"]?.ToString(),
                            Activo = Convert.ToBoolean(dr["Activo"]),
                            FechaAlta = Convert.ToDateTime(dr["FechaAlta"])
                        });
                    }
                }
            }

            return lista;
        }
    }
}
