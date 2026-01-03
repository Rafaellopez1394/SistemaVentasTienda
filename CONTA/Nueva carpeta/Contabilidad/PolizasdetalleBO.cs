using System;
using Entity;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Polizasdetalle
    /// </summary>
    internal class PolizasdetalleBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public PolizasdetalleBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public void GuardarPolizasdetalle(ListaDeEntidades<Entity.Contabilidad.Polizasdetalle> listaPolizasdetalle)
        {
            MobileDAL.Contabilidad.Polizasdetalle.Guardar(ref listaPolizasdetalle);
        }

        public void GuardarPorXML(string xmlDatos)
        {
            MobileDAL.Contabilidad.Polizasdetalle.GuardarPorXML(xmlDatos);
        }

        public Entity.Contabilidad.Polizasdetalle TraerPolizasdetalle(string polizadetalleid)
        {
            return MobileDAL.Contabilidad.Polizasdetalle.TraerPolizasdetalle(polizadetalleid);
        }

        public ListaDeEntidades<Entity.Contabilidad.Polizasdetalle> TraerPolizasdetalle()
        {
            return MobileDAL.Contabilidad.Polizasdetalle.TraerPolizasdetalle();
        }

        public System.Data.DataSet TraerPolizasdetalleDS()
        {
            return MobileDAL.Contabilidad.Polizasdetalle.TraerPolizasdetalleDS();
        }

        #endregion //Métodos Públicos

    }
}
