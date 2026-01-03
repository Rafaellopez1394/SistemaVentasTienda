using AutoSproc;
using System.Data;
using System.Data.SqlClient;
using System;
using Entity;
using System.Collections.Generic;

namespace MobileDAL.Contabilidad
{
    #region Interfaz GeneracionPLD
    /// <summary>
    /// Interfaz utilizada para utilizar AutoSproc como medio de almacenamiento y consulta a base de datos
    /// </summary>
    public interface IGeneracionPLD : ISprocBase
    {
        DataSet ConsultaCreditosPLD(string fechaInicio, string fechaFin);

        int Pldcredito_Save(
        int idcredito,
        int iddctepld,
        int Idfactoraje,
        string Rfc,
        string Tipocredito,
        DateTime Fechainicio);

        DataSet ConsultaCreditosPLD();

        DataSet ConsultaPagosPLD(string fechaInicio, string fechaFin);
        int ConsultaPagosPLD(
         string id_credito,
         string folio,
         string fecha,
         string id_instrumento_monetario,
         string banco,
         string cuenta,
         string referencia,
         string tipo_operacion,
         string tipo_moneda,
         string monto_total,
         string observaciones,
         string transaccion,
         string pais,
         string tipo_pago);
    }

    #endregion 

    /// <summary>
    /// Clase utilizada para controlar el acceso a datos así como la persistencia de Acvctam
    /// </summary>
    public class GeneracionPLD
    {
        #region Atributos
        #endregion //Atributos

        #region Constructor, Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public GeneracionPLD()
        {
        }

        #endregion //Constructor, Destructor

        #region Métodos Públicos

