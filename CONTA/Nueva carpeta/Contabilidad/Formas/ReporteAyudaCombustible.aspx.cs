using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class ReporteAyudaCombustible : System.Web.UI.Page
    {
        [WebMethod]
        public static Entity.Response<List<object>> TraeAños()
        {
            List<object> _anios = new List<object>();
            try
            {

                DateTime _date = new DateTime();
                List<Entity.Ventas.Comision> _comisiones = MobileBO.ControlVentas.TraerComisiones(null, null, null);
                if (_comisiones.Count == 0)
                {
                    var obj = new
                    {
                        anio = _date.Year
                    };
                    _anios.Add(obj);
                }
                else
                {
                    foreach (Entity.Ventas.Comision c in _comisiones)
                    {
                        var obj = new
                        {
                            anio = Convert.ToDateTime(c.Año).Year
                        };
                        _anios.Add(obj);
                    };
                }

                _anios.Add(new { anio = _date.AddYears(1).Year });

                return Entity.Response<List<object>>.CrearResponse<List<object>>(true, _anios);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<object>>.CrearResponseVacio<List<object>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<object> ValidaFecha(int anio, int mes)
        {
            try
            {
                mes++;
                object _obj = new object();
                bool _existe = false;
                List<Entity.Ventas.Comision> _comisiones = new List<Entity.Ventas.Comision>();
                _comisiones = MobileBO.ControlVentas.TraerComisiones(null, anio, mes);
                foreach (Entity.Ventas.Comision c in _comisiones)
                {
                    if (c.Estatus == 1)
                    {
                        _existe = true;

                    }
                }

                var _objres = new object();

                if (_existe)
                {
                    var obj = new { Existe = 1 };
                    _objres = obj;
                }
                else
                {
                    var obj = new { Existe = 0 };
                    _objres = obj;
                }

                return Entity.Response<object>.CrearResponse<object>(true, _objres);
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }


        [WebMethod]
        public static Entity.Response<object> TraerComisiones(string fechaini, string fechafin, string empresaid, string vendedorid)
        {
            try
            {
                //mes++;

                List<Entity.Ventas.Comisionesc1> _comC1 = new List<Entity.Ventas.Comisionesc1>();
                List<Entity.Ventas.Comisionesb1> _comB1 = new List<Entity.Ventas.Comisionesb1>();
                List<Entity.Ventas.Comisionesb1a1pagosnuevoscliente> _comB1A1 = new List<Entity.Ventas.Comisionesb1a1pagosnuevoscliente>();
                List<Entity.Ventas.Comisionesa1clientesrecurrente> _comA1Recurrente = new List<Entity.Ventas.Comisionesa1clientesrecurrente>();
                List<Entity.Ventas.Comisionesgerente> _comGerentes = new List<Entity.Ventas.Comisionesgerente>();

                //if (mes<1 || mes > 12)
                //{
                //    return Entity.Response<object>.CrearResponseVacio<object>(false, "mes no válido");
                //}

                DateTime FechaIni = DateTime.Parse(fechaini);
                DateTime FechaFin = DateTime.Parse(fechafin);

                //DateTime fechaini = new DateTime(anio, mes, 1);

                //int _ultimodiadelmes = DateTime.DaysInMonth(anio, mes);
                //DateTime fechafin = new DateTime(anio, mes, _ultimodiadelmes);

                if (empresaid != null)
                {
                    if (empresaid.Trim() == "" || empresaid.Trim() == "*")
                    {
                        empresaid = null;
                    }
                }

                if (vendedorid != null)
                {
                    if (vendedorid.Trim() == "" || vendedorid.Trim() == "*")
                    {
                        vendedorid = null;
                    }
                }

                object _obj = new object();
                bool _existe = false;
                DataSet ds = MobileBO.ControlVentas.GenerarComisiones(FechaIni, FechaFin, empresaid, vendedorid);

                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    Entity.Ventas.Comisionesc1 c1 = new Entity.Ventas.Comisionesc1();
                    c1.Candidatoid = r["CandidatoID"].ToString();
                    //c1.Comisionc1id = r["ComisionC1ID"].ToString();
                    c1.Comisionid = r["ComisionesID"].ToString();
                    c1.Comisionvendedor = Convert.ToDecimal(r["ComisionVendedor"]);
                    c1.Contactoid = r["ContactoID"].ToString();
                    //c1.Estatus = Convert.ToInt32(r["Estatus"]);
                    //c1.Fecha = Convert.ToDateTime(r["Fecha"]);
                    c1.Incentivoconcepto = r["IncentivoConcepto"].ToString();
                    c1.Incentivoid = r["IncentivoID"].ToString();
                    c1.Nombrecandidato = r["NombreCandidato"].ToString();
                    c1.Nombrevendedor = r["NombreVendedor"].ToString();
                    c1.Nombrevendedorasignado = r["NombreVendedorAsignado"].ToString();
                    //c1.UltimaAct = Convert.ToInt32(r["UltimaAct"]);
                    //c1.Usuario = Convert.ToString(r["Usuario"]);
                    c1.Vendedorasignadoid = Convert.ToString(r["VendedorAsignadoID"]).ToString();
                    c1.Vendedorid = r["VendedorID"].ToString();

                    _comC1.Add(c1);


                    var obj = new
                    {
                        ComisionID = Convert.ToInt32(r["ComisionesID"]),
                        IncentivoID = r["IncentivoID"].ToString(),
                        IncentivoConcepto = r["IncentivoConcepto"].ToString(),
                        VendedorID = r["VendedorID"].ToString(),
                        NombreVendedor = r["NombreVendedor"].ToString(),
                        CandidatoID = r["CandidatoID"].ToString(),
                        NombreCandidato = r["NombreCandidato"].ToString(),
                        Comision = Convert.ToDouble(r["ComisionVendedor"])
                    };
                    //_comC1.Add(obj);
                }

                foreach (DataRow r in ds.Tables[1].Rows)
                {

                    Entity.Ventas.Comisionesb1 b1 = new Entity.Ventas.Comisionesb1();

                    b1.Clienteid = r["ClienteID"].ToString();
                    //b1.Comisionb11id = r["ComisionB11ID"].ToString();
                    b1.Comisionid = r["ComisionesID"].ToString();
                    b1.Comisionvendedor = Convert.ToDecimal(r["ComisionVendedor"]);
                    b1.Contratoanualid = r["ContratoAnualID"].ToString();
                    b1.Empresa = r["Empresa"].ToString();
                    b1.Empresaid = r["EmpresaID"].ToString();
                    b1.Nombreempresa = r["NombreEmpresa"].ToString();
                    //b1.Estatus = Convert.ToInt16(r["Estatus"]);
                    //b1.Fecha = Convert.ToDateTime(r["Fecha"]);
                    b1.Fechainicio = Convert.ToDateTime(r["FechaInicio"]);
                    b1.Importelinea = Convert.ToDecimal(r["ImporteLinea"]);
                    b1.Incentivoconcepto = r["IncentivoConcepto"].ToString();
                    b1.Incentivoid = r["IncentivoID"].ToString();
                    b1.Numerocontrato = Convert.ToInt32(r["NumeroContrato"]);
                    b1.Pagareanual = r["PagareAnual"].ToString();
                    b1.Rangofin = Convert.ToDecimal(r["RangoFin"]);
                    b1.Rangoini = Convert.ToDecimal(r["RangoIni"]);
                    b1.Tabulador = r["Tabulador"].ToString();
                    b1.Tabuladorcomisionid = Convert.ToInt32(r["TabuladorComisionID"]);
                    b1.Tabuladorid = Convert.ToInt32(r["TabuladorID"]);
                    //b1.UltimaAct = Convert.ToInt32(r["UltimaAct"]);
                    //b1.Usuario = r["Usuario"].ToString();
                    b1.Vendedor = r["Vendedor"].ToString();
                    b1.Vendedorid = r["VendedorID"].ToString();
                    b1.Vigenciameses = Convert.ToInt32(r["VigenciaMeses"]);
                    _comB1.Add(b1);


                    //var obj = new
                    //{
                    //    ComisionID = Convert.ToInt32(r["ComisionesID"]),
                    //    IncentivoID = r["IncentivoID"].ToString(),
                    //    IncentivoConcepto = r["IncentivoConcepto"].ToString(),
                    //    VendedorID = r["VendedorID"].ToString(),
                    //    NombreVendedor = r["Vendedor"].ToString(),
                    //    ClienteID = r["ClienteID"].ToString(),
                    //    NombreCandidato = r["Empresa"].ToString(),
                    //    PagareAnual = r["PagareAnual"].ToString(),
                    //    ContratoID = r["ContratoAnual"].ToString(),
                    //    FechaInicio = r["FechaInicio"].ToString(),
                    //    NumeroContrato = Convert.ToInt32(r["NumeroContrato"]),
                    //    VigenciaMeses = Convert.ToInt32(r["VigenciaMenes"]),
                    //    ImporteLinea = Convert.ToDouble(r["ImporteLinea"]),
                    //    RangoInicial = Convert.ToDouble(r["RangoIni"]),
                    //    RangoFinal = Convert.ToDouble(r["RangoFin"]),
                    //    Comision = Convert.ToDouble(r["ComisionVendedor"])
                    //};
                    //_comB1.Add(obj);
                }

                foreach (DataRow r in ds.Tables[2].Rows)
                {

                    Entity.Ventas.Comisionesb1a1pagosnuevoscliente comb1a1 = new Entity.Ventas.Comisionesb1a1pagosnuevoscliente();

                    comb1a1.Cesiondetalleid = r["CesionDetalleID"].ToString();
                    comb1a1.Cesionid = r["CesionID"].ToString();
                    comb1a1.Clienteid = r["ClienteID"].ToString();
                    comb1a1.Comision = Convert.ToDecimal(r["Comision"]);
                    //comb1a1.Comisionb1a1id = r["ComisionB1A1ID"].ToString();
                    //comb1a1.Comisionid = r["ComisionID"].ToString();
                    comb1a1.Comisionvendedor = Convert.ToDecimal(r["ComisionVendedor"]);
                    comb1a1.Empresaid = r["EmpresaID"].ToString();
                    comb1a1.Nombreempresa = r["NombreEmpresa"].ToString();
                    comb1a1.Foliocesion = Convert.ToInt32(r["Folio"]);
                    //comb1a1.Estatus = Convert.ToInt32(r["Estatus"]);
                    //comb1a1.Fecha = Convert.ToDateTime(r["Fecha"]);
                    comb1a1.FechaApli = Convert.ToDateTime(r["Fecha_Apli"]);
                    comb1a1.Incentivoconcepto = r["IncentivoConcepto"].ToString();
                    comb1a1.Incentivoid = r["IncentivoID"].ToString();
                    comb1a1.Nombrecompleto = r["NombreCompleto"].ToString();
                    comb1a1.Tipo = r["Tipo"].ToString();
                    comb1a1.TipoMov = Convert.ToInt32(r["Tipo_Mov"]);
                    //comb1a1.UltimaAct = Convert.ToInt32(r["UltimaAct"]);
                    //comb1a1.Usuario = r["Usuario"].ToString();
                    comb1a1.Vendedor = r["Vendedor"].ToString();
                    comb1a1.Vendedorid = r["VendedorID"].ToString();
                    _comB1A1.Add(comb1a1);


                    //var obj = new
                    //{
                    //    ComisionID = Convert.ToInt32(r["ComisionesID"]),
                    //    IncentivoID = r["IncentivoID"].ToString(),
                    //    IncentivoConcepto = r["IncentivoConcepto"].ToString(),
                    //    VendedorID = r["VendedorID"].ToString(),
                    //    NombreVendedor = r["Vendedor"].ToString(),
                    //    ClienteID = r["ClienteID"].ToString(),
                    //    NombreCandidato = r["Empresa"].ToString(),
                    //    PagareAnual = r["PagareAnual"].ToString(),
                    //    ContratoID = r["ContratoAnual"].ToString(),
                    //    FechaInicio = r["FechaInicio"].ToString(),
                    //    NumeroContrato = Convert.ToInt32(r["NumeroContrato"]),
                    //    VigenciaMeses = Convert.ToInt32(r["VigenciaMenes"]),
                    //    ImporteLinea = Convert.ToDouble(r["ImporteLinea"]),
                    //    RangoInicial = Convert.ToDouble(r["RangoIni"]),
                    //    RangoFinal = Convert.ToDouble(r["RangoFin"]),
                    //    Comision = Convert.ToDouble(r["ComisionVendedor"])
                    //};
                    //_comB1A1.Add(obj);
                }


                foreach (DataRow r in ds.Tables[3].Rows)
                {

                    Entity.Ventas.Comisionesa1clientesrecurrente coma1 = new Entity.Ventas.Comisionesa1clientesrecurrente();

                    //coma1.Cesiondetalleid = r["CesionDetalleID"].ToString();
                    coma1.Cesionid = r["CesionID"].ToString();
                    coma1.Clienteid = r["ClienteID"].ToString();
                    coma1.Comision = Convert.ToDecimal(r["Comision"]);
                    //coma1.Comisiona1clienterecurrenteid = r["ComisionA1ClienteRecurrenteID"].ToString();
                    //coma1.Comisionid = r["ComisionID"].ToString();
                    coma1.Comisionvendedor = Convert.ToDecimal(r["ComisionVendedor"]);
                    coma1.Empresaid = r["EmpresaID"].ToString();
                    coma1.Nombreempresa = r["NombreEmpresa"].ToString();
                    coma1.Foliocesion = Convert.ToInt32(r["Folio"]);
                    //coma1.Estatus = Convert.ToInt32(r["Estatus"]);
                    //coma1.Fecha = Convert.ToDateTime(r["Fecha"]);
                    coma1.FechaApli = Convert.ToDateTime(r["Fecha_Apli"]);
                    coma1.Incentivoconcepto = r["IncentivoConcepto"].ToString();
                    coma1.Incentivoid = r["IncentivoID"].ToString();
                    coma1.Nombrecompleto = r["NombreCompleto"].ToString();
                    coma1.Tipo = r["Tipo"].ToString();
                    //coma1.UltimaAct = Convert.ToInt32(r["UltimaAct"]);
                    //coma1.Usuario = r["Usuario"].ToString();
                    coma1.Vendedor = r["Vendedor"].ToString();
                    coma1.Vendedorid = r["VendedorID"].ToString();
                    coma1.Calificacionconstacia = MobileBO.ControlOperacion.TraerCalificacionClienteSeparada(coma1.Clienteid, coma1.FechaApli).califConstancia;
                    coma1.Calificacionrecalificacion = MobileBO.ControlOperacion.TraerCalificacionClienteSeparada(coma1.Clienteid, coma1.FechaApli).califReanalisis;
                    coma1.TipoMov = r["Tipo_Mov"].ToString();

                    //if(Convert.ToInt32(coma1.Calificacionconstacia)<=3 && Convert.ToInt32(coma1.Calificacionrecalificacion) <= 12)
                    if (Convert.ToInt32(coma1.Calificacionconstacia) <= 3 && Convert.ToInt32(coma1.Calificacionrecalificacion) <= 13)
                    {
                        _comA1Recurrente.Add(coma1);
                    }




                    //var obj = new
                    //{
                    //    ComisionID = Convert.ToInt32(r["ComisionesID"]),
                    //    IncentivoID = r["IncentivoID"].ToString(),
                    //    IncentivoConcepto = r["IncentivoConcepto"].ToString(),
                    //    VendedorID = r["VendedorID"].ToString(),
                    //    NombreVendedor = r["Vendedor"].ToString(),
                    //    ClienteID = r["ClienteID"].ToString(),
                    //    NombreCandidato = r["Empresa"].ToString(),
                    //    PagareAnual = r["PagareAnual"].ToString(),
                    //    ContratoID = r["ContratoAnual"].ToString(),
                    //    FechaInicio = r["FechaInicio"].ToString(),
                    //    NumeroContrato = Convert.ToInt32(r["NumeroContrato"]),
                    //    VigenciaMeses = Convert.ToInt32(r["VigenciaMenes"]),
                    //    ImporteLinea = Convert.ToDouble(r["ImporteLinea"]),
                    //    RangoInicial = Convert.ToDouble(r["RangoIni"]),
                    //    RangoFinal = Convert.ToDouble(r["RangoFin"]),
                    //    Comision = Convert.ToDouble(r["ComisionVendedor"])
                    //};
                    //_comB1A1.Add(obj);
                }

                foreach (DataRow r in ds.Tables[4].Rows)
                {
                    Entity.Ventas.Comisionesgerente g = new Entity.Ventas.Comisionesgerente();

                    //g.Comisiongerenteid = r["ComisionGerenteID"].ToString();
                    //g.Comisionid = r["ComisionID"].ToString();
                    g.Comisionvendedor = Convert.ToDecimal(r["ComisionVendedor"]);
                    //g.Estatus = Convert.ToInt32(r["Estatus"]);
                    //g.Fecha = Convert.ToDateTime(r["Fecha"]);
                    g.Gerenteid = r["GerenteID"].ToString();
                    g.Incentivoconcepto = r["IncentivoConcepto"].ToString();
                    g.Incentivoid = r["IncentivoID"].ToString();
                    g.Nombregerente = r["NombreGerente"].ToString();
                    //g.UltimaAct = Convert.ToInt32(r["UltimaAct"]);
                    //g.Usuario = r["Usuario"].ToString();

                    _comGerentes.Add(g);

                }

                Entity.ModeloComisionVendedores comisiones = new Entity.ModeloComisionVendedores();
                comisiones.ComisionesA1 = _comA1Recurrente;
                comisiones.ComisionesA1B1 = _comB1A1;
                comisiones.ComisionesB1 = _comB1;
                comisiones.ComisionesC1 = _comC1;
                comisiones.ComisionesGerente = _comGerentes;


                return Entity.Response<object>.CrearResponse<object>(true, comisiones);
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }

        }


        [WebMethod]
        public static Entity.Response<object> GuardarComisiones(string fechaini, string fechafin, string empresaid, string vendedorid, string usuario)
        {
            try
            {


                List<Entity.Ventas.Comisionesc1> _comC1 = new List<Entity.Ventas.Comisionesc1>();
                List<Entity.Ventas.Comisionesb1> _comB1 = new List<Entity.Ventas.Comisionesb1>();
                List<Entity.Ventas.Comisionesb1a1pagosnuevoscliente> _comB1A1 = new List<Entity.Ventas.Comisionesb1a1pagosnuevoscliente>();
                List<Entity.Ventas.Comisionesa1clientesrecurrente> _comA1Recurrente = new List<Entity.Ventas.Comisionesa1clientesrecurrente>();
                List<Entity.Ventas.Comisionesgerente> _comGerentes = new List<Entity.Ventas.Comisionesgerente>();

                DateTime FechaIni = DateTime.Parse(fechaini);
                DateTime FechaFin = DateTime.Parse(fechafin);

                //Verificamos el rango de fechas para validar en primer lugar si el rango de fechas cae dentro del mismo mes del mismo año
                if ((FechaIni.Year + FechaIni.Month) != (FechaFin.Year + FechaFin.Month))
                {
                    return Entity.Response<object>.CrearResponseVacio<object>(false, "El rango de fechas debe abarcar el mismo mes del mismo año, verifique.");
                }

                //Verificamos el rango de fechas para validar que se grabe el mes completo y no parcial
                if (DateTime.DaysInMonth(FechaFin.Year, FechaFin.Month) != FechaFin.Day)
                {
                    return Entity.Response<object>.CrearResponseVacio<object>(false, "Para grabar las comisiones el rango de fechas debe abarcar el mes completo, verifique.");
                }



                if (empresaid != null)
                {
                    if (empresaid.Trim() == "" || empresaid.Trim() == "*")
                    {
                        empresaid = null;
                    }
                }

                if (vendedorid != null)
                {
                    if (vendedorid.Trim() == "" || vendedorid.Trim() == "*")
                    {
                        vendedorid = null;
                    }
                }

                object _obj = new object();
                bool _existe = false;
                DataSet ds = MobileBO.ControlVentas.GenerarComisiones(FechaIni, FechaFin, empresaid, vendedorid);


                Entity.Ventas.Comision com = new Entity.Ventas.Comision();
                com.Comisionid = Guid.Empty.ToString();
                com.Año = FechaIni.Year;
                com.Mes = FechaIni.Month;
                com.Estatus = 1;
                com.Fecha = DateTime.Now;
                com.Motivocancela = null;
                com.Usuario = usuario;
                com.Usuariocancela = null;


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    Entity.Ventas.Comisionesc1 c1 = new Entity.Ventas.Comisionesc1();

                    c1.Comisionc1id = Guid.Empty.ToString();
                    c1.Candidatoid = r["CandidatoID"].ToString();
                    c1.Comisionid = r["ComisionesID"].ToString();
                    c1.Comisionvendedor = Convert.ToDecimal(r["ComisionVendedor"]);
                    c1.Contactoid = r["ContactoID"].ToString();
                    c1.Estatus = 1;
                    c1.Fecha = DateTime.Now;
                    c1.Incentivoconcepto = r["IncentivoConcepto"].ToString();
                    c1.Incentivoid = r["IncentivoID"].ToString();
                    c1.Nombrecandidato = r["NombreCandidato"].ToString();
                    c1.Nombrevendedor = r["NombreVendedor"].ToString();
                    c1.Nombrevendedorasignado = r["NombreVendedorAsignado"].ToString();
                    c1.Usuario = usuario;
                    c1.Vendedorasignadoid = Convert.ToString(r["VendedorAsignadoID"]).ToString();
                    c1.Vendedorid = r["VendedorID"].ToString();

                    _comC1.Add(c1);
                }

                foreach (DataRow r in ds.Tables[1].Rows)
                {

                    Entity.Ventas.Comisionesb1 b1 = new Entity.Ventas.Comisionesb1();

                    b1.Comisionb11id = Guid.Empty.ToString();
                    b1.Clienteid = r["ClienteID"].ToString();
                    b1.Comisionid = r["ComisionesID"].ToString();
                    b1.Comisionvendedor = Convert.ToDecimal(r["ComisionVendedor"]);
                    b1.Contratoanualid = r["ContratoAnualID"].ToString();
                    b1.Empresa = r["Empresa"].ToString();
                    b1.Empresaid = r["EmpresaID"].ToString();
                    b1.Nombreempresa = r["NombreEmpresa"].ToString();
                    b1.Estatus = 1;
                    b1.Fecha = DateTime.Now;
                    b1.Fechainicio = Convert.ToDateTime(r["FechaInicio"]);
                    b1.Importelinea = Convert.ToDecimal(r["ImporteLinea"]);
                    b1.Incentivoconcepto = r["IncentivoConcepto"].ToString();
                    b1.Incentivoid = r["IncentivoID"].ToString();
                    b1.Numerocontrato = Convert.ToInt32(r["NumeroContrato"]);
                    b1.Pagareanual = r["PagareAnual"].ToString();
                    b1.Rangofin = Convert.ToDecimal(r["RangoFin"]);
                    b1.Rangoini = Convert.ToDecimal(r["RangoIni"]);
                    b1.Tabulador = r["Tabulador"].ToString();
                    b1.Tabuladorcomisionid = Convert.ToInt32(r["TabuladorComisionID"]);
                    b1.Tabuladorid = Convert.ToInt32(r["TabuladorID"]);
                    b1.Usuario = usuario;
                    b1.Vendedor = r["Vendedor"].ToString();
                    b1.Vendedorid = r["VendedorID"].ToString();
                    b1.Vigenciameses = Convert.ToInt32(r["VigenciaMeses"]);
                    _comB1.Add(b1);

                }

                foreach (DataRow r in ds.Tables[2].Rows)
                {

                    Entity.Ventas.Comisionesb1a1pagosnuevoscliente comb1a1 = new Entity.Ventas.Comisionesb1a1pagosnuevoscliente();

                    comb1a1.Comisionb1a1id = Guid.Empty.ToString();
                    comb1a1.Cesiondetalleid = r["CesionDetalleID"].ToString();
                    comb1a1.Cesionid = r["CesionID"].ToString();
                    comb1a1.Clienteid = r["ClienteID"].ToString();
                    comb1a1.Comision = Convert.ToDecimal(r["Comision"]);

                    comb1a1.Comisionvendedor = Convert.ToDecimal(r["ComisionVendedor"]);
                    comb1a1.Empresaid = r["EmpresaID"].ToString();
                    comb1a1.Nombreempresa = r["NombreEmpresa"].ToString();
                    comb1a1.Foliocesion = Convert.ToInt32(r["Folio"]);
                    comb1a1.Estatus = 1;
                    comb1a1.Fecha = DateTime.Now;
                    comb1a1.FechaApli = Convert.ToDateTime(r["Fecha_Apli"]);
                    comb1a1.Incentivoconcepto = r["IncentivoConcepto"].ToString();
                    comb1a1.Incentivoid = r["IncentivoID"].ToString();
                    comb1a1.Nombrecompleto = r["NombreCompleto"].ToString();
                    comb1a1.Tipo = r["Tipo"].ToString();
                    comb1a1.TipoMov = Convert.ToInt32(r["Tipo_Mov"]);
                    comb1a1.Usuario = usuario;
                    comb1a1.Vendedor = r["Vendedor"].ToString();
                    comb1a1.Vendedorid = r["VendedorID"].ToString();
                    _comB1A1.Add(comb1a1);
                }


                foreach (DataRow r in ds.Tables[3].Rows)
                {

                    Entity.Ventas.Comisionesa1clientesrecurrente coma1 = new Entity.Ventas.Comisionesa1clientesrecurrente();


                    coma1.Comisiona1clienterecurrenteid = Guid.Empty.ToString();
                    coma1.Cesionid = r["CesionID"].ToString();
                    coma1.Cesiondetalleid = r["CesionDetalleID"].ToString();
                    coma1.Clienteid = r["ClienteID"].ToString();
                    coma1.Comision = Convert.ToDecimal(r["Comision"]);
                    coma1.Comisionvendedor = Convert.ToDecimal(r["ComisionVendedor"]);
                    coma1.Empresaid = r["EmpresaID"].ToString();
                    coma1.Nombreempresa = r["NombreEmpresa"].ToString();
                    coma1.Foliocesion = Convert.ToInt32(r["Folio"]);
                    coma1.Estatus = 1;
                    coma1.Fecha = DateTime.Now;
                    coma1.FechaApli = Convert.ToDateTime(r["Fecha_Apli"]);
                    coma1.Incentivoconcepto = r["IncentivoConcepto"].ToString();
                    coma1.Incentivoid = r["IncentivoID"].ToString();
                    coma1.Nombrecompleto = r["NombreCompleto"].ToString();
                    coma1.Tipo = r["Tipo"].ToString();
                    coma1.Usuario = usuario;
                    coma1.Vendedor = r["Vendedor"].ToString();
                    coma1.Vendedorid = r["VendedorID"].ToString();
                    coma1.Calificacionconstacia = MobileBO.ControlOperacion.TraerCalificacionClienteSeparada(coma1.Clienteid, coma1.FechaApli).califConstancia;
                    coma1.Calificacionrecalificacion = MobileBO.ControlOperacion.TraerCalificacionClienteSeparada(coma1.Clienteid, coma1.FechaApli).califReanalisis;
                    coma1.TipoMov = r["Tipo_Mov"].ToString();

                    //if (Convert.ToInt32(coma1.Calificacionconstacia) <= 3 && Convert.ToInt32(coma1.Calificacionrecalificacion) <= 12)
                    if (Convert.ToInt32(coma1.Calificacionconstacia) <= 3 && Convert.ToInt32(coma1.Calificacionrecalificacion) <= 13)
                    {
                        _comA1Recurrente.Add(coma1);
                    }

                }

                foreach (DataRow r in ds.Tables[4].Rows)
                {
                    Entity.Ventas.Comisionesgerente g = new Entity.Ventas.Comisionesgerente();

                    g.Comisiongerenteid = Guid.Empty.ToString();
                    g.Comisionvendedor = Convert.ToDecimal(r["ComisionVendedor"]);
                    g.Estatus = 1;
                    g.Fecha = DateTime.Now;
                    g.Gerenteid = r["GerenteID"].ToString();
                    g.Incentivoconcepto = r["IncentivoConcepto"].ToString();
                    g.Incentivoid = r["IncentivoID"].ToString();
                    g.Nombregerente = r["NombreGerente"].ToString();
                    g.Usuario = usuario;
                    _comGerentes.Add(g);

                }

                using (TransactionScope tran = new TransactionScope())
                {
                    MobileBO.ControlVentas.GuardarComision(new List<Entity.Ventas.Comision>() { com });

                    foreach (Entity.Ventas.Comisionesc1 c1 in _comC1)
                    {
                        c1.Comisionid = com.Comisionid;
                    }

                    foreach (Entity.Ventas.Comisionesb1 b1 in _comB1)
                    {
                        b1.Comisionid = com.Comisionid;
                    }

                    foreach (Entity.Ventas.Comisionesb1a1pagosnuevoscliente comb1a1 in _comB1A1)
                    {
                        comb1a1.Comisionid = com.Comisionid;
                    }

                    foreach (Entity.Ventas.Comisionesa1clientesrecurrente coma1 in _comA1Recurrente)
                    {
                        coma1.Comisionid = com.Comisionid;
                    }

                    foreach (Entity.Ventas.Comisionesgerente g in _comGerentes)
                    {
                        g.Comisionid = com.Comisionid;
                    }

                    MobileBO.ControlVentas.GuardarComisionesc1(_comC1);
                    MobileBO.ControlVentas.GuardarComisionesb1(_comB1);
                    MobileBO.ControlVentas.GuardarComisionesb1a1pagosnuevoscliente(_comB1A1);
                    MobileBO.ControlVentas.GuardarComisionesa1clientesrecurrente(_comA1Recurrente);
                    MobileBO.ControlVentas.GuardarComisionesgerente(_comGerentes);

                    tran.Complete();
                }


                Entity.ModeloComisionVendedores comisiones = new Entity.ModeloComisionVendedores();

                comisiones.ComisionesA1 = _comA1Recurrente;
                comisiones.ComisionesA1B1 = _comB1A1;
                comisiones.ComisionesB1 = _comB1;
                comisiones.ComisionesC1 = _comC1;
                comisiones.ComisionesGerente = _comGerentes;


                return Entity.Response<object>.CrearResponse<object>(true, comisiones);
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }

        }


        [WebMethod]
        public static Entity.Response<List<object>> TraerEmpresas()
        {
            Entity.ListaDeEntidades<Entity.Configuracion.Catempresa> empresas;
            List<object> listaElementos = new List<object>();
            try
            {
                empresas = MobileBO.ControlConfiguracion.TraerCatempresas();
                if (empresas != null)
                {
                    foreach (Entity.Configuracion.Catempresa empresa in empresas)
                    {
                        object elemento = new { id = empresa.Empresaid, nombre = empresa.Descripcion };
                        listaElementos.Add(elemento);
                    }
                }
                return Entity.Response<List<object>>.CrearResponse<List<object>>(true, listaElementos);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<object>>.CrearResponseVacio<List<object>>(false, ex.Message);
            }
        }


        [WebMethod]
        public static Entity.Response<List<object>> TraerVendedores()
        {
            Entity.ListaDeEntidades<Entity.Analisis.Catvendedor> vendedores;
            List<object> listaElementos = new List<object>();
            try
            {
                vendedores = MobileBO.ControlAnalisis.TraerCatvendedores();
                if (vendedores != null)
                {
                    foreach (Entity.Analisis.Catvendedor vendedor in vendedores)
                    {
                        if (vendedor.Estatus == 1 && vendedor.EstatusCRM == 1)
                        {
                            object elemento = new { id = vendedor.Vendedorid, nombre = vendedor.Nombre + ' ' + vendedor.Apellidopaterno + ' ' + vendedor.Apellidomaterno };
                            listaElementos.Add(elemento);
                        }
                    }
                }
                return Entity.Response<List<object>>.CrearResponse<List<object>>(true, listaElementos);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<object>>.CrearResponseVacio<List<object>>(false, ex.Message);
            }
        }
    }




    public class ControladorReporteAyudaCombustible : Base.Clases.BaseReportes
    {
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            DataSet ds = new DataSet();

            DateTime FechaIni = DateTime.Parse(parametros.Get("fechaini"));
            DateTime FechaFin = DateTime.Parse(parametros.Get("fechafin"));
            string gerenteid = parametros.Get("gerenteid");
            string vendedorid = parametros.Get("vendedorid");
            string candidatoid = parametros.Get("candidatoid");
            string formatoreporte = parametros.Get("Formato");
            
            



            if (gerenteid != null)
            {
                if (gerenteid.Trim() == "" || gerenteid.Trim() == "*")
                {
                    gerenteid = null;
                }
            }

            if (vendedorid != null)
            {
                if (vendedorid.Trim() == "" || vendedorid.Trim() == "*")
                {
                    vendedorid = null;
                }
            }

            if (candidatoid != null)
            {
                if (candidatoid.Trim() == "" || candidatoid.Trim() == "*")
                {
                    candidatoid = null;
                }
            }

            try
            {

                ds = MobileBO.ControlCRMWeb.TraeReporteAyudaCombustible(FechaIni, FechaFin, gerenteid, vendedorid, candidatoid);

                DataTable dtReporte = new DataTable("tblDatosReporte");
                dtReporte.Columns.Add("FechaIni", typeof(DateTime));
                dtReporte.Columns.Add("FechaFin", typeof(DateTime));
                dtReporte.Rows.Add(FechaIni, FechaFin);
                ds.Tables.Add(dtReporte);

                DataSet dsEmpresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull("A7D3E5A4-6508-483B-8A3D-0E379FF06755");
                dsEmpresa.Tables[0].TableName = "tblEmpresa";
                ds.Tables.Add(dsEmpresa.Tables[0].Copy());


                base.NombreReporte = "ReporteAyudaCombustible";
                base.FormatoReporte = 3;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\ReporteAyudaCombustible.xml", System.Data.XmlWriteMode.WriteSchema);
                
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GroupBy(string i_sGroupByColumn, string i_sAggregateColumn, DataTable i_dSourceTable)
        {

            DataView dv = new DataView(i_dSourceTable);

            //getting distinct values for group column
            DataTable dtGroup = dv.ToTable(true, new string[] { i_sGroupByColumn });

            //adding column for the row count
            dtGroup.Columns.Add("Count", typeof(int));

            //looping thru distinct values for the group, counting
            foreach (DataRow dr in dtGroup.Rows)
            {
                dr["Count"] = i_dSourceTable.Compute("Sum(" + i_sAggregateColumn + ")", i_sGroupByColumn + " = '" + dr[i_sGroupByColumn] + "'");
            }

            //returning grouped/counted result
            return dtGroup;
        }


    }


}