using System;
using System.Collections.Generic;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Documentosadicionalespoliza
    /// </summary>
    internal class DocumentosAdicionalesPolizasBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public DocumentosAdicionalesPolizasBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarDocumentosadicionalespoliza(List<Entity.Contabilidad.DocumentosAdicionalesPolizas> listaDocumentosadicionalespoliza)
        {
            MobileDAL.Contabilidad.DocumentosAdicionalesPolizas.Guardar(ref listaDocumentosadicionalespoliza);
        }
        public static void GuardarDocumentosadicionalespolizaBLT(List<Entity.Contabilidad.DocumentosAdicionalesPolizas> listaDocumentosadicionalespoliza)
        {
            MobileDAL.Contabilidad.DocumentosAdicionalesPolizas.GuardarBLT(ref listaDocumentosadicionalespoliza);
        }
        public static Entity.Contabilidad.DocumentosAdicionalesPolizas TraerDocumentosAdicionalesPolizasuuid(string uuid)
        {
            return MobileDAL.Contabilidad.DocumentosAdicionalesPolizas.TraerDocumentosAdicionalesPolizasuuid(uuid);
        }
        public static Entity.Contabilidad.DocumentosAdicionalesPolizas TraerDocumentosAdicionalesPolizasuuidBLT(string uuid)
        {
            return MobileDAL.Contabilidad.DocumentosAdicionalesPolizas.TraerDocumentosAdicionalesPolizasuuidBLT(uuid);
        }

        public static List<Entity.Contabilidad.DocumentosAdicionalesPolizas> TraerDocumentosadicionalespolizas(string PolizaID)
        {
            return MobileDAL.Contabilidad.DocumentosAdicionalesPolizas.TraerDocumentosAdicionalesPolizas(PolizaID);
        }

        public static System.Data.DataSet TraerDocumentosadicionalespolizasDS()
        {
            return MobileDAL.Contabilidad.DocumentosAdicionalesPolizas.TraerDocumentosAdicionalesPolizasDS();
        }
        #endregion //Métodos Públicos

    }
}