        public static System.Data.DataSet TraerAcvctamDS()
        {
            IAcvctam proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IAcvctam>();
                DataSet ds = proc.Acvctam_Select();
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static ListaDeEntidades<Entity.Contabilidad.GeneracionPLD> GenerarListaClientesPLD(string fechaInicio, string fechaFin)
        {
            IGeneracionPLD proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.GeneracionPLD> ListaClientesPLD = new ListaDeEntidades<Entity.Contabilidad.GeneracionPLD>();
                proc = Utilerias.GenerarSproc<IGeneracionPLD>();
                DataSet dsAcvctam = proc.ConsultaCreditosPLD(fechaInicio,fechaFin);
                foreach (DataRow row in dsAcvctam.Tables[0].Rows)
                {
                    Entity.Contabilidad.GeneracionPLD elemento = BuildEntity(row, true);
                    ListaClientesPLD.Add(elemento);
                }
                ListaClientesPLD.AcceptChanges();
                return ListaClientesPLD;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
      
        public static string guardarPagos(ref List<Entity.Contabilidad.Creditos> datosExtraidos)
        {
            IGeneracionPLD proc = null;
            try
            {
                proc = Utilerias.GenerarSproc<IGeneracionPLD>(false);
                proc.Transaction = proc.Connection.BeginTransaction();
                

                foreach (Entity.Contabilidad.Creditos elemento in datosExtraidos)
                { 
                    proc.Pldcredito_Save(
                        elemento.IdCredito,
                        elemento.IdCliente,
                        elemento.NoCredito,
                        elemento.RFC,
                        elemento.TipoDeCredito,
                        elemento.FechaDeInicio
                    );
                }
                proc.Transaction.Commit();
            }
            catch (Exception ex)
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
            return "Se cargo correctamente";
        }

        public static ListaDeEntidades<Entity.Contabilidad.PagosCreditosPLD> GenerarListapagosClientesPLD(string fechaInicio, string fechaFin)
        {
            IGeneracionPLD proc = null;
            try
            {
                ListaDeEntidades<Entity.Contabilidad.PagosCreditosPLD> ListaClientespagosPLD = new ListaDeEntidades<Entity.Contabilidad.PagosCreditosPLD>();
                proc = Utilerias.GenerarSproc<IGeneracionPLD>();
                DataSet dspagos = proc.ConsultaPagosPLD(fechaInicio, fechaFin);
                foreach (DataRow row in dspagos.Tables[0].Rows)
                {
                    Entity.Contabilidad.PagosCreditosPLD elemento = BuildEntity2(row, true);
                    ListaClientespagosPLD.Add(elemento);
                }
                ListaClientespagosPLD.AcceptChanges();
                return ListaClientespagosPLD;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion
        #region Métodos Privados
        /// <summary>
        /// Construye una Entidad de tipo Acvctam A partir de un DataRow
        /// </summary>
        /// 
        private static Entity.Contabilidad.GeneracionPLD BuildEntity(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.GeneracionPLD elemento = new Entity.Contabilidad.GeneracionPLD();
            if (!Convert.IsDBNull(row["Id_cliente"]))
            {
                elemento.Id_cliente = row["id_cliente"].ToString();
            }

            if (!Convert.IsDBNull(row["fecha_inicio"]))
            {
                elemento.Fecha_inicio = row["fecha_inicio"].ToString();
            }

            if (!Convert.IsDBNull(row["importe_inicial"]))
            {
                elemento.Importe_inicial = row["importe_inicial"].ToString();
            }

            if (!Convert.IsDBNull(row["plazos"]))
            {
                elemento.Plazos = row["plazos"].ToString();
            }

            if (!Convert.IsDBNull(row["periodicidad"]))
            {
                elemento.Periodicidad = row["periodicidad"].ToString();
            }

            if (!Convert.IsDBNull(row["tasa_anual"]))
            {
                elemento.Tasa_anual = row["tasa_anual"].ToString();
            }

            if (!Convert.IsDBNull(row["comision"]))
            {
                elemento.Comision = row["comision"].ToString();
            }

            if (!Convert.IsDBNull(row["tipo_credito"]))
            {
                elemento.Tipo_credito = row["tipo_credito"].ToString();
            }

            if (!Convert.IsDBNull(row["no_credito"]))
            {
                elemento.No_credito = row["no_credito"].ToString();
            }

            if (!Convert.IsDBNull(row["tipo_moneda"]))
            {
                elemento.Tipo_moneda = row["tipo_moneda"].ToString();
            }

            if (!Convert.IsDBNull(row["instrumentos_monetarios"]))
            {
                elemento.Instrumentos_monetarios = row["instrumentos_monetarios"].ToString();
            }

            if (!Convert.IsDBNull(row["iva"]))
            {
                elemento.Iva = row["iva"].ToString();
            }

            if (!Convert.IsDBNull(row["origen"]))
            {
                elemento.Origen = row["origen"].ToString();
            }

            if (!Convert.IsDBNull(row["observaciones"]))
            {
                elemento.Observaciones = row["observaciones"].ToString();
            }

            if (!Convert.IsDBNull(row["tipo_calculo"]))
            {
                elemento.Tipo_calculo = row["tipo_calculo"].ToString();
            }

            if (!Convert.IsDBNull(row["convenio"]))
            {
                elemento.Convenio = row["convenio"].ToString();
            }

            if (!Convert.IsDBNull(row["fecha_inicio_pago"]))
            {
                elemento.Fecha_inicio_pago = row["fecha_inicio_pago"].ToString();
            }

            if (!Convert.IsDBNull(row["tasa_moratorio_anual"]))
            {
                elemento.Tasa_moratorio_anual = row["tasa_moratorio_anual"].ToString();
            }

            if (!Convert.IsDBNull(row["comision_disposicion"]))
            {
                elemento.Comision_disposicion = row["comision_disposicion"].ToString();
            }

            if (!Convert.IsDBNull(row["comision_prepago"]))
            {
                elemento.Comision_prepago = row["comision_prepago"].ToString();
            }

            if (!Convert.IsDBNull(row["comision_operacion"]))
            {
                elemento.Comision_operacion = row["comision_operacion"].ToString();
            }

            if (!Convert.IsDBNull(row["importe_seguro"]))
            {
                elemento.Importe_seguro = row["importe_seguro"].ToString();
            }

            if (!Convert.IsDBNull(row["importe_gps"]))
            {
                elemento.Importe_gps = row["importe_gps"].ToString();
            }

            if (!Convert.IsDBNull(row["importe_comision"]))
            {
                elemento.Importe_comision = row["importe_comision"].ToString();
            }

            if (!Convert.IsDBNull(row["importe_residual"]))
            {
                elemento.Importe_residual = row["importe_residual"].ToString();
            }

            if (!Convert.IsDBNull(row["seguro_informativo"]))
            {
                elemento.Seguro_informativo = row["seguro_informativo"].ToString();
            }

            if (!Convert.IsDBNull(row["incluye_gps_importe"]))
            {
                elemento.Incluye_gps_importe = row["incluye_gps_importe"].ToString();
            }

            if (!Convert.IsDBNull(row["importe_enganche"]))
            {
                elemento.Importe_enganche = row["importe_enganche"].ToString();
            }

            if (!Convert.IsDBNull(row["cat"]))
            {
                elemento.Cat = row["cat"].ToString();
            }
           
            return elemento;
        }

        private static Entity.Contabilidad.PagosCreditosPLD BuildEntity2(DataRow row, bool getChilds)
        {
            Entity.Contabilidad.PagosCreditosPLD elemento = new Entity.Contabilidad.PagosCreditosPLD();
           
            if (!Convert.IsDBNull(row["id_credito"]))
            {
                elemento.Id_credito = row["id_credito"].ToString();
            }
            if (!Convert.IsDBNull(row["Folio"]))
            {
                elemento.Folio = row["Folio"].ToString();
            }
            if (!Convert.IsDBNull(row["fecha"]))
            {
                elemento.Fecha = row["fecha"].ToString();
            }
            if (!Convert.IsDBNull(row["id_instrumento_monetario"]))
            {
                elemento.Id_instrumento_monetario = row["id_instrumento_monetario"].ToString();
            }
            if (!Convert.IsDBNull(row["banco"]))
            {
                elemento.Banco = row["banco"].ToString();
            }
            if (!Convert.IsDBNull(row["cuenta"]))
            {
                elemento.Cuenta = row["cuenta"].ToString();
            }
            if (!Convert.IsDBNull(row["referencia"]))
            {
                elemento.Referencia = row["referencia"].ToString();
            }
            if (!Convert.IsDBNull(row["tipo_operacion"]))
            {
                elemento.Tipo_operacion = row["tipo_operacion"].ToString();
            }
            if (!Convert.IsDBNull(row["tipo_moneda"]))
            {
                elemento.Tipo_moneda = row["tipo_moneda"].ToString();
            }
            if (!Convert.IsDBNull(row["monto_total"]))
            {
                elemento.Monto_total = row["monto_total"].ToString();
            }
            if (!Convert.IsDBNull(row["observaciones"]))
            {
                elemento.Observaciones = row["observaciones"].ToString();
            }
            if (!Convert.IsDBNull(row["transaccion"]))
            {
                elemento.Transaccion = row["transaccion"].ToString();
            }
            if (!Convert.IsDBNull(row["pais"]))
            {
                elemento.Pais = row["pais"].ToString();
            }
            if (!Convert.IsDBNull(row["tipo_pago"]))
            {
                elemento.Tipo_pago = row["tipo_pago"].ToString();
            }


            return elemento;
        }
        #endregion //Métodos Privados
    }
}
