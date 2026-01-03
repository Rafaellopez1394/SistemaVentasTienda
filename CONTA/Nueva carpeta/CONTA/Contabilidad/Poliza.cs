using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;

namespace MobileDAL.Contabilidad
{
    #region Interfaz IPoliza
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface IPoliza : ISprocBase
    {
        DataSet Poliza_Select(string polizaid, string EmpresaID);

        DataSet Poliza_Select(string EmpresaID, int? año, int? mes, bool? pendiente, string tip_pol);

        DataSet Poliza_Select();

        DataSet Poliza_Select_PorFolio(string folio, string tippol, string EmpresaID, DateTime fechapol);

        DataSet Poliza_Select_PorConcepto(string concepto, string tippol, string EmpresaID, DateTime fechapol, bool Pendiente);
        DataSet Poliza_Select_General( string tippol, string EmpresaID, DateTime fechapol,string folio, bool Pendiente);
        DataSet ReporteCapturaPolizas(string polizaid, int Ingles);

        DataSet ReporteCapturaPolizasMasivo(string EmpresaID, DateTime FechaInicial, DateTime FechaFinal, string TipPol, int Ingles, string folioInicial, string folioFinal);

        int Poliza_Save(
        ref string polizaid,
        string empresaid,
        ref string folio,
        string tipPol,
        DateTime fechapol,
        string concepto,
        decimal importe,
        int estatus,
        DateTime fecha,
        string usuario,
        bool pendiente,
        bool pagoprogramado,
        ref int ultimaAct);
        int PolizaC_Save(
        ref string polizaid,
        string empresaid,
        string folio,
        string tipPol,
        DateTime fechapol,
        string concepto,
        decimal importe,
        int estatus,
        DateTime fecha,
        string usuario,
        bool pendiente,
        bool pagoprogramado,
        ref int ultimaAct,
        ref string folioR);
        // Poliza detalle
        int Polizasdetalle_Save(
        ref string polizadetalleid,
        string polizaid,
        string cuentaid,
        string tipMov,
        string concepto,
        decimal cantidad,
        decimal importe,
        int estatus,
        DateTime fecha,
        string usuario,
        string presupuestodetalleId,
        string inventariocostoid,
        ref int ultimaAct);

        int Polizasdetalle_Delete_PorPoliza(string polizaid);

        DataSet sp_ConsultarPolizaPorUUID(string UUID);
        DataSet ValidarTipoContabilidad(string EmpresaID);
    }

