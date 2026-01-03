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
    public partial class ProgramacionDePagos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<List<Entity.Contabilidad.Catsolicitantespago>> TraerSolicitantes()
        {
            try
            {
                List<Entity.Contabilidad.Catsolicitantespago> lst = MobileBO.ControlContabilidad.TraerCatsolicitantespago();

                return Entity.Response<List<Entity.Contabilidad.Catsolicitantespago>>.CrearResponse<List<Entity.Contabilidad.Catsolicitantespago>>(true, lst.OrderBy(x => x.Solicitante).ToList());
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.Contabilidad.Catsolicitantespago>>.CrearResponseVacio<List<Entity.Contabilidad.Catsolicitantespago>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<object> TraerPagosProgramados(string empresaid, string fi, string ff)
        {
            DateTime FI;
            DateTime FF;
            DateTime semanaIni, semanaFin, nfecha;
            nfecha = DateTime.Today;

            DayOfWeek diaSemana = nfecha.DayOfWeek;
            int diaLunes = DayOfWeek.Monday - diaSemana;
            if (diaLunes > 0) { diaLunes -= 7; }
            semanaIni = nfecha.AddDays(diaLunes);

            int diaDomingo = (DayOfWeek.Sunday - diaSemana);
            if (diaDomingo < 0) { diaDomingo += 7; }
            semanaFin = nfecha.AddDays(diaDomingo);


            try { FI = DateTime.Parse(fi); }
            catch { FI = semanaIni; }

            try { FF = DateTime.Parse(ff); }
            catch { FF = semanaFin; }

            try
            {
                DataSet ds = MobileBO.ControlContabilidad.TraerDatosProgramacionPagos(empresaid, FI, FF);

                var lstPagos = (from a in ds.Tables[0].AsEnumerable()
                                orderby a.Field<DateTime>("FechaProgramada")
                                select new
                                {
                                    Pagoid = a.Field<Guid>("Programacionpagoid").ToString(),
                                    Nombre = a.Field<string>("Nombre").ToString(),
                                    FechaProgramada = a.Field<DateTime>("FechaProgramada").ToShortDateString(),
                                    Factura = a.Field<string>("Factura").ToString(),
                                    Concepto = a.Field<string>("Concepto").ToString(),
                                    Importe = decimal.Parse(a.Field<decimal>("Importe").ToString()),
                                    Solicitante = a.Field<string>("Solicitante").ToString(),
                                }).ToList();

                return Entity.Response<object>.CrearResponse<object>(true, lstPagos);
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<object> TraerTodosPagosProgramados(string empresaid)
        {
            try
            {
                DataSet ds = MobileBO.ControlContabilidad.TraerDatosProgramacionPagos(empresaid, null, null);

                var lstPagos = (from a in ds.Tables[0].AsEnumerable()
                                orderby a.Field<DateTime>("FechaProgramada")
                                select new
                                {
                                    Pagoid = a.Field<Guid>("Programacionpagoid").ToString(),
                                    Nombre = a.Field<string>("Nombre").ToString(),
                                    FechaProgramada = a.Field<DateTime>("FechaProgramada").ToShortDateString(),
                                    Factura = a.Field<string>("Factura").ToString(),
                                    Concepto = a.Field<string>("Concepto").ToString(),
                                    Importe = decimal.Parse(a.Field<decimal>("Importe").ToString()),
                                    Solicitante = a.Field<string>("Solicitante").ToString(),
                                }).ToList();

                return Entity.Response<object>.CrearResponse<object>(true, lstPagos);
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<object> EliminarPagoProgramado(string empresaid, string programacionpagoid, string usuario)
        {
            try
            {
                Entity.Contabilidad.Programacionpago pago = MobileBO.ControlContabilidad.TraerProgramacionpagos(empresaid, programacionpagoid);
                pago.Estatus = 2;
                pago.Usuario = usuario;
                pago.Fecha = DateTime.Now;

                MobileBO.ControlContabilidad.GuardarProgramacionpago(new List<Entity.Contabilidad.Programacionpago>() { pago });

                DataSet ds = MobileBO.ControlContabilidad.TraerDatosProgramacionPagos(empresaid, null, null);

                var lstPagos = (from a in ds.Tables[0].AsEnumerable()
                                select new
                                {
                                    Pagoid = a.Field<Guid>("Programacionpagoid").ToString(),
                                    Nombre = a.Field<string>("Nombre").ToString(),
                                    FechaProgramada = a.Field<DateTime>("FechaProgramada").ToShortDateString(),
                                    Factura = a.Field<string>("Factura").ToString(),
                                    Concepto = a.Field<string>("Concepto").ToString(),
                                    Importe = decimal.Parse(a.Field<decimal>("Importe").ToString()),
                                    Solicitante = a.Field<string>("Solicitante").ToString(),
                                }).ToList();

                return Entity.Response<object>.CrearResponse<object>(true, lstPagos);
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

        [WebMethod]
        //public static Entity.Response<object> GuardarPago(string programacionpagoid, string empresaid, string cuenta, string proveedorid, string factura, string fechapago, string concepto, string importe, string solicitante, string usuario)
        public static Entity.Response<object> GuardarPago(string programacionpagoid, string empresaid, string proveedorid, string fechapago, string concepto, string importe, string solicitante, string usuario)
        {
            try
            {
                Entity.Contabilidad.Programacionpago pago = MobileBO.ControlContabilidad.TraerProgramacionpagos(empresaid, programacionpagoid);
                if (pago != null)
                {
                    pago.Proveedorid = proveedorid;
                    pago.Factura = "";
                    pago.Fechaprogramada = DateTime.Parse(fechapago);
                    pago.Concepto = concepto;
                    pago.Importe = decimal.Parse(importe);
                    pago.Solicitanteid = solicitante;
                    pago.Usuario = usuario;
                    pago.Fecha = DateTime.Today;
                }
                else
                {
                    pago = new Entity.Contabilidad.Programacionpago();
                    pago.Programacionpagoid = Guid.Empty.ToString();
                    pago.Empresaid = empresaid;
                    pago.Proveedorid = proveedorid;
                    pago.Factura = "";
                    pago.Fechaprogramada = DateTime.Parse(fechapago);
                    pago.Concepto = concepto;
                    pago.Importe = decimal.Parse(importe);
                    pago.Solicitanteid = solicitante;
                    pago.Usuario = usuario;
                    pago.Fecha = DateTime.Today;
                    pago.Estatus = 1;
                    pago.UltimaAct = 0;
                }

                MobileBO.ControlContabilidad.GuardarProgramacionpago(new List<Entity.Contabilidad.Programacionpago>() { pago });

                DataSet ds = MobileBO.ControlContabilidad.TraerDatosProgramacionPagos(empresaid, null, null);

                var lstPagos = (from a in ds.Tables[0].AsEnumerable()
                                orderby a.Field<DateTime>("FechaProgramada")
                                select new
                                {
                                    Pagoid = a.Field<Guid>("Programacionpagoid").ToString(),
                                    Nombre = a.Field<string>("Nombre").ToString(),
                                    FechaProgramada = a.Field<DateTime>("FechaProgramada").ToShortDateString(),
                                    Factura = a.Field<string>("Factura").ToString(),
                                    Concepto = a.Field<string>("Concepto").ToString(),
                                    Importe = decimal.Parse(a.Field<decimal>("Importe").ToString()),
                                    Solicitante = a.Field<string>("Solicitante").ToString(),
                                }).ToList();



                return Entity.Response<object>.CrearResponse<object>(true, lstPagos);
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<object> AsignarPago(string empresaid, string programacionpagoid, string polizaid, string usuario)
        {
            try
            {
                Entity.Contabilidad.Programacionpago pago = MobileBO.ControlContabilidad.TraerProgramacionpagos(empresaid, programacionpagoid);
                pago.Polizaid = polizaid;
                pago.Fechapagada = DateTime.Now;
                pago.Usuario = usuario;
                pago.Fecha = DateTime.Now;

                MobileBO.ControlContabilidad.GuardarProgramacionpago(new List<Entity.Contabilidad.Programacionpago>() { pago });

                DataSet ds = MobileBO.ControlContabilidad.TraerDatosProgramacionPagos(empresaid, null, null);

                var lstPagos = (from a in ds.Tables[0].AsEnumerable()
                                orderby a.Field<DateTime>("FechaProgramada")
                                select new
                                {
                                    Pagoid = a.Field<Guid>("Programacionpagoid").ToString(),
                                    Nombre = a.Field<string>("Nombre").ToString(),
                                    FechaProgramada = a.Field<DateTime>("FechaProgramada").ToShortDateString(),
                                    Factura = a.Field<string>("Factura").ToString(),
                                    Concepto = a.Field<string>("Concepto").ToString(),
                                    Importe = decimal.Parse(a.Field<decimal>("Importe").ToString()),
                                    Solicitante = a.Field<string>("Solicitante").ToString(),
                                }).ToList();

                return Entity.Response<object>.CrearResponse<object>(true, lstPagos);
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }


        [WebMethod]
        public static Entity.Response<int> ValidaPolizaProgamada(string empresaid, string polizaid)
        {
            try
            {
                DataSet ds = MobileBO.ControlContabilidad.ValidaPolizaPagoProgramado(empresaid, polizaid);
                int estatus = int.Parse(ds.Tables[0].Rows[0]["Estatus"].ToString());
               
                return Entity.Response<int>.CrearResponse<int>(true, estatus);
            }
            catch (Exception ex)
            {
                return Entity.Response<int>.CrearResponseVacio<int>(false, ex.Message);
            }
        }
    }

    public class ControladorReporteListadoPagos : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            string empresaid = parametros.Get("empresaid");

            try
            {
                DataSet ds = MobileBO.ControlContabilidad.TraerListadoPagosProgramados(empresaid);
                ds.Tables[0].TableName = "Datos";
                DataSet empresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull((empresaid == null || empresaid == string.Empty ? null : empresaid));
                empresa.Tables[0].TableName = "Empresa";
                ds.Tables.Add(empresa.Tables[0].Copy());

                base.NombreReporte = "PagoProgramadoListado";
                base.FormatoReporte = 3;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\ListadoPagosProgramados.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }

    public class ControladorReporteRelacionPagosPagosProgramados : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            string fechaInicial = parametros.Get("fechaInicial");
            string fechaFin = parametros.Get("fechaFin");
            string empresaid = parametros.Get("empresaid");

            DateTime semanaIni, semanaFin, nfecha;
            nfecha = DateTime.Today;

            DateTime FI;
            DateTime FF;
            DayOfWeek diaSemana = nfecha.DayOfWeek;
            int diaLunes = DayOfWeek.Monday - diaSemana;
            if (diaLunes > 0) { diaLunes -= 7; }
            semanaIni = nfecha.AddDays(diaLunes);

            int diaDomingo = (DayOfWeek.Sunday - diaSemana);
            if (diaDomingo < 0) { diaDomingo += 7; }
            semanaFin = nfecha.AddDays(diaDomingo);

            try { FI = DateTime.Parse(fechaInicial); }
            catch { FI = semanaIni; }

            try { FF = DateTime.Parse(fechaFin); }
            catch { FF = semanaFin; }

            try
            {
                DataSet ds = MobileBO.ControlContabilidad.TraerRelacionPagosProgramados(FI, FF, empresaid);
                ds.Tables[0].TableName = "Datos";
                ds.Tables[1].TableName = "Concentrado";
                DataSet empresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull((empresaid == null || empresaid == string.Empty ? null : empresaid));
                empresa.Tables[0].TableName = "Empresa";
                ds.Tables.Add(empresa.Tables[0].Copy());

                DataTable dt = new DataTable();
                dt.TableName = "Parametros";
                dt.Columns.Add("FechaInicio", typeof(DateTime));
                dt.Columns.Add("FechaFin", typeof(DateTime));
                DataRow row = dt.NewRow();
                row[0] = FI;
                row[1] = FF;
                dt.Rows.Add(row);
                ds.Tables.Add(dt);

                base.NombreReporte = "PagoProgramadoRelacion";
                base.FormatoReporte = 3;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\RelacionPagosProgramados.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}