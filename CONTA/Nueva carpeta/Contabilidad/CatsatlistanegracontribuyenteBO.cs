using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileBO.Contabilidad
{
    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Catsatlistanegracontribuyente
    /// </summary>
    internal class CatsatlistanegracontribuyenteBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CatsatlistanegracontribuyenteBO() { }
        #endregion //Constructor

        #region Métodos Públicos
        public static void GuardarCatsatlistanegracontribuyente(List<Entity.Contabilidad.Catsatlistanegracontribuyente> listaCatsatlistanegracontribuyente)
        {
            MobileDAL.Contabilidad.Catsatlistanegracontribuyente.Guardar(ref listaCatsatlistanegracontribuyente);
        }

        public static Entity.Contabilidad.Catsatlistanegracontribuyente TraerCatsatlistanegracontribuyentes(int numero)
        {
            return MobileDAL.Contabilidad.Catsatlistanegracontribuyente.TraerCatsatlistanegracontribuyentes(numero);
        }

        public static List<Entity.Contabilidad.Catsatlistanegracontribuyente> TraerCatsatlistanegracontribuyentes()
        {
            return MobileDAL.Contabilidad.Catsatlistanegracontribuyente.TraerCatsatlistanegracontribuyentes();
        }

        public static System.Data.DataSet TraerCatsatlistanegracontribuyentesDS(int numero)
        {
            return MobileDAL.Contabilidad.Catsatlistanegracontribuyente.TraerCatsatlistanegracontribuyentesDS(numero);
        }

        public static void EliminarTodoCatsatlistanegracontribuyentes()
        {
            MobileDAL.Contabilidad.Catsatlistanegracontribuyente.EliminarTodo();
        }

        public static System.Data.DataSet TraerCatsatlistanegracontribuyentesGridDS(bool SoloBalor, string nombre, string rfc)
        {
            return MobileDAL.Contabilidad.Catsatlistanegracontribuyente.TraerCatsatlistanegracontribuyentesGridDS(SoloBalor, nombre, rfc);
        }

        public static System.Data.DataSet TraerPolizasProveedorEnListaNegra(string ProveedorID)
        {
            return MobileDAL.Contabilidad.Catsatlistanegracontribuyente.TraerPolizasProveedorEnListaNegra(ProveedorID);
        }

        public static System.Data.DataSet TraerCatsatlistanegracontribuyentesDesdeRfc(string rfc)
        {
            return MobileDAL.Contabilidad.Catsatlistanegracontribuyente.TraerCatsatlistanegracontribuyentesDesdeRfc(rfc);
        }

        public static void GuardarCatsatlistanegracontribuyenteTEMP(List<Entity.Contabilidad.Catsatlistanegracontribuyente> listaCatsatlistanegracontribuyente)
        {
            MobileDAL.Contabilidad.Catsatlistanegracontribuyente.GuardarTEMP(ref listaCatsatlistanegracontribuyente);
        }
        public static void EliminarTodoCatsatlistanegracontribuyentesTEMP(string usuario, string sesionid)
        {
            MobileDAL.Contabilidad.Catsatlistanegracontribuyente.EliminarTodoTEMP(usuario, sesionid);
        }
        public static void CopiarTodoCatsatlistanegracontribuyentesTEMP(string usuario, string sesionid)
        {
            MobileDAL.Contabilidad.Catsatlistanegracontribuyente.CopiarTEMP(usuario, sesionid);
        }
        #endregion //Métodos Públicos

    }
}
