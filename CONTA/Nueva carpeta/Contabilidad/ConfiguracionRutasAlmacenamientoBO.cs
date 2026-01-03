using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de ConfiguracionRutasAlmacenamiento
    /// </summary>
    internal class ConfiguracionRutasAlmacenamientoBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ConfiguracionRutasAlmacenamientoBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarConfiguracionRutasAlmacenamiento(List<Entity.Contabilidad.ConfiguracionRutasAlmacenamiento> listaConfiguracionRutasAlmacenamiento)
        {
            MobileDAL.Contabilidad.ConfiguracionRutasAlmacenamiento.Guardar(ref listaConfiguracionRutasAlmacenamiento);
        }

        public static Entity.Contabilidad.ConfiguracionRutasAlmacenamiento TraerConfiguracionRutasAlmacenamiento(int configuracionid)
        {
            return MobileDAL.Contabilidad.ConfiguracionRutasAlmacenamiento.TraerConfiguracionRutasAlmacenamiento(configuracionid);
        }
        public static Entity.Contabilidad.ConfiguracionRutasAlmacenamiento TraerConfiguracionRutasAlmacenamientoBLT(int configuracionid)
        {
            return MobileDAL.Contabilidad.ConfiguracionRutasAlmacenamiento.TraerConfiguracionRutasAlmacenamientoBLT(configuracionid);
        }
        public static List<Entity.Contabilidad.ConfiguracionRutasAlmacenamiento> TraerConfiguracionRutasAlmacenamiento()
        {
            return MobileDAL.Contabilidad.ConfiguracionRutasAlmacenamiento.TraerConfiguracionRutasAlmacenamiento();
        }

        public static System.Data.DataSet TraerConfiguracionRutasAlmacenamientoDS()
        {
            return MobileDAL.Contabilidad.ConfiguracionRutasAlmacenamiento.TraerConfiguracionRutasAlmacenamientoDS();
        }

        #endregion //Métodos Públicos

    }
}
