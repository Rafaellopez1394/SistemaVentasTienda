using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class CancelarCuentasResultado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<bool> CuentaAfectable(string cuenta, string empresaid)
        {
            try
            {
                bool EsAfectable = false;
                Entity.Contabilidad.Catcuenta Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta(cuenta, empresaid);
                EsAfectable = Cuenta.Afecta;

                return Entity.Response<bool>.CrearResponse<bool>(true, EsAfectable);
            }
            catch (Exception ex)
            {
                return Entity.Response<bool>.CrearResponseVacio<bool>(false, ex.Message);
            }
        }


        [WebMethod]
        public static Entity.Response<object> ProcesarCancelacionCuenta(string empresaid, int anio, string NumPol, string tippol, string cuentaResultado, string comentario, string usuario)
        {
            try
            {
                DataSet ds = MobileBO.ControlContabilidad.CancelarCuentasResultado(anio, empresaid);
                decimal cargo = 0m, abono = 0m, sumCargos = 0m, sumAbonos = 0m, diferencia = 0m;
                string cuenta;
                Entity.Contabilidad.Catcuenta Cuenta;
                Entity.Contabilidad.Polizasdetalle acvmov;

                Entity.Contabilidad.Poliza Poliza = new Entity.Contabilidad.Poliza();
                Poliza.Polizaid = Guid.Empty.ToString();
                Poliza.EmpresaId = empresaid;
                Poliza.Folio = NumPol;
                Poliza.TipPol = tippol;
                Poliza.Fechapol = new DateTime(anio, 12, 31);
                Poliza.Concepto = comentario;
                Poliza.Importe = 0m;
                Poliza.Estatus = 1;
                Poliza.Fecha = DateTime.Now;
                Poliza.Usuario = usuario;
                Poliza.Pendiente = false;


                Entity.Contabilidad.Cierrecontabilidad cierre = MobileBO.ControlContabilidad.TraerCierrecontabilidad(Poliza.EmpresaId);
                if (cierre != null)
                {
                    DateTime fechaPol = Poliza.Fechapol;
                    if (fechaPol <= cierre.Fechacierre)
                    {
                        throw new Exception("Contabilidad cerrada al " + cierre.Fechacierre.ToShortDateString());
                    }
                }


                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    cuenta = row["Cuenta"].ToString();
                    cargo = decimal.Parse(row["Cargo"].ToString());
                    abono = decimal.Parse(row["Abono"].ToString());

                    int tipMov = 0;
                    if (cargo > abono)
                    {
                        diferencia = cargo - abono;
                        sumAbonos += diferencia;
                        tipMov = 2;
                    }
                    if (abono > cargo)
                    {
                        diferencia = abono - cargo;
                        sumCargos += diferencia;
                        tipMov = 1;
                    }

                    acvmov = new Entity.Contabilidad.Polizasdetalle();
                    Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta(cuenta, empresaid);
                    acvmov = new Entity.Contabilidad.Polizasdetalle();
                    acvmov.Polizadetalleid = Guid.Empty.ToString();
                    acvmov.Polizaid = Poliza.Polizaid;
                    acvmov.Cuentaid = Cuenta.Cuentaid.ToUpper();
                    acvmov.TipMov = tipMov.ToString();
                    acvmov.Concepto = comentario;
                    acvmov.Cantidad = 1;
                    acvmov.Importe = diferencia;
                    acvmov.Usuario = usuario;
                    acvmov.Estatus = 1;
                    acvmov.Fecha = DateTime.Now;
                    //acvmov.Clienteid = Guid.Empty.ToString();
                    //acvmov.OperacionID = Guid.Empty.ToString();
                    Poliza.ListaPolizaDetalle.Add(acvmov);
                }

                if (sumCargos > sumAbonos)
                {
                    acvmov = new Entity.Contabilidad.Polizasdetalle();
                    Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta(cuentaResultado.Replace("-", ""), empresaid);
                    acvmov = new Entity.Contabilidad.Polizasdetalle();
                    acvmov.Polizadetalleid = Guid.Empty.ToString();
                    acvmov.Polizaid = Poliza.Polizaid;
                    acvmov.Cuentaid = Cuenta.Cuentaid.ToUpper();
                    acvmov.TipMov = "2";
                    acvmov.Concepto = comentario;
                    acvmov.Cantidad = 1;
                    acvmov.Importe = (sumCargos - sumAbonos);
                    acvmov.Usuario = usuario;
                    acvmov.Estatus = 1;
                    acvmov.Fecha = DateTime.Now;
                    //acvmov.Clienteid = Guid.Empty.ToString();
                    //acvmov.OperacionID = Guid.Empty.ToString();
                    Poliza.ListaPolizaDetalle.Add(acvmov);
                }

                if (sumAbonos > sumCargos)
                {
                    acvmov = new Entity.Contabilidad.Polizasdetalle();
                    Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta(cuentaResultado.Replace("-", ""), empresaid);
                    acvmov = new Entity.Contabilidad.Polizasdetalle();
                    acvmov.Polizadetalleid = Guid.Empty.ToString();
                    acvmov.Polizaid = Poliza.Polizaid;
                    acvmov.Cuentaid = Cuenta.Cuentaid.ToUpper();
                    acvmov.TipMov = "1";
                    acvmov.Concepto = comentario;
                    acvmov.Cantidad = 1;
                    acvmov.Importe = (sumAbonos - sumCargos);
                    acvmov.Usuario = usuario;
                    acvmov.Estatus = 1;
                    acvmov.Fecha = DateTime.Now;
                    //acvmov.Clienteid = Guid.Empty.ToString();
                    //acvmov.OperacionID = Guid.Empty.ToString();
                    Poliza.ListaPolizaDetalle.Add(acvmov);
                }

                foreach (Entity.Contabilidad.Polizasdetalle d in Poliza.ListaPolizaDetalle)
                {
                    if (d.TipMov == "1")
                    {
                        Poliza.Importe += d.Importe;
                    }
                }


                new MobileBO.ControlContabilidad().GuardarPolizaCierre(new Entity.ListaDeEntidades<Entity.Contabilidad.Poliza>() { Poliza });


                return Entity.Response<object>.CrearResponse<object>(true, "");
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }


        [WebMethod]
        public static Entity.Response<object> TraerFolioMaximoPorTipoPoliza(string tippol, string EmpresaId, string fechapol)
        {
            try
            {
                DateTime.Parse(fechapol);
            }
            catch
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, "Favor de revisar la fecha de la poliza");
            }
            try
            {
                Entity.Configuracion.Catempresa empresa = MobileBO.ControlConfiguracion.TraerCatempresas(EmpresaId);
                DataSet ds = MobileBO.ControlContabilidad.TraerFolioMaximoPorTipoPoliza(tippol, empresa.Empresa, DateTime.Parse(fechapol));
                string folio = "0";
                if (ds.Tables[0].Rows.Count > 0)
                {
                    folio = ds.Tables[0].Rows[0]["Folio"].ToString();
                }
                return Entity.Response<object>.CrearResponse<object>(true, new { Folio = folio });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

    }
}