using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Homex.Core.Utilities;
using System.Web.Services;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ReporteRelacionSaldosPorMes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }

    public class ControladorReporteRelacionSaldosPorMes : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte

        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            try
            {
                string fechaFinal = parametros.Get("fechaFinal");
                string empresaid = parametros.Get("empresaid");
                string ejercicio = parametros.Get("ejercicio");
                string cuentaInicio = parametros.Get("cuentaInicial");
                string cuentaFinal = parametros.Get("cuentaFinal");
                int gastospersonales = int.Parse(parametros.Get("gastospersonales"));
                int Mes = DateTime.Parse(fechaFinal).Month;
                string FormatoImprimir = parametros.Get("FormatoImprimir");
                List<Entity.Contabilidad.Saldo> lstSaldos;
                if (gastospersonales == 1)
                {
                   lstSaldos = MobileBO.ControlContabilidad.TraerSaldosPorRangodeCuentaEjercicioGastosPersonales((empresaid == "" ? null : empresaid), ejercicio, cuentaInicio.Replace("-", ""), cuentaFinal.Replace("-", ""));

                   
                }
                else
                {
                    lstSaldos = MobileBO.ControlContabilidad.TraerSaldosPorRangodeCuentaEjercicio((empresaid == "" ? null : empresaid), ejercicio, cuentaInicio.Replace("-", ""), cuentaFinal.Replace("-", ""));


                }


                List<Entity.ModeloSaldoPorCuentaPorMes> ListaSaldos = new List<Entity.ModeloSaldoPorCuentaPorMes>();
                MobileBO.ControlContabilidad ControlContabilidad = new MobileBO.ControlContabilidad();

                Dictionary<string, string> catCuentas = new Dictionary<string, string>();

                foreach (Entity.Contabilidad.Saldo s in lstSaldos)
                {
                    Entity.ModeloSaldoPorCuentaPorMes Saldos = new Entity.ModeloSaldoPorCuentaPorMes();
                    string NatCuenta = "";
                    if (s.Nivel == 1)
                    {
                        if (!catCuentas.ContainsKey(s.Cuenta))
                        {
                            Entity.Contabilidad.Acvctam acvCtam = MobileBO.ControlContabilidad.TraerAcvctamPorCuenta(s.Cuenta, s.EmpresaId);
                            NatCuenta = acvCtam.NatCta;

                            catCuentas.Add(s.Cuenta, NatCuenta);
                        }
                        else
                        {
                            NatCuenta = catCuentas[s.Cuenta].ToString();
                        }
                    }
                    else
                    {
                        if (!catCuentas.ContainsKey(s.Cuenta.Substring(0, 4).ToString().PadRight(24, '0')))
                        {
                            Entity.Contabilidad.Acvctam acvCtam = MobileBO.ControlContabilidad.TraerAcvctamPorCuenta(s.Cuenta.Substring(0, 4).ToString().PadRight(24, '0'), s.EmpresaId);
                            NatCuenta = acvCtam.NatCta;

                            catCuentas.Add(s.Cuenta.Substring(0, 4).ToString().PadRight(24, '0'), NatCuenta);
                        }
                        else
                        {
                            NatCuenta = catCuentas[s.Cuenta.Substring(0, 4).ToString().PadRight(24, '0')].ToString();
                        }
                    }


                    Entity.Contabilidad.Catcuenta datosCuenta = ControlContabilidad.TraerCatCuentasPorCuenta(s.Cuenta, s.EmpresaId);

                    Saldos.Cuenta = s.Cuenta;
                    Saldos.DescCuenta = datosCuenta.Descripcion;
                    Saldos.SaldoInicial = s.Sdoini;


                    //Voy a comentar este codigo por que lupita necesita solamente los cargos de la cuenta de gastos
                    /*
                    Saldos.Enero = (1 <= Mes ? (NatCuenta == "D" ? (s.Sdoini + s.Car1 - s.Abo1) : (s.Sdoini + s.Abo1 - s.Car1)) : 0);
                    Saldos.Febrero = (2 <= Mes ? (NatCuenta == "D" ? (Saldos.Enero + s.Car2 - s.Abo2) : (Saldos.Enero + s.Abo2 - s.Car2)) : 0);
                    Saldos.Marzo = (3 <= Mes ? (NatCuenta == "D" ? (Saldos.Febrero + s.Car3 - s.Abo3) : (Saldos.Febrero + s.Abo3 - s.Car3)) : 0);
                    Saldos.Abril = (4 <= Mes ? (NatCuenta == "D" ? (Saldos.Marzo + s.Car4 - s.Abo4) : (Saldos.Marzo + s.Abo4 - s.Car4)) : 0);
                    Saldos.Mayo = (5 <= Mes ? (NatCuenta == "D" ? (Saldos.Abril + s.Car5 - s.Abo5) : (Saldos.Abril + s.Abo5 - s.Car5)) : 0);
                    Saldos.Junio = (6 <= Mes ? (NatCuenta == "D" ? (Saldos.Mayo + s.Car6 - s.Abo6) : (Saldos.Mayo + s.Abo6 - s.Car6)) : 0);
                    Saldos.Julio = (7 <= Mes ? (NatCuenta == "D" ? (Saldos.Junio + s.Car7 - s.Abo7) : (Saldos.Junio + s.Abo7 - s.Car7)) : 0);
                    Saldos.Agosto = (8 <= Mes ? (NatCuenta == "D" ? (Saldos.Julio + s.Car8 - s.Abo8) : (Saldos.Julio + s.Abo8 - s.Car8)) : 0);
                    Saldos.Septiembre = (9 <= Mes ? (NatCuenta == "D" ? (Saldos.Agosto + s.Car9 - s.Abo9) : (Saldos.Agosto + s.Abo9 - s.Car9)) : 0);
                    Saldos.Octubre = (10 <= Mes ? (NatCuenta == "D" ? (Saldos.Septiembre + s.Car10 - s.Abo10) : (Saldos.Septiembre + s.Abo10 - s.Car10)) : 0);
                    Saldos.Noviembre = (11 <= Mes ? (NatCuenta == "D" ? (Saldos.Octubre + s.Car11 - s.Abo11) : (Saldos.Octubre + s.Abo11 - s.Car11)) : 0);
                    Saldos.Diciembre = (12 <= Mes ? (NatCuenta == "D" ? (Saldos.Noviembre + s.Car12 - s.Abo12) : (Saldos.Noviembre + s.Abo12 - s.Car12)) : 0);
                    */
                    //Codigo que saca los cargos por mes
                    Saldos.Enero = (1 <= Mes ? s.Car1 : 0);
                    Saldos.Febrero = (2 <= Mes ? s.Car2 : 0);
                    Saldos.Marzo = (3 <= Mes ? s.Car3 : 0);
                    Saldos.Abril = (4 <= Mes ? s.Car4 : 0);
                    Saldos.Mayo = (5 <= Mes ? s.Car5 : 0);
                    Saldos.Junio = (6 <= Mes ? s.Car6 : 0);
                    Saldos.Julio = (7 <= Mes ? s.Car7 : 0);
                    Saldos.Agosto = (8 <= Mes ? s.Car8 : 0);
                    Saldos.Septiembre = (9 <= Mes ? s.Car9 : 0);
                    Saldos.Octubre = (10 <= Mes ? s.Car10 : 0);
                    Saldos.Noviembre = (11 <= Mes ? s.Car11 : 0);
                    Saldos.Diciembre = (12 <= Mes ? s.Car12 : 0);

                    Saldos.Afecta = datosCuenta.Afecta;
                    Saldos.Nivel = datosCuenta.Nivel;

                    Saldos.NatCuenta = NatCuenta;

                    ListaSaldos.Add(Saldos);
                }

                
                DataSet ds = MobileBO.ControlConfiguracion.TraerDsEmpresasFull((empresaid == null || empresaid == string.Empty ? null : empresaid));
                if (ds.Tables[0].Rows.Count == 0)
                {
                    DataRow r = ds.Tables[0].NewRow();
                    r["Empresa"] = 0;
                    r["Descripcion"] = "CONSOLIDADO";
                    r["Rfc"] = "";
                    r["DomicilioCompleto"] = "Av. Álvaro Obregón Sur #585-2, Col. Jorge Almada, C.P. 80200";                    
                    ds.Tables[0].Rows.Add(r);
                }
                ds.Tables[0].TableName = "Empresa";

                ds.Tables.Add(ListaSaldos.ToDataTable());
                ds.Tables[1].TableName = "Saldos";

                DataTable dt = new DataTable();
                dt.TableName = "Datos";
                dt.Columns.Add("Ejercicio", typeof(string));
                dt.Columns.Add("CuentaInicio", typeof(string));
                dt.Columns.Add("CuentaFin", typeof(string));
                dt.Columns.Add("Gastospersonales",typeof(int));
                DataRow row = dt.NewRow();
                row[0] = ejercicio;
                row[1] = cuentaInicio;
                row[2] = cuentaFinal;
                row[3] = gastospersonales;
                dt.Rows.Add(row);
                ds.Tables.Add(dt);

                base.NombreReporte = "rptRelacionSaldosPorMes";
                base.FormatoReporte = int.Parse(FormatoImprimir);
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\rptRelacionSaldosPorMes.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Inicializa Reporte
    }
}