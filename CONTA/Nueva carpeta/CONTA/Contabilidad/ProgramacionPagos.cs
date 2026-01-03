using AutoSproc;
using System.Data;
using System.Collections.Generic;
using System;
using Entity;

namespace MobileDAL.Contabilidad
{
    #region Interfaz IProgramacionpago
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface IProgramacionpago : ISprocBase
    {
        DataSet Programacionpago_Select(string programacionpagoid, string empresaid, string proveedorid, string solicitanteid, string polizaid);

        DataSet Programacionpago_Select();

        int Programacionpago_Save(
        ref string programacionpagoid,
        string empresaid,
        string proveedorid,
        string factura,
        DateTime fechaprogramada,
        DateTime fechapagada,
        string concepto,
        decimal importe,
        string solicitanteid,
        string polizaid,
        string cuenta,
        DateTime fecha,
        string usuario,
        int estatus,
        ref int ultimaAct);

        DataSet TraerDatosProgramacionPagos(string empresaid, DateTime? fi, DateTime? ff);

        DataSet TraerListadoPagosProgramados(string empresaid);

        DataSet TraerRelacionPagosProgramados(DateTime FechaIni, DateTime FechaFin, string empresaid);

        DataSet TraerProgramacionPorPolizaID(string polizaid);

        DataSet ValidaPolizaPagoProgramado(string empresaid, string polizaid);
    }

