using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;

namespace MobileDAL.Contabilidad
{
    #region Interfaz ICatcuenta
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface ICatcuenta : ISprocBase
    {
        DataSet TraerCatCuentasSat(string EmpresaID, DateTime FechaCorte);

        DataSet CatCuenta_Select(string cuentaid, string EmpresaID, string Cuenta);

        DataSet CatCuenta_Select(string cuentaid);

        DataSet CatCuenta_Select();

        DataSet CatCuentaAfecta_Select(string empresaid, string Cuenta);

        DataSet Catcuenta_Select_PorCuenta(string cuenta, string empresaid);

        DataSet Catcuenta_Select_PorCuenta(string cuenta, string nivel1, string nivel2,string empresaid);

        DataSet TraerDatosCuentaContable(string CuentaID);

        DataSet Catcuenta_Select_PorDescripcion(string descripcion, string empresaid);
        DataSet Catcuenta_Select_PorDescripcion(string descripcion, string nivel1, string nivel2, string empresaid);

        DataSet Catflujo_Select(string Flujo, string Descripcion);

        DataSet Catcuenta_Select_PorCuentaAfectable(string cuenta,string empresaid);

        DataSet Catcuenta_Select_PorDescripcionAfectable(string descripcion, string empresaid);

        DataSet ValidarExistenciaCuentas(string Cuenta, string Descripcion, int CodEmpresa);

        int CatCuenta_Save(
        ref string cuentaid,
        string empresaid,
        string codEmpresa,
        string cuenta,
        string descripcion,
        string descripcioningles,
        int nivel,
        bool afecta,
        bool sistema,
        bool ietu,
        bool isr,
        decimal saldo,
        string flujoCar,
        string flujoAbo,
        int estatus,
        DateTime fecha,
        string CtaSat,
        int moneda,
        ref int ultimaAct);


        

        // Saldos
        int Saldo_Save(
        ref string saldoid,
        string empresaid,
        string codEmpresa,
        string ejercicio,
        string cuentaid,
        string cuenta,
        int nivel,
        decimal sdoini,
        decimal car1,
        decimal car2,
        decimal car3,
        decimal car4,
        decimal car5,
        decimal car6,
        decimal car7,
        decimal car8,
        decimal car9,
        decimal car10,
        decimal car11,
        decimal car12,
        decimal abo1,
        decimal abo2,
        decimal abo3,
        decimal abo4,
        decimal abo5,
        decimal abo6,
        decimal abo7,
        decimal abo8,
        decimal abo9,
        decimal abo10,
        decimal abo11,
        decimal abo12,
        decimal sdoinia,
        decimal cara1,
        decimal cara2,
        decimal cara3,
        decimal cara4,
        decimal cara5,
        decimal cara6,
        decimal cara7,
        decimal cara8,
        decimal cara9,
        decimal cara10,
        decimal cara11,
        decimal cara12,
        decimal aboa1,
        decimal aboa2,
        decimal aboa3,
        decimal aboa4,
        decimal aboa5,
        decimal aboa6,
        decimal aboa7,
        decimal aboa8,
        decimal aboa9,
        decimal aboa10,
        decimal aboa11,
        decimal aboa12,
        ref int ultimaAct);

        DataSet Saldo_Select_PorCuentaEjercicio(string cuenta, string ejercicio, string empresaid);

        DataSet spcgenerainformedetallecuentas(string codempresa);

        DataSet TraerUltimaCuentaContable(string empresaid, string Cuenta);

        DataSet TraerPrimeraCuentaContable(string empresaid);


        int ValidarEjercioSeanCuentasFiscales(string EmpresaID, int Anio, ref bool SonFiscales);

        DataSet TraerCatCuentasPorEjercicio(string EmpresaID, int Anio);

        int ValidaCTASATPorEmpresaCuentaNivel(string EmpresaID, string Cuenta, int Nivel, ref string CTA);

        int SP_ValidarCuentasCliente(string codigocliente, int cod_empresa);
    }

