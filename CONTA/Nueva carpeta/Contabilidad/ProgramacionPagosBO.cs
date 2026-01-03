using System;
using System.Collections.Generic;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Programacionpago
    /// </summary>
    internal class ProgramacionpagoBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ProgramacionpagoBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarProgramacionpago(List<Entity.Contabilidad.Programacionpago> listaProgramacionpago)
        {
            MobileDAL.Contabilidad.Programacionpago.Guardar(ref listaProgramacionpago);
        }

        public static Entity.Contabilidad.Programacionpago TraerProgramacionpagos(string empresaid, string programacionpagoid)
        {
            return MobileDAL.Contabilidad.Programacionpago.TraerProgramacionpagos(empresaid, programacionpagoid);
        }

        public static List<Entity.Contabilidad.Programacionpago> TraerProgramacionpagos()
        {
            return MobileDAL.Contabilidad.Programacionpago.TraerProgramacionpagos();
        }

        public static List<Entity.Contabilidad.Programacionpago> TraerProgramacionpagosPorProveedorID(string empresaid, string proveedorid)
        {
            return MobileDAL.Contabilidad.Programacionpago.TraerProgramacionpagosPorProveedorID(empresaid, proveedorid);
        }

        public static List<Entity.Contabilidad.Programacionpago> TraerProgramacionpagosPorSolicitanteID(string empresaid, string solicitanteid)
        {
            return MobileDAL.Contabilidad.Programacionpago.TraerProgramacionpagosPorSolicitanteID(empresaid, solicitanteid);
        }

        public static Entity.Contabilidad.Programacionpago TraerProgramacionpagosPorPolizaID(string polizaid)
        {
            return MobileDAL.Contabilidad.Programacionpago.TraerProgramacionpagosPorPolizaID(polizaid);
        }

        public static System.Data.DataSet TraerProgramacionpagosDS()
        {
            return MobileDAL.Contabilidad.Programacionpago.TraerProgramacionpagosDS();
        }

        public static System.Data.DataSet TraerDatosProgramacionPagos(string empresaid, DateTime? fi, DateTime? ff)
        {
            return MobileDAL.Contabilidad.Programacionpago.TraerDatosProgramacionPagos(empresaid, fi, ff);
        }

        public static System.Data.DataSet TraerListadoPagosProgramados(string empresaid)
        {
            return MobileDAL.Contabilidad.Programacionpago.TraerListadoPagosProgramados(empresaid);
        }

        public static System.Data.DataSet TraerRelacionPagosProgramados(DateTime fechaInicio, DateTime fechaFin, string empresaid)
        {
            return MobileDAL.Contabilidad.Programacionpago.TraerRelacionPagosProgramados(fechaInicio, fechaFin, empresaid);
        }

        public static System.Data.DataSet ValidaPolizaPagoProgramado(string empresaid, string polizaid)
        {
            return MobileDAL.Contabilidad.Programacionpago.ValidaPolizaPagoProgramado(empresaid, polizaid);
        }
        #endregion //Métodos Públicos

    }
}
