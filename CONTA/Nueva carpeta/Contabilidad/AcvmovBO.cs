using System;
using Entity;
using System.Collections.Generic;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Acvmov
    /// </summary>
    internal class AcvmovBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public AcvmovBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public void GuardarAcvmov(ListaDeEntidades<Entity.Contabilidad.Acvmov> listaAcvmov)
        {
            MobileDAL.Contabilidad.Acvmov.Guardar(ref listaAcvmov);
        }

        public Entity.Contabilidad.Acvmov TraerAcvmov(string acvmovid)
        {
            return MobileDAL.Contabilidad.Acvmov.TraerAcvmov(acvmovid);
        }

        public ListaDeEntidades<Entity.Contabilidad.Acvmov> TraerAcvmov()
        {
            return MobileDAL.Contabilidad.Acvmov.TraerAcvmov();
        }

        public System.Data.DataSet TraerAcvmovDS()
        {
            return MobileDAL.Contabilidad.Acvmov.TraerAcvmovDS();
        }

        public static ListaDeEntidades<Entity.Contabilidad.Acvmov> TraerAcvmovPorAcvGral(string acvgralid)
        {
            return MobileDAL.Contabilidad.Acvmov.TraerAcvmovPorAcvGral(acvgralid);
        }

        public static System.Data.DataSet TraerFolioMaximoPorTipoPoliza(string tippol, int empresa, DateTime fecha)
        {
            return MobileDAL.Contabilidad.Acvmov.TraerFolioMaximoPorTipoPoliza(tippol, empresa, fecha);
        }

        public static System.Data.DataSet VerificarFolioPoliza(string NumPol, string TipPol, DateTime FecPol, string EmpresaID, bool Pendiente)
        {
            return MobileDAL.Contabilidad.Acvmov.VerificarFolioPoliza(NumPol, TipPol, FecPol, EmpresaID, Pendiente);
        }
        public static System.Data.DataSet TraerGatosDelReporteCostos(DateTime fechainicio, DateTime fechafin, string empresaid)
        {
            return MobileDAL.Contabilidad.Acvmov.TraerGatosDelReporteCostos(fechainicio, fechafin, empresaid);
        }

        public static System.Data.DataSet CancelarCuentasResultado(int anio, string empresaid)
        {
            return MobileDAL.Contabilidad.Acvmov.CancelarCuentasResultado(anio, empresaid);
        }

        public static List<Entity.Contabilidad.Acvmov> TraerAcvmovPorCuentaYReferencia(string cuenta, string refer)
        {
            return MobileDAL.Contabilidad.Acvmov.TraerAcvmovPorCuentaYReferencia(cuenta, refer);
        }

        public static System.Data.DataSet ReporteContabilidadExcelDS(string EmpresaId, DateTime FechaInicio, DateTime FechaFin)
        {
            return MobileDAL.Contabilidad.Acvmov.ReporteContabilidadExcelDS(EmpresaId,FechaInicio,FechaFin);
        }

        public static List<Entity.Contabilidad.Acvmov> TraerAcvmovPorCesion(string concepto, string cuenta, string empresaid)
        {
            return MobileDAL.Contabilidad.Acvmov.TraerAcvmovPorCesion(concepto, cuenta, empresaid);
        }

        public static System.Data.DataSet TraeGastos(DateTime fecha, string empresaid)
        {
            return MobileDAL.Contabilidad.Acvmov.TraeGastos(fecha, empresaid);
        }
        public static System.Data.DataSet TraerFolioMaximoPorTipoPolizaBLT(string tippol, int empresa, DateTime fecha)
        {
            return MobileDAL.Contabilidad.Acvmov.TraerFolioMaximoPorTipoPolizaBLT(tippol, empresa, fecha);
        }
        public static System.Data.DataSet ConsultarCatDocumentosPolizas()
        {
            return MobileDAL.Contabilidad.Acvmov.ConsultarCatDocumentosPolizas();
        }
        #endregion //Métodos Públicos

    }
}