    #endregion //Interfaz ICatcuenta

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Catcuenta
    /// </summary>
    public class Catcuenta
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Catcuenta()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Catcuentas A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Catcuenta BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Catcuenta elemento = new Entity.Contabilidad.Catcuenta();
            if (!Convert.IsDBNull(row["CuentaID"]))
            {
                elemento.Cuentaid = row["CuentaID"].ToString();
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.Empresaid = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["Cod_Empresa"]))
            {
                elemento.CodEmpresa = row["Cod_Empresa"].ToString();
            }
            if (!Convert.IsDBNull(row["Cuenta"]))
            {
                elemento.Cuenta = row["Cuenta"].ToString();
            }
            if (!Convert.IsDBNull(row["Descripcion"]))
            {
                elemento.Descripcion = row["Descripcion"].ToString();
            }
            if (!Convert.IsDBNull(row["DescripcionIngles"]))
            {
                elemento.Descripcioningles = row["DescripcionIngles"].ToString();
            }
            if (!Convert.IsDBNull(row["Nivel"]))
            {
                elemento.Nivel = int.Parse(row["Nivel"].ToString());
            }
            if (!Convert.IsDBNull(row["Afecta"]))
            {
                elemento.Afecta = bool.Parse(row["Afecta"].ToString());
            }
            if (!Convert.IsDBNull(row["Sistema"]))
            {
                elemento.Sistema = bool.Parse(row["Sistema"].ToString());
            }
            if (!Convert.IsDBNull(row["IETU"]))
            {
                elemento.Ietu = bool.Parse(row["IETU"].ToString());
            }
            if (!Convert.IsDBNull(row["ISR"]))
            {
                elemento.Isr = bool.Parse(row["ISR"].ToString());
            }
            if (!Convert.IsDBNull(row["Saldo"]))
            {
                elemento.Saldo = decimal.Parse(row["Saldo"].ToString());
            }
            if (!Convert.IsDBNull(row["Flujo_Car"]))
            {
                elemento.FlujoCar = row["Flujo_Car"].ToString();
            }
            if (!Convert.IsDBNull(row["Flujo_Abo"]))
            {
                elemento.FlujoAbo = row["Flujo_Abo"].ToString();
            }
            if (!Convert.IsDBNull(row["Estatus"]))
            {
                elemento.Estatus = int.Parse(row["Estatus"].ToString());
            }
            if (!Convert.IsDBNull(row["Fecha"]))
            {
                elemento.Fecha = DateTime.Parse(row["Fecha"].ToString());
            }
            if (!Convert.IsDBNull(row["CtaSat"]))
            {
                elemento.CtaSat = row["CtaSat"].ToString();
            }
            if (!Convert.IsDBNull(row["Moneda"]))
            {
                elemento.Moneda = int.Parse(row["Moneda"].ToString());
            }
            elemento.UltimaAct = int.Parse(row["UltimaAct"].ToString());
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref ListaDeEntidades<Entity.Contabilidad.Catcuenta> listaCatcuentas)
        {
            ICatcuenta proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuenta>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Catcuenta elemento in listaCatcuentas)
                {
                    codigo = (elemento.Cuentaid == string.Empty || elemento.Cuentaid == null || elemento.Cuentaid.ToUpper() == "NULL" || elemento.Cuentaid == "0" ? Guid.Empty.ToString() : elemento.Cuentaid);
                    ultimaAct = elemento.UltimaAct;

                    proc.CatCuenta_Save(
                    ref codigo,
                     (elemento.Empresaid != null) ? elemento.Empresaid : null,
                     (elemento.CodEmpresa != null) ? elemento.CodEmpresa : null,
                     (elemento.Cuenta != null) ? elemento.Cuenta : null,
                     (elemento.Descripcion != null) ? elemento.Descripcion : null,
                     (elemento.Descripcioningles != null) ? elemento.Descripcioningles : null,
                     (elemento.Nivel != null) ? elemento.Nivel : int.MinValue,
                     elemento.Afecta,
                     elemento.Sistema,
                     elemento.Ietu,
                     elemento.Isr,
                     (elemento.Saldo != null) ? elemento.Saldo : decimal.MinValue,
                     (elemento.FlujoCar != null) ? elemento.FlujoCar : null,
                     (elemento.FlujoAbo != null) ? elemento.FlujoAbo : null,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.CtaSat != null && elemento.CtaSat != string.Empty) ? elemento.CtaSat : string.Empty,
                     (elemento.Moneda != null)  ? elemento.Moneda : int.MinValue,
                    ref ultimaAct);
                    elemento.Cuentaid = codigo;
                    elemento.UltimaAct = ultimaAct;
                }
                proc.Transaction.Commit();
                listaCatcuentas.AcceptChanges();
            }
            catch (Exception)
            {
                if (proc != null)
                {
                    proc.Transaction.Rollback();
                }
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }

        public static ListaDeEntidades<Entity.Contabilidad.Catcuenta> TraerCatcuentas()
        {
            ICatcuenta proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Catcuenta> listaCatcuentas = new ListaDeEntidades<Entity.Contabilidad.Catcuenta>();
                proc = Utilerias.GenerarSproc<ICatcuenta>();
                DataSet dsCatcuentas = proc.CatCuenta_Select();
                foreach (DataRow row in dsCatcuentas.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catcuenta elemento = BuildEntity(row, true);
                    listaCatcuentas.Add(elemento);
                }
                listaCatcuentas.AcceptChanges();
                return listaCatcuentas;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }

        public static Entity.Contabilidad.Catcuenta TraerCatcuentas(string cuentaid, string empresaid, string cuenta)
        {
            ICatcuenta proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuenta>();
                Entity.Contabilidad.Catcuenta elemento = null;
                DataSet ds = null;
                ds = proc.CatCuenta_Select(cuentaid, empresaid, cuenta);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    elemento = BuildEntity(row, false);
                    elemento.AcceptChanges();
                }
                return elemento;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }

        public static Entity.Contabilidad.Catcuenta TraerCatCuentas(string cuentaid)
        {
            ICatcuenta proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuenta>(false);
                Entity.Contabilidad.Catcuenta elemento = null;
                DataSet ds = null;
                ds = proc.CatCuenta_Select(cuentaid);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    elemento = BuildEntity(row, false);
                    elemento.AcceptChanges();
                }
                return elemento;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }

        public static Entity.Contabilidad.Catcuenta TraerCatCuentasPorCuenta(string cuenta,string empresaid)
        {
            ICatcuenta proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuenta>(false);
                Entity.Contabilidad.Catcuenta elemento = null;
                DataSet ds = null;
                ds = proc.Catcuenta_Select_PorCuenta(cuenta, empresaid);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    elemento = BuildEntity(row, false);
                    elemento.AcceptChanges();
                }
                return elemento;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }

        public static Entity.Contabilidad.Catcuenta TraerCatCuentasPorCuenta(string cuenta, string nivel1, string nivel2,string empresaid)
        {
            ICatcuenta proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuenta>(false);
                Entity.Contabilidad.Catcuenta elemento = null;
                DataSet ds = null;
                ds = proc.Catcuenta_Select_PorCuenta(cuenta, nivel1, nivel2, empresaid);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    elemento = BuildEntity(row, false);
                    elemento.AcceptChanges();
                }
                return elemento;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }

        public static System.Data.EnumerableRowCollection<Entity.ModeloDatosCuentas> TraerDatosCuentaContable(string cuentaid)
        {
            ICatcuenta proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuenta>(false);
                DataSet ds = null;

                ds = proc.TraerDatosCuentaContable(cuentaid);
                return from a in ds.Tables[0].AsEnumerable()
                       select new Entity.ModeloDatosCuentas
                       {
                           CuentaID = a.Field<Guid>("CuentaID").ToString(),
                           AcvCtamID = a.Field<Guid>("AcvCtamID").ToString(),
                           Cuenta = a.Field<string>("Cuenta"),
                           Descripcion = a.Field<string>("Descripcion"),
                           DescripcionIngles = a.Field<string>("DescripcionIngles"),
                           Afecta = a.Field<bool>("Afecta"),
                           IETU = a.Field<bool>("IETU"),
                           ISR = a.Field<bool>("ISR"),
                           Sistema = a.Field<bool>("Sistema"),
                           Nat_Cta = a.Field<string>("Nat_Cta"),
                           Cod_Gpo = a.Field<string>("Cod_Gpo"),
                           Tipo_Cta = a.Field<string>("Tipo_Cta"),
                           FlujoIDCargo = a.Field<Guid>("FlujoIDCargo").ToString(),
                           Cod_FlujoCargo = a.Field<string>("Cod_FlujoCargo"),
                           DescripcionCargo = a.Field<string>("DescripcionCargo"),
                           FlujoIDAbono = a.Field<Guid>("FlujoIDAbono").ToString(),
                           Cod_FlujoAbono = a.Field<string>("Cod_FlujoAbono"),
                           DescripcionAbono = a.Field<string>("DescripcionAbono"),
                           Movimientos = a.Field<int>("Movimientos"),
                           MovimientosHijos = a.Field<int>("MovimientosHijos"),
                           Nivel = a.Field<int>("Nivel"),
                           UltimaActCuenta = a.Field<int>("UltimaActCuenta"),
                           UltimaActCuentaMayor = a.Field<int>("UltimaActCuentaMayor"),
                           CtaSat = a.Field<string>("CtaSat"),
                           DescripcionCtaSat = a.Field<string>("DescripcionCtaSat"),
                           Moneda = a.Field<int?>("Moneda") ?? 0,
            };
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }

        public static ListaDeEntidades<Entity.Contabilidad.Catcuenta> TraerCatCuentasPorDescripcion(string descripcion,string empresaid)
        {
            ICatcuenta proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Catcuenta> listaCatCuentas = new ListaDeEntidades<Entity.Contabilidad.Catcuenta>();
                proc = Utilerias.GenerarSproc<ICatcuenta>(false);
                DataSet dsCatCuentas = proc.Catcuenta_Select_PorDescripcion(descripcion, empresaid);
                foreach (DataRow row in dsCatCuentas.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catcuenta elemento = BuildEntity(row, true);
                    listaCatCuentas.Add(elemento);
                }
                listaCatCuentas.AcceptChanges();
                return listaCatCuentas;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }


        public static ListaDeEntidades<Entity.Contabilidad.Catcuenta> TraerCatCuentasPorDescripcion(string descripcion, string nivel1, string nivel2,string empresaid)
        {
            ICatcuenta proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Catcuenta> listaCatCuentas = new ListaDeEntidades<Entity.Contabilidad.Catcuenta>();
                proc = Utilerias.GenerarSproc<ICatcuenta>(false);
                DataSet dsCatCuentas = proc.Catcuenta_Select_PorDescripcion(descripcion, nivel1, nivel2, empresaid);
                foreach (DataRow row in dsCatCuentas.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catcuenta elemento = BuildEntity(row, true);
                    listaCatCuentas.Add(elemento);
                }
                listaCatCuentas.AcceptChanges();
                return listaCatCuentas;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }



        public static Entity.Contabilidad.Catcuenta TraerCatCuentasPorCuentaAfectable(string cuenta,string empresaid)
        {
            ICatcuenta proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuenta>(false);
                Entity.Contabilidad.Catcuenta elemento = null;
                DataSet ds = null;
                ds = proc.Catcuenta_Select_PorCuentaAfectable(cuenta, empresaid);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    elemento = BuildEntity(row, false);
                    elemento.AcceptChanges();
                }
                return elemento;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }

        public static ListaDeEntidades<Entity.Contabilidad.Catcuenta> TraerCatCuentasPorDescripcionAfectable(string descripcion,string empresaid)
        {
            ICatcuenta proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Catcuenta> listaCatCuentas = new ListaDeEntidades<Entity.Contabilidad.Catcuenta>();
                proc = Utilerias.GenerarSproc<ICatcuenta>(false);
                DataSet dsCatCuentas = proc.Catcuenta_Select_PorDescripcionAfectable(descripcion, empresaid);
                foreach (DataRow row in dsCatCuentas.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catcuenta elemento = BuildEntity(row, true);
                    listaCatCuentas.Add(elemento);
                }
                listaCatCuentas.AcceptChanges();
                return listaCatCuentas;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }

        public static System.Data.DataSet TraerCatcuentasDS()
        {
            ICatcuenta proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuenta>();
                DataSet ds = proc.CatCuenta_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }

        public static DataSet TraerFlujos(string flujo, string descripcion)
        {
            ICatcuenta proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuenta>(false);
                DataSet ds = proc.Catflujo_Select(flujo, descripcion);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }

        public static DataSet spcgenerainformedetallecuentas(string empresa)
        {
            ICatcuenta proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuenta>(false);
                DataSet ds = proc.spcgenerainformedetallecuentas(empresa);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }



        public static DataSet TraerUltimaCuentaContable(string empresaid, string Cuenta)
        {
            ICatcuenta proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuenta>(false);
                DataSet ds = proc.TraerUltimaCuentaContable(empresaid, Cuenta);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }


        public static DataSet TraerPrimeraCuentaContable(string empresaid)
        {
            ICatcuenta proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuenta>(false);
                DataSet ds = proc.TraerPrimeraCuentaContable(empresaid);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }

        public static DataSet TraerCuentaAfecta(string empresaid, string cuenta)
        {
            ICatcuenta proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuenta>(false);
                DataSet ds = proc.CatCuentaAfecta_Select(empresaid, cuenta);
                return ds;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }

        public static DataSet TraerCatCuentasSat(string empresaid, DateTime FechaCorte)
        {
            ICatcuenta proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuenta>();
                DataSet ds = proc.TraerCatCuentasSat(empresaid, FechaCorte);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }         
        }



        public static bool ValidaCuentasFiscales(string EmpresaID, int Anio)
        {
            ICatcuenta proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuenta>();
                
                bool SonFiscales = false;
                proc.ValidarEjercioSeanCuentasFiscales(EmpresaID, Anio, ref SonFiscales);
                return SonFiscales;
            }
            catch
            {
                throw;
            }
        }

        public static DataSet TraerCatCuentasPorEjercicio(string EmpresaID, int Anio)
        {
            ICatcuenta proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuenta>();
                DataSet cuentas = proc.TraerCatCuentasPorEjercicio(EmpresaID, Anio);
                return cuentas;
            }
            catch
            {
                throw;
            }
        }

        public static string ValidaCtaSATPorEmpresaCuentaNivel(string EmpresaID, string Cuenta, int Nivel)
        {
            ICatcuenta proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuenta>();
                string ctaSAT = "";
                proc.ValidaCTASATPorEmpresaCuentaNivel(EmpresaID, Cuenta, Nivel, ref ctaSAT);
                return ctaSAT;
            }
            catch
            {
                throw;
            }
        }


        public static List<Entity.Contabilidad.Catcuenta> TraerCatcuentasPorEmpresa(string empresaid)
        {
            ICatcuenta proc = null;
            try
            {
                List<Entity.Contabilidad.Catcuenta> listaCatcuentas = new List<Entity.Contabilidad.Catcuenta>();
                proc = Utilerias.GenerarSproc<ICatcuenta>();
                DataSet dsCatcuentas = proc.CatCuenta_Select(null, empresaid, null);
                foreach (DataRow row in dsCatcuentas.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catcuenta elemento = BuildEntity(row, true);
                    listaCatcuentas.Add(elemento);
                }                
                return listaCatcuentas;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }
        public static List<Entity.Contabilidad.Catcuenta> TraerCatcuentasPorEmpresaBLT(string empresaid)
        {
            ICatcuenta proc = null;
            try
            {
                List<Entity.Contabilidad.Catcuenta> listaCatcuentas = new List<Entity.Contabilidad.Catcuenta>();
                proc = Utilerias.GenerarSprocBLT<ICatcuenta>();

                DataSet dsCatcuentas = proc.CatCuenta_Select(null, empresaid, null);
                foreach (DataRow row in dsCatcuentas.Tables[0].Rows)
                {
                    Entity.Contabilidad.Catcuenta elemento = BuildEntity(row, true);
                    listaCatcuentas.Add(elemento);
                }
                return listaCatcuentas;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }
        public static bool ValidaCuentasFiscalesBLT(string EmpresaID, int Anio)
        {
            ICatcuenta proc = null;
            try
            {
                proc = Utilerias.GenerarSprocBLT<ICatcuenta>();
                bool SonFiscales = false;
                proc.ValidarEjercioSeanCuentasFiscales(EmpresaID, Anio, ref SonFiscales);
                return SonFiscales;
            }
            catch
            {
                throw;
            }
        }
        public static Entity.Contabilidad.Catcuenta TraerCatCuentasPorCuentaBLT(string cuenta, string empresaid)
        {
            ICatcuenta proc = null;
            try
            {
                proc = Utilerias.GenerarSprocBLT<ICatcuenta>(false);
                
                Entity.Contabilidad.Catcuenta elemento = null;
                DataSet ds = null;
                ds = proc.Catcuenta_Select_PorCuenta(cuenta, empresaid);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    elemento = BuildEntity(row, false);
                    elemento.AcceptChanges();
                }
                return elemento;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }
        

        public static List<Entity.Contabilidad.Respuesta> ValidarExistenciaCuentasBalor(string Cuenta,string Descripcion, int CodEmpresa)
        {
            ICatcuenta proc = null;
            try
            {

                proc = Utilerias.GenerarSproc<ICatcuenta>(false);
                List<Entity.Contabilidad.Respuesta> resultados = new List<Entity.Contabilidad.Respuesta>();
                DataSet ds = null;
                ds = proc.ValidarExistenciaCuentas(Cuenta, Descripcion, CodEmpresa);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        Entity.Contabilidad.Respuesta resultado = new Entity.Contabilidad.Respuesta();
                        resultado.Estado = Convert.ToInt32(row["Estado"]);
                        resultado.Mensaje = row["Mensaje"].ToString();
                        resultados.Add(resultado);
                    }
                }
                return resultados;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }
        public static List<Entity.Contabilidad.Respuesta> ValidarExistenciaCuentasBalorLandTrading(string Cuenta, string Descripcion, int CodEmpresa)
        {
            ICatcuenta proc = null;
            try
            {

                proc = Utilerias.GenerarSprocBLT<ICatcuenta>(false);
                List<Entity.Contabilidad.Respuesta> resultados = new List<Entity.Contabilidad.Respuesta>();
                DataSet ds = null;
                ds = proc.ValidarExistenciaCuentas(Cuenta, Descripcion, CodEmpresa);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        Entity.Contabilidad.Respuesta resultado = new Entity.Contabilidad.Respuesta();
                        resultado.Estado = Convert.ToInt32(row["Estado"]);
                        resultado.Mensaje = row["Mensaje"].ToString();
                        resultados.Add(resultado);
                    }
                }
                return resultados;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }
        public static void ValidarCuentasCliente(string codigocliente, int cod_empresa)
        {
            ICatcuenta proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<ICatcuenta>(false);
                proc.Transaction = proc.Connection.BeginTransaction();
                
                    proc.SP_ValidarCuentasCliente(
                     codigocliente,
                     cod_empresa);
                
                proc.Transaction.Commit();
            }
            catch (Exception)
            {
                if (proc != null)
                {
                    proc.Transaction.Rollback();
                }
                throw;
            }
            finally
            {
                if (proc != null)
                {
                    proc.Connection.Close();
                }
            }
        }
        #endregion Métodos Públicos
    }
}
