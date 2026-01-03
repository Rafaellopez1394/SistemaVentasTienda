using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;
using Entity;
using System.Globalization;
using Homex.Core.Utilities;
using System.Text;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class InteresesDevengados : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<List<object>> ObtenerFechaCierre(string EmpresaID)
        {
            try
            {
                List<object> CierresPorProcesar = new List<object>();
                DataSet ds = MobileBO.ControlContabilidad.TraerUltimoCierre(EmpresaID);
                DateTime FechaUltimoCierre = DateTime.Today.AddDays(-1);
                if (ds.Tables[0].Rows.Count > 0)
                    FechaUltimoCierre = DateTime.Parse(ds.Tables[0].Rows[0]["FechaCierre"].ToString());
                DateTime FechaActual = DateTime.Today;
                if (FechaActual > FechaUltimoCierre)
                {
                    DateTime FechaAux = FechaUltimoCierre.AddDays(1);
                    while (FechaAux <= FechaActual)
                    {
                        CierresPorProcesar.Add(new { Fecha = FechaAux.ToShortDateString() });
                        FechaAux = FechaAux.AddDays(1);
                    }
                }
                return Entity.Response<List<object>>.CrearResponse<List<object>>(true, CierresPorProcesar);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<object>>.CrearResponseVacio<List<object>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static object GenerarPolizaMensual(string tippol, string EmpresaId, string fechapol, string NumPol, string Usuario)
        {
            string CesionID = string.Empty;
            string FolioCesion = string.Empty;
            TimeSpan tsp;
            try
            {
                 Entity.Configuracion.Catempresa empresa = MobileBO.ControlConfiguracion.TraerCatempresas(EmpresaId);
                 DateTime FechaCierre = DateTime.Parse(fechapol);
                 DataSet CesionesPorCobrar = MobileBO.ControlOperacion.CesionesPorCobrar_Select(null, FechaCierre, empresa.Empresa);
                 if (CesionesPorCobrar.Tables[0].Rows.Count == 0)
                 {
                     return Entity.Response<object>.CrearResponse<object>(true, new { Generado = false, Fecha = "", Mensaje = "No se encontraron movimientos de cesiones para generar la poliza de intereses devengados" });
                 }

                if (string.IsNullOrEmpty(NumPol))
                {
                    throw new Exception("Error el Numero de poliza va vacío ");
                }
                if (string.IsNullOrEmpty(tippol))
                {
                    throw new Exception("Error el Tipo de poliza va vacío ");
                }
                Entity.Contabilidad.Poliza Poliza = new Entity.Contabilidad.Poliza();
                 Poliza.Polizaid = Guid.Empty.ToString();
                 Poliza.EmpresaId = EmpresaId;
                 Poliza.Folio = NumPol;
                 Poliza.TipPol = tippol;
                 Poliza.Fechapol = FechaCierre;
                 Poliza.Concepto = "Comisiones del " + FechaCierre.Day.ToString() + " al " + FechaCierre.ToString("D", CultureInfo.CreateSpecificCulture("es-MX")).Split(',')[1];
                 Poliza.Importe = 0m;
                 Poliza.Estatus = 1;
                 Poliza.Fecha = DateTime.Now;
                 Poliza.Usuario = Usuario;
                 Poliza.Pendiente = false;


                 Entity.Contabilidad.Cierrecontabilidad cierreConta = MobileBO.ControlContabilidad.TraerCierrecontabilidad(Poliza.EmpresaId);
                 if (cierreConta != null)
                 {
                     DateTime fechaPol = Poliza.Fechapol;
                     if (fechaPol <= cierreConta.Fechacierre)
                     {
                         throw new Exception("Contabilidad cerrada al " + cierreConta.Fechacierre.ToShortDateString());
                     }
                 }


                 var gcte = (from a in CesionesPorCobrar.Tables[0].AsEnumerable()
                                 //where a.Field<int>("Codigo") == 206
                             select new { Codigo = a.Field<int>("Codigo"), ClienteID = a.Field<Guid>("ClienteID").ToString() }).Distinct().OrderBy(x => x.Codigo);

                 Dictionary<int, int> ctesEclusion = new Dictionary<int, int>();
                 ctesEclusion.Add(176, 176);
                 ctesEclusion.Add(368, 368);
                 ctesEclusion.Add(235, 235);
                 ctesEclusion.Add(30, 30);
                 ctesEclusion.Add(470, 470);
                 ctesEclusion.Add(199, 199);

                foreach (var cte in gcte)
                 {
                    if(cte.Codigo == 206)
                    {
                        continue;
                    }
                     //Verificamos si el cliente se encuentra DEMANDADO O ES CLIENTE INCOBRABLE
                     Entity.Operacion.Catclientesmoroso moroso = MobileBO.ControlOperacion.TraerCatclientesmorosos(null, EmpresaId, cte.ClienteID, 1);
                     if (moroso != null)
                     {
                         if (moroso.Fecha <= FechaCierre)
                             continue;
                         else
                             FechaCierre = moroso.Fecha;
                     }
                     else
                     {
                         moroso = MobileBO.ControlOperacion.TraerCatclientesmorosos(null, EmpresaId, cte.ClienteID, 3);
                         if (moroso != null)
                         {
                             if (moroso.Fecha <= FechaCierre)
                                 continue;
                             else
                                 FechaCierre = moroso.Fecha;
                         }
                     }


                    Entity.Analisis.Catcliente _clienteTransportista = MobileBO.ControlAnalisis.TraerCatclientes(cte.ClienteID, null, null);
                    if(_clienteTransportista.Tipocliente == 5)
                    {
                        continue;
                    }

                    //Se modifica esta parte del codigo para tomar en cuenta la reestructura del cliente 206 agregados
                    //Este es solo un parche para que el sistema calcule esa restructura en particular ya que esa reestructura tiene una manera particular generar los intereses
                    var CesionesXCte = from a in CesionesPorCobrar.Tables[0].AsEnumerable()
                                        where
                                        (a.Field<int>("Codigo") == cte.Codigo && int.Parse(a.Field<string>("TipoContrato")) != 3) ||
                                        (a.Field<int>("Codigo") == cte.Codigo && a.Field<Guid>("CesionID").ToString().ToUpper().Equals("552B4A72-7A9E-491E-B7B9-9755EA1E9EC5")) ||
                                        (a.Field<int>("Codigo") == cte.Codigo && a.Field<int>("Folio") == -5161 && a.Field<Guid>("ClienteID").ToString().ToUpper().Equals("19BA09FF-E3AC-4307-A15E-CD8FB2D46ABA")) ||
                                        (a.Field<int>("Codigo") == cte.Codigo && a.Field<Guid>("CesionID").ToString().ToUpper().Equals("D833B069-8546-4F0B-949A-747F265E11A3") && a.Field<Guid>("ClienteID").ToString().ToUpper().Equals("4FBA542C-6B68-4001-B319-4A1766330AA8")) ||
                                        (a.Field<int>("Codigo") == cte.Codigo && a.Field<Guid>("CesionID").ToString().ToUpper().Equals("5CFC274E-8234-4AC5-BAA3-EA48B96E4B7E") && a.Field<Guid>("ClienteID").ToString().ToUpper().Equals("19BA09FF-E3AC-4307-A15E-CD8FB2D46ABA")) ||
                                        (a.Field<int>("Codigo") == cte.Codigo && a.Field<Guid>("CesionID").ToString().ToUpper().Equals("805D9592-AC4B-4CB9-BAAE-48CE62601187") && a.Field<Guid>("ClienteID").ToString().ToUpper().Equals("5929236A-97D5-47DB-9585-0F9751CB15CC")) ||
                                        (a.Field<int>("Codigo") == cte.Codigo && a.Field<Guid>("CesionID").ToString().ToUpper().Equals("29B15C01-8FFC-45EC-850D-2815A0A9A31E") && a.Field<Guid>("ClienteID").ToString().ToUpper().Equals("DBC9688F-2846-4960-AB45-33E14BC1C8B3")) ||
                                        //(a.Field<int>("Codigo") == cte.Codigo && a.Field<Guid>("CesionID").ToString().ToUpper().Equals("F5902E06-03C7-41E8-92B6-60A4AD55D48C") && a.Field<Guid>("ClienteID").ToString().ToUpper().Equals("6F7CB58F-8F45-4CD9-9754-A3EEB3DDDE5F")) || //nuevo grupo visual
                                        (a.Field<int>("Codigo") == cte.Codigo && a.Field<Guid>("CesionID").ToString().ToUpper().Equals("BFA3BC09-E363-45B3-9427-4B8689E124EB") && a.Field<Guid>("ClienteID").ToString().ToUpper().Equals("01A8AE8B-8A7B-428B-B727-C2B0B32716D4")) || //859-MIGUEL  ANGEL SOTO  GAXIOLA 
                                        (a.Field<int>("Codigo") == cte.Codigo && a.Field<Guid>("CesionID").ToString().ToUpper().Equals("CAFD2D57-ADBC-4629-9D50-81F3F2D94E91") && a.Field<Guid>("ClienteID").ToString().ToUpper().Equals("8B855B84-D8D0-48BA-AF75-1E7492BD5ACC")) || //813 
                                        (a.Field<int>("Codigo") == cte.Codigo && a.Field<Guid>("CesionID").ToString().ToUpper().Equals("11D0CA33-FB10-4C73-98C9-5A70BF8788DC") && a.Field<Guid>("ClienteID").ToString().ToUpper().Equals("8B855B84-D8D0-48BA-AF75-1E7492BD5ACC")) ||//813 videxport nuevo
                                        (a.Field<int>("Codigo") == cte.Codigo && a.Field<Guid>("CesionID").ToString().ToUpper().Equals("93CED04E-C6CA-4379-B35F-C34BFC1A676A") && a.Field<Guid>("ClienteID").ToString().ToUpper().Equals("E492D0AA-0060-4322-A468-797907937666")) ||//CLAFOR LOGISTICS S DE RL DE CV 
                                        (a.Field<int>("Codigo") == cte.Codigo && a.Field<Guid>("CesionID").ToString().ToUpper().Equals("6FEEF0A4-4483-41E7-B1C6-1E7B89567F04") && a.Field<Guid>("ClienteID").ToString().ToUpper().Equals("349C7485-9CBC-4CEA-AAF9-B27847241B11")) ||//TELERADIO DE SINALOA, S.A DE C.V.
                                        (a.Field<int>("Codigo") == cte.Codigo && a.Field<Guid>("CesionID").ToString().ToUpper().Equals("386B32C5-727F-4CF5-8384-F98DF3B085EB") && a.Field<Guid>("ClienteID").ToString().ToUpper().Equals("36800651-53DE-4282-A2F9-14E3D19288A9"))|| //MEDA SOLUTIONS SAPI DE CV
                                        (a.Field<int>("Codigo") == cte.Codigo && a.Field<Guid>("CesionID").ToString().ToUpper().Equals("29CAD406-2122-485C-ADE6-F5C40683EBFE") && a.Field<Guid>("ClienteID").ToString().ToUpper().Equals("8612B69E-47D0-4B44-871A-2DF251F4FA5C")) ||//NASO
                                        (a.Field<int>("Codigo") == cte.Codigo && a.Field<Guid>("CesionID").ToString().ToUpper().Equals("5A6B9E06-9FF8-4DEA-B41C-CB161CD10389") && a.Field<Guid>("ClienteID").ToString().ToUpper().Equals("5734215A-C2E0-4352-8D0B-CD456EA2B545")) ||//casler
                                        (a.Field<int>("Codigo") == cte.Codigo && a.Field<Guid>("CesionID").ToString().ToUpper().Equals("F74C5FCC-C20A-4705-8BC4-40430FFBCFDF") && a.Field<Guid>("ClienteID").ToString().ToUpper().Equals("92A76BFF-87AC-42FD-B6E3-52EB18A65F25")) //BREAK POINT CONSULTING SEGURIDAD PRIVADA  SC 
                                       select new { CesionID = a.Field<Guid>("CesionID").ToString(), Fecha_Docu = a.Field<DateTime>("Fecha_Docu"), Fecha_Vence = a.Field<DateTime>("Fecha_Vence") };
                     foreach (var Cesion in CesionesXCte)
                     {
                        CesionID = Cesion.CesionID;
                        
                        decimal ImporteProvision = 0m, ImporteProvisionMoratorio = 0m, ImporteProvisionMoratorio120 = 0m, ImporteProvisionComision = 0m;
                        List<Entity.Operacion.Cesionesdetalle> Pagos = MobileBO.ControlOperacion.TraerCesionesdetallePorCesion(Cesion.CesionID, 11, FechaCierre);
                        ModeloSaldoCesion cesion = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, FechaCierre, true);
                        ModeloSaldoCesion CesionAnterior = new ModeloSaldoCesion(), CesionAnterior120;
                        DateTime FechaCierreAnterior = FechaCierre.AddDays(-1);
                        TimeSpan ts = DateTime.Parse(cesion.FechaVence) - DateTime.Parse(cesion.FechaDocu);
                        int DiasCesion = ts.Days;
                        bool _generarDevengadosMoratoriosHonorarios = true; //Esto es para saber si se graban en la contabilidad, pero en sistemas siempre se grabará lo generado

                        

                         if (cesion.ClienteID.ToUpper().Equals("19BA09FF-E3AC-4307-A15E-CD8FB2D46ABA") && cesion.CesionID.ToUpper().Equals("5CFC274E-8234-4AC5-BAA3-EA48B96E4B7E"))
                         {
                             Entity.Contabilidad.Catcuenta _cuenta;

                             //###### Insertamos en pólizas el cargo por interés diario en la cuenta de intereses del cliente #########
                             Entity.Contabilidad.Polizasdetalle _acvmovDiario = new Entity.Contabilidad.Polizasdetalle();
                             _cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("1104" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "0001000000000000", cesion.EmpresaID);
                             //_cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("11050001" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "0000000000000000", cesion.EmpresaID);
                             //"1104" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "0001000000000000"

                             //Codigo para redondeo
                             //List<Entity.Contabilidad.Acvmov> _asientos = MobileBO.ControlContabilidad.TraerAcvmovPorCuentaYReferencia("1104" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "0001000000000000", cesion.Cesion);
                             //decimal _sumatoriaInteres = _asientos.Count > 0 ? _asientos.Where(x => x.TipMov == "1").Sum(x => x.Importe) : 0;


                             _acvmovDiario = new Entity.Contabilidad.Polizasdetalle();
                             _acvmovDiario.Polizadetalleid = Guid.Empty.ToString();
                             _acvmovDiario.Polizaid = Poliza.Polizaid;
                             _acvmovDiario.Cuentaid = _cuenta.Cuentaid;
                             _acvmovDiario.TipMov = "1";
                             _acvmovDiario.Concepto = (empresa.Sofom ? "Interes Ordinario Cesion " : "Comision por servicios administrativos y financieros Cesion ") + cesion.Cesion.Replace("E - ", "");
                             _acvmovDiario.Cantidad = 1;
                             _acvmovDiario.Importe = cesion.InteresOrdinario;
                             _acvmovDiario.Usuario = Usuario;
                             _acvmovDiario.Estatus = 1;
                             _acvmovDiario.Fecha = DateTime.Now;
                             _acvmovDiario.Referencia = cesion.Cesion.ToString();
                             Poliza.ListaPolizaDetalle.Add(_acvmovDiario);

                             //###### Insertamos en pólizas el abono por interés diario en la cuenta de ventas #########
                             Entity.Contabilidad.Polizasdetalle _acvmovVentas = new Entity.Contabilidad.Polizasdetalle();
                             _cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("60000001" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "000000000000", cesion.EmpresaID);
                             _acvmovVentas = new Entity.Contabilidad.Polizasdetalle();
                             _acvmovVentas.Polizadetalleid = Guid.Empty.ToString();
                             _acvmovVentas.Polizaid = Poliza.Polizaid;
                             _acvmovVentas.Cuentaid = _cuenta.Cuentaid;
                             _acvmovVentas.TipMov = "2";
                             _acvmovVentas.Concepto = (empresa.Sofom ? "Interes Ordinario Cesion " : "Comision por servicios administrativos y financieros Cesion ") + cesion.Cesion.Replace("E - ", "");
                             _acvmovVentas.Cantidad = 1;
                             _acvmovVentas.Importe = cesion.InteresOrdinario;
                             _acvmovVentas.Usuario = Usuario;
                             _acvmovVentas.Estatus = 1;
                             _acvmovVentas.Fecha = DateTime.Now;
                             _acvmovVentas.Referencia = cesion.Cesion.ToString();
                             Poliza.ListaPolizaDetalle.Add(_acvmovVentas);

                             if ((FechaCierre.AddDays(1).Month != FechaCierre.Month) && cesion.InteresAcumuladoMensual > 0) //Si es fin de mes obtenemos y tenemos itereses acumulados el interés acumulado
                             {
                                 //###### Insertamos en pólizas el cargo del interés acumulado en la cuenta de documentos por cobrar #########
                                 Entity.Contabilidad.Polizasdetalle _acvmovDoctos = new Entity.Contabilidad.Polizasdetalle();
                                 _cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("11050001" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "000000000000", cesion.EmpresaID);
                                 _acvmovDoctos = new Entity.Contabilidad.Polizasdetalle();
                                 _acvmovDoctos.Polizadetalleid = Guid.Empty.ToString();
                                 _acvmovDoctos.Polizaid = Poliza.Polizaid;
                                 _acvmovDoctos.Cuentaid = _cuenta.Cuentaid;
                                 _acvmovDoctos.TipMov = "1";
                                 _acvmovDoctos.Concepto = (empresa.Sofom ? "Interes Ordinario Cesion " : "Comision por servicios administrativos y financieros Cesion ") + cesion.Cesion.Replace("E - ", "");
                                 _acvmovDoctos.Cantidad = 1;
                                 _acvmovDoctos.Importe = cesion.InteresAcumuladoMensual;
                                 _acvmovDoctos.Usuario = Usuario;
                                 _acvmovDoctos.Estatus = 1;
                                 _acvmovDoctos.Fecha = DateTime.Now;
                                 _acvmovDoctos.Referencia = cesion.Cesion.ToString();
                                 Poliza.ListaPolizaDetalle.Add(_acvmovDoctos);



                                 //###### Insertamos en pólizas el abono del interés acumulado en la cuenta de intereses del cliente #########
                                 Entity.Contabilidad.Polizasdetalle _acvmovInteres = new Entity.Contabilidad.Polizasdetalle();
                                 _cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("1104" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "0001000000000000", cesion.EmpresaID);
                                 _acvmovInteres = new Entity.Contabilidad.Polizasdetalle();
                                 _acvmovInteres.Polizadetalleid = Guid.Empty.ToString();
                                 _acvmovInteres.Polizaid = Poliza.Polizaid;
                                 _acvmovInteres.Cuentaid = _cuenta.Cuentaid;
                                 _acvmovInteres.TipMov = "2";
                                 _acvmovInteres.Concepto = (empresa.Sofom ? "Interes Ordinario Cesion " : "Comision por servicios administrativos y financieros Cesion ") + cesion.Cesion.Replace("E - ", "");
                                 _acvmovInteres.Cantidad = 1;
                                 _acvmovInteres.Importe = cesion.InteresAcumuladoMensual;
                                 _acvmovInteres.Usuario = Usuario;
                                 _acvmovInteres.Estatus = 1;
                                 _acvmovInteres.Fecha = DateTime.Now;
                                 _acvmovInteres.Referencia = cesion.Cesion.ToString();
                                 Poliza.ListaPolizaDetalle.Add(_acvmovInteres);

                             }


                             goto Continua;
                         }

                         if (cesion.Dias == 0)
                             continue;
                         if (Pagos.Count > 0 && Pagos.Max(a => a.FechaApli) >= FechaCierre)
                             continue;
                        //if (cesion.Cesion.Substring(0, 1) == "R" && cesion.DiasUltimoPago > 90)
                        //{
                        //    continue;
                        //}

                        //se realiza modificacion para reestructuras que cuenten los dias de pago en base a la fecha vence de parcialidad vencida en caso de no tener abonos de caso contrario
                        //toma en cuenta la fecha de ultimo pago
                        //20240412
                        if (cesion.Cesion.Substring(0, 1) == "R")
                        {
                            var primerparcialidad = cesion.Parcialidades.OrderBy(p => p.FechaVence).FirstOrDefault();

                            tsp = FechaCierre - primerparcialidad.FechaVence;

                            if (Pagos.Count > 0 && cesion.DiasUltimoPago > 90)
                            {
                                continue;
                            }
                            else if (Pagos.Count == 0 && tsp.Days > 90)
                            {
                                continue;
                            }

                        }
                        //Nueva estructura de la poliza de interes devengandos un solo esquema para las dos empresas, antes estaba separado por empresa ver historial
                        if (Cesion.Fecha_Docu.Month < FechaCierre.Month || Cesion.Fecha_Docu.Year < FechaCierre.Year || Cesion.Fecha_Vence < FechaCierre)
                         {
                             //Si la cesion esta vencida
                             CesionAnterior = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, FechaCierreAnterior, true);

                             //Obtenemos la relacion de pagos aplicados a la Cesion para verificar si se realizo algun pago despues de la fecha de inicio del mes                    
                             if (Pagos.Count > 0)
                             {
                                 if (Pagos.Max(x => x.FechaApli) > FechaCierreAnterior)
                                 {
                                     CesionAnterior = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, FechaCierre, true);
                                 }
                             }

                             if (FechaCierre > DateTime.Parse(cesion.FechaVence))
                             {
                                 if (empresa.Sofom)
                                 {
                                     ImporteProvision = (cesion.InteresOrdinario - cesion.InteresOrdinarioM30D - cesion.PagoInteresOrdinario);
                                     if (ImporteProvision > 0)
                                     {
                                         ImporteProvision -= (CesionAnterior.InteresOrdinario - CesionAnterior.InteresOrdinarioM30D - CesionAnterior.PagoInteresOrdinario);
                                     }

                                     //Calulo de la provision moratoria de cesiones que aun no cumplen los 120 dias antes de la fecha de inicio del corte
                                     if (CesionAnterior.Dias < (90 + DiasCesion))
                                     {
                                         if (cesion.Dias >= (90 + DiasCesion))
                                         {
                                             //Intereses moratorios de cesiones que cumplieron los 120 dias durante 
                                             int Dias = (90 + DiasCesion) - CesionAnterior.Dias;
                                             CesionAnterior120 = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, FechaCierreAnterior.AddDays(Dias), true);
                                             ImporteProvisionMoratorio += (CesionAnterior120.InteresMoratorio + CesionAnterior120.InteresOrdinarioM30D - CesionAnterior120.PagoInteresMoratorio) - (CesionAnterior.InteresMoratorio + CesionAnterior.InteresOrdinarioM30D - CesionAnterior.PagoInteresMoratorio);
                                             //ImporteProvisionMoratorio120 += cesion.InteresMoratorioM120D;
                                         }
                                         else
                                         {
                                             ImporteProvisionMoratorio += (cesion.InteresMoratorio + cesion.InteresOrdinarioM30D - cesion.PagoInteresMoratorio) - (CesionAnterior.InteresMoratorio + CesionAnterior.InteresOrdinarioM30D - CesionAnterior.PagoInteresMoratorio);
                                             if (CesionAnterior.Vencida == "SI")
                                                 ImporteProvision = 0m;
                                         }
                                     }
                                     else
                                     {
                                         //Si ya se cumplieron los 120 dias despues de elaborada la cesion siempre preguntamos por los dias transcurridos despues de su ultimo pago para volverle a calcular interes apartir de esta fecha
                                         if (cesion.DiasUltimoPago < 91)//cambio 20250715 antes if (cesion.DiasUltimoPago <= 91) estaba grabando mal
                                        {
                                            //Aqui ya entra todo como moratorio financiero
                                            //Comente esta linea para que el sistema contablemente ya no acumule el interes moratorio despues de los 120 de la cesion lupita me dijo que este dato ella lo quitaba manualmente cuando el sistema le genera este importe.

                                            /*
                                                2019-08-09
                                                Fausto Montenegro
                                                La Linea siguiente la comenté porque cuando está vencida la cesión y el último pago tiene menos de 91 dias, 
                                                se debe cargar a los abonos moratorios de la cuenta 1104-2 en vez de la 1104-3, no debe acumularlo a la variable 
                                                "ImporteProvisionMoratorio120" sino a "ImporteProvisionMoratorio"
                                                Una vez vencida la cesión y pasan mas de 90 dias sin abono, el sistema ya no debe generar nada de moratorios en ninguna cuenta
                                                
                                                ImporteProvisionMoratorio120 += (cesion.InteresMoratorio + cesion.InteresOrdinarioM30D - cesion.PagoInteresMoratorio) - (CesionAnterior.InteresMoratorio + CesionAnterior.InteresOrdinarioM30D - CesionAnterior.PagoInteresMoratorio);
                                            */


                                            ImporteProvisionMoratorio += (cesion.InteresMoratorio + cesion.InteresOrdinarioM30D - cesion.PagoInteresMoratorio) - (CesionAnterior.InteresMoratorio + CesionAnterior.InteresOrdinarioM30D - CesionAnterior.PagoInteresMoratorio);
                                        }
                                        else
                                        {
                                            //Eliminar Moratorios sobre honorarios cuando los honorarios ya no se cobran
                                            _generarDevengadosMoratoriosHonorarios = false;
                                            //cesion.CostoDiarioMoratorioSobreHonorarios = 0;
                                        }
                                    }
                                 }
                                 else
                                 {
                                     ImporteProvision = (cesion.InteresOrdinario - cesion.InteresOrdinarioM30D - (cesion.PagoInteresOrdinario - cesion.PagoInteresMoratorio5Porciento));
                                     if (ImporteProvision > 0)
                                     {
                                         ImporteProvision -= (CesionAnterior.InteresOrdinario - CesionAnterior.InteresOrdinarioM30D - (CesionAnterior.PagoInteresOrdinario - CesionAnterior.PagoInteresMoratorio5Porciento));
                                     }

                                     //Provision de moratorios para FACTUR
                                     if (cesion.TipoContrato == 2)
                                     {
                                         if (CesionAnterior.Dias < (90 + DiasCesion))
                                         {
                                             if (cesion.Dias >= (90 + DiasCesion))
                                             {
                                                 //Intereses moratorios de cesiones que cumplieron los 90 dias durante el corte 
                                                 int Dias = (90 + DiasCesion) - CesionAnterior.Dias;
                                                 CesionAnterior120 = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, FechaCierreAnterior.AddDays(Dias), true);
                                                 ImporteProvisionMoratorio += (CesionAnterior120.InteresMoratorio + CesionAnterior120.InteresOrdinarioM30D - CesionAnterior120.PagoInteresMoratorio - CesionAnterior120.PagoInteresMoratorio5Porciento) - (CesionAnterior.InteresMoratorio + CesionAnterior.InteresOrdinarioM30D - CesionAnterior.PagoInteresMoratorio - CesionAnterior.PagoInteresMoratorio5Porciento);

                                                 //Dejar de acumular el interes moratorio en factur despues de los 120 dias TICKET D394
                                                 if (cesion.DiasUltimoPago <= 91)
                                                 {
                                                     if (!ctesEclusion.ContainsKey(cte.Codigo))
                                                         ImporteProvisionMoratorio120 += cesion.InteresMoratorioM90D;
                                                     else
                                                         ImporteProvisionMoratorio120 = 0m;
                                                 }
                                                else
                                                {
                                                    //Eliminar Moratorios sobre honorarios cuando los honorarios ya no se cobran
                                                    _generarDevengadosMoratoriosHonorarios = false;
                                                    //cesion.CostoDiarioMoratorioSobreHonorarios = 0;
                                                }
                                            }
                                             else
                                             {
                                                 if (ctesEclusion.ContainsKey(cte.Codigo))
                                                 {
                                                     ImporteProvisionMoratorio += (cesion.InteresOrdinarioM30D - cesion.PagoInteresMoratorio5Porciento) - (CesionAnterior.InteresOrdinarioM30D - CesionAnterior.PagoInteresMoratorio5Porciento);
                                                 }
                                                 else
                                                 {
                                                     ImporteProvisionMoratorio += (cesion.InteresMoratorio + cesion.InteresOrdinarioM30D - cesion.PagoInteresMoratorio - cesion.PagoInteresMoratorio5Porciento) - (CesionAnterior.InteresMoratorio + CesionAnterior.InteresOrdinarioM30D - CesionAnterior.PagoInteresMoratorio - CesionAnterior.PagoInteresMoratorio5Porciento);
                                                 }


                                                 if (CesionAnterior.Vencida == "SI" && ImporteProvision < 1m)
                                                     ImporteProvision = 0m;
                                             }
                                         }
                                         else
                                         {
                                             //Dejar de acumular el interes moratorio en factur despues de los 120 dias TICKET D394
                                             //Si ya se cumplieron los 120 dias despues de elaborada la cesion siempre preguntamos por los dias transcurridos despues de su ultimo pago para volverle a calcular interes apartir de esta fecha
                                             if (cesion.DiasUltimoPago <= 91)
                                             {
                                                 //Aqui ya entra todo como moratorio financiero
                                                 if (ctesEclusion.ContainsKey(cte.Codigo))
                                                 {
                                                     ImporteProvisionMoratorio += (cesion.InteresOrdinarioM30D - cesion.PagoInteresMoratorio5Porciento) - (CesionAnterior.InteresOrdinarioM30D - CesionAnterior.PagoInteresMoratorio5Porciento);
                                                     ImporteProvisionMoratorio120 = 0m;
                                                 }
                                                 else
                                                 {
                                                     ImporteProvisionMoratorio120 += (cesion.InteresMoratorioM90D) - (CesionAnterior.InteresMoratorioM90D);
                                                 }
                                             }
                                            else
                                            {
                                                //Eliminar Moratorios sobre honorarios cuando los honorarios ya no se cobran
                                                _generarDevengadosMoratoriosHonorarios = false;
                                                //cesion.CostoDiarioMoratorioSobreHonorarios = 0; 
                                            }
                                         }
                                     }
                                     else
                                     {
                                         if (ctesEclusion.ContainsKey(cte.Codigo))
                                         {
                                             ImporteProvisionMoratorio += (cesion.InteresOrdinarioM30D - cesion.PagoInteresMoratorio5Porciento) - (CesionAnterior.InteresOrdinarioM30D - CesionAnterior.PagoInteresMoratorio5Porciento);
                                         }
                                         else
                                         {
                                             ImporteProvisionMoratorio += (cesion.InteresMoratorio + cesion.InteresOrdinarioM30D - cesion.PagoInteresMoratorio - cesion.PagoInteresMoratorio5Porciento) - (CesionAnterior.InteresMoratorio + CesionAnterior.InteresOrdinarioM30D - CesionAnterior.PagoInteresMoratorio - CesionAnterior.PagoInteresMoratorio5Porciento);
                                         }

                                         if (CesionAnterior.Vencida == "SI" && ImporteProvision < 1m)
                                             ImporteProvision = 0m;
                                     }
                                 }

                             }
                             else
                             {
                                 ImporteProvision = (cesion.InteresOrdinario - cesion.PagoInteresOrdinario) - (CesionAnterior.InteresOrdinario - CesionAnterior.PagoInteresOrdinario);
                                 //Agregamos el importe generado por comisiones en el caso de balor
                                 ImporteProvisionComision = (cesion.ComisionAnalisis - cesion.PagoComisionAnalisis) - (CesionAnterior.ComisionAnalisis - CesionAnterior.PagoComisionAnalisis);
                                 ImporteProvisionComision += (cesion.ComisionDisposicion - cesion.PagoComisionDisposicion) - (CesionAnterior.ComisionDisposicion - CesionAnterior.PagoComisionDisposicion);

                                //Este codigo sirve unicamente para la reestructura
                               // if (cesion.Cesion == "R5161" || cesion.Cesion == "R6013" || cesion.Cesion == "R13758" || cesion.Cesion == "R14809" || cesion.Cesion == "R7129" || cesion.Cesion == "R7761" || cesion.Cesion == "R16161" || cesion.Cesion == "R16348" || cesion.Cesion == "R8750" || cesion.Cesion == "R8785" || cesion.Cesion == "R16600" || cesion.Cesion == "R8973")
                               //se agrega clafor
                                if (cesion.Cesion == "R5161" || cesion.Cesion == "R6013" || cesion.Cesion == "R13758"  || cesion.Cesion == "R7129" || cesion.Cesion == "R7761" || cesion.Cesion == "R16161" || cesion.Cesion == "R16348" || cesion.Cesion == "R8750" || cesion.Cesion == "R8785" || cesion.Cesion == "R16600" ||cesion.Cesion == "R8973" || cesion.Cesion == "16814" || cesion.Cesion == "R8624") 
                                     ImporteProvisionMoratorio += (cesion.InteresMoratorio - cesion.PagoInteresMoratorio) - (CesionAnterior.InteresMoratorio - CesionAnterior.PagoInteresMoratorio);
                             }
                         }
                         else
                         {
                             //Si la cesion esta hecha dentro del mismo mes
                             CesionAnterior = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, FechaCierreAnterior, true);
                             //Obtenemos la relacion de pagos aplicados a la Cesion para verificar si se realizo algun pago despues de la fecha de inicio del mes                    
                             if (Pagos.Count > 0)
                             {
                                 if (Pagos.Max(x => x.FechaApli) > FechaCierreAnterior)
                                 {
                                     CesionAnterior = MobileBO.ControlOperacion.CalculaInteresOptimizado(Cesion.CesionID, FechaCierre, true);
                                 }
                             }
                             ImporteProvision = (cesion.InteresOrdinario - cesion.PagoInteresOrdinario) - (CesionAnterior.InteresOrdinario - CesionAnterior.PagoInteresOrdinario);
                             //Agregamos el importe generado por comisiones en el caso de balor
                             ImporteProvisionComision = (cesion.ComisionAnalisis - cesion.PagoComisionAnalisis) - (CesionAnterior.ComisionAnalisis - CesionAnterior.PagoComisionAnalisis);
                             ImporteProvisionComision += (cesion.ComisionDisposicion - cesion.PagoComisionDisposicion) - (CesionAnterior.ComisionDisposicion - CesionAnterior.PagoComisionDisposicion);

                            //Este codigo sirve unicamente para la reestructura
                            //Se quita cesion de nuevo grupo visual para no generar mov en contabilidad
                            //if (cesion.Cesion == "R5161" || cesion.Cesion == "R6013" || cesion.Cesion == "R13758" || cesion.Cesion == "R14809" || cesion.Cesion == "R7129" || cesion.Cesion == "R7761" || cesion.Cesion == "R16161" || cesion.Cesion == "R16348" || cesion.Cesion == "R8750" || cesion.Cesion == "R8785" || cesion.Cesion == "R16600" || cesion.Cesion == "R8973")

                            if (cesion.Cesion == "R5161" || cesion.Cesion == "R6013" || cesion.Cesion == "R13758" || cesion.Cesion == "R7129" || cesion.Cesion == "R7761" || cesion.Cesion == "R16161" || cesion.Cesion == "R16348" || cesion.Cesion == "R8750" || cesion.Cesion == "R8785" || cesion.Cesion == "R16600" || cesion.Cesion == "R8973" || cesion.Cesion == "16814" || cesion.Cesion == "R8624")
                                 ImporteProvisionMoratorio += (cesion.InteresMoratorio - cesion.PagoInteresMoratorio) - (CesionAnterior.InteresMoratorio - CesionAnterior.PagoInteresMoratorio);
                         }

                        if (ImporteProvision > 0)
                        {
                            RecalculaImporteDeInteres(ref cesion, FechaCierre, ref ImporteProvision);
                        }
                        

                        Entity.Contabilidad.Catcuenta Cuenta;
                        Entity.Contabilidad.Polizasdetalle acvmov = new Entity.Contabilidad.Polizasdetalle();



                         //Intereses devengados de las parcialidades vencidas para el cliente Alfasin
                         if (cesion.parcialidadVencida)
                         {
                             //ImporteProvision = 0;

                             cesion.Parcialidades.FindAll(x => (x.InteresMoratorio - x.Abonomoratorio) > 0).ForEach(par => {

                                 Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("1104" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "0002000000000000", cesion.EmpresaID);
                                 acvmov = new Entity.Contabilidad.Polizasdetalle();
                                 acvmov.Polizadetalleid = Guid.Empty.ToString();
                                 acvmov.Polizaid = Poliza.Polizaid;
                                 acvmov.Cuentaid = Cuenta.Cuentaid;
                                 acvmov.TipMov = "1";
                                 acvmov.Concepto = (empresa.Sofom ? "Interes Moratorio Cesion " : "Comision Adicional Por Servicios Administrativos y Financieros de C. ") + cesion.Cesion.Replace("E - ", "");
                                 acvmov.Cantidad = 1;
                                 acvmov.Importe = par.costoDiarioMoratorio;
                                 acvmov.Usuario = Usuario;
                                 acvmov.Estatus = 1;
                                 acvmov.Fecha = DateTime.Now;
                                 acvmov.Referencia = cesion.Cesion.ToString();
                                 Poliza.ListaPolizaDetalle.Add(acvmov);


                                 if (cesion.Sofom)
                                     Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("60000001" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "000000000000", cesion.EmpresaID);
                                 else
                                 {
                                     DateTime _fecha = new DateTime(2018, 9, 1);
                                     if (acvmov.Fecha < _fecha)
                                     {
                                         Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("710000080000000000000000", cesion.EmpresaID);
                                     }
                                     else
                                     {
                                         Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("60000002" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "000000000000", cesion.EmpresaID);
                                     }
                                 }
                                 acvmov = new Entity.Contabilidad.Polizasdetalle();
                                 acvmov.Polizadetalleid = Guid.Empty.ToString();
                                 acvmov.Polizaid = Poliza.Polizaid;
                                 acvmov.Cuentaid = Cuenta.Cuentaid;
                                 acvmov.TipMov = "2";
                                 acvmov.Concepto = cesion.Cesion.Replace("E - ", "");
                                 acvmov.Cantidad = 1;
                                 acvmov.Importe = par.costoDiarioMoratorio;
                                 acvmov.Usuario = Usuario;
                                 acvmov.Estatus = 1;
                                 acvmov.Fecha = DateTime.Now;
                                 acvmov.Referencia = cesion.Cesion.ToString();
                                 Poliza.ListaPolizaDetalle.Add(acvmov);
                             });
                         }

                         //Se crean pólizas para generar los honorarios devengados del cliente Alfasin el último día del mes
                        if (FechaCierre.AddDays(1).Month != FechaCierre.Month && cesion.Saldo >= 1)
                        {
                            Entity.Cartera.Cesioneshonorariosconfiguracion _conf = MobileBO.ControlCartera.TraerCesioneshonorariosconfiguracion(null, cesion.CesionID, FechaCierre);
                            if (_conf != null)
                            {
                                Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("1104" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "0004000000000000", cesion.EmpresaID);
                                acvmov = new Entity.Contabilidad.Polizasdetalle();
                                acvmov.Polizadetalleid = Guid.Empty.ToString();
                                acvmov.Polizaid = Poliza.Polizaid;
                                acvmov.Cuentaid = Cuenta.Cuentaid;
                                acvmov.TipMov = "1";
                                acvmov.Concepto = (empresa.Sofom ? "Interes Moratorio Cesion " : "Comision Adicional Por Servicios Administrativos y Financieros de C. ") + cesion.Cesion.Replace("E - ", "");
                                acvmov.Cantidad = 1;
                                acvmov.Importe = _conf.Honorario;
                                acvmov.Usuario = Usuario;
                                acvmov.Estatus = 1;
                                acvmov.Fecha = DateTime.Now;
                                acvmov.Referencia = cesion.Cesion.ToString();
                                Poliza.ListaPolizaDetalle.Add(acvmov);

                                Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("60000002" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "000000000000", cesion.EmpresaID);
                                acvmov = new Entity.Contabilidad.Polizasdetalle();
                                acvmov.Polizadetalleid = Guid.Empty.ToString();
                                acvmov.Polizaid = Poliza.Polizaid;
                                acvmov.Cuentaid = Cuenta.Cuentaid;
                                acvmov.TipMov = "2";
                                acvmov.Concepto = (empresa.Sofom ? "Interes Moratorio Cesion " : "Comision Adicional Por Servicios Administrativos y Financieros de C. ") + cesion.Cesion.Replace("E - ", "");
                                acvmov.Cantidad = 1;
                                acvmov.Importe = _conf.Honorario;
                                acvmov.Usuario = Usuario;
                                acvmov.Estatus = 1;
                                acvmov.Fecha = DateTime.Now;
                                acvmov.Referencia = cesion.Cesion.ToString();
                                Poliza.ListaPolizaDetalle.Add(acvmov);

                                Entity.Cartera.Cesioneshonorario _honorario = new Entity.Cartera.Cesioneshonorario();
                                _honorario.Aniomes = FechaCierre.Year.ToString() + Microsoft.VisualBasic.Strings.Right("00" + FechaCierre.Month.ToString(), 2);
                                _honorario.Cesionhonorarioconfiguracionid = _conf.Cesionhonorarioconfiguracionid;
                                _honorario.Cesionhonorarioid = Guid.Empty.ToString();
                                _honorario.Cesionid = Cesion.CesionID;
                                _honorario.Estatus = 1;
                                _honorario.Fecha = DateTime.Now;
                                _honorario.Fechadocu = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                                _honorario.Fechavence = FechaCierre;
                                _honorario.Honorario = _conf.Honorario;
                                _honorario.Usuario = Usuario;

                                MobileBO.ControlCartera.GuardarCesioneshonorario(new List<Entity.Cartera.Cesioneshonorario>() { _honorario });
                            }
                        }

                        if(cesion.CostoDiarioMoratorioSobreHonorarios > 0)
                        {
                            Entity.Cartera.Cesioneshonorario _honorario = MobileBO.ControlCartera.TraerCesioneshonorarios(null, cesion.CesionID, null, null, null).OrderByDescending(x => x.Aniomes).First();
                            if (_generarDevengadosMoratoriosHonorarios)
                            {
                                Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("1104" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "0002000000000000", cesion.EmpresaID);
                                acvmov = new Entity.Contabilidad.Polizasdetalle();
                                acvmov.Polizadetalleid = Guid.Empty.ToString();
                                acvmov.Polizaid = Poliza.Polizaid;
                                acvmov.Cuentaid = Cuenta.Cuentaid;
                                acvmov.TipMov = "1";
                                acvmov.Concepto = (empresa.Sofom ? "Interes Moratorio Sobre Honorarios Cesion " : "Comision Adicional Por Servicios Administrativos y Financieros de C. ") + cesion.Cesion.Replace("E - ", "");
                                acvmov.Cantidad = 1;
                                acvmov.Importe = cesion.CostoDiarioMoratorioSobreHonorarios;
                                acvmov.Usuario = Usuario;
                                acvmov.Estatus = 1;
                                acvmov.Fecha = DateTime.Now;
                                acvmov.Referencia = cesion.Cesion.ToString();
                                Poliza.ListaPolizaDetalle.Add(acvmov);

                                Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("60000001" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "000000000000", cesion.EmpresaID);
                                acvmov = new Entity.Contabilidad.Polizasdetalle();
                                acvmov.Polizadetalleid = Guid.Empty.ToString();
                                acvmov.Polizaid = Poliza.Polizaid;
                                acvmov.Cuentaid = Cuenta.Cuentaid;
                                acvmov.TipMov = "2";
                                acvmov.Concepto = (empresa.Sofom ? "Interes Moratorio Sobre Honorarios Cesion " : "Comision Adicional Por Servicios Administrativos y Financieros de C. ") + cesion.Cesion.Replace("E - ", "");
                                acvmov.Cantidad = 1;
                                acvmov.Importe = cesion.CostoDiarioMoratorioSobreHonorarios;
                                acvmov.Usuario = Usuario;
                                acvmov.Estatus = 1;
                                acvmov.Fecha = DateTime.Now;
                                acvmov.Referencia = cesion.Cesion.ToString();
                                Poliza.ListaPolizaDetalle.Add(acvmov);
                            }

                            Entity.Cartera.Cesioneshonorariosmoratorio _moratorio = new Entity.Cartera.Cesioneshonorariosmoratorio();
                            _moratorio.Cesionhonorariomoratorioid = Guid.Empty.ToString();
                            _moratorio.Cesionhonorarioid = _honorario == null ? null : _honorario.Cesionhonorarioid;
                            _moratorio.Estatus = 1;
                            _moratorio.Fecha = DateTime.Now;
                            _moratorio.Fechamoratorio = DateTime.Now.Date;
                            _moratorio.Importemoratorio = cesion.CostoDiarioMoratorioSobreHonorarios;
                            _moratorio.Usuario = Usuario;

                            MobileBO.ControlCartera.GuardarCesioneshonorariosmoratorio(new List<Entity.Cartera.Cesioneshonorariosmoratorio>() { _moratorio });


                        }

                         if (ImporteProvision > 0)
                         {
                             Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("1104" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "0001000000000000", cesion.EmpresaID);
                             acvmov = new Entity.Contabilidad.Polizasdetalle();
                             acvmov.Polizadetalleid = Guid.Empty.ToString();
                             acvmov.Polizaid = Poliza.Polizaid;
                             acvmov.Cuentaid = Cuenta.Cuentaid;
                             acvmov.TipMov = "1";
                             acvmov.Concepto = (empresa.Sofom ? "Interes Ordinario Cesion " : "Comision por servicios administrativos y financieros Cesion ") + cesion.Cesion.Replace("E - ", "");
                             acvmov.Cantidad = 1;
                             acvmov.Importe = ImporteProvision;
                             acvmov.Usuario = Usuario;
                             acvmov.Estatus = 1;
                             acvmov.Fecha = DateTime.Now;
                             acvmov.Referencia = cesion.Cesion.ToString();
                             Poliza.ListaPolizaDetalle.Add(acvmov);

                             Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("60000001" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "000000000000", cesion.EmpresaID);
                             acvmov = new Entity.Contabilidad.Polizasdetalle();
                             acvmov.Polizadetalleid = Guid.Empty.ToString();
                             acvmov.Polizaid = Poliza.Polizaid;
                             acvmov.Cuentaid = Cuenta.Cuentaid;
                             acvmov.TipMov = "2";
                             acvmov.Concepto = cesion.Cesion.Replace("E - ", "");
                             acvmov.Cantidad = 1;
                             acvmov.Importe = ImporteProvision;
                             acvmov.Usuario = Usuario;
                             acvmov.Estatus = 1;
                             acvmov.Fecha = DateTime.Now;
                             acvmov.Referencia = cesion.Cesion.ToString();
                             Poliza.ListaPolizaDetalle.Add(acvmov);


                         }
                         if (ImporteProvisionMoratorio > 0)
                         {
                             Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("1104" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "0002000000000000", cesion.EmpresaID);
                             acvmov = new Entity.Contabilidad.Polizasdetalle();
                             acvmov.Polizadetalleid = Guid.Empty.ToString();
                             acvmov.Polizaid = Poliza.Polizaid;
                             acvmov.Cuentaid = Cuenta.Cuentaid;
                             acvmov.TipMov = "1";
                             acvmov.Concepto = (empresa.Sofom ? "Interes Moratorio Cesion " : "Comision Adicional Por Servicios Administrativos y Financieros de C. ") + cesion.Cesion.Replace("E - ", "");
                             acvmov.Cantidad = 1;
                             acvmov.Importe = ImporteProvisionMoratorio;
                             acvmov.Usuario = Usuario;
                             acvmov.Estatus = 1;
                             acvmov.Fecha = DateTime.Now;
                             acvmov.Referencia = cesion.Cesion.ToString();
                             Poliza.ListaPolizaDetalle.Add(acvmov);

                             if (cesion.Sofom)
                                 Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("60000001" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "000000000000", cesion.EmpresaID);
                             else
                             {
                                 DateTime _fecha = new DateTime(2018, 9, 1);
                                 if (acvmov.Fecha < _fecha)
                                 {
                                     Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("710000080000000000000000", cesion.EmpresaID);
                                 }
                                 else
                                 {
                                     Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("60000002" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "000000000000", cesion.EmpresaID);
                                 }

                             }
                             acvmov = new Entity.Contabilidad.Polizasdetalle();
                             acvmov.Polizadetalleid = Guid.Empty.ToString();
                             acvmov.Polizaid = Poliza.Polizaid;
                             acvmov.Cuentaid = Cuenta.Cuentaid;
                             acvmov.TipMov = "2";
                             acvmov.Concepto = cesion.Cesion.Replace("E - ", "");
                             acvmov.Cantidad = 1;
                             acvmov.Importe = ImporteProvisionMoratorio;
                             acvmov.Usuario = Usuario;
                             acvmov.Estatus = 1;
                             acvmov.Fecha = DateTime.Now;
                             acvmov.Referencia = cesion.Cesion.ToString();
                             Poliza.ListaPolizaDetalle.Add(acvmov);

                         }
                         if (ImporteProvisionMoratorio120 > 0)
                         {
                             Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("1104" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "0003000000000000", cesion.EmpresaID);
                             acvmov = new Entity.Contabilidad.Polizasdetalle();
                             acvmov.Polizadetalleid = Guid.Empty.ToString();
                             acvmov.Polizaid = Poliza.Polizaid;
                             acvmov.Cuentaid = Cuenta.Cuentaid;
                             acvmov.TipMov = "1";
                             acvmov.Concepto = (empresa.Sofom ? "Interes Moratorio Cesion " : "Comision Adicional Por Servicios Administrativos y Financieros de C. ") + cesion.Cesion.Replace("E - ", "");
                             acvmov.Cantidad = 1;
                             acvmov.Importe = (CesionAnterior.Dias < (120 + DiasCesion) ? ImporteProvisionMoratorio120 : ImporteProvision + ImporteProvisionMoratorio120);
                             acvmov.Usuario = Usuario;
                             acvmov.Estatus = 1;
                             acvmov.Fecha = DateTime.Now;
                             acvmov.Referencia = cesion.Cesion.ToString();
                             Poliza.ListaPolizaDetalle.Add(acvmov);

                             if (cesion.Sofom)
                                 Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("60000001" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "000000000000", cesion.EmpresaID);
                             else
                             {
                                 DateTime _fecha = new DateTime(2018, 9, 1);
                                 if (acvmov.Fecha < _fecha)
                                 {
                                     Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("710000080000000000000000", cesion.EmpresaID);
                                 }
                                 else
                                 {
                                     Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("60000002" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "000000000000", cesion.EmpresaID);
                                 }

                             }
                             acvmov = new Entity.Contabilidad.Polizasdetalle();
                             acvmov.Polizadetalleid = Guid.Empty.ToString();
                             acvmov.Polizaid = Poliza.Polizaid;
                             acvmov.Cuentaid = Cuenta.Cuentaid;
                             acvmov.TipMov = "2";
                             acvmov.Concepto = cesion.Cesion.Replace("E - ", "");
                             acvmov.Cantidad = 1;
                             acvmov.Importe = ImporteProvisionMoratorio120;
                             acvmov.Usuario = Usuario;
                             acvmov.Estatus = 1;
                             acvmov.Fecha = DateTime.Now;
                             acvmov.Referencia = cesion.Cesion.ToString();
                             Poliza.ListaPolizaDetalle.Add(acvmov);
                         }

                         //Este aciento contable es unicamente para la empresa de BALOR
                         if (ImporteProvisionComision > 0)
                         {
                             Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("1104" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "0004000000000000", cesion.EmpresaID);
                             acvmov = new Entity.Contabilidad.Polizasdetalle();
                             acvmov.Polizadetalleid = Guid.Empty.ToString();
                             acvmov.Polizaid = Poliza.Polizaid;
                             acvmov.Cuentaid = Cuenta.Cuentaid;
                             acvmov.TipMov = "1";
                             acvmov.Concepto = "Comision por disposicion y analisis de cesion: " + cesion.Cesion.Replace("E - ", "");
                             acvmov.Cantidad = 1;
                             acvmov.Importe = ImporteProvisionComision;
                             acvmov.Usuario = Usuario;
                             acvmov.Estatus = 1;
                             acvmov.Fecha = DateTime.Now;
                             acvmov.Referencia = cesion.Cesion.ToString();
                             Poliza.ListaPolizaDetalle.Add(acvmov);

                             //Abono a la venta
                             Cuenta = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta("60000002" + Microsoft.VisualBasic.Strings.Right("0000" + cte.Codigo.ToString().Trim(), 4) + "000000000000", cesion.EmpresaID);
                             acvmov = new Entity.Contabilidad.Polizasdetalle();
                             acvmov.Polizadetalleid = Guid.Empty.ToString();
                             acvmov.Polizaid = Poliza.Polizaid;
                             acvmov.Cuentaid = Cuenta.Cuentaid;
                             acvmov.TipMov = "2";
                             acvmov.Concepto = "Comision por disposicion y por analisis de riesgo de cesion : " + cesion.Cesion.Replace("E - ", "");
                             acvmov.Cantidad = 1;
                             acvmov.Importe = ImporteProvisionComision;
                             acvmov.Usuario = Usuario;
                             acvmov.Estatus = 1;
                             acvmov.Fecha = DateTime.Now;
                             acvmov.Referencia = cesion.Cesion.ToString();
                             Poliza.ListaPolizaDetalle.Add(acvmov);
                         }

                         Continua:; //Llega aqui directamente cuando se capitalizan los intereses del cliente con código 206 desde la parte superior
                     }
                 }


                 new MobileBO.ControlContabilidad().GuardarPolizaCierre(new ListaDeEntidades<Entity.Contabilidad.Poliza>() { Poliza });

                 Entity.Contabilidad.Catcierre cierre = new Entity.Contabilidad.Catcierre();
                 cierre.Empresaid = empresa.Empresaid;
                 cierre.Anomes = DateTime.Parse(fechapol).Year.ToString() + Microsoft.VisualBasic.Strings.Right("00" + DateTime.Parse(fechapol).Month.ToString(), 2);
                 cierre.Fechacierre = DateTime.Parse(fechapol);
                 cierre.TipPol = tippol;
                 cierre.NumPol = NumPol;
                 decimal ImportePoliza = 0m;
                 foreach (Entity.Contabilidad.Polizasdetalle item in Poliza.ListaPolizaDetalle) {
                     if (item.TipMov == "1")
                         ImportePoliza += item.Importe;
                 }
                 cierre.Intereses = ImportePoliza;
                 cierre.Estatus = 1;
                 cierre.Usuario = Usuario;
                 cierre.Fecha = DateTime.Now;

                 MobileBO.ControlContabilidad.GuardarCatcierre(new ListaDeEntidades<Entity.Contabilidad.Catcierre>() { cierre });



                //SE AGREGARA LAS NOTIFICACIONES POR CORREO DE CLIENTES CON VENCIDO DE 5 DIAS ( MENOR A 30 DIAS )
                GenerarNotificacionesCorreo();
                GenerarNotificacionesCorreoB1();
                GenerarNotificacionesCorreoA1();
                GenerarNotificacionesCorreoDocumentacionPendienteA1();

                #region Validación de póliza generada

                bool polizavalida = true;

                Entity.Contabilidad.Acvgral _acvgral = MobileBO.ControlContabilidad.TraerAcvgralPorReferenciaId(Poliza.Polizaid);
                Entity.Contabilidad.Acvmov _acvmov = MobileBO.ControlContabilidad.TraerAcvmovPorAcvGral(_acvgral.Acvgralid)[0];

                if (Poliza.Fechapol != cierre.Fechacierre || Poliza.TipPol != cierre.TipPol || Poliza.Folio != cierre.NumPol)
                {
                    polizavalida = false;
                }
                if (Poliza.Fechapol != _acvgral.FecPol || Poliza.TipPol != _acvgral.TipPol || Poliza.Folio != _acvgral.NumPol)
                {
                    polizavalida = false;
                }
                if (Poliza.Fechapol != _acvmov.FecPol || Poliza.TipPol != _acvmov.TipPol || Poliza.Folio != _acvmov.NumPol)
                {
                    polizavalida = false;
                }
                if (Poliza.TipPol.Trim() == "" || Poliza.Folio.Trim() == "" || _acvmov.NumPol.Trim() == "" || _acvmov.TipPol.Trim() == "" || _acvgral.NumPol.Trim() == "" || _acvgral.TipPol.Trim() == "")
                {
                    polizavalida = false;
                }

                #endregion


                if (!polizavalida)
                {
                    return Entity.Response<object>.CrearResponse<object>(false, new { PolizaID = Poliza.Polizaid }, "Se generó la póliza pero ocurrió un problema, por favor, avise al departamento de sistemas.");
                }

                return Entity.Response<object>.CrearResponse<object>(true, new { PolizaID = Poliza.Polizaid });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message + Environment.NewLine + "CesionID:" + CesionID);
            }
            
        }

        public static void RecalculaImporteDeInteres(ref ModeloSaldoCesion cesion, DateTime fechaCorte, ref decimal ImporteProvisionFinal)
        {
            decimal _totalContabilidad = 0;
            decimal _totalSistemas = 0;
            decimal ImporteProvision = 0;
            decimal _sumaCostoDiario = 0;
            
            DateTime _fechaInicial;
            DateTime _fechaAux = new DateTime();
            DateTime _fechaVencimiento;
            
            TimeSpan ts = DateTime.Parse(cesion.FechaVence) - DateTime.Parse(cesion.FechaDocu);
            int DiasCesion = ts.Days;

            _fechaInicial = Convert.ToDateTime(cesion.FechaDocu);
            _fechaVencimiento = Convert.ToDateTime(cesion.FechaVence);
            _fechaAux = _fechaInicial;

            if (cesion.Cesion.ToUpper().Contains("R"))
            {
                return;
            }

            try
            {

                //ModeloSaldoCesion _cesionCompleta = MobileBO.ControlOperacion.CalculaInteresOptimizado(cesion.CesionID, fechaCorte, true);
                ModeloSaldoCesion _cesionCompleta;


                Entity.Analisis.Catcliente _cliente = MobileBO.ControlAnalisis.TraerCatclientes(cesion.ClienteID, null, null);
                Entity.Configuracion.Catempresa empresa = MobileBO.ControlConfiguracion.TraerCatempresas(cesion.EmpresaID);

                
                DateTime FechaCierreAnterior = fechaCorte.AddDays(-1);

                List<Entity.Contabilidad.Acvmov> _movimientos = MobileBO.ControlContabilidad.TraerAcvmovPorCesion("interes-" + cesion.Cesion, "1104" + Microsoft.VisualBasic.Strings.Right("0000" + _cliente.Codigo.ToString().Trim(), 4) + "0001000000000000", cesion.EmpresaID);
                _totalContabilidad = _movimientos.Sum(x => x.Importe);

                
                while (_fechaAux <= fechaCorte)
                {
                    ts = _fechaAux - Convert.ToDateTime(cesion.FechaDocu);
                    if (ts.Days > 0 )
                    {
                        _cesionCompleta = MobileBO.ControlOperacion.CalculaInteresOptimizado(cesion.CesionID, _fechaAux, true);
                        ModeloSaldoCesion CesionAnterior = MobileBO.ControlOperacion.CalculaInteresOptimizado(cesion.CesionID, _fechaAux.AddDays(-1), true);
                        List<Entity.Operacion.Cesionesdetalle> Pagos = MobileBO.ControlOperacion.TraerCesionesdetallePorCesion(cesion.CesionID, 11, fechaCorte);

                        if (Convert.ToDateTime(_cesionCompleta.FechaDocu).Month < fechaCorte.Month || Convert.ToDateTime(_cesionCompleta.FechaDocu).Year < fechaCorte.Year || Convert.ToDateTime(_cesionCompleta.FechaVence) < fechaCorte)
                        {
                            //Si la cesion esta vencida
                            CesionAnterior = MobileBO.ControlOperacion.CalculaInteresOptimizado(cesion.CesionID, _fechaAux.AddDays(-1), true);

                            //Obtenemos la relacion de pagos aplicados a la Cesion para verificar si se realizo algun pago despues de la fecha de inicio del mes                    
                            if (Pagos.Count > 0)
                            {
                                if (Pagos.Max(x => x.FechaApli) > _fechaAux.AddDays(-1))
                                {
                                    CesionAnterior = MobileBO.ControlOperacion.CalculaInteresOptimizado(cesion.CesionID, _fechaAux.AddDays(-1), true);
                                }
                            }

                            if (_fechaAux > DateTime.Parse(cesion.FechaVence))
                            {
                                if (empresa.Sofom)
                                {
                                    ImporteProvision = (_cesionCompleta.InteresOrdinario - _cesionCompleta.InteresOrdinarioM30D - _cesionCompleta.PagoInteresOrdinario);
                                    if (ImporteProvision > 0)
                                    {
                                        ImporteProvision -= (CesionAnterior.InteresOrdinario - CesionAnterior.InteresOrdinarioM30D - CesionAnterior.PagoInteresOrdinario);
                                    }

                                    //Calulo de la provision moratoria de cesiones que aun no cumplen los 120 dias antes de la fecha de inicio del corte
                                    if (CesionAnterior.Dias < (90 + DiasCesion))
                                    {
                                        if (cesion.Dias >= (90 + DiasCesion))
                                        {

                                        }
                                        else
                                        {
                                            if (CesionAnterior.Vencida == "SI")
                                                ImporteProvision = 0m;
                                        }
                                    }
                                    else
                                    {

                                    }
                                }
                                else
                                {
                                    ImporteProvision = (_cesionCompleta.InteresOrdinario - _cesionCompleta.InteresOrdinarioM30D - (_cesionCompleta.PagoInteresOrdinario - _cesionCompleta.PagoInteresMoratorio5Porciento));
                                    if (ImporteProvision > 0)
                                    {
                                        ImporteProvision -= (CesionAnterior.InteresOrdinario - CesionAnterior.InteresOrdinarioM30D - (CesionAnterior.PagoInteresOrdinario - CesionAnterior.PagoInteresMoratorio5Porciento));
                                    }

                                    //Provision de moratorios para FACTUR
                                    if (cesion.TipoContrato == 2)
                                    {
                                        if (CesionAnterior.Dias < (90 + DiasCesion))
                                        {
                                            if (cesion.Dias >= (90 + DiasCesion))
                                            {
                                            }
                                            else
                                            {

                                                if (CesionAnterior.Vencida == "SI" && ImporteProvision < 1m)
                                                    ImporteProvision = 0m;
                                            }
                                        }
                                        else
                                        {

                                        }
                                    }
                                    else
                                    {


                                        if (CesionAnterior.Vencida == "SI" && ImporteProvision < 1m)
                                            ImporteProvision = 0m;
                                    }
                                }

                            }
                            else
                            {
                                ImporteProvision = (_cesionCompleta.InteresOrdinario - _cesionCompleta.PagoInteresOrdinario) - (CesionAnterior.InteresOrdinario - CesionAnterior.PagoInteresOrdinario);


                            }
                        }
                        else
                        {
                            //Si la cesion esta hecha dentro del mismo mes
                            CesionAnterior = MobileBO.ControlOperacion.CalculaInteresOptimizado(cesion.CesionID, _fechaAux.AddDays(-1), true);
                            //Obtenemos la relacion de pagos aplicados a la Cesion para verificar si se realizo algun pago despues de la fecha de inicio del mes                    
                            if (Pagos.Count > 0)
                            {
                                if (Pagos.Max(x => x.FechaApli) > FechaCierreAnterior)
                                {
                                    CesionAnterior = MobileBO.ControlOperacion.CalculaInteresOptimizado(cesion.CesionID, _fechaAux.AddDays(-1), true);
                                }
                            }
                            ImporteProvision = (_cesionCompleta.InteresOrdinario - _cesionCompleta.PagoInteresOrdinario) - (CesionAnterior.InteresOrdinario - CesionAnterior.PagoInteresOrdinario);
                            //Agregamos el importe generado por comisiones en el caso de balor
                        }

                        _totalSistemas += ImporteProvision;
                        _sumaCostoDiario += cesion.CostoDiario;

                        CesionAnterior = null;
                    }
                    _fechaAux = _fechaAux.AddDays(1);
                }
                
                if (_totalContabilidad + ImporteProvision > _totalSistemas)
                {
                    decimal _diffMayor = Math.Abs((_totalContabilidad + ImporteProvision) - _totalSistemas);
                    if (_diffMayor == .01M)
                    {
                        ImporteProvisionFinal = ImporteProvision - _diffMayor;
                    }
                    
                }
                if (_totalContabilidad + ImporteProvision < _totalSistemas)
                {
                    decimal _diffMenor = Math.Abs(_totalSistemas - (_totalContabilidad + ImporteProvision));
                    if (_diffMenor == .01M )
                    {
                        ImporteProvisionFinal = ImporteProvision + _diffMenor;
                    }
                }
                if (_totalContabilidad + ImporteProvision == _totalSistemas)
                {
                    ImporteProvisionFinal = ImporteProvision;
                }


            }
            catch
            {
                throw;
            }

            
        }

        public static void GenerarNotificacionesCorreo()
        {
            List<string> lsCorreoCC = null;

            string sCorreo = "";
            string sCorreoCC = "";
            string sAsunto = "Cliente Vencido";
            string sMensajeLargo = "";

            DataSet CesionesNotificar = MobileBO.ControlOperacion.TraerCesionesNotificar();

            foreach (DataRow row in CesionesNotificar.Tables[0].Rows)
            {
                try
                {
                    sCorreo     = row["CorreoProspecto"].ToString();
                    sCorreoCC   = row["CorreoCC"].ToString();

                    lsCorreoCC = sCorreoCC.Split(',').ToList();

                    lsCorreoCC.RemoveAll(c => c.Equals(sCorreo));

                    sMensajeLargo = "Se ha registrado un cliente con los siguientes datos que presenta saldo vencido, favor de verificar en el sistema.";
                    sMensajeLargo += "<br />";
                    sMensajeLargo += "===========================================";
                    sMensajeLargo += "<br />";
                    sMensajeLargo += "<br />";
                    sMensajeLargo += "<strong>CESION:</strong> " + row["Folio"];
                    sMensajeLargo += "<br />";
                    sMensajeLargo += "<br />";
                    sMensajeLargo += "<strong>CLIENTE:</strong> " + row["CodigoCliente"] + " - " + row["NombreCliente"];
                    sMensajeLargo += "<br />";
                    sMensajeLargo += "<br />";
                    sMensajeLargo += "<strong>FECHA VENCIMIENTO:</strong> " + row["Fecha_Vence"];
                    sMensajeLargo += "<br />";
                    sMensajeLargo += "<br />";
                    sMensajeLargo += "<strong>IMPORTE:</strong> " + row["Importe"];
                    sMensajeLargo += "<br />";
                    sMensajeLargo += "<br />";
                    sMensajeLargo += "<strong>ABONADO:</strong> " + row["Abonado"];
                    sMensajeLargo += "<br />";
                    sMensajeLargo += "<br />";
                    sMensajeLargo += "<strong>DIAS VENCIDOS:</strong> " + row["DiasVencidos"];
                    sMensajeLargo += "<br />";
                    sMensajeLargo += "<br />";
                    sMensajeLargo += "===========================================";
                    sMensajeLargo += "<br />";

                    MobileBO.ControlConfiguracion.GenerarNotificacionVencidosSoloCorreo(sCorreo, lsCorreoCC, sAsunto, sMensajeLargo, false);
                    
                }
                catch (Exception ex)
                {
                }

            }
        }

        public static void GenerarNotificacionesCorreoB1()
        {
            List<string> lsCorreoCC = null;

            int iTipoAlerta = 0;
            string sCorreo = "";
            string sAsunto = "NOTIFICACIONES AGENDA/COMPROMISOS";
            string sMensajeLargo = "";

            DataSet dsAlertasNotificar = MobileBO.ControlContabilidad.TraerAlertasNotificarB1(null);
            DataTable dtAlertas = dsAlertasNotificar.Tables[0].Copy();

            var gpoVendedor =
                from Vendedor in dtAlertas.AsEnumerable()
                group Vendedor
                by new { VendedorID = Vendedor.Field<Guid>("VendedorID"), NombreVendedor = Vendedor.Field<string>("NombreVendedor") }
                into eGroup
                select new
                {
                    VendedorID = eGroup.Key.VendedorID,
                    NombreVendedor = eGroup.Key.NombreVendedor
                };

            foreach(var gv in gpoVendedor)
            {
                var Registros = from Vendedor in dtAlertas.AsEnumerable()
                                where Vendedor.Field<Guid>("VendedorID") == gv.VendedorID
                                select new
                                {
                                    TipoAlertaID = Vendedor.Field<int>("TipoAlertaID"),
                                    Alerta = Vendedor.Field<string>("Alerta"),
                                    TipoDescripcion = Vendedor.Field<int>("TipoDescripcion"),
                                    Descripcion = Vendedor.Field<string>("Descripcion"),
                                    VendedorID = Vendedor.Field<Guid>("VendedorID"),
                                    NombreVendedor = Vendedor.Field<string>("NombreVendedor"),
                                    Correo = Vendedor.Field<string>("Correo"),
                                    CandidatoID = Vendedor.Field<Guid>("CandidatoID"),
                                    NombreEmpresa = Vendedor.Field<string>("NombreEmpresa"),
                                    FechaCompromiso = Vendedor.Field<DateTime>("FechaCompromiso"),
                                    Hora = Vendedor.Field<string>("Hora"),
                                    Comentarios = Vendedor.Field<string>("Comentarios")
                                };

                sMensajeLargo = "<table>";
                iTipoAlerta = 0;
                int iCont = 0;
                foreach (var alertas in Registros)
                {
                    sCorreo = alertas.Correo.ToString();
                    lsCorreoCC = sCorreo.Split(',').ToList();

                    if (iTipoAlerta != alertas.TipoAlertaID)
                    {
                        iTipoAlerta = alertas.TipoAlertaID;
                        int iColSpan = 5;

                        if(iTipoAlerta == 1)
                        {
                            iColSpan = 4;
                        }

                        if(iCont > 0)
                        {
                            sMensajeLargo += "</table>";
                            sMensajeLargo += "<br />";
                            sMensajeLargo += "<br />";
                            sMensajeLargo += "<table>";
                        }
                        
                        sMensajeLargo += "<tr>";
                        sMensajeLargo += "<th colspan='"+ iColSpan.ToString() +"' style='background-color:#C80000; color:white;'><strong>" + alertas.Alerta.ToString().ToUpper() + "</strong></th>";
                        sMensajeLargo += "</tr>";
                        sMensajeLargo += "<td style='text-align:center; background-color:#C80000; color:white; border:1px solid black; padding:10px'>CANDIDATO</td>";
                        sMensajeLargo += "<td style='text-align:center; background-color:#C80000; color:white; border:1px solid black; padding:10px'>TIPO</td>";
                        sMensajeLargo += "<td style='text-align:center; background-color:#C80000; color:white; border:1px solid black; padding:10px'>FECHA COMPROMISO</td>";
                        if( iTipoAlerta == 2)
                        {
                            sMensajeLargo += "<td style='text-align:center; background-color:#C80000; color:white; border:1px solid black; padding:10px'>HORA</td>";
                        }
                        sMensajeLargo += "<td style='text-align:center; background-color:#C80000; color:white; border:1px solid black; padding:10px'>OBSERVACIONES</td>";
                        sMensajeLargo += "</tr>";
                    }

                    sMensajeLargo += "<tr>";

                    sMensajeLargo += "<td style='border:1px solid black; padding:2px'>" + alertas.NombreEmpresa.ToString() + "</td>";
                    sMensajeLargo += "<td style='text-align:center; border:1px solid black; padding:2px'>" + alertas.Descripcion.ToString() + "</td>";
                    sMensajeLargo += "<td style='text-align:center; border:1px solid black; padding:2px'>" + alertas.FechaCompromiso.ToShortDateString() + "</td>";

                    if (iTipoAlerta == 2)
                    {
                        sMensajeLargo += "<td style='text-align:center; border:1px solid black; padding:2px'>" + alertas.Hora.ToString() + "</td>";
                    }
                    sMensajeLargo += "<td style='text-align:center; border:1px solid black; padding:2px'>" + alertas.Comentarios.ToString() + "  </td>";
                    sMensajeLargo += "</tr>";
                    iCont++;
                }//FOR REGISTROS
                sMensajeLargo += "</table>";
                sMensajeLargo += "<br />";
                sMensajeLargo += "<br />";
                
                MobileBO.ControlConfiguracion.GenerarNotificacionVencidosSoloCorreo(sCorreo, lsCorreoCC, sAsunto, sMensajeLargo, false);

            }//FOR VENDEDORES
        }

        public static void GenerarNotificacionesCorreoA1()
        {
            List<string> lsCorreoCC = null;

            //int iTipoAlerta = 0;
            string sCorreo = "";
            //string sCorreoCC = "jchucuan@balor.mx,vrodriguez@balor.mx";
            string sCorreoCC = "";
            DataSet cor = MobileBO.ControlConfiguracion.TraerCorreosParaEnvioDS("DG,E1");

            if (cor != null && cor.Tables.Count > 0 && cor.Tables[0].Rows.Count > 0)
            {
                StringBuilder correosBuilder = new StringBuilder();

                foreach (DataRow row in cor.Tables[0].Rows)
                {
                    string corr = row["Correo"].ToString();

                    if (correosBuilder.Length > 0)
                    {
                        correosBuilder.Append(", ");
                    }
                    correosBuilder.Append(corr);
                }

                sCorreoCC = correosBuilder.ToString();
            }
            else
            {
                Console.WriteLine("No se encontraron correos para añadir a CC.");
            }
            string sAsunto = "NOTIFICACION CLIENTES POR VENCER";
            string sMensajeLargo = "";

            DataSet dsAlertasNotificar = MobileBO.ControlContabilidad.TraerAlertasNotificarA1(null);
            
            var gpoVendedor =
                from Vendedor in dsAlertasNotificar.Tables[0].AsEnumerable()
                group Vendedor
                by new { VendedorID = Vendedor.Field<Guid>("VendedorID")}
                into eGroup
                select new
                {
                    VendedorID = eGroup.Key.VendedorID
                };

            foreach (var gv in gpoVendedor)
            {
                var Registros = from Vendedor in dsAlertasNotificar.Tables[0].AsEnumerable()
                                where Vendedor.Field<Guid>("VendedorID") == gv.VendedorID
                                select new
                                {
                                    ClienteID = Vendedor.Field<Guid>("ClienteID"),
                                    NombreCompleto = Vendedor.Field<string>("NombreCompleto"),
                                    CesionID = Vendedor.Field<Guid>("CesionID"),
                                    Folio = Vendedor.Field<int>("Folio"),
                                    Fecha_Vence = Vendedor.Field<DateTime>("Fecha_Vence"),
                                    VendedorID = Vendedor.Field<Guid>("VendedorID"),
                                    Correo = Vendedor.Field<string>("Correo")
                                };

                sMensajeLargo = "<table>";

                sMensajeLargo += "<tr>";
                sMensajeLargo += "<th colspan='3' style='background-color:#C80000; color:white;'><strong>" + "CLIENTES CON 15 DIAS ANTES A FECHA VENCIMIENTO" + "</strong></th>";
                sMensajeLargo += "</tr>";

                sMensajeLargo += "<tr>";
                sMensajeLargo += "<td style='text-align:center; background-color:#C80000; color:white; border:1px solid black; padding:10px'>CLIENTE</td>";
                sMensajeLargo += "<td style='text-align:center; background-color:#C80000; color:white; border:1px solid black; padding:10px'>FOLIO CESION</td>";
                sMensajeLargo += "<td style='text-align:center; background-color:#C80000; color:white; border:1px solid black; padding:10px'>FECHA VENCIMIENTO</td>";
                sMensajeLargo += "</tr>";

                //iTipoAlerta = 0;
                int iCont = 0;
                foreach (var alertas in Registros)
                {
                    sCorreo = alertas.Correo.ToString();
                    sMensajeLargo += "<tr>";

                    sMensajeLargo += "<td style='border:1px solid black; padding:2px'>" + alertas.NombreCompleto.ToString() + "</td>";
                    sMensajeLargo += "<td style='text-align:center; border:1px solid black; padding:2px'>" + alertas.Folio.ToString() + "</td>";
                    sMensajeLargo += "<td style='text-align:center; border:1px solid black; padding:2px'>" + alertas.Fecha_Vence.ToShortDateString() + "</td>";

                    sMensajeLargo += "</tr>";

                    iCont++;
                }//FOR REGISTROS
                sMensajeLargo += "</table>";
                sMensajeLargo += "<br />";
                sMensajeLargo += "<br />";

                //sCorreo = "faraujo@balor.mx";
                lsCorreoCC = sCorreoCC.Split(',').ToList();

                MobileBO.ControlConfiguracion.GenerarNotificacionVencidosSoloCorreo(sCorreo, lsCorreoCC, sAsunto, sMensajeLargo, false);

            }//FOR VENDEDORES AVISO DE CLIENTES B O MAYOR POR VENCER

            sAsunto = "NOTIFICACION CLIENTES R";
            //CLIENTES R
            var gpoVendedorR =
                from Vendedor in dsAlertasNotificar.Tables[1].AsEnumerable()
                group Vendedor
                by new { VendedorID = Vendedor.Field<Guid>("VendedorID") }
                into eGroup
                select new
                {
                    VendedorID = eGroup.Key.VendedorID
                };

            foreach (var gvR in gpoVendedorR)
            {
                var RegistrosR = from Vendedor in dsAlertasNotificar.Tables[1].AsEnumerable()
                                where Vendedor.Field<Guid>("VendedorID") == gvR.VendedorID
                                select new
                                {
                                    ClienteID = Vendedor.Field<Guid>("ClienteID"),
                                    NombreCompleto = Vendedor.Field<string>("NombreCompleto"),
                                    CantAnalisis = Vendedor.Field<int>("CantAnalisis"),
                                    CantCleanUp = Vendedor.Field<int>("CantCleanUp"),
                                    UltimoCleanUp = Vendedor.Field<DateTime>("UltimoCleanUp"),
                                    R = Vendedor.Field<int>("R"),
                                    CortoDespuesCleanUp = Vendedor.Field<bool>("CortoDespuesCleanUp"),
                                    VendedorID = Vendedor.Field<Guid>("VendedorID"),
                                    Correo = Vendedor.Field<string>("Correo")
                                };

                sMensajeLargo = "<table>";

                sMensajeLargo += "<tr>";
                sMensajeLargo += "<th colspan='4' style='background-color:#C80000; color:white;'><strong>" + "CLIENTES R" + "</strong></th>";
                sMensajeLargo += "</tr>";

                sMensajeLargo += "<tr>";
                sMensajeLargo += "<td style='text-align:center; background-color:#C80000; color:white; border:1px solid black; padding:10px'>CLIENTE</td>";
                sMensajeLargo += "<td style='text-align:center; background-color:#C80000; color:white; border:1px solid black; padding:10px'>R</td>";
                sMensajeLargo += "<td style='text-align:center; background-color:#C80000; color:white; border:1px solid black; padding:10px'>CORTO DESPUES DE CLEAN UP</td>";
                sMensajeLargo += "<td style='text-align:center; background-color:#C80000; color:white; border:1px solid black; padding:10px'>FECHA ULTIMO CLEAN UP</td>";
                sMensajeLargo += "</tr>";
                
                int iCont = 0;
                foreach (var alertasR in RegistrosR)
                {
                    string sCorto = "NO";
                    sCorreo = alertasR.Correo.ToString();
                    sMensajeLargo += "<tr>";
                    if(alertasR.CortoDespuesCleanUp == true)
                    {
                        sCorto = "SI";
                    }
                    sMensajeLargo += "<td style='border:1px solid black; padding:2px'>" + alertasR.NombreCompleto.ToString() + "</td>";
                    sMensajeLargo += "<td style='text-align:center; border:1px solid black; padding:2px'>" + alertasR.R.ToString() + "</td>";
                    sMensajeLargo += "<td style='text-align:center; border:1px solid black; padding:2px'>" + sCorto + "</td>";
                    sMensajeLargo += "<td style='text-align:center; border:1px solid black; padding:2px'>" + alertasR.UltimoCleanUp.ToShortDateString() + "</td>";

                    sMensajeLargo += "</tr>";

                    iCont++;
                }//FOR REGISTROS
                sMensajeLargo += "</table>";
                sMensajeLargo += "<br />";
                sMensajeLargo += "<br />";

                //sCorreo = "faraujo@balor.mx";
                lsCorreoCC = sCorreoCC.Split(',').ToList();

                MobileBO.ControlConfiguracion.GenerarNotificacionVencidosSoloCorreo(sCorreo, lsCorreoCC, sAsunto, sMensajeLargo, false);

            }//FOR VENDEDORES AVISO DE CLIENTES B O MAYOR POR VENCER


        }

        public static void GenerarNotificacionesCorreoDocumentacionPendienteA1()
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE            
            string fechaInicial = DateTime.Now.ToShortDateString();
            int VendInicial = 0;
            int VendFinal = 1000;
            int Equipo = 0;
            int Zona = 0;

            List<string> lsCorreoCC = null;

            //int iTipoAlerta = 0;
            string sCorreo = "";
            //string sCorreoCC = "jchucuan@balor.mx,vrodriguez @balor.mx";
            string sCorreoCC = "";
            DataSet cor = MobileBO.ControlConfiguracion.TraerCorreosParaEnvioDS("DG,E1");

            if (cor != null && cor.Tables.Count > 0 && cor.Tables[0].Rows.Count > 0)
            {
                StringBuilder correosBuilder = new StringBuilder();

                foreach (DataRow row in cor.Tables[0].Rows)
                {
                    string corr = row["Correo"].ToString();

                    if (correosBuilder.Length > 0)
                    {
                        correosBuilder.Append(", ");
                    }
                    correosBuilder.Append(corr);
                }

                sCorreoCC = correosBuilder.ToString();
            }
            else
            {
                Console.WriteLine("No se encontraron correos para añadir a sCorreoCC.");
            }

            string sAsunto = "NOTIFICACIÓN DE CLIENTES CON DOCUMENTACIÓN PENDIENTE";
            string sMensajeLargo = "";

            try
            {
                Entity.ListaDeEntidades<Entity.Configuracion.Catempresa> empresas;
                empresas = MobileBO.ControlConfiguracion.TraerCatempresas();

                if (empresas != null)
                {
                    foreach (Entity.Configuracion.Catempresa empresa in empresas)
                    {
                        sAsunto = "NOTIFICACIÓN DE CLIENTES CON DOCUMENTACIÓN PENDIENTE DE LA EMPRESA: " + empresa.Descripcion.ToString();
                        DataSet CesionesPorCobrar = MobileBO.ControlOperacion.CesionesPorCobrar_Papeleria(DateTime.Parse(fechaInicial), empresa.Empresa);
                        List<ModeloReportePapeleriaPendiente> ListaDatosReporte = new List<ModeloReportePapeleriaPendiente>();

                        foreach (DataRow row in CesionesPorCobrar.Tables[0].Rows)
                        {
                            //Armamos el objeto que vamos a agregar en la lista al inicio para no repetir codigo.
                            ModeloReportePapeleriaPendiente elemento = new ModeloReportePapeleriaPendiente();
                            elemento.Empresa = empresa.Descripcion;
                            elemento.EmpresaID = empresa.Empresaid;

                            elemento.EquipoID = row["EquipoID"].ToString();
                            elemento.Equipo = row["Equipo"].ToString();
                            elemento.EquipoCodigo = int.Parse(row["CodigoEquipo"].ToString());

                            elemento.ZonaID = row["ZonaID"].ToString();
                            elemento.Zona = row["Zona"].ToString();
                            elemento.ZonaCodigo = int.Parse(row["ZonaCodigo"].ToString());

                            elemento.VendedorID = row["VendedorID"].ToString();
                            elemento.Vendedor = row["NombreVendedor"].ToString();
                            elemento.CorreoVendedor = row["CorreoVendedor"].ToString();
                            elemento.CodigoVendedor = int.Parse(row["CodigoVendedor"].ToString());


                            elemento.ClienteID = row["ClienteID"].ToString();
                            elemento.Codigo = int.Parse(row["Codigo"].ToString());
                            elemento.Nombre = row["NombreCompleto"].ToString();


                            elemento.CesionID = row["CesionID"].ToString();
                            elemento.Folio = int.Parse(row["Folio"].ToString());
                            elemento.Fecha_Docu = DateTime.Parse(row["Fecha_Docu"].ToString());
                            elemento.FechaRevision = DateTime.Parse(fechaInicial);
                            elemento.Financiamiento = decimal.Parse(row["Financiamiento"].ToString());

                            //Obtenemos la calificacion de operacion para este cliente y esta operacion
                            //ModeloCalificacionOperacion calOperacion = MobileBO.ControlOperacion.ObtenerCalificacionPapeleria(empresa.Empresaid, row["ClienteID"].ToString(), DateTime.Parse(fechaInicial).AddYears(-1));
                            ModeloCalificacionOperacion calOperacion = MobileBO.ControlOperacion.ObtenerCalificacionPapeleria(row["ClienteID"].ToString(), DateTime.Parse(fechaInicial).AddYears(-1));
                            elemento.Promedio = calOperacion.CalGeneral;

                            //Quitar domingos y dias festivos
                            int CantDomingos = 0, DiasFestivos = 0;
                            List<Entity.Operacion.Catdiasfestivo> lstDiasFestivos = MobileBO.ControlOperacion.TraerCatdiasfestivosPorEjercicio(DateTime.Today.Year);
                            lstDiasFestivos.AddRange(MobileBO.ControlOperacion.TraerCatdiasfestivosPorEjercicio(DateTime.Today.AddYears(1).Year));
                            CantDomingos = ((DateTime.Parse(fechaInicial) - elemento.Fecha_Docu).Days + ((int)elemento.Fecha_Docu.DayOfWeek)) / 7;
                            DiasFestivos = lstDiasFestivos.FindAll(x => DateTime.Parse(x.Fechaasueto) >= elemento.Fecha_Docu && DateTime.Parse(x.Fechaasueto) <= DateTime.Parse(fechaInicial)).Count();
                            elemento.Dias = (DateTime.Parse(fechaInicial) - elemento.Fecha_Docu).Days;
                            elemento.Dias = (elemento.Dias >= CantDomingos ? elemento.Dias - CantDomingos : 0);
                            elemento.Dias = (elemento.Dias >= DiasFestivos ? elemento.Dias - DiasFestivos : 0);

                            //Preguntamos si el cliente ya entrego algo de papeleria o no ha entregado nada.
                            Entity.Operacion.Cesionespapeleria papeleria = MobileBO.ControlOperacion.TraerCesionespapeleriaPorCesionID(row["CesionID"].ToString());
                            //No ha entregado nada agregamos el registo a la lista de resultados.
                            if (papeleria == null)
                            {
                                if (empresa.Empresa == 1)
                                {
                                    elemento.DocumentosPendientes = "PAGARE, CHEQUE Y CONTRATO";
                                    if (decimal.Parse(row["Financiamiento"].ToString()) - decimal.Parse(row["Abono"].ToString()) <= 10)
                                        elemento.DocumentosPendientes = "CONTRATO";
                                }
                                else
                                {
                                    elemento.DocumentosPendientes = "PAGARE, CHEQUE Y FACTURA ENDOSADA";
                                }
                                ListaDatosReporte.Add(elemento);
                            }
                            else
                            {
                                //Aun no esta completa la papeleria, verificamos que hace falta y agregamos el registro de la cesion a la lista de resultados.
                                if (papeleria.Estatus != 3)
                                {
                                    List<string> docpdte = new List<string>();

                                    elemento.DocumentosPendientes = "";
                                    if (papeleria.Fechapagare == null)
                                        docpdte.Add("PAGARE");
                                    if (papeleria.Fechacheque == null)
                                        docpdte.Add("CHEQUE");
                                    if (papeleria.Fechafacturacontrato == null)
                                    {
                                        if (empresa.Empresa == 1)
                                            docpdte.Add("CONTRATO");
                                        else
                                            docpdte.Add("FACTURA ENDOSADA");
                                    }
                                    if (docpdte.Count == 1)
                                        elemento.DocumentosPendientes = docpdte[0];
                                    if (docpdte.Count == 2)
                                        elemento.DocumentosPendientes = docpdte[0] + " Y " + docpdte[1];
                                    if (docpdte.Count == 3)
                                        elemento.DocumentosPendientes = docpdte[0] + ", " + docpdte[1] + " Y " + docpdte[2];

                                    if (empresa.Empresa == 1)
                                    {
                                        if (decimal.Parse(row["Financiamiento"].ToString()) - decimal.Parse(row["Abono"].ToString()) <= 10)
                                        {
                                            //Cesiones Pagadas de factur
                                            if (papeleria.Fechafacturacontrato == null)
                                            {
                                                elemento.DocumentosPendientes = "CONTRATO";
                                                ListaDatosReporte.Add(elemento);
                                            }
                                        }
                                        else
                                        {
                                            //Cesiones que no estan pagagas se agregan por default sin importar que tipo de papeleria es la que le hace falta.
                                            ListaDatosReporte.Add(elemento);
                                        }
                                    }
                                    else if (empresa.Empresa == 2)
                                        ListaDatosReporte.Add(elemento);
                                }
                            }
                        }

                        //Aplicamos el primer filtro de vendedores
                        ListaDatosReporte = ListaDatosReporte.FindAll(x => x.CodigoVendedor >= VendInicial && x.CodigoVendedor <= VendFinal);

                        //Aplicamos el segundo filtro de equipos
                        if (Equipo != 0)
                            ListaDatosReporte = ListaDatosReporte.FindAll(x => x.EquipoCodigo == Equipo);

                        //Aplicamos el tercer filtro que es la zona
                        if (Zona != 0)
                            ListaDatosReporte = ListaDatosReporte.FindAll(x => x.ZonaCodigo == Zona);


                        DataSet dsempresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(empresa.Empresaid);
                        dsempresa.Tables[0].TableName = "DatosEmpresa";

                        DataSet dsDatosRpt = new DataSet();
                        dsDatosRpt.Tables.Add(ListaDatosReporte.ToDataTable());
                        dsDatosRpt.Tables[0].TableName = "DatosReporte";
                        dsDatosRpt.Tables.Add(dsempresa.Tables[0].Copy());
                        

                        var gpoVendedorDoctos =
                            from Vendedor in dsDatosRpt.Tables[0].AsEnumerable()
                            group Vendedor
                            by new { VendedorID = Vendedor.Field<string>("VendedorID") }
                            into eGroup
                            select new
                            {
                                VendedorID = eGroup.Key.VendedorID
                            };
                        
                        foreach (var gvD in gpoVendedorDoctos)
                        {
                            var RegistrosD = from Vendedor in dsDatosRpt.Tables[0].AsEnumerable()
                                             where Vendedor.Field<string>("VendedorID") == gvD.VendedorID
                                             select new
                                             {
                                                 Folio = Vendedor.Field<int>("Folio"),
                                                 Fecha_Docu = Vendedor.Field<DateTime>("Fecha_Docu"),
                                                 Financiamiento = Vendedor.Field<decimal>("Financiamiento"),
                                                 Codigo = Vendedor.Field<int>("Codigo"),
                                                 Nombre = Vendedor.Field<string>("Nombre"),
                                                 CorreoVendedor = Vendedor.Field<string>("CorreoVendedor"),
                                                 DocumentosPendientes = Vendedor.Field<string>("DocumentosPendientes"),
                                                 Dias = Vendedor.Field<int>("Dias"),
                                                 Promedio = Vendedor.Field<decimal>("Promedio")
                                             };

                            sMensajeLargo = "<table>";

                            sMensajeLargo += "<tr>";
                            sMensajeLargo += "<th colspan='7' style='background-color:#C80000; color:white;'><strong>" + sAsunto + "</strong></th>";
                            sMensajeLargo += "</tr>";

                            sMensajeLargo += "<tr>";
                            sMensajeLargo += "<td style='text-align:center; background-color:#C80000; color:white; border:1px solid black; padding:10px'>CESION</td>";
                            sMensajeLargo += "<td style='text-align:center; background-color:#C80000; color:white; border:1px solid black; padding:10px'>FECHA</td>";
                            sMensajeLargo += "<td style='text-align:center; background-color:#C80000; color:white; border:1px solid black; padding:10px'>FINANCIAMIENTO</td>";
                            sMensajeLargo += "<td style='text-align:center; background-color:#C80000; color:white; border:1px solid black; padding:10px'>CLIENTE</td>";
                            sMensajeLargo += "<td style='text-align:center; background-color:#C80000; color:white; border:1px solid black; padding:10px'>DOCUMENTOS PENDIENTES</td>";
                            sMensajeLargo += "<td style='text-align:center; background-color:#C80000; color:white; border:1px solid black; padding:10px'>DIAS</td>";
                            sMensajeLargo += "<td style='text-align:center; background-color:#C80000; color:white; border:1px solid black; padding:10px'>PROMEDIO</td>";

                            sMensajeLargo += "</tr>";

                            int iCont = 0;
                            foreach (var alertasDoctos in RegistrosD)
                            {
                                sCorreo = alertasDoctos.CorreoVendedor.ToString();
                                sMensajeLargo += "<tr>";

                                sMensajeLargo += "<td style='border:1px solid black; padding:2px'>" + alertasDoctos.Folio.ToString() + "</td>";
                                sMensajeLargo += "<td style='text-align:center; border:1px solid black; padding:2px'>" + alertasDoctos.Fecha_Docu.ToShortDateString() + "</td>";
                                sMensajeLargo += "<td style='text-align:center; border:1px solid black; padding:2px'>$" + alertasDoctos.Financiamiento + "</td>";
                                sMensajeLargo += "<td style='text-align:center; border:1px solid black; padding:2px'>" + alertasDoctos.Codigo.ToString() + " - " +alertasDoctos.Nombre.ToString() +"</td>";
                                sMensajeLargo += "<td style='text-align:center; border:1px solid black; padding:2px'>" + alertasDoctos.DocumentosPendientes.ToString() + "</td>";
                                sMensajeLargo += "<td style='text-align:center; border:1px solid black; padding:2px'>" + alertasDoctos.Dias.ToString() + "</td>";
                                sMensajeLargo += "<td style='text-align:center; border:1px solid black; padding:2px'>" + alertasDoctos.Promedio.ToString() + "</td>";

                                sMensajeLargo += "</tr>";

                                iCont++;
                            }//FOR REGISTROS
                            sMensajeLargo += "</table>";
                            sMensajeLargo += "<br />";
                            sMensajeLargo += "<br />";

                            //sCorreo = "faraujo@balor.mx";
                            lsCorreoCC = sCorreoCC.Split(',').ToList();

                            MobileBO.ControlConfiguracion.GenerarNotificacionVencidosSoloCorreo(sCorreo, lsCorreoCC, sAsunto, sMensajeLargo, false);

                        }//FOR VENDEDORES AVISO DE CLIENTES B O MAYOR POR VENCER

                    }
                }


            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
