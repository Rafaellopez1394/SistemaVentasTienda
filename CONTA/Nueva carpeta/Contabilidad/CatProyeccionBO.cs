using System;
using Entity;
using System.Collections.Generic;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de CatProyeccion
    /// </summary>
    internal class CatProyeccionBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CatProyeccionBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCatProyeccion(List<Entity.Contabilidad.CatProyeccion> listaCatProyeccion)
        {
            MobileDAL.Contabilidad.CatProyeccion.Guardar(ref listaCatProyeccion);
        }

        public static Entity.Contabilidad.CatProyeccion TraerCatProyeccion(string proyeccionid)
        {
            return MobileDAL.Contabilidad.CatProyeccion.TraerCatProyeccion(proyeccionid);
        }

        public static List<Entity.Contabilidad.CatProyeccion> TraerCatProyeccion()
        {
            return MobileDAL.Contabilidad.CatProyeccion.TraerCatProyeccion();
        }

        public static System.Data.DataSet TraerCatProyeccionDS()
        {
            return MobileDAL.Contabilidad.CatProyeccion.TraerCatProyeccionDS();
        }

        public static List<Entity.Contabilidad.CatProyeccion> TraerCatProyeccionPorEjercicio(int empresa, int Ejercicio) {
            return MobileDAL.Contabilidad.CatProyeccion.TraerCatProyeccionPorEjercicio(empresa, Ejercicio);
        }
        public static Entity.Contabilidad.CatProyeccion TraerCatProyeccionPorCuenta(int empresa, int ejercicio, string cuenta) {
            return MobileDAL.Contabilidad.CatProyeccion.TraerCatProyeccionPorCuenta(empresa, ejercicio, cuenta);
        }

        public static List<Entity.Contabilidad.CatProyeccion> Catproyeccion_Select_Acumulado(int empresa, int Ejercicio, string Cuenta, int Nivel)
        {
            return MobileDAL.Contabilidad.CatProyeccion.Catproyeccion_Select_Acumulado(empresa, Ejercicio, Cuenta, Nivel);
        }

        #endregion //Métodos Públicos

    }
}
