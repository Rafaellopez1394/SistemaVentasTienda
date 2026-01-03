using System;
using Entity;
using System.Collections.Generic;
namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Catcetestiie
    /// </summary>
    internal class CatcetestiieBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CatcetestiieBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCatcetestiie(List<Entity.Contabilidad.Catcetestiie> listaCatcetestiie)
        {
            MobileDAL.Contabilidad.Catcetestiie.Guardar(ref listaCatcetestiie);
        }

        public static Entity.Contabilidad.Catcetestiie TraerCatcetestiie(int? cetetiieid, int? año, int? mes)
        {
            return MobileDAL.Contabilidad.Catcetestiie.TraerCatcetestiie(cetetiieid, año, mes);
        }

        public static List<Entity.Contabilidad.Catcetestiie> TraerCatcetestiie()
        {
            return MobileDAL.Contabilidad.Catcetestiie.TraerCatcetestiie();
        }

        public static System.Data.DataSet TraerCatcetestiieDS()
        {
            return MobileDAL.Contabilidad.Catcetestiie.TraerCatcetestiieDS();
        }

        public static List<Entity.Contabilidad.Catcetestiie> TraerCatcetestiiePorAnio(int Anio)
        {
            return MobileDAL.Contabilidad.Catcetestiie.TraerCatcetestiiePorAnio(Anio);
        }

            #endregion //Métodos Públicos

        }
}
