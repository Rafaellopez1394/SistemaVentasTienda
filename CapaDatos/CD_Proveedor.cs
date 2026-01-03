// CapaDatos/CD_Proveedor.cs
using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Proveedor
    {
        private static CD_Proveedor _instancia;
        public static CD_Proveedor Instancia => _instancia ??= new CD_Proveedor();

        public List<Proveedor> ObtenerTodos()
        {
            var lista = new List<Proveedor>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                string query = @"
                    SELECT 
                        p.ProveedorID,
                        p.RFC,
                        p.RazonSocial,
                        p.RegimenFiscalID,
                        ISNULL(rf.Descripcion, '') AS RegimenFiscal,
                        p.CodigoPostal,
                        p.ContactoNombre,
                        p.ContactoCorreo,
                        p.ContactoTelefono,
                        p.BancoID,
                        ISNULL(b.Nombre, '') AS Banco,
                        p.Cuenta,
                        p.CLABE,
                        p.TitularCuenta,
                        p.TipoProveedorID,
                        ISNULL(tp.Descripcion, '') AS TipoProveedor,
                        p.DiasCredito,
                        p.Condiciones,
                        CAST(1 AS BIT) AS Estatus,
                        p.FechaAlta,
                        p.Usuario,
                        p.UltimaAct
                    FROM Proveedores p
                    LEFT JOIN CatRegimenFiscal rf ON p.RegimenFiscalID = rf.RegimenFiscalID
                    LEFT JOIN CatBancos b ON p.BancoID = b.BancoID
                    LEFT JOIN CatTipoProveedor tp ON p.TipoProveedorID = tp.TipoProveedorID
                    ORDER BY p.RazonSocial";

                SqlCommand cmd = new SqlCommand(query, cnx);
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Proveedor
                        {
                            ProveedorID = Guid.Parse(dr["ProveedorID"].ToString()),
                            RFC = dr["RFC"].ToString(),
                            RazonSocial = dr["RazonSocial"].ToString(),
                            RegimenFiscalID = dr["RegimenFiscalID"].ToString(),
                            RegimenFiscal = dr["RegimenFiscal"].ToString(),
                            CodigoPostal = dr["CodigoPostal"].ToString(),
                            ContactoNombre = dr["ContactoNombre"] == DBNull.Value ? null : dr["ContactoNombre"].ToString(),
                            ContactoCorreo = dr["ContactoCorreo"].ToString(),
                            ContactoTelefono = dr["ContactoTelefono"] == DBNull.Value ? null : dr["ContactoTelefono"].ToString(),
                            BancoID = Convert.ToInt32(dr["BancoID"]),
                            Banco = dr["Banco"].ToString(),
                            Cuenta = dr["Cuenta"] == DBNull.Value ? null : dr["Cuenta"].ToString(),
                            CLABE = dr["CLABE"].ToString(),
                            TitularCuenta = dr["TitularCuenta"].ToString(),
                            TipoProveedorID = Convert.ToInt32(dr["TipoProveedorID"]),
                            TipoProveedor = dr["TipoProveedor"].ToString(),
                            DiasCredito = dr["DiasCredito"] == DBNull.Value ? null : (int?)Convert.ToInt32(dr["DiasCredito"]),
                            Condiciones = dr["Condiciones"] == DBNull.Value ? null : dr["Condiciones"].ToString(),
                            Estatus = Convert.ToBoolean(dr["Estatus"]),
                            FechaAlta = Convert.ToDateTime(dr["FechaAlta"]),
                            Usuario = dr["Usuario"].ToString(),
                            UltimaAct = Convert.ToDateTime(dr["UltimaAct"])
                        });
                    }
                }
            }
            return lista;
        }

        public bool AltaProveedor(Proveedor p)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("AltaProveedor", cnx) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@RFC", p.RFC);
                cmd.Parameters.AddWithValue("@RazonSocial", p.RazonSocial);
                cmd.Parameters.AddWithValue("@RegimenFiscalID", p.RegimenFiscalID);
                cmd.Parameters.AddWithValue("@CodigoPostal", p.CodigoPostal);
                cmd.Parameters.AddWithValue("@ContactoNombre", (object)p.ContactoNombre ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ContactoCorreo", p.ContactoCorreo);
                cmd.Parameters.AddWithValue("@ContactoTelefono", (object)p.ContactoTelefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@BancoID", p.BancoID);
                cmd.Parameters.AddWithValue("@Cuenta", (object)p.Cuenta ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CLABE", p.CLABE);
                cmd.Parameters.AddWithValue("@TitularCuenta", p.TitularCuenta);
                cmd.Parameters.AddWithValue("@TipoProveedorID", p.TipoProveedorID);
                cmd.Parameters.AddWithValue("@DiasCredito", (object)p.DiasCredito ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Condiciones", (object)p.Condiciones ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Usuario", p.Usuario ?? "system");
                try { cnx.Open(); cmd.ExecuteNonQuery(); return true; }
                catch { return false; }
            }
        }

        public bool ActualizarProveedor(Proveedor p)
        {
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("ActualizarProveedor", cnx) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@ProveedorID", p.ProveedorID);
                cmd.Parameters.AddWithValue("@RFC", p.RFC);
                cmd.Parameters.AddWithValue("@RazonSocial", p.RazonSocial);
                cmd.Parameters.AddWithValue("@RegimenFiscalID", p.RegimenFiscalID);
                cmd.Parameters.AddWithValue("@CodigoPostal", p.CodigoPostal);
                cmd.Parameters.AddWithValue("@ContactoNombre", (object)p.ContactoNombre ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ContactoCorreo", p.ContactoCorreo);
                cmd.Parameters.AddWithValue("@ContactoTelefono", (object)p.ContactoTelefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@BancoID", p.BancoID);
                cmd.Parameters.AddWithValue("@Cuenta", (object)p.Cuenta ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CLABE", p.CLABE);
                cmd.Parameters.AddWithValue("@TitularCuenta", p.TitularCuenta);
                cmd.Parameters.AddWithValue("@TipoProveedorID", p.TipoProveedorID);
                cmd.Parameters.AddWithValue("@DiasCredito", (object)p.DiasCredito ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Condiciones", (object)p.Condiciones ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Usuario", p.Usuario ?? "system");
                try { cnx.Open(); cmd.ExecuteNonQuery(); return true; }
                catch { return false; }
            }
        }
        public List<Proveedor> BuscarPorNombreORFC(string texto)
        {
            var lista = new List<Proveedor>();
            using (SqlConnection cnx = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("ConsultaProveedoresPorNombreORFC", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Texto", $"%{texto}%");
                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Proveedor
                        {
                            ProveedorID = Guid.Parse(dr["ProveedorID"].ToString()),
                            RFC = dr["RFC"].ToString(),
                            RazonSocial = dr["RazonSocial"].ToString(),
                            ContactoNombre = dr["ContactoNombre"] == DBNull.Value ? null : dr["ContactoNombre"].ToString()
                        });
                    }
                }
            }
            return lista;
        }
    }
}