    #endregion //Interfaz IPoliza

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Poliza
    /// </summary>
    public class Poliza
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Poliza()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Polizas A partir de un DataRow
        /// </summary>
        private static Entity.Contabilidad.Poliza BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.Poliza elemento = new Entity.Contabilidad.Poliza();
            if (!Convert.IsDBNull(row["PolizaID"]))
            {
                elemento.Polizaid = row["PolizaID"].ToString();
            }
            if (!Convert.IsDBNull(row["EmpresaID"]))
            {
                elemento.EmpresaId = row["EmpresaID"].ToString();
            }
            if (!Convert.IsDBNull(row["Folio"]))
            {
                elemento.Folio = row["Folio"].ToString();
            }
            if (!Convert.IsDBNull(row["Tip_Pol"]))
            {
                elemento.TipPol = row["Tip_Pol"].ToString();
            }
            if (!Convert.IsDBNull(row["FechaPol"]))
            {
                elemento.Fechapol = DateTime.Parse(row["FechaPol"].ToString());
            }
            if (!Convert.IsDBNull(row["FechaPol"]))
            {
                elemento.Fec_pol = DateTime.Parse(row["FechaPol"].ToString()).ToShortDateString();
            }
            if (!Convert.IsDBNull(row["Concepto"]))
            {
                elemento.Concepto = row["Concepto"].ToString();
            }
            if (!Convert.IsDBNull(row["Importe"]))
            {
                elemento.Importe = decimal.Parse(row["Importe"].ToString());
            }
            if (!Convert.IsDBNull(row["Estatus"]))
            {
                elemento.Estatus = int.Parse(row["Estatus"].ToString());
            }
            if (!Convert.IsDBNull(row["Fecha"]))
            {
                elemento.Fecha = DateTime.Parse(row["Fecha"].ToString());
            }
            if (!Convert.IsDBNull(row["Usuario"]))
            {
                elemento.Usuario = row["Usuario"].ToString();
            }
            if (getChilds)
            {
                // Llenar lista detalle
                if (elemento.Polizaid != string.Empty)
                {
                    elemento.ListaPolizaDetalle = MobileDAL.Contabilidad.Polizasdetalle.TraerPolizasdetallePorPoliza(elemento.Polizaid);
                }
            }
            if (!Convert.IsDBNull(row["Pendiente"]))
            {
                elemento.Pendiente = bool.Parse(row["Pendiente"].ToString());
            }
            if (!Convert.IsDBNull(row["PagoProgramado"]))
            {
                elemento.Pagoprogramado = bool.Parse(row["PagoProgramado"].ToString());
            }
            elemento.UltimaAct = int.Parse(row["UltimaAct"].ToString());
            return elemento;
        }
        #endregion //Métodos Privados

        #region Métodos Públicos
        public static void Guardar(ref ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizas)
        {
            IPoliza proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IPoliza>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;
                string folio;

                foreach (Entity.Contabilidad.Poliza elemento in listaPolizas)
                {
                    codigo = elemento.Polizaid;
                    ultimaAct = elemento.UltimaAct;
                    folio = elemento.Folio == "0" ? "000000000" : elemento.Folio;

                    proc.Poliza_Save(
                    ref codigo,
                     (elemento.EmpresaId != null) ? elemento.EmpresaId : null,
                     ref folio,
                     elemento.TipPol,
                     elemento.Fechapol,
                     elemento.Concepto,
                     elemento.Importe,
                     elemento.Estatus,
                     elemento.Fecha,
                     elemento.Usuario,
                     elemento.Pendiente,
                     elemento.Pagoprogramado,
                    ref ultimaAct);
                    elemento.Polizaid = codigo;
                    elemento.UltimaAct = ultimaAct;
                    elemento.Folio = folio.ToString();

                    // Guarda polizas detalle
                    if (elemento.ListaPolizaDetalle != null && elemento.ListaPolizaDetalle.Count > 0)
                    {
                        string codigoDetalle;
                        int ultimaActDetalle;

                        // Elimina el detalle de polizas anterior si existe
                        proc.Polizasdetalle_Delete_PorPoliza(elemento.Polizaid);

                        // Guarda polizas detalle
                        foreach (Entity.Contabilidad.Polizasdetalle detalle in elemento.ListaPolizaDetalle)
                        {
                            codigoDetalle = detalle.Polizadetalleid;
                            ultimaActDetalle = detalle.UltimaAct;

                            proc.Polizasdetalle_Save(
                            ref codigoDetalle,
                             elemento.Polizaid,
                             (detalle.Cuentaid != null) ? detalle.Cuentaid : null,
                             detalle.TipMov,
                             (detalle.Concepto != null) ? detalle.Concepto : null,
                             detalle.Cantidad,
                             detalle.Importe,
                             detalle.Estatus,
                             detalle.Fecha,
                             detalle.Usuario,
                             detalle.PresupuestodetalleId != Guid.Empty.ToString() ? detalle.PresupuestodetalleId : null,
                             detalle.Inventariocostoid != Guid.Empty.ToString() ? detalle.Inventariocostoid : null,
                            ref ultimaActDetalle);
                            detalle.Polizadetalleid = codigoDetalle;
                            detalle.UltimaAct = ultimaActDetalle;
                        }
                    }
                }
                proc.Transaction.Commit();
                listaPolizas.AcceptChanges();
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

        public static void GuardarC(ref ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizas)
        {
            IPoliza proc = null;
            try
            {

                proc = Utilerias.GenerarSproc<IPoliza>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;
                string folio;
                string folioAnt;
                string folioR;

                foreach (Entity.Contabilidad.Poliza elemento in listaPolizas)
                {
                    codigo = elemento.Polizaid;
                    ultimaAct = elemento.UltimaAct;
                    folio = elemento.Folio == "0" ? "000000000" : elemento.Folio;
                    //folio = elemento.Folio == "0" ? "000000000" : elemento.Folio.PadLeft(9, '0'); // Forzar formato de 9 dígitos 
                    folioAnt = elemento.Folio;
                    folioR = elemento.Folio == "0" ? "000000000" : elemento.Folio.PadLeft(9, '0');// Forzar formato de 9 dígitos 

                    proc.PolizaC_Save(
                    ref codigo,
                     (elemento.EmpresaId != null) ? elemento.EmpresaId : null,
                     folio,
                     elemento.TipPol,
                     elemento.Fechapol,
                     elemento.Concepto,
                     elemento.Importe,
                     elemento.Estatus,
                     elemento.Fecha,
                     elemento.Usuario,
                     elemento.Pendiente,
                     elemento.Pagoprogramado,
                    ref ultimaAct,
                    ref folioR);
                    elemento.Polizaid = codigo;
                    elemento.UltimaAct = ultimaAct;
                    elemento.Folio = AjustarFolio(folioAnt, folioR.ToString());


                    // Guarda polizas detalle
                    if (elemento.ListaPolizaDetalle != null && elemento.ListaPolizaDetalle.Count > 0)
                    {
                        string codigoDetalle;
                        int ultimaActDetalle;

                        // Elimina el detalle de polizas anterior si existe
                        proc.Polizasdetalle_Delete_PorPoliza(elemento.Polizaid);

                        // Guarda polizas detalle
                        foreach (Entity.Contabilidad.Polizasdetalle detalle in elemento.ListaPolizaDetalle)
                        {
                            codigoDetalle = detalle.Polizadetalleid;
                            ultimaActDetalle = detalle.UltimaAct;

                            proc.Polizasdetalle_Save(
                            ref codigoDetalle,
                             elemento.Polizaid,
                             (detalle.Cuentaid != null) ? detalle.Cuentaid : null,
                             detalle.TipMov,
                             (detalle.Concepto != null) ? detalle.Concepto : null,
                             detalle.Cantidad,
                             detalle.Importe,
                             detalle.Estatus,
                             detalle.Fecha,
                             detalle.Usuario,
                             detalle.PresupuestodetalleId != Guid.Empty.ToString() ? detalle.PresupuestodetalleId : null,
                             detalle.Inventariocostoid != Guid.Empty.ToString() ? detalle.Inventariocostoid : null,
                            ref ultimaActDetalle);
                            detalle.Polizadetalleid = codigoDetalle;
                            detalle.UltimaAct = ultimaActDetalle;
                        }
                    }
                }
                proc.Transaction.Commit();
                listaPolizas.AcceptChanges();
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
        public static string AjustarFolio(string folioAnterior, string folioNuevo)
        {
            if (!string.IsNullOrEmpty(folioAnterior) && folioAnterior.StartsWith("0"))
            {
                if (!folioNuevo.StartsWith("0"))
                {
                    folioNuevo = "0" + folioNuevo;
                }
            }
            return folioNuevo;
        }


        public static List<Entity.Contabilidad.Poliza> TraerPolizasPorFiltros(string EmpresaID, int? año, int? mes, bool? pendiente, string tip_pol)
        {
            IPoliza proc = null;
            List<Entity.Contabilidad.Poliza> ListaPolizas = new List<Entity.Contabilidad.Poliza>();
            try
            {
                proc = Utilerias.GenerarSproc<IPoliza>();
                DataSet dsPolizas = proc.Poliza_Select(EmpresaID, año, mes, pendiente, tip_pol);
                foreach (DataRow row in dsPolizas.Tables[0].Rows)
                {
                    Entity.Contabilidad.Poliza elemento = BuildEntity(row, false);
                    string pol = elemento.Polizaid.ToLower();
                    if (elemento.TipPol != "IF")
                    {
                        int pagos = 0;
                        List<Entity.Operacion.Catfacturasproveedor> Facturas = MobileDAL.Operacion.Catfacturasproveedor.TraerFacturasProveedoresPorPolizaID(elemento.Polizaid);
                        List<Entity.Contabilidad.Polizasnomina> pn = MobileDAL.Contabilidad.Polizasnomina.TraerPolizasnominaPorPolizaID(elemento.Polizaid);
                        elemento.Tienefacturas = (Facturas.Count > 0 ? true : false);
                        elemento.TieneComplementoDeNomina = (pn == null || pn.Count == 0) ? 0 : 1;
                        foreach (Entity.Operacion.Catfacturasproveedor f in Facturas)
                        {
                            if (f.Metodopago == "PUE")
                                pagos += 1;
                            else
                            {
                                Entity.Operacion.Catfacturaspago pago;
                                pago = MobileDAL.Operacion.Catfacturaspago.TraerCatfacturaspagosPorUUID(f.Uuid);
                                if (pago == null)
                                {
                                     pago = MobileDAL.Operacion.Catfacturaspago.TraerCatFacturaPagoPorIdDocumento(f.Uuid);
                                }
                                
                                if (pago != null)
                                    pagos += 1;
                            }
                        }

                        if (Facturas.Count > 0)
                        {
                            if (pagos == Facturas.Count)
                                elemento.TieneComplemento = 1;
                            else
                                elemento.TieneComplemento = 2;
                        }
                        else
                            elemento.TieneComplemento = 0;
                    }
                    else
                    {
                        Entity.ModeloFacturaPoliza mp = MobileDAL.Operacion.Catfacturaselectronica.TraerFacturasPorPoliza(elemento.Polizaid);                        
                        elemento.Tienefacturas = (mp != null ? true : false);
                        elemento.TieneComplemento = 0;                        
                    }

                    ListaPolizas.Add(elemento);
                }
                return ListaPolizas;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Poliza TraerPolizas(string polizaid)
        {
            IPoliza proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IPoliza>();
                Entity.Contabilidad.Poliza elemento = null;
                DataSet ds = null;
                ds = proc.Poliza_Select(polizaid, null);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    elemento = BuildEntity(row, true);
                    elemento.AcceptChanges();
                }
                return elemento;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Entity.Contabilidad.Poliza TraerPolizaPorFolio(string folio, string tippol, string empresaid, DateTime fechapol)
        {
            IPoliza proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IPoliza>();
                Entity.Contabilidad.Poliza elemento = null;
                DataSet ds = null;
                ds = proc.Poliza_Select_PorFolio(folio, tippol, empresaid, fechapol);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    elemento = BuildEntity(row, true);
                    elemento.AcceptChanges();
                }
                return elemento;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static ListaDeEntidades<Entity.Contabilidad.Poliza> TraerPolizasPorDescripcion(string descripcion, string tippol, string empresaid, DateTime fechapol, bool Pendiente)
        {
            IPoliza proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizas = new ListaDeEntidades<Entity.Contabilidad.Poliza>();
                proc = Utilerias.GenerarSproc<IPoliza>();
                DataSet dsPolizas = proc.Poliza_Select_PorConcepto(descripcion, tippol, empresaid, fechapol, Pendiente);
                foreach (DataRow row in dsPolizas.Tables[0].Rows)
                {
                    Entity.Contabilidad.Poliza elemento = BuildEntity(row, false);
                    listaPolizas.Add(elemento);
                }
                listaPolizas.AcceptChanges();
                return listaPolizas;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerPolizasDS()
        {
            IPoliza proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IPoliza>();
                DataSet ds = proc.Poliza_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerDatosReportePolizas(string polizaid, int Ingles)
        {
            IPoliza proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IPoliza>();
                DataSet ds = proc.ReporteCapturaPolizas(polizaid, Ingles);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static System.Data.DataSet TraerTipoContabilidad(string EmpresaID)
        {
            IPoliza proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IPoliza>();
                DataSet ds = proc.ValidarTipoContabilidad(EmpresaID);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static DataSet ReporteCapturaPolizasMasivo(string EmpresaID, DateTime FechaInicial, DateTime FechaFinal, string TipPol, int Ingles, string folioInicial, string folioFinal)
        {
            IPoliza proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IPoliza>();
                DataSet ds = proc.ReporteCapturaPolizasMasivo(EmpresaID, FechaInicial, FechaFinal, TipPol, Ingles, folioInicial, folioFinal);
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static void GuardarBLT(ref ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizas)
        {
            IPoliza proc = null;
            try
            {
                proc = Utilerias.GenerarSprocBLT<IPoliza>(false);
                proc.Transaction = proc.Connection.BeginTransaction();

                string codigo;
                int ultimaAct;
                string folio;

                foreach (Entity.Contabilidad.Poliza elemento in listaPolizas)
                {
                    codigo = elemento.Polizaid;
                    ultimaAct = elemento.UltimaAct;
                    folio = elemento.Folio == "0" ? "000000000" : elemento.Folio;

                    proc.Poliza_Save(
                    ref codigo,
                     (elemento.EmpresaId != null) ? elemento.EmpresaId : null,
                     ref folio,
                     elemento.TipPol,
                     elemento.Fechapol,
                     elemento.Concepto,
                     elemento.Importe,
                     elemento.Estatus,
                     elemento.Fecha,
                     elemento.Usuario,
                     elemento.Pendiente,
                     elemento.Pagoprogramado,
                    ref ultimaAct);
                    elemento.Polizaid = codigo;
                    elemento.UltimaAct = ultimaAct;
                    elemento.Folio = folio.ToString();

                    // Guarda polizas detalle
                    if (elemento.ListaPolizaDetalle != null && elemento.ListaPolizaDetalle.Count > 0)
                    {
                        string codigoDetalle;
                        int ultimaActDetalle;

                        // Elimina el detalle de polizas anterior si existe
                        proc.Polizasdetalle_Delete_PorPoliza(elemento.Polizaid);

                        // Guarda polizas detalle
                        foreach (Entity.Contabilidad.Polizasdetalle detalle in elemento.ListaPolizaDetalle)
                        {
                            codigoDetalle = detalle.Polizadetalleid;
                            ultimaActDetalle = detalle.UltimaAct;

                            proc.Polizasdetalle_Save(
                            ref codigoDetalle,
                             elemento.Polizaid,
                             (detalle.Cuentaid != null) ? detalle.Cuentaid : null,
                             detalle.TipMov,
                             (detalle.Concepto != null) ? detalle.Concepto : null,
                             detalle.Cantidad,
                             detalle.Importe,
                             detalle.Estatus,
                             detalle.Fecha,
                             detalle.Usuario,
                             detalle.PresupuestodetalleId != Guid.Empty.ToString() ? detalle.PresupuestodetalleId : null,
                             detalle.Inventariocostoid != Guid.Empty.ToString() ? detalle.Inventariocostoid : null,
                            ref ultimaActDetalle);
                            detalle.Polizadetalleid = codigoDetalle;
                            detalle.UltimaAct = ultimaActDetalle;
                        }
                    }
                }
                proc.Transaction.Commit();
                listaPolizas.AcceptChanges();
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
        public static ListaDeEntidades<Entity.Contabilidad.Poliza> TraerPolizasGenerales( string tippol, string empresaid, DateTime fechapol,string folio, bool Pendiente)
        {
            IPoliza proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizas = new ListaDeEntidades<Entity.Contabilidad.Poliza>();
                proc = Utilerias.GenerarSproc<IPoliza>();
                DataSet dsPolizas = proc.Poliza_Select_General(tippol, empresaid, fechapol,folio, Pendiente);
                foreach (DataRow row in dsPolizas.Tables[0].Rows)
                {
                    Entity.Contabilidad.Poliza elemento = BuildEntity(row, false);
                    listaPolizas.Add(elemento);
                }
                listaPolizas.AcceptChanges();
                return listaPolizas;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //ReporteCapturaPolizasMasivo
        #endregion Métodos Públicos

        public static DataSet TraerPolizasPorUUID(string UUID)
        {
            IPoliza proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IPoliza>();
                DataSet dsPolizas = proc.sp_ConsultarPolizaPorUUID(UUID);
                return dsPolizas;
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
    }
}
