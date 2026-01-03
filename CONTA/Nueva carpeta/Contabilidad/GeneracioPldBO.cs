using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entity;

namespace MobileBO
{
    #region Constructor
    /// <summary>
    /// Constructor
    /// </summary>
    public  class GeneracioPldBO
    {

        #region Métodos Públicos
        //public void GuardarPLD(ListaDeEntidades<Entity.Contabilidad.Acvctam> listaAcvctam)
        //{
        //    MobileDAL.Contabilidad.Acvctam.Guardar(ref listaAcvctam);
        //}

        public static ListaDeEntidades<Entity.Contabilidad.GeneracionPLD> GenerarListaClientesPLD(string fechaInicio, string fechaFin)
        {
            return MobileDAL.Contabilidad.GeneracionPLD.GenerarListaClientesPLD(fechaInicio, fechaFin);
        }
        
        public static string guardarlistaPagos(List<Entity.Contabilidad.Creditos> datosExtraidos)
        {
            return MobileDAL.Contabilidad.GeneracionPLD.guardarPagos(ref datosExtraidos);
        }


        public static ListaDeEntidades<Entity.Contabilidad.PagosCreditosPLD> GenerarListapagosClientesPLD(string fechaInicio, string fechaFin)
        {
            return MobileDAL.Contabilidad.GeneracionPLD.GenerarListapagosClientesPLD(fechaInicio, fechaFin);
        }

        #endregion
    }
    #endregion //Constructor
}