    #endregion //Interfaz IProgramacionpago

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Programacionpago
    /// </summary>
    public class Programacionpago
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Programacionpago()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Programacionpagos A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Programacionpago BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Programacionpago elemento = new Entity.Contabilidad.Programacionpago();
            if (!Convert.IsDBNull(row["ProgramacionPagoID"]))
            {
                elemento.Programacionpagoid = row["ProgramacionPagoID"].ToString();
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.Empresaid = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["ProveedorID"]))
            {
                elemento.Proveedorid = row["ProveedorID"].ToString();
            }
            if (!Convert.IsDBNull(row["Factura"]))
            {
                elemento.Factura = row["Factura"].ToString();
            }
            if (!Convert.IsDBNull(row["FechaProgramada"]))
            {
                elemento.Fechaprogramada = DateTime.Parse(row["FechaProgramada"].ToString());
            }
            if (!Convert.IsDBNull(row["FechaPagada"]))
            {
                elemento.Fechapagada = DateTime.Parse(row["FechaPagada"].ToString());
            }
            if (!Convert.IsDBNull(row["Concepto"]))
            {
                elemento.Concepto = row["Concepto"].ToString();
            }
            if (!Convert.IsDBNull(row["Importe"]))
            {
                elemento.Importe = decimal.Parse(row["Importe"].ToString());
            }
            if (!Convert.IsDBNull(row["SolicitanteID"]))
            {
                elemento.Solicitanteid = row["SolicitanteID"].ToString();
            }
            if (!Convert.IsDBNull(row["PolizaID"]))
            {
                elemento.Polizaid = row["PolizaID"].ToString();
            }
            if (!Convert.IsDBNull(row["Cuenta"]))
            {
                elemento.Cuenta = row["Cuenta"].ToString();
            }
            if (!Convert.IsDBNull(row["Fecha"]))
            {
                elemento.Fecha = DateTime.Parse(row["Fecha"].ToString());
            }
            if (!Convert.IsDBNull(row["Usuario"]))
            {
                elemento.Usuario = row["Usuario"].ToString();
            }
            if (!Convert.IsDBNull(row["Estatus"]))
            {
                elemento.Estatus = int.Parse(row["Estatus"].ToString());
            }
            elemento.UltimaAct = int.Parse(row["UltimaAct"].ToString());
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref List<Entity.Contabilidad.Programacionpago> listaProgramacionpagos)
        {
            IProgramacionpago proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IProgramacionpago>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;

                foreach (Entity.Contabilidad.Programacionpago elemento in listaProgramacionpagos)
                {
                    codigo = elemento.Programacionpagoid;
                    ultimaAct = elemento.UltimaAct;

                    proc.Programacionpago_Save(
                    ref codigo,
                     (elemento.Empresaid != null) ? elemento.Empresaid : null,
                     (elemento.Proveedorid != null) ? elemento.Proveedorid : null,
                     (elemento.Factura != null) ? elemento.Factura : null,
                     (elemento.Fechaprogramada != null) ? elemento.Fechaprogramada : DateTime.MinValue,
                     (elemento.Fechapagada != null) ? elemento.Fechapagada : DateTime.MinValue,
                     (elemento.Concepto != null) ? elemento.Concepto : null,
                     (elemento.Importe != null) ? elemento.Importe : decimal.MinValue,
                     (elemento.Solicitanteid != null) ? elemento.Solicitanteid : null,
                     (elemento.Polizaid != null) ? elemento.Polizaid : null,
                     (elemento.Cuenta != null) ? elemento.Cuenta : null,
                     (elemento.Fecha != null) ? elemento.Fecha : DateTime.MinValue,
                     (elemento.Usuario != null) ? elemento.Usuario : null,
                     (elemento.Estatus != null) ? elemento.Estatus : int.MinValue,
                    ref ultimaAct);
                    elemento.Programacionpagoid = codigo;
                    elemento.UltimaAct = ultimaAct;
                }
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

        public static List<Entity.Contabilidad.Programacionpago> TraerProgramacionpagos()
        {
            IProgramacionpago proc = null;
            try
            {
                List<Entity.Contabilidad.Programacionpago> listaProgramacionpagos = new List<Entity.Contabilidad.Programacionpago>();
                proc = Utilerias.GenerarSproc<IProgramacionpago>();
                DataSet dsProgramacionpagos = proc.Programacionpago_Select();
                foreach (DataRow row in dsProgramacionpagos.Tables[0].Rows)
                {
                    Entity.Contabilidad.Programacionpago elemento = BuildEntity(row, true);
                    listaProgramacionpagos.Add(elemento);
                }
                return listaProgramacionpagos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Entity.Contabilidad.Programacionpago> TraerProgramacionpagosPorProveedorID(string empresaid, string proveedorid)
        {
            IProgramacionpago proc = null;
            try
            {
                List<Entity.Contabilidad.Programacionpago> listaProgramacionpagos = new List<Entity.Contabilidad.Programacionpago>();
                proc = Utilerias.GenerarSproc<IProgramacionpago>();
                DataSet dsProgramacionpagos = proc.Programacionpago_Select(null, empresaid, proveedorid, null, null);
                foreach (DataRow row in dsProgramacionpagos.Tables[0].Rows)
                {
                    Entity.Contabilidad.Programacionpago elemento = BuildEntity(row, true);
                    listaProgramacionpagos.Add(elemento);
                }
                return listaProgramacionpagos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Entity.Contabilidad.Programacionpago> TraerProgramacionpagosPorSolicitanteID(string empresaid, string solicitanteid)
        {
            IProgramacionpago proc = null;
            try
            {
                List<Entity.Contabilidad.Programacionpago> listaProgramacionpagos = new List<Entity.Contabilidad.Programacionpago>();
                proc = Utilerias.GenerarSproc<IProgramacionpago>();
                DataSet dsProgramacionpagos = proc.Programacionpago_Select(null, empresaid, null, solicitanteid, null);
                foreach (DataRow row in dsProgramacionpagos.Tables[0].Rows)
                {
                    Entity.Contabilidad.Programacionpago elemento = BuildEntity(row, true);
                    listaProgramacionpagos.Add(elemento);
                }
                return listaProgramacionpagos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Programacionpago TraerProgramacionpagosPorPolizaID(string polizaid)
        {
            IProgramacionpago proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IProgramacionpago>();
                Entity.Contabilidad.Programacionpago elemento = null;
                DataSet ds = null;
                ds = proc.TraerProgramacionPorPolizaID(polizaid);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    elemento = BuildEntity(row, false);
                }
                return elemento;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Programacionpago TraerProgramacionpagos(string empresaid, string programacionpagoid)
        {
            IProgramacionpago proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IProgramacionpago>();
                Entity.Contabilidad.Programacionpago elemento = null;
                DataSet ds = null;
                ds = proc.Programacionpago_Select(programacionpagoid, empresaid, null, null, null);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    elemento = BuildEntity(row, false);
                }
                return elemento;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerProgramacionpagosDS()
        {
            IProgramacionpago proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IProgramacionpago>();
                DataSet ds = proc.Programacionpago_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerDatosProgramacionPagos(string empresaid, DateTime? fi, DateTime? ff)
        {
            IProgramacionpago proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IProgramacionpago>();
                DataSet ds = proc.TraerDatosProgramacionPagos(empresaid, fi, ff);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerListadoPagosProgramados(string empresaid)
        {
            IProgramacionpago proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IProgramacionpago>();
                DataSet ds = proc.TraerListadoPagosProgramados(empresaid);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerRelacionPagosProgramados(DateTime fechaInicio, DateTime fechaFin, string empresaid)
        {
            IProgramacionpago proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IProgramacionpago>();
                DataSet ds = proc.TraerRelacionPagosProgramados(fechaInicio, fechaFin, empresaid);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet ValidaPolizaPagoProgramado(string empresaid, string polizaid)
        {
            IProgramacionpago proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IProgramacionpago>();
                DataSet ds = proc.ValidaPolizaPagoProgramado(empresaid, polizaid);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Métodos Públicos
    }
}
