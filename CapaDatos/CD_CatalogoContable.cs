using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_CatalogoContable
    {
        private static CD_CatalogoContable _instancia;
        public static CD_CatalogoContable Instancia => _instancia ??= new CD_CatalogoContable();
        private CD_CatalogoContable() { }

        /// <summary>
        /// Obtiene una cuenta por su ID
        /// </summary>
        public CatalogoContable ObtenerPorID(int cuentaID)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT CuentaID, CodigoCuenta, NombreCuenta, TipoCuenta, SubTipo, Descripcion, Activo, FechaAlta
                    FROM CatalogoContable
                    WHERE CuentaID = @CuentaID AND Activo = 1";

                SqlCommand cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@CuentaID", cuentaID);

                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new CatalogoContable
                        {
                            CuentaID = Convert.ToInt32(dr["CuentaID"]),
                            CodigoCuenta = dr["CodigoCuenta"]?.ToString(),
                            NombreCuenta = dr["NombreCuenta"]?.ToString(),
                            TipoCuenta = dr["TipoCuenta"]?.ToString(),
                            SubTipo = dr["SubTipo"]?.ToString(),
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
        /// Obtiene una cuenta por subtipo (ej: "CLIENTE", "CAJA", "IVA_COBRADO")
        /// </summary>
        public CatalogoContable ObtenerPorSubTipo(string subTipo)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT CuentaID, CodigoCuenta, NombreCuenta, TipoCuenta, SubTipo, Descripcion, Activo, FechaAlta
                    FROM CatalogoContable
                    WHERE SubTipo = @SubTipo AND Activo = 1";

                SqlCommand cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@SubTipo", subTipo);

                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new CatalogoContable
                        {
                            CuentaID = Convert.ToInt32(dr["CuentaID"]),
                            CodigoCuenta = dr["CodigoCuenta"]?.ToString(),
                            NombreCuenta = dr["NombreCuenta"]?.ToString(),
                            TipoCuenta = dr["TipoCuenta"]?.ToString(),
                            SubTipo = dr["SubTipo"]?.ToString(),
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
        /// Obtiene todas las cuentas activas (o filtra por tipo)
        /// </summary>
        public List<CatalogoContable> ObtenerTodas(string tipoCuenta = null)
        {
            var lista = new List<CatalogoContable>();

            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT CuentaID, CodigoCuenta, NombreCuenta, TipoCuenta, SubTipo, Descripcion, Activo, FechaAlta
                    FROM CatalogoContable
                    WHERE Activo = 1";

                if (!string.IsNullOrEmpty(tipoCuenta))
                    query += " AND TipoCuenta = @TipoCuenta";

                query += " ORDER BY CodigoCuenta ASC";

                SqlCommand cmd = new SqlCommand(query, cnx);
                if (!string.IsNullOrEmpty(tipoCuenta))
                    cmd.Parameters.AddWithValue("@TipoCuenta", tipoCuenta);

                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new CatalogoContable
                        {
                            CuentaID = Convert.ToInt32(dr["CuentaID"]),
                            CodigoCuenta = dr["CodigoCuenta"]?.ToString(),
                            NombreCuenta = dr["NombreCuenta"]?.ToString(),
                            TipoCuenta = dr["TipoCuenta"]?.ToString(),
                            SubTipo = dr["SubTipo"]?.ToString(),
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
