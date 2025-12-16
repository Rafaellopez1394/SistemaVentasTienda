using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CapaDatos
{
    public class CD_Empleado
    {
        public static CD_Empleado Instancia = new CD_Empleado();

        public List<Empleado> ObtenerTodos()
        {
            var lista = new List<Empleado>();
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = @"SELECT e.*, s.Nombre AS NombreSucursal 
                                FROM Empleados e
                                INNER JOIN Sucursal s ON e.SucursalID = s.SucursalID
                                ORDER BY e.NumeroEmpleado";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Empleado
                            {
                                EmpleadoID = Convert.ToInt32(dr["EmpleadoID"]),
                                NumeroEmpleado = dr["NumeroEmpleado"]?.ToString(),
                                Nombre = dr["Nombre"]?.ToString(),
                                ApellidoPaterno = dr["ApellidoPaterno"]?.ToString(),
                                ApellidoMaterno = dr["ApellidoMaterno"]?.ToString(),
                                RFC = dr["RFC"]?.ToString(),
                                CURP = dr["CURP"]?.ToString(),
                                NSS = dr["NSS"]?.ToString(),
                                FechaNacimiento = Convert.ToDateTime(dr["FechaNacimiento"]),
                                FechaIngreso = Convert.ToDateTime(dr["FechaIngreso"]),
                                FechaBaja = dr["FechaBaja"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FechaBaja"]),
                                SucursalID = Convert.ToInt32(dr["SucursalID"]),
                                NombreSucursal = dr["NombreSucursal"]?.ToString(),
                                Puesto = dr["Puesto"]?.ToString(),
                                Departamento = dr["Departamento"]?.ToString(),
                                TipoContrato = dr["TipoContrato"]?.ToString(),
                                TipoJornada = dr["TipoJornada"]?.ToString(),
                                PeriodicidadPago = dr["PeriodicidadPago"]?.ToString(),
                                SalarioDiario = Convert.ToDecimal(dr["SalarioDiario"]),
                                SalarioMensual = Convert.ToDecimal(dr["SalarioMensual"]),
                                SalarioDiarioIntegrado = dr["SalarioDiarioIntegrado"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["SalarioDiarioIntegrado"]),
                                Telefono = dr["Telefono"]?.ToString(),
                                Email = dr["Email"]?.ToString(),
                                Domicilio = dr["Domicilio"]?.ToString(),
                                CodigoPostal = dr["CodigoPostal"]?.ToString(),
                                Banco = dr["Banco"]?.ToString(),
                                CuentaBancaria = dr["CuentaBancaria"]?.ToString(),
                                CLABE = dr["CLABE"]?.ToString(),
                                Estatus = dr["Estatus"]?.ToString(),
                                Usuario = dr["Usuario"]?.ToString(),
                                FechaAlta = Convert.ToDateTime(dr["FechaAlta"]),
                                UltimaAct = Convert.ToDateTime(dr["UltimaAct"])
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public List<Empleado> ObtenerActivos()
        {
            var lista = new List<Empleado>();
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = @"SELECT e.*, s.Nombre AS NombreSucursal 
                                FROM Empleados e
                                INNER JOIN Sucursal s ON e.SucursalID = s.SucursalID
                                WHERE e.Estatus = 'ACTIVO'
                                ORDER BY e.NumeroEmpleado";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Empleado
                            {
                                EmpleadoID = Convert.ToInt32(dr["EmpleadoID"]),
                                NumeroEmpleado = dr["NumeroEmpleado"]?.ToString(),
                                Nombre = dr["Nombre"]?.ToString(),
                                ApellidoPaterno = dr["ApellidoPaterno"]?.ToString(),
                                ApellidoMaterno = dr["ApellidoMaterno"]?.ToString(),
                                RFC = dr["RFC"]?.ToString(),
                                CURP = dr["CURP"]?.ToString(),
                                NSS = dr["NSS"]?.ToString(),
                                FechaNacimiento = Convert.ToDateTime(dr["FechaNacimiento"]),
                                FechaIngreso = Convert.ToDateTime(dr["FechaIngreso"]),
                                SucursalID = Convert.ToInt32(dr["SucursalID"]),
                                NombreSucursal = dr["NombreSucursal"]?.ToString(),
                                Puesto = dr["Puesto"]?.ToString(),
                                Departamento = dr["Departamento"]?.ToString(),
                                TipoContrato = dr["TipoContrato"]?.ToString(),
                                TipoJornada = dr["TipoJornada"]?.ToString(),
                                PeriodicidadPago = dr["PeriodicidadPago"]?.ToString(),
                                SalarioDiario = Convert.ToDecimal(dr["SalarioDiario"]),
                                SalarioMensual = Convert.ToDecimal(dr["SalarioMensual"]),
                                SalarioDiarioIntegrado = dr["SalarioDiarioIntegrado"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["SalarioDiarioIntegrado"]),
                                Telefono = dr["Telefono"]?.ToString(),
                                Email = dr["Email"]?.ToString(),
                                Banco = dr["Banco"]?.ToString(),
                                CuentaBancaria = dr["CuentaBancaria"]?.ToString(),
                                CLABE = dr["CLABE"]?.ToString(),
                                Estatus = dr["Estatus"]?.ToString(),
                                Usuario = dr["Usuario"]?.ToString()
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public Empleado ObtenerPorId(int empleadoId)
        {
            Empleado empleado = null;
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = @"SELECT e.*, s.Nombre AS NombreSucursal 
                                FROM Empleados e
                                INNER JOIN Sucursal s ON e.SucursalID = s.SucursalID
                                WHERE e.EmpleadoID = @EmpleadoID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmpleadoID", empleadoId);
                    conn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            empleado = new Empleado
                            {
                                EmpleadoID = Convert.ToInt32(dr["EmpleadoID"]),
                                NumeroEmpleado = dr["NumeroEmpleado"]?.ToString(),
                                Nombre = dr["Nombre"]?.ToString(),
                                ApellidoPaterno = dr["ApellidoPaterno"]?.ToString(),
                                ApellidoMaterno = dr["ApellidoMaterno"]?.ToString(),
                                RFC = dr["RFC"]?.ToString(),
                                CURP = dr["CURP"]?.ToString(),
                                NSS = dr["NSS"]?.ToString(),
                                FechaNacimiento = Convert.ToDateTime(dr["FechaNacimiento"]),
                                FechaIngreso = Convert.ToDateTime(dr["FechaIngreso"]),
                                FechaBaja = dr["FechaBaja"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FechaBaja"]),
                                SucursalID = Convert.ToInt32(dr["SucursalID"]),
                                NombreSucursal = dr["NombreSucursal"]?.ToString(),
                                Puesto = dr["Puesto"]?.ToString(),
                                Departamento = dr["Departamento"]?.ToString(),
                                TipoContrato = dr["TipoContrato"]?.ToString(),
                                TipoJornada = dr["TipoJornada"]?.ToString(),
                                PeriodicidadPago = dr["PeriodicidadPago"]?.ToString(),
                                SalarioDiario = Convert.ToDecimal(dr["SalarioDiario"]),
                                SalarioMensual = Convert.ToDecimal(dr["SalarioMensual"]),
                                SalarioDiarioIntegrado = dr["SalarioDiarioIntegrado"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["SalarioDiarioIntegrado"]),
                                Telefono = dr["Telefono"]?.ToString(),
                                Email = dr["Email"]?.ToString(),
                                Domicilio = dr["Domicilio"]?.ToString(),
                                CodigoPostal = dr["CodigoPostal"]?.ToString(),
                                Banco = dr["Banco"]?.ToString(),
                                CuentaBancaria = dr["CuentaBancaria"]?.ToString(),
                                CLABE = dr["CLABE"]?.ToString(),
                                Estatus = dr["Estatus"]?.ToString(),
                                Usuario = dr["Usuario"]?.ToString(),
                                FechaAlta = Convert.ToDateTime(dr["FechaAlta"]),
                                UltimaAct = Convert.ToDateTime(dr["UltimaAct"])
                            };
                        }
                    }
                }
            }
            return empleado;
        }

        public bool Guardar(Empleado empleado)
        {
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = @"INSERT INTO Empleados 
                    (NumeroEmpleado, Nombre, ApellidoPaterno, ApellidoMaterno, RFC, CURP, NSS, FechaNacimiento, FechaIngreso, 
                     SucursalID, Puesto, Departamento, TipoContrato, TipoJornada, PeriodicidadPago, 
                     SalarioDiario, SalarioMensual, SalarioDiarioIntegrado, 
                     Telefono, Email, Domicilio, CodigoPostal, Banco, CuentaBancaria, CLABE, 
                     Estatus, Usuario)
                    VALUES 
                    (@NumeroEmpleado, @Nombre, @ApellidoPaterno, @ApellidoMaterno, @RFC, @CURP, @NSS, @FechaNacimiento, @FechaIngreso,
                     @SucursalID, @Puesto, @Departamento, @TipoContrato, @TipoJornada, @PeriodicidadPago,
                     @SalarioDiario, @SalarioMensual, @SalarioDiarioIntegrado,
                     @Telefono, @Email, @Domicilio, @CodigoPostal, @Banco, @CuentaBancaria, @CLABE,
                     @Estatus, @Usuario)";
                
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NumeroEmpleado", empleado.NumeroEmpleado);
                    cmd.Parameters.AddWithValue("@Nombre", empleado.Nombre);
                    cmd.Parameters.AddWithValue("@ApellidoPaterno", empleado.ApellidoPaterno);
                    cmd.Parameters.AddWithValue("@ApellidoMaterno", empleado.ApellidoMaterno ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RFC", empleado.RFC);
                    cmd.Parameters.AddWithValue("@CURP", empleado.CURP);
                    cmd.Parameters.AddWithValue("@NSS", empleado.NSS ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaNacimiento", empleado.FechaNacimiento);
                    cmd.Parameters.AddWithValue("@FechaIngreso", empleado.FechaIngreso);
                    cmd.Parameters.AddWithValue("@SucursalID", empleado.SucursalID);
                    cmd.Parameters.AddWithValue("@Puesto", empleado.Puesto);
                    cmd.Parameters.AddWithValue("@Departamento", empleado.Departamento ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TipoContrato", empleado.TipoContrato);
                    cmd.Parameters.AddWithValue("@TipoJornada", empleado.TipoJornada);
                    cmd.Parameters.AddWithValue("@PeriodicidadPago", empleado.PeriodicidadPago);
                    cmd.Parameters.AddWithValue("@SalarioDiario", empleado.SalarioDiario);
                    cmd.Parameters.AddWithValue("@SalarioMensual", empleado.SalarioMensual);
                    cmd.Parameters.AddWithValue("@SalarioDiarioIntegrado", empleado.SalarioDiarioIntegrado ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Telefono", empleado.Telefono ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", empleado.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Domicilio", empleado.Domicilio ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CodigoPostal", empleado.CodigoPostal ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Banco", empleado.Banco ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CuentaBancaria", empleado.CuentaBancaria ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CLABE", empleado.CLABE ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estatus", empleado.Estatus);
                    cmd.Parameters.AddWithValue("@Usuario", empleado.Usuario);
                    
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Actualizar(Empleado empleado)
        {
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = @"UPDATE Empleados SET 
                    Nombre = @Nombre, ApellidoPaterno = @ApellidoPaterno, ApellidoMaterno = @ApellidoMaterno,
                    RFC = @RFC, CURP = @CURP, NSS = @NSS, FechaNacimiento = @FechaNacimiento, FechaIngreso = @FechaIngreso, FechaBaja = @FechaBaja,
                    SucursalID = @SucursalID, Puesto = @Puesto, Departamento = @Departamento,
                    TipoContrato = @TipoContrato, TipoJornada = @TipoJornada, PeriodicidadPago = @PeriodicidadPago,
                    SalarioDiario = @SalarioDiario, SalarioMensual = @SalarioMensual, SalarioDiarioIntegrado = @SalarioDiarioIntegrado,
                    Telefono = @Telefono, Email = @Email, Domicilio = @Domicilio, CodigoPostal = @CodigoPostal,
                    Banco = @Banco, CuentaBancaria = @CuentaBancaria, CLABE = @CLABE,
                    Estatus = @Estatus, UltimaAct = GETDATE()
                    WHERE EmpleadoID = @EmpleadoID";
                
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmpleadoID", empleado.EmpleadoID);
                    cmd.Parameters.AddWithValue("@Nombre", empleado.Nombre);
                    cmd.Parameters.AddWithValue("@ApellidoPaterno", empleado.ApellidoPaterno);
                    cmd.Parameters.AddWithValue("@ApellidoMaterno", empleado.ApellidoMaterno ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RFC", empleado.RFC);
                    cmd.Parameters.AddWithValue("@CURP", empleado.CURP);
                    cmd.Parameters.AddWithValue("@NSS", empleado.NSS ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaNacimiento", empleado.FechaNacimiento);
                    cmd.Parameters.AddWithValue("@FechaIngreso", empleado.FechaIngreso);
                    cmd.Parameters.AddWithValue("@FechaBaja", empleado.FechaBaja ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SucursalID", empleado.SucursalID);
                    cmd.Parameters.AddWithValue("@Puesto", empleado.Puesto);
                    cmd.Parameters.AddWithValue("@Departamento", empleado.Departamento ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TipoContrato", empleado.TipoContrato);
                    cmd.Parameters.AddWithValue("@TipoJornada", empleado.TipoJornada);
                    cmd.Parameters.AddWithValue("@PeriodicidadPago", empleado.PeriodicidadPago);
                    cmd.Parameters.AddWithValue("@SalarioDiario", empleado.SalarioDiario);
                    cmd.Parameters.AddWithValue("@SalarioMensual", empleado.SalarioMensual);
                    cmd.Parameters.AddWithValue("@SalarioDiarioIntegrado", empleado.SalarioDiarioIntegrado ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Telefono", empleado.Telefono ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", empleado.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Domicilio", empleado.Domicilio ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CodigoPostal", empleado.CodigoPostal ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Banco", empleado.Banco ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CuentaBancaria", empleado.CuentaBancaria ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CLABE", empleado.CLABE ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estatus", empleado.Estatus);
                    
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DarDeBaja(int empleadoId, DateTime fechaBaja, string usuario)
        {
            using (SqlConnection conn = new SqlConnection(Conexion.CN))
            {
                string query = @"UPDATE Empleados SET 
                    Estatus = 'BAJA', FechaBaja = @FechaBaja, UltimaAct = GETDATE()
                    WHERE EmpleadoID = @EmpleadoID";
                
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmpleadoID", empleadoId);
                    cmd.Parameters.AddWithValue("@FechaBaja", fechaBaja);
                    
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
