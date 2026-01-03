using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BalorFinanciera.Cartera.Formas
{
    public partial class ReporteResumenRelacionSaldos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public static System.Data.DataSet ProcesaResumenRelacionDeSaldos(string empresaid, DateTime fecha)
        {

            bool incluirDemandasCaducadas = true;
            bool ajustecontable = true;
            bool incluirDemandasPagadas = true;
            DataSet ds = new DataSet();

            DataTable dtMororsos = new DataTable("TblMorosos");
            dtMororsos.Columns.Add("Codigo", typeof(int)).DefaultValue = 0;
            dtMororsos.Columns.Add("Nombre", typeof(string)).DefaultValue = "";
            dtMororsos.Columns.Add("Clasificacion", typeof(int)).DefaultValue = 0;
            dtMororsos.Columns.Add("DescripcionClasificacion", typeof(string)).DefaultValue = "";
            dtMororsos.Columns.Add("Calificacion", typeof(string)).DefaultValue = "";
            dtMororsos.Columns.Add("Capital", typeof(decimal)).DefaultValue = 0;
            dtMororsos.Columns.Add("Comisiones", typeof(decimal)).DefaultValue = 0;
            dtMororsos.Columns.Add("ComisionesMoratorios", typeof(decimal)).DefaultValue = 0;
            dtMororsos.Columns.Add("Iva", typeof(decimal)).DefaultValue = 0;
            dtMororsos.Columns.Add("Saldo", typeof(decimal)).DefaultValue = 0;
            dtMororsos.Columns.Add("Dias", typeof(int)).DefaultValue = 0;
            dtMororsos.Columns.Add("FechaDemanda", typeof(DateTime)).DefaultValue = DateTime.MinValue;
            dtMororsos.Columns.Add("Cartera", typeof(int)).DefaultValue = 0;
            dtMororsos.Columns.Add("CarteraDescripcion", typeof(string)).DefaultValue = "";

            Entity.Configuracion.Catempresa _empresa = MobileBO.ControlConfiguracion.TraerCatempresas(empresaid);

            if (empresaid != null)
            {
                if (empresaid.Trim() == "" || empresaid.Trim() == "*")
                {
                    empresaid = null;
                }
            }

            try
            {

                Entity.Configuracion.Catempresa empresa = MobileBO.ControlConfiguracion.TraerCatempresas(empresaid);
                ds = MobileBO.ControlCartera.ResumenRelacionSaldos(fecha, empresaid);

                ds.Tables[0].TableName = "TblVencidos30Dias";
                ds.Tables[1].TableName = "TblVencidos60Dias";
                ds.Tables[2].TableName = "TblVencidos90Dias";
                ds.Tables[3].TableName = "TblDemandadosEjercicioActual";
                ds.Tables[4].TableName = "TblDemandadosEjerciciosAnteriores";
                ds.Tables[5].TableName = "TblDemandadosEjerciciosAnterioresNoRegistradosEnContabilidad";
                ds.Tables[6].TableName = "TblDemandadosEjerciciosAnterioresConSaldoLiquidado";
                ds.Tables[7].TableName = "TblDemandadosEjercicioActualNoRegistradosEnContabilidad";
                ds.Tables[8].TableName = "TblTrabajo"; //Tabla que trae el codigo de cliente y los saldos de las cuentas 1105,1103 y 1104
                ds.Tables[9].TableName = "TblPotros"; //Tabla que trae el codigo de cliente y los saldos de las cuentas 1105,1103 y 1104
                ds.Tables[10].TableName = "TblAbonosDemandadosRegistradosDirectoContabilidad"; //Tabla que trae los abonos registrados de clientes demandados directamente en contabilidad sin pasar por cobranza                
                DataTable dt = new DataTable("TblDemandadosEjerciciosAnteriores");
                dt.Columns.Add("Codigo", typeof(int));
                dt.Columns.Add("ClienteID", typeof(int));
                dt.Columns.Add("Descripcion", typeof(string));
                dt.Columns.Add("Total", typeof(decimal));
                dt.Columns.Add("Ejercicio", typeof(int));
                ds.Tables[12].TableName = "TblIncobrablesEjercicioActual";
                DataTable dtComprobacion = new DataTable("TblComprobacionCuentas");
                dtComprobacion.Columns.Add("Cuenta", typeof(string));
                dtComprobacion.Columns.Add("Importe", typeof(decimal));


                DataTable dtAbonosDespuesDeDemanda = new DataTable("TblAbonosDespuesDeDemanda");
                dtAbonosDespuesDeDemanda.Columns.Add("Clienteid", typeof(string)).DefaultValue = "";
                dtAbonosDespuesDeDemanda.Columns.Add("Codigo", typeof(int)).DefaultValue = 0;
                dtAbonosDespuesDeDemanda.Columns.Add("Financiamiento", typeof(decimal)).DefaultValue = 0;
                dtAbonosDespuesDeDemanda.Columns.Add("Ordinarios", typeof(decimal)).DefaultValue = 0;
                dtAbonosDespuesDeDemanda.Columns.Add("Moratorios", typeof(decimal)).DefaultValue = 0;



                DataSet CesionesPorCobrar = new DataSet();

                CesionesPorCobrar = MobileBO.ControlOperacion.CesionesPorCobrarReexDemandado(empresa.Empresa, fecha, null, null, null, null, null, null, null);

                Dictionary<string, Entity.ModeloCarteraDetalleGrafica> DetalleGrafica = new Dictionary<string, Entity.ModeloCarteraDetalleGrafica>();
                string clienteid = "";
                foreach (DataRow row in CesionesPorCobrar.Tables[0].Rows)
                {
                    bool caducada = false;
                    bool pagada = false;

                    //Entity.ModeloSaldoCesion Saldo = MobileBO.ControlOperacion.CalculaInteresOptimizado(row["CesionID"].ToString(), fecha, true, true, false, true);
                    Entity.ModeloSaldoCesion Saldo = MobileBO.ControlOperacion.CalculaInteresOptimizado(row["CesionID"].ToString(), fecha, true, true, false, false);


                    //Obtenemos los pagos realizados después de la fecha de demanda para restarlos al Saldo
                    List<Entity.Operacion.Cesionesdetalle> _pagos = MobileBO.ControlOperacion.TraerCesionesdetallePorCesion(row["CesionID"].ToString(), 11, fecha);
                    decimal _importePagosPosterioresADemanda = 0;
                    if (_pagos.Count > 0)
                    {
                        if (row["FechaDemanda"] != DBNull.Value)
                        {
                            if (_pagos.Where(x => x.FechaApli >= Convert.ToDateTime(row["FechaDemanda"])).Count() > 0)
                            {
                                _importePagosPosterioresADemanda = _pagos.Where(x => x.FechaApli >= Convert.ToDateTime(row["FechaDemanda"]) && x.FechaApli <= fecha).Sum(i => i.Financiamiento + i.InteresOrdinario + i.InteresMoratorio);

                                //Agregamos a la tabla los abonos para aplicarlos mas a delante para la conciliación con la tabla "tblTrabajo"
                                if (dtAbonosDespuesDeDemanda.AsEnumerable().Where(x => x.Field<string>("Clienteid").ToUpper() == row["ClienteID"].ToString().ToUpper()).Count() > 0)
                                {
                                    DataRow r = dtAbonosDespuesDeDemanda.AsEnumerable().Where(x => x.Field<string>("Clienteid").ToUpper() == row["ClienteID"].ToString().ToUpper()).FirstOrDefault();
                                    r["Clienteid"] = row["ClienteID"].ToString();
                                    r["Codigo"] = Convert.ToInt32(row["Codigo"]);
                                    r["Financiamiento"] = Convert.ToDecimal(r["Financiamiento"]) + _pagos.Where(x => x.FechaApli >= Convert.ToDateTime(row["FechaDemanda"]) && x.FechaApli <= fecha).Sum(i => i.Financiamiento);
                                    r["Ordinarios"] = Convert.ToDecimal(r["Ordinarios"]) + _pagos.Where(x => x.FechaApli >= Convert.ToDateTime(row["FechaDemanda"]) && x.FechaApli <= fecha).Sum(i => i.InteresOrdinario);
                                    r["Moratorios"] = Convert.ToDecimal(r["Moratorios"]) + _pagos.Where(x => x.FechaApli >= Convert.ToDateTime(row["FechaDemanda"]) && x.FechaApli <= fecha).Sum(i => i.InteresMoratorio);
                                }
                                else
                                {
                                    DataRow r = dtAbonosDespuesDeDemanda.NewRow();
                                    r["Clienteid"] = row["ClienteID"].ToString();
                                    r["Codigo"] = Convert.ToInt32(row["Codigo"]);
                                    r["Financiamiento"] = Convert.ToDecimal(r["Financiamiento"]) + _pagos.Where(x => x.FechaApli >= Convert.ToDateTime(row["FechaDemanda"]) && x.FechaApli <= fecha).Sum(i => i.Financiamiento);
                                    r["Ordinarios"] = Convert.ToDecimal(r["Ordinarios"]) + _pagos.Where(x => x.FechaApli >= Convert.ToDateTime(row["FechaDemanda"]) && x.FechaApli <= fecha).Sum(i => i.InteresOrdinario);
                                    r["Moratorios"] = Convert.ToDecimal(r["Moratorios"]) + _pagos.Where(x => x.FechaApli >= Convert.ToDateTime(row["FechaDemanda"]) && x.FechaApli <= fecha).Sum(i => i.InteresMoratorio);
                                    dtAbonosDespuesDeDemanda.Rows.Add(r);
                                }

                                dtAbonosDespuesDeDemanda.AcceptChanges();
                            }
                        }
                    }
                    //Obtenemos los registros de abonos de clientes demandados registrados directamente en contabilidad sin registro en cobranza
                    decimal _abonosRegistradosEnContabilidad = 0;
                    _abonosRegistradosEnContabilidad = ds.Tables["TblAbonosDemandadosRegistradosDirectoContabilidad"].AsEnumerable().Where(x => x.Field<string>("ClienteID").ToUpper() == row["ClienteID"].ToString().ToUpper() && x.Field<DateTime>("FechaPoliza") >= Convert.ToDateTime(row["FechaDemanda"])).Sum(y => y.Field<decimal>("Importe"));


                    //############################################################################################################################################
                    //Fausto Montenegro
                    //2023-07-17
                    //############################################################################################################################################
                    //Para el caso de cesiones de Balor con tipoContrato o tipoCliente igual a 2 (Proveedores) se hace el recálculo de la cesion
                    //ya que para estos clientes el sistema separa de los moratorios el 5.556% que corresponde a los ordinarios para mostrarlos en reportes
                    //pero no lo considera para sumar el moratorio real que viene siendo el 8.88%, para ello se le resta al interes ordinario el interes ordinario calculado después del vencimiento
                    //y al moratorio se le suma el interes ordinario generado después del moratorio y queda como sigue:
                    //
                    //Capital = Saldo.Financioamiento
                    //Interes Ordinario = Saldo.InteresOrdinario - Saldo.InteresOrdinarioM30D - Saldo.PagoInteresOrdinario
                    //Interes Moratorio = Saldo.InteresMoratorio + Saldo.InteresOrdinarioM30D - Saldo.PagoInteresMoratorio

                    Saldo.Iva = ((Saldo.ComisionAnalisis + Saldo.ComisionDisposicion) * 1.16m) - (Saldo.ComisionAnalisis + Saldo.ComisionDisposicion);
                    Saldo.PagoIva = ((Saldo.PagoComisionAnalisis + Saldo.PagoComisionDisposicion) * 1.16m) - (Saldo.PagoComisionAnalisis + Saldo.PagoComisionDisposicion);
                    Saldo.Saldo = Math.Round((Saldo.Sofom ? Saldo.Financiamiento : Saldo.SaldoFinanciado) + (Saldo.InteresOrdinario - Saldo.InteresOrdinarioM30D - Saldo.PagoInteresOrdinario) + (Saldo.ComisionAnalisis - Saldo.PagoComisionAnalisis) + (Saldo.ComisionDisposicion - Saldo.PagoComisionDisposicion) + (Saldo.InteresMoratorio + Saldo.InteresOrdinarioM30D - Saldo.PagoInteresMoratorio) + (Saldo.Iva - Saldo.PagoIva), 2);

                    //FIN recálculo de cesión para clientes proveedores (tipocontrato = 2) de Balor
                    //############################################################################################################################################


                    Saldo.Saldo = Saldo.Saldo - _importePagosPosterioresADemanda;

                    //Saldo.Saldo = Saldo.Saldo - _abonosRegistradosEnContabilidad - _importePagosPosterioresADemanda + Saldo.InteresOrdinarioM30D;

                    Entity.Operacion.Catclientesmoroso moroso = MobileBO.ControlOperacion.TraerCatclientesmorosos(null, Saldo.EmpresaID, Saldo.ClienteID, 1);
                    if (moroso != null)
                    {
                        caducada = moroso.Caducada;
                        if (moroso.Caducada && !incluirDemandasCaducadas)
                            continue;
                        pagada = moroso.Pagada;
                        if (moroso.Pagada && !incluirDemandasPagadas)
                            continue;
                    }
                    else
                    {
                        moroso = MobileBO.ControlOperacion.TraerCatclientesmorosos(null, Saldo.EmpresaID, Saldo.ClienteID, 3);
                        if (moroso != null)
                        {
                            caducada = moroso.Caducada;
                            if (moroso.Caducada && !incluirDemandasCaducadas)
                                continue;
                            pagada = moroso.Pagada;
                            if (moroso.Pagada && !incluirDemandasPagadas)
                                continue;
                        }
                    }

                    Entity.Operacion.Cesionesconajustescontable CesionAjuste = MobileBO.ControlOperacion.TraerCesionesconajustescontables(null, Saldo.CesionID);



                    clienteid = row["ClienteID"].ToString();
                    if (!DetalleGrafica.ContainsKey(clienteid))
                    {
                        Entity.Analisis.Catcliente Cliente = MobileBO.ControlAnalisis.TraerCatclientes(clienteid, null, null);
                        Entity.Analisis.Catclasificacion Clasificacion = MobileBO.ControlAnalisis.TraerCatclasificaciones(Cliente.Clasificacionid);
                        Entity.ModeloCarteraDetalleGrafica detalle = new Entity.ModeloCarteraDetalleGrafica();
                        detalle.Codigo = Cliente.Codigo;
                        detalle.Nombre = Cliente.Nombrecompleto;
                        detalle.Clasificacion = Clasificacion.Codigo;
                        detalle.DescripcionClasificacion = Clasificacion.Descripcion;
                        if (CesionAjuste == null || !ajustecontable)
                        {
                            if (empresa.Sofom)
                            {
                                detalle.Capital = Saldo.SaldoFinanciado;
                                detalle.Comisiones = (Saldo.InteresOrdinario - Saldo.PagoInteresOrdinario) + (Saldo.ComisionDisposicion - Saldo.PagoComisionDisposicion) + (Saldo.ComisionAnalisis - Saldo.PagoComisionAnalisis) + (Saldo.InteresMoratorio - Saldo.PagoInteresMoratorio);
                            }
                            else
                            {
                                detalle.Capital = Saldo.SaldoFinanciado;
                                detalle.Comisiones = (Saldo.InteresOrdinario - Saldo.PagoInteresOrdinario);
                                detalle.ComisionesMoratorios = (Saldo.InteresMoratorio - Saldo.PagoInteresMoratorio);
                            }
                            detalle.Iva = (Saldo.Iva - Saldo.PagoIva);
                            detalle.Saldo = (Saldo.Saldo - (Saldo.Iva - Saldo.PagoIva));
                        }
                        else
                        {
                            if (empresa.Sofom)
                            {
                                detalle.Capital = CesionAjuste.Saldofinanciamiento;
                                detalle.Comisiones = CesionAjuste.Saldocomision + CesionAjuste.Saldointeresordinario + CesionAjuste.Saldointeresmoratorio;
                            }
                            else
                            {
                                detalle.Capital = CesionAjuste.Saldofinanciamiento;
                                detalle.Comisiones = CesionAjuste.Saldointeresordinario;
                                detalle.ComisionesMoratorios = CesionAjuste.Saldointeresmoratorio;
                            }
                            detalle.Iva = 0m;
                            detalle.Saldo = CesionAjuste.Saldo;
                        }

                        TimeSpan ts = fecha - DateTime.Parse(Saldo.FechaDocu);
                        detalle.Dias = ts.Days;
                        detalle.Cartera = (row["tipo"].ToString() == "R" ? 1 : (row["tipo"].ToString() == "1" ? (!caducada && !pagada ? 2 : (caducada ? 3 : 4)) : (row["tipo"].ToString() == "3" ? 5 : 6)));
                        detalle.CarteraDescripcion = detalle.Cartera == 1 ? "Clientes con Reestructura" : detalle.Cartera == 2 ? "Clientes en Demanda" : detalle.Cartera == 3 ? "Clientes en Demanda Caducada" : detalle.Cartera == 4 ? "Clientes en Demanda Pagada" : detalle.Cartera == 5 ? "Clientes Incobrables" : "Desconocido";
                        if (row["tipo"].ToString() == "1")
                            detalle.FechaDemanda = DateTime.Parse(row["FechaDemanda"].ToString());

                        DetalleGrafica.Add(clienteid, detalle);

                        DataRow r = dtMororsos.NewRow();
                        r["Codigo"] = detalle.Codigo;
                        r["Nombre"] = detalle.Nombre;
                        r["Clasificacion"] = detalle.Clasificacion;
                        r["DescripcionClasificacion"] = detalle.DescripcionClasificacion;
                        r["Capital"] = detalle.Capital;
                        r["Comisiones"] = detalle.Comisiones;
                        r["ComisionesMoratorios"] = detalle.ComisionesMoratorios;
                        r["Iva"] = detalle.Iva;
                        r["Dias"] = detalle.Dias;
                        r["FechaDemanda"] = detalle.FechaDemanda;
                        r["Cartera"] = detalle.Cartera;
                        r["CarteraDescripcion"] = detalle.CarteraDescripcion;
                        dtMororsos.Rows.Add(r);

                    }
                    else
                    {
                        if (CesionAjuste == null || !ajustecontable)
                        {
                            DetalleGrafica[clienteid].Capital += Saldo.SaldoFinanciado;
                            if (empresa.Sofom)
                                DetalleGrafica[clienteid].Comisiones += (Saldo.InteresOrdinario - Saldo.PagoInteresOrdinario) + (Saldo.ComisionDisposicion - Saldo.PagoComisionDisposicion) + (Saldo.ComisionAnalisis - Saldo.PagoComisionAnalisis) + (Saldo.InteresMoratorio - Saldo.PagoInteresMoratorio);
                            else
                            {
                                DetalleGrafica[clienteid].Comisiones += (Saldo.InteresOrdinario - Saldo.PagoInteresOrdinario);
                                DetalleGrafica[clienteid].ComisionesMoratorios += (Saldo.InteresMoratorio - Saldo.PagoInteresMoratorio);
                            }
                            DetalleGrafica[clienteid].Iva += (Saldo.Iva - Saldo.PagoIva);
                            DetalleGrafica[clienteid].Saldo += (Saldo.Saldo - (Saldo.Iva - Saldo.PagoIva));
                        }
                        else
                        {
                            DetalleGrafica[clienteid].Capital += CesionAjuste.Saldofinanciamiento;
                            if (empresa.Sofom)
                                DetalleGrafica[clienteid].Comisiones += CesionAjuste.Saldocomision + CesionAjuste.Saldointeresordinario + CesionAjuste.Saldointeresmoratorio;
                            else
                            {
                                DetalleGrafica[clienteid].Comisiones += CesionAjuste.Saldointeresordinario;
                                DetalleGrafica[clienteid].ComisionesMoratorios += CesionAjuste.Saldointeresmoratorio;
                            }
                            DetalleGrafica[clienteid].Iva += 0m;
                            DetalleGrafica[clienteid].Saldo += CesionAjuste.Saldo;
                        }
                        TimeSpan ts = fecha - DateTime.Parse(Saldo.FechaDocu);
                        if (ts.Days > DetalleGrafica[clienteid].Dias)
                        {
                            DetalleGrafica[clienteid].Dias = ts.Days;
                        }

                        DataRow r = dtMororsos.NewRow();
                        r["Codigo"] = DetalleGrafica[clienteid].Codigo;
                        r["Nombre"] = DetalleGrafica[clienteid].Nombre;
                        r["Clasificacion"] = DetalleGrafica[clienteid].Clasificacion;
                        r["DescripcionClasificacion"] = DetalleGrafica[clienteid].DescripcionClasificacion;
                        r["Capital"] = DetalleGrafica[clienteid].Capital;
                        r["Comisiones"] = DetalleGrafica[clienteid].Comisiones;
                        r["ComisionesMoratorios"] = DetalleGrafica[clienteid].ComisionesMoratorios;
                        r["Iva"] = DetalleGrafica[clienteid].Iva;
                        r["Dias"] = DetalleGrafica[clienteid].Dias;
                        r["FechaDemanda"] = DetalleGrafica[clienteid].FechaDemanda;
                        r["Cartera"] = DetalleGrafica[clienteid].Cartera;
                        r["CarteraDescripcion"] = DetalleGrafica[clienteid].CarteraDescripcion;
                        dtMororsos.Rows.Add(r);
                    }



                }

                foreach (DataRow r in ds.Tables["TblDemandadosEjerciciosAnteriores"].Rows)
                {
                    foreach (KeyValuePair<string, Entity.ModeloCarteraDetalleGrafica> saldo in DetalleGrafica)
                    {
                        if (r["Codigo"].ToString() == saldo.Value.Codigo.ToString())
                        {
                            if (r["Codigo"].ToString() != "331" && r["Codigo"].ToString() != "705" && r["Codigo"].ToString() != "707") //Se excluye el cliente 331 - OBRAS CIVILES Y MARITIMAS, SA DE CV por traer un calculo especial desde el procedimiento
                            {
                                r["Total"] = saldo.Value.Saldo;
                            }

                        }
                    }
                }
                ds.Tables["TblDemandadosEjerciciosAnteriores"].AcceptChanges();


                foreach (DataRow r in ds.Tables["TblDemandadosEjerciciosAnterioresNoRegistradosEnContabilidad"].Rows)
                {
                    foreach (KeyValuePair<string, Entity.ModeloCarteraDetalleGrafica> saldo in DetalleGrafica)
                    {
                        if (r["Codigo"].ToString() == saldo.Value.Codigo.ToString())
                        {
                            r["Total"] = saldo.Value.Saldo;
                        }
                    }
                }
                ds.Tables["TblDemandadosEjerciciosAnterioresNoRegistradosEnContabilidad"].AcceptChanges();



                foreach (DataRow r in ds.Tables["TblDemandadosEjerciciosAnterioresConSaldoLiquidado"].Rows)
                {
                    foreach (KeyValuePair<string, Entity.ModeloCarteraDetalleGrafica> saldo in DetalleGrafica)
                    {
                        if (r["Codigo"].ToString() == saldo.Value.Codigo.ToString())
                        {
                            r["Total"] = saldo.Value.Saldo;
                        }
                    }
                }
                ds.Tables["TblDemandadosEjerciciosAnterioresConSaldoLiquidado"].AcceptChanges();

                //Obtenemos los saldos de los cientes por parte de sistemas
                int _codCliente = 0;
                foreach (DataRow r in ds.Tables["TblTrabajo"].Rows)
                {
                    _codCliente = Convert.ToInt32(r["Codigo"]);
                   
                    bool _clienteIntercompañia = false;
                  
                    if (MobileBO.ControlCartera.ExisteCatclientesespeciales(_codCliente, empresaid.ToUpper()) != null)
                    {
                        r["SaldoSistemas1105"] = Convert.ToDecimal(r["SaldoContabilidad1105"]);
                        r["SaldoSistemas1104"] = Convert.ToDecimal(r["SaldoContabilidad1104"]);
                        _clienteIntercompañia = true;

                    }

                    //Los clientes PRIMOS, BALOR y TATIVO se hace manual y se asignan los valores obtenidos de contabilidad

                    //if (empresaid.ToUpper() == "A7D3E5A4-6508-483B-8A3D-0E379FF06755" && (_codCliente == 36 || _codCliente == 351 || _codCliente == 674 || _codCliente == 641))
                    //{
                    //    r["SaldoSistemas1105"] = Convert.ToDecimal(r["SaldoContabilidad1105"]);
                    //    r["SaldoSistemas1104"] = Convert.ToDecimal(r["SaldoContabilidad1104"]);
                    //    _clienteIntercompañia = true;
                    //}


                    //if (empresaid.ToUpper() == "FA764836-BB07-4EB3-9B30-2B69206174C2" && (_codCliente == 1 || _codCliente == 6 || _codCliente == 9 || _codCliente == 36 || _codCliente == 641 || _codCliente == 672 || _codCliente == 673 ))
                    //{
                    //    r["SaldoSistemas1105"] = Convert.ToDecimal(r["SaldoContabilidad1105"]);
                    //    r["SaldoSistemas1104"] = Convert.ToDecimal(r["SaldoContabilidad1104"]);
                    //    _clienteIntercompañia = true;
                    //}
                    ////ARMANDO PICOS TORERRO CLIENTE NO DEMANDADO ESPECIAL
                    //if (empresaid.ToUpper() == "FA764836-BB07-4EB3-9B30-2B69206174C2" && _codCliente == 705 || _codCliente == 206)
                    //{
                    //    r["SaldoSistemas1105"] = Convert.ToDecimal(r["SaldoContabilidad1105"]);
                    //    r["SaldoSistemas1104"] = Convert.ToDecimal(r["SaldoContabilidad1104"]);
                    //    _clienteIntercompañia = true;
                    //}

                    ////CLIENTES DEMANDADO 2024
                    if (empresaid.ToUpper() == "FA764836-BB07-4EB3-9B30-2B69206174C2" &&  _codCliente == 829)
                    {
                        Entity.Response<List<List<Entity.ModeloSaldoCesion>>> _saldosPorSistemas765 = BalorFinanciera.Operacion.Formas.Saldos.CalcularSaldo(_codCliente, fecha.ToShortDateString(), false, true);
                        foreach (List<Entity.ModeloSaldoCesion> _lista in _saldosPorSistemas765.Datos)
                        {

                            decimal _saldo1105_765 = _lista.Sum(x => x.SaldoFinanciado);
                            decimal _saldo1103_765 = _lista.Sum(x => (x.ComisionAnalisis - x.PagoComisionAnalisis) + (x.ComisionDisposicion - x.PagoComisionDisposicion) + (x.Iva - x.PagoIva) - x.Bonificaciones);
                            decimal _saldo1104_765 = _lista.Sum(x => (x.InteresOrdinario - x.PagoInteresOrdinario) + (x.InteresMoratorio - x.PagoInteresMoratorio));

                            //decimal _saldo1103_765 = _lista.Sum(x => (x.InteresMoratorio - x.PagoInteresMoratorio));
                            //decimal _saldo1104_765 = _lista.Sum(x => (x.InteresOrdinario - x.PagoInteresOrdinario)  + (x.ComisionAnalisis - x.PagoComisionAnalisis) + (x.ComisionDisposicion - x.PagoComisionDisposicion) +(x.Iva - x.PagoIva) - x.Bonificaciones);
                            r["SaldoSistemas1104"] = Convert.ToDecimal(r["SaldoSistemas1104"]) + _saldo1104_765;
                            r["SaldoSistemas1103"] = Convert.ToDecimal(r["SaldoSistemas1103"]) + _saldo1103_765;
                            r["SaldoSistemas1105"] = Convert.ToDecimal(r["SaldoSistemas1105"]) + _saldo1105_765;

                            _clienteIntercompañia = true;
                        }
                    }

                    if (!_clienteIntercompañia)
                    {
                        Entity.Response<List<List<Entity.ModeloSaldoCesion>>> _saldosPorSistemas = BalorFinanciera.Operacion.Formas.Saldos.CalcularSaldo(_codCliente, fecha.ToShortDateString(), false, true);
                        foreach (List<Entity.ModeloSaldoCesion> _lista in _saldosPorSistemas.Datos)
                        {
                            decimal _saldo1104;
                            decimal _saldo1103 = 0;
                            bool esTransportista = _lista.Any(x => x.TipoContrato == 5);
                            if (esTransportista == true)
                            {
                                _saldo1104 = _lista.Sum(x => (x.InteresOrdinario - x.PagoInteresOrdinario) + (x.Iva - x.PagoIva) + (x.ComisionAnalisis - x.PagoComisionAnalisis) + (x.ComisionDisposicion - x.PagoComisionDisposicion) - x.Bonificaciones);
                            }
                            else
                            {
                                //_saldo1104 = _lista.Sum(x => (x.InteresOrdinario - x.PagoInteresOrdinario) + (x.InteresMoratorio - x.PagoInteresMoratorio) + (x.ComisionAnalisis - x.PagoComisionAnalisis) + (x.ComisionDisposicion - x.PagoComisionDisposicion) - x.Bonificaciones);
                               
                                // si la cesion se encuentra vencida se agrega el IVA de esa cesion para cuadrar
                                bool anyVencida = _lista.Any(x => x.Vencida == "SI");
                                if (anyVencida == true)
                                {
                                    _saldo1103 = _lista.Sum(x => (x.ComisionAnalisis - x.PagoComisionAnalisis) + (x.ComisionDisposicion - x.PagoComisionDisposicion) - x.Bonificaciones);
                                    _saldo1104 = _lista.Sum(x => (x.InteresOrdinario - x.PagoInteresOrdinario) + (x.InteresMoratorio - x.PagoInteresMoratorio));
                                    _saldo1103 += _lista.Where(x => x.Vencida == "SI") 
                                       .Sum(x => (x.Iva - x.PagoIva));
                                }
                                else
                                {
                                    _saldo1104 = _lista.Sum(x => (x.InteresOrdinario - x.PagoInteresOrdinario) + (x.InteresMoratorio - x.PagoInteresMoratorio) + (x.ComisionAnalisis - x.PagoComisionAnalisis) + (x.ComisionDisposicion - x.PagoComisionDisposicion) - x.Bonificaciones);

                                }

                            }
                            decimal _saldo1105 = _lista.Sum(x => x.SaldoFinanciado);
                           //decimal _saldo1104 = _lista.Sum(x => (x.InteresOrdinario - x.PagoInteresOrdinario) + (x.InteresMoratorio - x.PagoInteresMoratorio));
                            r["SaldoSistemas1105"] = Convert.ToDecimal(r["SaldoSistemas1105"]) + _saldo1105;
                            r["SaldoSistemas1104"] = Convert.ToDecimal(r["SaldoSistemas1104"]) + _saldo1104;
                            r["SaldoSistemas1103"] = Convert.ToDecimal(r["SaldoSistemas1103"]) + _saldo1103;
                        }

                        //Aqui descontamos los abonos aplicados después de la fecha de demanda que se obtuvieron arriba
                        r["SaldoSistemas1105"] = Convert.ToDecimal(r["SaldoSistemas1105"]) - dtAbonosDespuesDeDemanda.AsEnumerable().Where(x => x.Field<int>("Codigo") == _codCliente).Sum(y => y.Field<decimal>("Financiamiento"));
                        r["SaldoSistemas1104"] = Convert.ToDecimal(r["SaldoSistemas1104"]) - dtAbonosDespuesDeDemanda.AsEnumerable().Where(x => x.Field<int>("Codigo") == _codCliente).Sum(y => y.Field<decimal>("Ordinarios") + y.Field<decimal>("Moratorios"));

                    }
                }

                
                ds.Tables["TblTrabajo"].AcceptChanges();

                ds.Tables.Add(dtMororsos);


            }
            catch (Exception ex)
            {

            }

            return ds;
        }

        public static System.Data.DataSet ProcesaEstadoDeResultadosDetalle(string empresaid, DateTime fecha)
        {
            DataSet ds = new DataSet();
            try
            {
                

                #region Declaracion de Tabla

                DataTable dt = new DataTable("EstadoDeResultados");
                //VENTAS
                dt.Columns.Add("VentasContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("VentasReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("InteresesClientesTativo", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("InteresesIntercompañia", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("SubVentasTotalesContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("SubVentasTotalesReales", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("CarteraVencida30a59Dias", typeof(decimal));
                dt.Columns.Add("CarteraVencidaMayor60Dias", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("CarteraDemandados", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("VentasTotalesContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("VentasTotalesReales", typeof(decimal)).DefaultValue = 0;
                //COSTO DE FINANCIAMIENTO
                dt.Columns.Add("CostoFinanciamientoContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("CostoFinanciamientoReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("InteresesFinanciamientoIntercompañia", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("InteresesTativoPorClientesFacturco", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("CostoFinanciamientoRealContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("CostoFinanciamientoRealReales", typeof(decimal)).DefaultValue = 0;
                //UTILIDAD BRUTA
                dt.Columns.Add("UtilidadBrutaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("UtilidadBrutaReal", typeof(decimal)).DefaultValue = 0;
                //GASTOS DE OPERACION
                dt.Columns.Add("GastosDeVentaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("GastosDeVentaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("GastosDeVentaPersonales", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("GastoFintac", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("GastoTativo", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("GastosDeVentaBalorAbsorbidoPorFacturco", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("GastosDeVentaPersonalesBalorAbsorbidoPorFacturco", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("GastosDeVentaTotalContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("GastosDeVentaTotalReal", typeof(decimal)).DefaultValue = 0;
                //GASTOS DE ADMINISTRACION
                dt.Columns.Add("GastosDeAdministracionRealContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("GastosDeAdministracionRealReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("GastosDeAdministracionPersonales", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("GastosDeAdministracionFintac", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("GastosDeAdministracionTativo", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("GastosDeAdministracionBalorAbsorbidoPorFacturco", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("GastosDeAdministracionPersonalesBalorAbsorbidoPorFacturco", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("GastosDeAdministracionRealTotalContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("GastosDeAdministracionRealTotalReal", typeof(decimal)).DefaultValue = 0;

                dt.Columns.Add("DeduccionPorCuentasIncobrablesContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("DeduccionPorCuentasIncobrablesReal", typeof(decimal)).DefaultValue = 0;

                dt.Columns.Add("TotalGastosDeOperacionContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("TotalGastosDeOperacionReal", typeof(decimal)).DefaultValue = 0;

                //UTILIDADES DE OTROS INGRESOS
                dt.Columns.Add("UtilidadAntesDeOtrosIngresosContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("UtilidadAntesDeOtrosIngresosReal", typeof(decimal)).DefaultValue = 0;

                //OTROS INGRESOS Y GASTOS
                dt.Columns.Add("OtrosIngresosYGastosContableAgrupado", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("OtrosIngresosYGastosRealAgrupado", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("UtilidadCambiariaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("UtilidadCambiariaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PerdidaCambiariaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PerdidaCambiariaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("OtrosIngresosYGastosContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("OtrosIngresosYGastosReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("UtilidadOPerdidaContableContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("UtilidadOPerdidaContableReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("UtilidadOPerdidaSinEfectoCambiariaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("UtilidadOPerdidaSinEfectoCambiariaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("EfectoCambiarioContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("EfectoCambiarioReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("CarteraVencidaYDemandadosEjercicioActual", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("UtilidadOPerdidaSinCapitalDeCarteraVencidaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("UtilidadOPerdidaSinCapitalDeCarteraVencidaReal", typeof(decimal)).DefaultValue = 0;


                //PORCENTAJES
                dt.Columns.Add("PorcentajeVentasTotalesRealesContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeVentasTotalesRealesReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeCostoFinanciamientoRealesContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeCostoFinanciamientoRealesReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeUtilidadBrutaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeUtilidadBrutaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeGastosDeVentaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeGastosDeVentaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeGastosDeVentaRealContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeGastosDeVentaRealReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeGastosDeAdministracionContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeGastosDeAdministracionReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeGastosDeAdministracionRealContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeGastosDeAdministracionRealReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeDeduccionCuentasIncobrablesContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeDeduccionCuentasIncobrablesReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeTotalGastosDeOperacionContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeTotalGastosDeOperacionReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeUtilidadesAntesDeOtrosIngresosContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeUtilidadesAntesDeOtrosIngresosReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeOtrosIngresosYGastosAgrupadoContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeOtrosIngresosYGastosAgrupadoReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeUtilidadCambiariaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeUtilidadCambiariaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajePerdidaCambiariaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajePerdidaCambiariaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeOtrosIngresosYGastosContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeOtrosIngresosYGastosReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeUtilidadOPerdidaContableContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeUtilidadOPerdidaContableReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeEfectoCambiarioContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeEfectoCambiarioReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeUtilidadOPerdidaSinEfectoCambiarioContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeUtilidadOPerdidaSinEfectoCambiarioReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeUtilidadOPerdidaSinCarteraVencidaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PorcentajeUtilidadOPerdidaSinCarteraVencidaReal", typeof(decimal)).DefaultValue = 0;
                #endregion

                #region Declaracion de variables
                //VENTAS
                decimal _ventasContable = 0;
                decimal _ventasReal = 0;
                decimal _interesesClientesTativo = 0;
                decimal _interesesIntercompania = 0;
                decimal _subVentasTotalesContable = 0;
                decimal _subVentasTotalesReales = 0;

                decimal _carteraVencida30a59dias = 0;
                decimal _carteraVencidamayor60dias = 0;
                decimal _carteraDemandados = 0;

                decimal _carteraCapitalVencida30a59dias = 0;
                decimal _carteraCapitalVencidamayor60dias = 0;
                decimal _carteraCapitalDemandados = 0;

                decimal _ventasTotalesContable = 0;
                decimal _ventasTotalesReales = 0;


                //COSTO DE FINANCIAMIENTO
                decimal _costoFinanaciamientoContable = 0;
                decimal _costoFinanaciamientoReal = 0;
                decimal _interesesFinanciamientoIntercompania = 0;
                decimal _interesesTativoPorClientesFacturco = 0;
                decimal _costoFinanciamientoRealContable = 0;
                decimal _costoFinanciamientoRealReales = 0;

                //UTILIDAD BRUTA
                decimal _utilidadBrutaContable = 0;
                decimal _utilidadBrutaReal = 0;

                //GASTOS DE OPERACION
                decimal _gastosDeVentaContable = 0;
                decimal _gastosDeVentaReal = 0;
                decimal _gastoDeVentaPersonales = 0;
                decimal _gastoFintac = 0;
                decimal _gastoTativo = 0;
                decimal _gastosDeVentaBalorAbsorbidasPorFacturco = 0;
                decimal _gastosDeVentaPersonalesBalorAbsorbidasPorFacturco = 0;
                decimal _gastosDeVentaTotalContable = 0;
                decimal _gastosDeVentaTotalReal = 0;

                // GASTOS DE ADMINISTRACION
                decimal _gastosDeAdministracionRealContable = 0;
                decimal _gastosDeAdministracionRealReal = 0;

                decimal _gastosDeAdministracionPersonales = 0;
                decimal _gastosDeAdministracionFintac = 0;
                decimal _gastosDeAdministracionTativo = 0;
                decimal _gastosDeAdministracionBalorAbsorbidasPorFacturco = 0;
                decimal _gastosDeAdministracionPersonalesBalorAbsorbidasPorFacturco = 0;
                


                decimal _gastosDeAdministracionRealTotalContable = 0;
                decimal _gastosDeAdministracionRealTotalReal = 0;

                decimal _deduccionPorCuentasIncobrablesContable = 0;
                decimal _deduccionPorCuentasIncobrablesReal = 0;


                decimal _totalGastosDeOperacionContable = 0;
                decimal _totalGastosDeOperacionReal = 0;

                //UTILIDADES DE OTROS INGRESOS
                decimal _utilidadAntesDeOtrosIngresosContable = 0;
                decimal _utilidadAntesDeOtrosIngresosReal = 0;

                //OTROS INGRESOS Y GASTOS
                decimal _otrosIngresosYGastosContableAgrupado = 0;
                decimal _otrosIngresosYGastosRealAgrupado = 0;
                decimal _UtilidadCambiariaContable = 0;
                decimal _UtilidadCambiariaReal = 0;
                decimal _perdidaCambiariaContable = 0;
                decimal _perdidaCambiariaReal = 0;
                decimal _otrosIngresosYGastosContable = 0;
                decimal _otrosIngresosYGastosReal = 0;

                decimal _utilidadOPerdidaContableContable = 0;
                decimal _utilidadOPerdidaContableReal = 0;

                decimal _utilidadOPerdidaSinEfectoCambiariaContable = 0;
                decimal _utilidadOPerdidaSinEfectoCambiariaReal = 0;

                decimal _efectoCambiarioContable = 0;
                decimal _efectoCambiarioReal = 0;

                decimal _carteraVencidaYDemandadosEjercicioActual = 0;

                decimal _utilidadOPerdidaSinCapitalDeCarteraVencidaContable = 0;
                decimal _utilidadOPerdidaSinCapitalDeCarteraVencidaReal = 0;

                
                #endregion

                Entity.Configuracion.Catempresa empresa = MobileBO.ControlConfiguracion.TraerCatempresas(empresaid);

                DataSet dsResumenRelacionGastos = ReporteResumenRelacionSaldos.ProcesaResumenRelacionDeSaldos(empresaid, fecha);
                DataSet dsGastos = MobileBO.ControlContabilidad.TraeGastos(fecha, empresaid);
                DataRow r = dt.NewRow();


               

                #region Cálculo de operciones

                //OBTENCION VENTAS
                _ventasContable = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "INGRESO POR COMISION" || x.Field<string>("Descripcion") == "INGRESO POR INTERES" || x.Field<string>("Descripcion") == "VENTAS INTERCOMPAÑIA").Sum(x => x.Field<decimal>("Importe"));
                _ventasReal = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "INGRESO POR COMISION" || x.Field<string>("Descripcion") == "INGRESO POR INTERES").Sum(x => x.Field<decimal>("ImporteReal"));
                _interesesClientesTativo = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "INGRESO POR COMISION TATIVO").Sum(x => x.Field<decimal>("ImporteReal"));
                _interesesIntercompania = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "VENTAS INTERCOMPAÑIA").Sum(x => x.Field<decimal>("ImporteReal"));
                _subVentasTotalesContable = _ventasContable;
                _subVentasTotalesReales = _ventasReal + _interesesClientesTativo + _interesesIntercompania;

                _carteraVencida30a59dias = dsResumenRelacionGastos.Tables["TblVencidos60Dias"].AsEnumerable().Sum(x => x.Field<decimal>("Intereses") + x.Field<decimal>("Comisiones"));
                _carteraVencidamayor60dias = dsResumenRelacionGastos.Tables["TblVencidos90Dias"].AsEnumerable().Sum(x => x.Field<decimal>("Intereses") + x.Field<decimal>("Comisiones"));
                _carteraDemandados = dsResumenRelacionGastos.Tables["TblDemandadosEjercicioActual"].AsEnumerable().Sum(x => x.Field<decimal>("Intereses") + x.Field<decimal>("Comisiones"));
                _carteraDemandados = _carteraDemandados + dsResumenRelacionGastos.Tables["TblDemandadosEjercicioActualNoRegistradosEnContabilidad"].AsEnumerable().Sum(x => x.Field<decimal>("Intereses") + x.Field<decimal>("Comisiones"));
                _carteraDemandados = _carteraDemandados + dsResumenRelacionGastos.Tables["TblPotros"].AsEnumerable().Sum(x => x.Field<decimal>("Intereses") + x.Field<decimal>("Comisiones"));

                _carteraCapitalVencida30a59dias = dsResumenRelacionGastos.Tables["TblVencidos60Dias"].AsEnumerable().Sum(x => x.Field<decimal>("Capital"));
                _carteraCapitalVencidamayor60dias = dsResumenRelacionGastos.Tables["TblVencidos90Dias"].AsEnumerable().Sum(x => x.Field<decimal>("Capital"));
                _carteraCapitalDemandados = dsResumenRelacionGastos.Tables["TblDemandadosEjercicioActual"].AsEnumerable().Sum(x => x.Field<decimal>("Capital"));
                _carteraCapitalDemandados = _carteraCapitalDemandados + dsResumenRelacionGastos.Tables["TblDemandadosEjercicioActualNoRegistradosEnContabilidad"].AsEnumerable().Sum(x => x.Field<decimal>("Capital"));
                _carteraCapitalDemandados = _carteraCapitalDemandados + dsResumenRelacionGastos.Tables["TblPotros"].AsEnumerable().Sum(x => x.Field<decimal>("Capital"));

                _ventasTotalesContable = _subVentasTotalesContable;
                _ventasTotalesReales = _subVentasTotalesReales - _carteraVencida30a59dias - _carteraVencidamayor60dias - _carteraDemandados;

                //OBTENCION COSTO DE FINANCIAMIENTO
                _costoFinanaciamientoContable = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "COSTO DE FINANCIAMIENTO" || x.Field<string>("Descripcion") == "COSTO FINANCIAMIENTO INTERCOMPAÑIA").Sum(x => x.Field<decimal>("Importe"));
                _interesesFinanciamientoIntercompania = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "COSTO FINANCIAMIENTO INTERCOMPAÑIA").Sum(x => x.Field<decimal>("ImporteReal"));
                _interesesTativoPorClientesFacturco = 0;//Falta buscar de donde se obtiene este dato
                _costoFinanaciamientoReal = _costoFinanaciamientoContable - _interesesFinanciamientoIntercompania - _interesesTativoPorClientesFacturco;

                _costoFinanciamientoRealContable = _costoFinanaciamientoContable;
                _costoFinanciamientoRealReales = _costoFinanaciamientoReal + _interesesFinanciamientoIntercompania + _interesesTativoPorClientesFacturco;

                //OBTENCION DE UTILIDAD BRUTA
                _utilidadBrutaContable = _ventasTotalesContable - _costoFinanciamientoRealContable;
                _utilidadBrutaReal = _ventasTotalesReales - _costoFinanciamientoRealReales;


                //OBTENCION DE GASTOS DE OPERACION
                _gastosDeVentaContable = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "GASTOS DE VENTA").Sum(x => x.Field<decimal>("Importe"));
                _gastosDeVentaReal = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "GASTOS DE VENTA").Sum(x => x.Field<decimal>("Importe"));
                _gastoDeVentaPersonales = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "GASTOS DE VENTA").Sum(x => x.Field<decimal>("ImportePersonal"));
                //_gastoFintac = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "GASTOS DE VENTA").Sum(x => x.Field<decimal>("ImporteFintac"));
                _gastoTativo = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "GASTOS DE VENTA").Sum(x => x.Field<decimal>("ImporteTativo"));
                if(empresa.Empresa == 1)
                {
                    _gastosDeVentaBalorAbsorbidasPorFacturco = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "GASTOS DE VENTA").Sum(x => x.Field<decimal>("ImporteFacturDeBalor") + x.Field<decimal>("ImportePersonalesFacturDeBalor"));
                }
                else
                {
                    _gastosDeVentaBalorAbsorbidasPorFacturco = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "GASTOS DE VENTA").Sum(x => x.Field<decimal>("ImporteFacturDeBalor") + x.Field<decimal>("ImportePersonalesFacturDeBalor"));
                }
                
                _gastosDeVentaPersonalesBalorAbsorbidasPorFacturco = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "GASTOS DE VENTA").Sum(x => x.Field<decimal>("ImportePersonalesFacturDeBalor"));
                _gastosDeVentaTotalContable = _gastosDeVentaContable;

                if (empresa.Empresa == 1)
                {
                    _gastosDeVentaTotalReal = _gastosDeVentaReal - _gastosDeVentaBalorAbsorbidasPorFacturco - _gastoDeVentaPersonales - _gastoTativo - _gastoFintac;
                }
                else
                {
                    _gastosDeVentaTotalReal = _gastosDeVentaReal + _gastosDeVentaBalorAbsorbidasPorFacturco - _gastoDeVentaPersonales - _gastoTativo - _gastoFintac - _gastosDeVentaPersonalesBalorAbsorbidasPorFacturco;
                }
                    

                //OBTENCIÓN DE GASTOS DE ADMINISTRACION
                _gastosDeAdministracionRealContable = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "GASTOS DE ADMINISTRACION").Sum(x => x.Field<decimal>("Importe"));
                _gastosDeAdministracionRealReal = _gastosDeAdministracionRealContable;

                _gastosDeAdministracionPersonales = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "GASTOS DE ADMINISTRACION").Sum(x => x.Field<decimal>("ImportePersonal"));
                if(empresa.Empresa == 1)
                {
                    _gastosDeAdministracionBalorAbsorbidasPorFacturco = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "GASTOS DE ADMINISTRACION").Sum(x => x.Field<decimal>("ImporteFacturDeBalor") + x.Field<decimal>("ImportePersonalesFacturDeBalor"));
                }
                else
                {
                    _gastosDeAdministracionBalorAbsorbidasPorFacturco = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "GASTOS DE ADMINISTRACION").Sum(x => x.Field<decimal>("ImporteFacturDeBalor") + x.Field<decimal>("ImportePersonalesFacturDeBalor"));
                }
                
                _gastosDeAdministracionPersonalesBalorAbsorbidasPorFacturco = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "GASTOS DE ADMINISTRACION").Sum(x => x.Field<decimal>("ImportePersonalesFacturDeBalor"));
                //Falta averiguar fintac
                //_gastosDeAdministracionFintac = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "GASTOS DE ADMINISTRACION").Sum(x => x.Field<decimal>("ImporteFintac"));
                _gastosDeAdministracionTativo = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "GASTOS DE ADMINISTRACION").Sum(x => x.Field<decimal>("ImporteTativo"));

                _gastosDeAdministracionRealTotalContable = _gastosDeAdministracionRealContable;
                if (empresa.Empresa == 1)
                {
                    _gastosDeAdministracionRealTotalReal = _gastosDeAdministracionRealReal - _gastosDeAdministracionBalorAbsorbidasPorFacturco - _gastosDeAdministracionPersonales - _gastosDeAdministracionFintac - _gastosDeAdministracionTativo;
                }
                else
                {
                    _gastosDeAdministracionRealTotalReal = _gastosDeAdministracionRealReal + _gastosDeAdministracionBalorAbsorbidasPorFacturco - _gastosDeAdministracionPersonales - _gastosDeAdministracionPersonalesBalorAbsorbidasPorFacturco - _gastosDeAdministracionFintac - _gastosDeAdministracionTativo;
                }
                    


                //OBTENCION DE DEDUCCION POR CUENTAS INCOBRABLES
                _deduccionPorCuentasIncobrablesContable = dsResumenRelacionGastos.Tables["TblMorosos"].AsEnumerable().Sum(x => x.Field<decimal>("Saldo"));
                _deduccionPorCuentasIncobrablesReal = dsResumenRelacionGastos.Tables["TblMorosos"].AsEnumerable().Sum(x => x.Field<decimal>("Saldo"));


                //OBTENCIÓN DE TOTAL DE GASTOS DE OPERACION

                _totalGastosDeOperacionContable = _gastosDeVentaTotalContable + _gastosDeAdministracionRealTotalContable + _deduccionPorCuentasIncobrablesContable;
                _totalGastosDeOperacionReal = _gastosDeVentaTotalReal + _gastosDeAdministracionRealTotalReal + _deduccionPorCuentasIncobrablesReal;

                //OBTENCION DE UTILIDADES ANTES DE OTROS INGRESOS, GASTOS
                _utilidadAntesDeOtrosIngresosContable = _utilidadBrutaContable - _totalGastosDeOperacionContable;
                _utilidadAntesDeOtrosIngresosReal = _utilidadBrutaReal - _totalGastosDeOperacionReal;

                //OBTENCION DE OTROS INGRESOS, GASTOS

                _UtilidadCambiariaContable = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "UTILIDAD CAMBIARIA" && x.Field<string>("Cuenta") == "710000050000000000000000").Sum(x => x.Field<decimal>("Importe"));
                _UtilidadCambiariaReal = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "UTILIDAD CAMBIARIA" && x.Field<string>("Cuenta") == "710000050000000000000000").Sum(x => x.Field<decimal>("ImporteReal"));

                _perdidaCambiariaContable = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "PERDIDA CAMBIARIA" && x.Field<string>("Cuenta") == "750000050000000000000000").Sum(x => x.Field<decimal>("Importe"));
                _perdidaCambiariaReal = dsGastos.Tables[0].AsEnumerable().Where(x => x.Field<string>("Descripcion") == "PERDIDA CAMBIARIA" && x.Field<string>("Cuenta") == "750000050000000000000000").Sum(x => x.Field<decimal>("ImporteReal"));
                
                //Otros ingresos y gastos se saca de las cuentas 7100,7200,7500, y 7750 exceptuando las subcuentas '710000050000000000000000' y '750000050000000000000000'
                //correspondientes a la Uitilidad Cambiaria y Perdida Cambiaria respectivamente ya que en el reporte se desglosan estas cuentas y se estarían sumando nuevamente

                _otrosIngresosYGastosContable = dsGastos.Tables[0].AsEnumerable().Where(x =>
                (x.Field<string>("Cuenta") == "710000000000000000000000" ||
                x.Field<string>("Cuenta") == "720000000000000000000000" 
                )).Sum(x => x.Field<decimal>("Importe"));

                _otrosIngresosYGastosContable = _otrosIngresosYGastosContable - dsGastos.Tables[0].AsEnumerable().Where(x =>
                (x.Field<string>("Cuenta") == "750000000000000000000000" ||
                x.Field<string>("Cuenta") == "775000000000000000000000")).Sum(x => x.Field<decimal>("Importe"));

                //restamos a _otrosIngresosYGastosContable el importe de las subcuentas 710000050000000000000000 y 750000050000000000000000                
                _otrosIngresosYGastosContable = _otrosIngresosYGastosContable - (_UtilidadCambiariaContable - _perdidaCambiariaContable);


                _otrosIngresosYGastosReal = dsGastos.Tables[0].AsEnumerable().Where(x =>
                (x.Field<string>("Cuenta") == "710000000000000000000000" ||
                x.Field<string>("Cuenta") == "720000000000000000000000"
                )).Sum(x => x.Field<decimal>("ImporteReal"));

                _otrosIngresosYGastosReal = _otrosIngresosYGastosReal - dsGastos.Tables[0].AsEnumerable().Where(x =>
                (x.Field<string>("Cuenta") == "750000000000000000000000" ||
                x.Field<string>("Cuenta") == "775000000000000000000000")).Sum(x => x.Field<decimal>("ImporteReal"));

                //restamos a _otrosIngresosYGastosReal el importe de las subcuentas 710000050000000000000000 y 750000050000000000000000
                _otrosIngresosYGastosReal = _otrosIngresosYGastosReal - (_UtilidadCambiariaReal - _perdidaCambiariaReal);



                //Multiplicamos por -1 en los conceptos de Perdida Cambiaria para que se reste (Especificado por contabilidad)
                _perdidaCambiariaContable = _perdidaCambiariaContable * -1;
                _perdidaCambiariaReal = _perdidaCambiariaReal * -1;


                _otrosIngresosYGastosContableAgrupado = _otrosIngresosYGastosContable + _UtilidadCambiariaContable + _perdidaCambiariaContable;
                _otrosIngresosYGastosRealAgrupado = _otrosIngresosYGastosReal + _UtilidadCambiariaReal + _perdidaCambiariaReal;

                _utilidadOPerdidaContableContable = _utilidadAntesDeOtrosIngresosContable + _otrosIngresosYGastosContableAgrupado;
                _utilidadOPerdidaContableReal = _utilidadAntesDeOtrosIngresosReal + _otrosIngresosYGastosRealAgrupado;

                _efectoCambiarioContable = _UtilidadCambiariaContable + _perdidaCambiariaContable;
                _efectoCambiarioReal = _UtilidadCambiariaReal + _perdidaCambiariaReal;

                _utilidadOPerdidaSinEfectoCambiariaContable = _utilidadOPerdidaContableContable - _efectoCambiarioContable;
                _utilidadOPerdidaSinEfectoCambiariaReal = _utilidadOPerdidaContableReal - _efectoCambiarioReal;


                _carteraVencidaYDemandadosEjercicioActual = _carteraCapitalVencida30a59dias + _carteraCapitalVencidamayor60dias + _carteraCapitalDemandados;
                

                _utilidadOPerdidaSinCapitalDeCarteraVencidaContable = _utilidadOPerdidaSinEfectoCambiariaContable;
                _utilidadOPerdidaSinCapitalDeCarteraVencidaReal = _utilidadOPerdidaSinEfectoCambiariaReal - _carteraVencidaYDemandadosEjercicioActual;

                #endregion


                //VENTAS
                r["VentasContable"] = _ventasContable;
                r["VentasReal"] = _ventasReal;
                r["InteresesClientesTativo"] = _interesesClientesTativo;
                r["InteresesIntercompañia"] = _interesesIntercompania;
                r["SubVentasTotalesContable"] = _subVentasTotalesContable;
                r["SubVentasTotalesReales"] = _subVentasTotalesReales;
                r["CarteraVencida30a59Dias"] = _carteraVencida30a59dias;
                r["CarteraVencidamayor60dias"] = _carteraVencidamayor60dias;
                r["CarteraDemandados"] = _carteraDemandados;
                r["VentasTotalesContable"] = _ventasTotalesContable;
                r["VentasTotalesReales"] = _ventasTotalesReales;

                //COSTO DE FINANCIAMIENTO
                r["CostoFinanciamientoContable"] = _costoFinanaciamientoContable;
                r["CostoFinanciamientoReal"] = _costoFinanaciamientoReal;
                r["InteresesFinanciamientoIntercompañia"] = _interesesFinanciamientoIntercompania;
                r["InteresesTativoPorClientesFacturco"] = _interesesTativoPorClientesFacturco;
                r["CostoFinanciamientoRealContable"] = _costoFinanciamientoRealContable;
                r["CostoFinanciamientoRealReales"] = _costoFinanciamientoRealReales;

                //UTILIDAD BRUTA
                r["UtilidadBrutaContable"] = _utilidadBrutaContable;
                r["UtilidadBrutaReal"] = _utilidadBrutaReal;

                //GASTOS DE OPERACION
                r["GastosDeVentaContable"] = _gastosDeVentaContable;
                r["GastosDeVentaReal"] = _gastosDeVentaReal;
                r["GastosDeVentaPersonales"] = _gastoDeVentaPersonales;
                r["GastoFintac"] = _gastoFintac;
                r["GastoTativo"] = _gastoTativo;
                r["GastosDeVentaBalorAbsorbidoPorFacturco"] = _gastosDeVentaBalorAbsorbidasPorFacturco;
                r["GastosDeVentaPersonalesBalorAbsorbidoPorFacturco"] = _gastosDeVentaPersonalesBalorAbsorbidasPorFacturco;
                r["GastosDeVentaTotalContable"] = _gastosDeVentaTotalContable;
                r["GastosDeVentaTotalReal"] = _gastosDeVentaTotalReal;

                //GASTOS DE ADMINISTRACION
                r["GastosDeAdministracionRealContable"] = _gastosDeAdministracionRealContable;
                r["GastosDeAdministracionRealReal"] = _gastosDeAdministracionRealReal;
                r["GastosDeAdministracionPersonales"] = _gastosDeAdministracionPersonales;
                r["GastosDeAdministracionFintac"] = _gastosDeAdministracionFintac;
                r["GastosDeAdministracionTativo"] = _gastosDeAdministracionTativo;
                r["GastosDeAdministracionBalorAbsorbidoPorFacturco"] = _gastosDeAdministracionBalorAbsorbidasPorFacturco;
                r["GastosDeAdministracionPersonalesBalorAbsorbidoPorFacturco"] = _gastosDeAdministracionPersonalesBalorAbsorbidasPorFacturco;                
                r["GastosDeAdministracionRealTotalContable"] = _gastosDeAdministracionRealTotalContable;
                r["GastosDeAdministracionRealTotalReal"] = _gastosDeAdministracionRealTotalReal;


                //CUENTAS INCOBRABLES
                r["DeduccionPorCuentasIncobrablesContable"] = _deduccionPorCuentasIncobrablesContable;
                r["DeduccionPorCuentasIncobrablesReal"] = _deduccionPorCuentasIncobrablesReal;

                //GASTOS DE OPERACION
                r["TotalGastosDeOperacionContable"] = _totalGastosDeOperacionContable;
                r["TotalGastosDeOperacionReal"] = _totalGastosDeOperacionReal;

                //UTILIDADES ANTES DE OTROS INGRESOS, GASTOS
                r["UtilidadAntesDeOtrosIngresosContable"] = _utilidadAntesDeOtrosIngresosContable;
                r["UtilidadAntesDeOtrosIngresosReal"] = _utilidadAntesDeOtrosIngresosReal;

                //OTROS INGRESOS, GASTOS
                r["UtilidadCambiariaContable"] = _UtilidadCambiariaContable;
                r["UtilidadCambiariaReal"] = _UtilidadCambiariaReal;
                r["PerdidaCambiariaContable"] = _perdidaCambiariaContable;
                r["PerdidaCambiariaReal"] = _perdidaCambiariaReal;
                r["OtrosIngresosYGastosContable"] = _otrosIngresosYGastosContable;
                r["OtrosIngresosYGastosReal"] = _otrosIngresosYGastosReal;
                r["OtrosIngresosYGastosContableAgrupado"] = _otrosIngresosYGastosContableAgrupado;
                r["OtrosIngresosYGastosRealAgrupado"] = _otrosIngresosYGastosRealAgrupado;
                r["UtilidadOPerdidaContableContable"] = _utilidadOPerdidaContableContable;
                r["UtilidadOPerdidaContableReal"] = _utilidadOPerdidaContableReal;
                r["EfectoCambiarioContable"] = _efectoCambiarioContable;
                r["EfectoCambiarioReal"] = _efectoCambiarioReal;
                r["UtilidadOPerdidaSinEfectoCambiariaContable"] = _utilidadOPerdidaSinEfectoCambiariaContable;
                r["UtilidadOPerdidaSinEfectoCambiariaReal"] = _utilidadOPerdidaSinEfectoCambiariaReal;
                r["CarteraVencidaYDemandadosEjercicioActual"] = _carteraVencidaYDemandadosEjercicioActual;
                r["UtilidadOPerdidaSinCapitalDeCarteraVencidaContable"] = _utilidadOPerdidaSinCapitalDeCarteraVencidaContable;
                r["UtilidadOPerdidaSinCapitalDeCarteraVencidaReal"] = _utilidadOPerdidaSinCapitalDeCarteraVencidaReal;

                r["PorcentajeVentasTotalesRealesContable"] = (_ventasContable == 0) ? 0 : (_ventasTotalesContable / _ventasContable) * 100;
                //r["PorcentajeVentasTotalesRealesReal"] = (_ventasTotalesReales / _subVentasTotalesReales) * 100;
                r["PorcentajeVentasTotalesRealesReal"] = (_ventasTotalesReales / _ventasTotalesReales) * 100;
                r["PorcentajeCostoFinanciamientoRealesContable"] = (_ventasContable == 0) ? 0 : (_costoFinanciamientoRealContable / _ventasContable) * 100;
                //r["PorcentajeCostoFinanciamientoRealesReal"] = (_costoFinanciamientoRealReales / _subVentasTotalesReales) * 100;
                r["PorcentajeCostoFinanciamientoRealesReal"] = (_costoFinanciamientoRealReales / _ventasTotalesReales) * 100;
                r["PorcentajeUtilidadBrutaContable"] = (_ventasContable == 0) ? 0 : (_utilidadBrutaContable / _ventasContable) * 100; ;
                //r["PorcentajeUtilidadBrutaReal"] = (_utilidadBrutaReal / _subVentasTotalesReales) * 100; ;
                r["PorcentajeUtilidadBrutaReal"] = (_utilidadBrutaReal / _ventasTotalesReales) * 100; ;
                r["PorcentajeGastosDeVentaContable"] = (_ventasContable == 0) ? 0 : (_gastosDeVentaContable / _ventasContable) * 100;
                //r["PorcentajeGastosDeVentaReal"] = (_gastosDeVentaReal / _subVentasTotalesReales) * 100;
                r["PorcentajeGastosDeVentaReal"] = (_gastosDeVentaReal / _ventasTotalesReales) * 100;
                r["PorcentajeGastosDeVentaRealContable"] = (_ventasContable == 0) ? 0 : (_gastosDeVentaTotalContable / _ventasContable) * 100;
                //r["PorcentajeGastosDeVentaRealReal"] = (_gastosDeVentaTotalReal / _subVentasTotalesReales) * 100;
                r["PorcentajeGastosDeVentaRealReal"] = (_gastosDeVentaTotalReal / _ventasTotalesReales) * 100;
                r["PorcentajeGastosDeAdministracionContable"] = (_ventasContable == 0) ? 0 : (_gastosDeAdministracionRealContable / _ventasContable) * 100;
                //r["PorcentajeGastosDeAdministracionReal"] = (_gastosDeAdministracionRealReal / _subVentasTotalesReales) * 100;
                r["PorcentajeGastosDeAdministracionReal"] = (_gastosDeAdministracionRealReal / _ventasTotalesReales) * 100;
                r["PorcentajeGastosDeAdministracionRealContable"] = (_ventasContable == 0) ? 0 : (_gastosDeAdministracionRealTotalContable / _ventasContable) * 100;
                //r["PorcentajeGastosDeAdministracionRealreal"] = (_gastosDeAdministracionRealTotalReal / _subVentasTotalesReales) * 100;
                r["PorcentajeGastosDeAdministracionRealreal"] = (_gastosDeAdministracionRealTotalReal / _ventasTotalesReales) * 100;
                r["PorcentajeDeduccionCuentasIncobrablesContable"] = (_ventasContable == 0) ? 0 : (_deduccionPorCuentasIncobrablesContable / _ventasContable) * 100;
                //r["PorcentajeDeduccionCuentasIncobrablesReal"] = (_deduccionPorCuentasIncobrablesReal / _subVentasTotalesReales) * 100;
                r["PorcentajeDeduccionCuentasIncobrablesReal"] = (_deduccionPorCuentasIncobrablesReal / _ventasTotalesReales) * 100;
                r["PorcentajeTotalGastosDeOperacionContable"] = (_ventasContable == 0) ? 0 : (_totalGastosDeOperacionContable / _ventasContable) * 100;
                //r["PorcentajeTotalGastosDeOperacionReal"] = (_totalGastosDeOperacionReal / _subVentasTotalesReales) * 100; 
                r["PorcentajeTotalGastosDeOperacionReal"] = (_totalGastosDeOperacionReal / _ventasTotalesReales) * 100; 
                r["PorcentajeUtilidadesAntesDeOtrosIngresosContable"] = (_ventasContable == 0) ? 0 : (_utilidadAntesDeOtrosIngresosContable / _ventasContable) * 100;
                //r["PorcentajeUtilidadesAntesDeOtrosIngresosReal"] = (_utilidadAntesDeOtrosIngresosReal / _subVentasTotalesReales) * 100;
                r["PorcentajeUtilidadesAntesDeOtrosIngresosReal"] = (_utilidadAntesDeOtrosIngresosReal / _ventasTotalesReales) * 100;
                r["PorcentajeOtrosIngresosYGastosAgrupadoContable"] = (_ventasContable == 0) ? 0 : (_otrosIngresosYGastosContableAgrupado / _ventasContable) * 100;
                //r["PorcentajeOtrosIngresosYGastosAgrupadoReal"] = (_otrosIngresosYGastosRealAgrupado / _subVentasTotalesReales) * 100;
                r["PorcentajeOtrosIngresosYGastosAgrupadoReal"] = (_otrosIngresosYGastosRealAgrupado / _ventasTotalesReales) * 100;
                r["PorcentajeUtilidadCambiariaContable"] = (_ventasContable == 0) ? 0 : (_UtilidadCambiariaContable / _ventasContable) * 100;
                //r["PorcentajeUtilidadCambiariaReal"] = (_UtilidadCambiariaReal / _subVentasTotalesReales) * 100;
                r["PorcentajeUtilidadCambiariaReal"] = (_UtilidadCambiariaReal / _ventasTotalesReales) * 100;
                r["PorcentajePerdidaCambiariaContable"] = (_ventasContable == 0) ? 0 : (_perdidaCambiariaContable / _ventasContable) * 100;
                //r["PorcentajePerdidaCambiariaReal"] = (_perdidaCambiariaReal / _subVentasTotalesReales) * 100;
                r["PorcentajePerdidaCambiariaReal"] = (_perdidaCambiariaReal / _ventasTotalesReales) * 100;
                r["PorcentajeOtrosIngresosYGastosContable"] = (_ventasContable == 0) ? 0 : (_otrosIngresosYGastosContable / _ventasContable) * 100;
                //r["PorcentajeOtrosIngresosYGastosReal"] = (_otrosIngresosYGastosReal / _subVentasTotalesReales) * 100;
                r["PorcentajeOtrosIngresosYGastosReal"] = (_otrosIngresosYGastosReal / _ventasTotalesReales) * 100;
                r["PorcentajeUtilidadOPerdidaContableContable"] = (_ventasContable == 0) ? 0 : (_utilidadOPerdidaContableContable / _ventasContable) * 100;
                //r["PorcentajeUtilidadOPerdidaContableReal"] = (_utilidadOPerdidaContableReal / _subVentasTotalesReales) * 100;
                r["PorcentajeUtilidadOPerdidaContableReal"] = (_utilidadOPerdidaContableReal / _ventasTotalesReales) * 100;
                r["PorcentajeEfectoCambiarioContable"] = (_ventasContable == 0) ? 0 : (_efectoCambiarioContable / _ventasContable) * 100;
                //r["PorcentajeEfectoCambiarioReal"] = (_efectoCambiarioReal / _subVentasTotalesReales) * 100;
                r["PorcentajeEfectoCambiarioReal"] = (_efectoCambiarioReal / _ventasTotalesReales) * 100;
                r["PorcentajeUtilidadOPerdidaSinEfectoCambiarioContable"] = (_ventasContable == 0) ? 0 : (_utilidadOPerdidaSinEfectoCambiariaContable / _ventasContable) * 100;
                //r["PorcentajeUtilidadOPerdidaSinEfectoCambiarioReal"] = (_utilidadOPerdidaSinEfectoCambiariaReal / _subVentasTotalesReales) * 100;
                r["PorcentajeUtilidadOPerdidaSinEfectoCambiarioReal"] = (_utilidadOPerdidaSinEfectoCambiariaReal / _ventasTotalesReales) * 100;
                r["PorcentajeUtilidadOPerdidaSinCarteraVencidaContable"] = (_ventasContable == 0) ? 0 : (_utilidadOPerdidaSinCapitalDeCarteraVencidaContable / _ventasContable) * 100;
                //r["PorcentajeUtilidadOPerdidaSinCarteraVencidaReal"] = (_utilidadOPerdidaSinCapitalDeCarteraVencidaReal / _subVentasTotalesReales) * 100;
                r["PorcentajeUtilidadOPerdidaSinCarteraVencidaReal"] = (_utilidadOPerdidaSinCapitalDeCarteraVencidaReal / _ventasTotalesReales) * 100;

                dt.Rows.Add(r);
                ds.Tables.Add(dt);



                DataSet dsEmpresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(empresa.Empresaid);
                dsEmpresa.Tables[0].TableName = "tblEmpresa";
                ds.Tables.Add(dsEmpresa.Tables[0].Copy());

                DataTable DatosReporte = new DataTable("DatosReporte");
                DatosReporte.Columns.Add("Fecha", typeof(DateTime));
                DataRow rd = DatosReporte.NewRow();
                rd["Fecha"] = fecha;
                DatosReporte.Rows.Add(rd);
                ds.Tables.Add(DatosReporte);


                dsGastos.Tables[0].TableName = "tblGastos1";
                dsGastos.Tables[1].TableName = "tblGastos2";
                dsGastos.Tables[2].TableName = "tblGastosCuentasCierre";
                dsGastos.Tables[3].TableName = "TblEstadoResultados2020"; //Tabla que trae el estado de resultados del 2020 ya que este se tiene datos fijos , en enero se realizarán los cambios para que se obtengan dinamicamente

                ds.Tables.Add(dsGastos.Tables[0].Copy());
                ds.Tables.Add(dsGastos.Tables[1].Copy());
                ds.Tables.Add(dsGastos.Tables[2].Copy());
                ds.Tables.Add(dsGastos.Tables[3].Copy());




            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ds;
        }

        public static System.Data.DataSet ProcesaEstadoDeResultadosConsolidado(DateTime fecha)
        {
            DataSet ds = new DataSet();
            DataTable tblEmpresaFacturco = new DataTable("tblEmpresaFacturco");
            DataTable tblEmpresaBalor = new DataTable("tblEmpresaBalor");

            DataTable dt = new DataTable("EstadoDeResultadosConsolidado");
            try
            {
                #region Configuracion del dataset
                //PAnt = "Periodo Anterior"
                //PAct = "Periodo Actual"
                
                
                //VENTAS
                dt.Columns.Add("PAntIngresosContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntIngresosReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActIngresosContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActIngresosReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeIngresosContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeIngresosReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeIngresosContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeIngresosReal", typeof(decimal)).DefaultValue = 0;
                //COSTO DE FINANCIAMIENTO
                dt.Columns.Add("PAntCostoFinanciamientoContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntCostoFinanciamientoReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActCostoFinanciamientoContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActCostoFinanciamientoReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeCostoFinanciamientoContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeCostoFinanciamientoReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeCostoFinanciamientoContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeCostoFinanciamientoReal", typeof(decimal)).DefaultValue = 0;
                //UTILIDAD BRUTA
                dt.Columns.Add("PAntUtilidadBrutaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntUtilidadBrutaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActUtilidadBrutaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActUtilidadBrutaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeUtilidadBrutaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeUtilidadBrutaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeUtilidadBrutaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeUtilidadBrutaReal", typeof(decimal)).DefaultValue = 0;
                //GASTOS DE VENTA
                dt.Columns.Add("PAntGastosDeVentaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntGastosDeVentaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActGastosDeVentaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActGastosDeVentaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeGastosDeVentaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeGastosDeVentaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeGastosDeVentaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeGastosDeVentaReal", typeof(decimal)).DefaultValue = 0;
                //GASTOS DE OPERACION
                dt.Columns.Add("PAntGastosDeOperacionContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntGastosDeOperacionReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActGastosDeOperacionContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActGastosDeOperacionReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeGastosDeOperacionContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeGastosDeOperacionReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeGastosDeOperacionContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeGastosDeOperacionReal", typeof(decimal)).DefaultValue = 0;
                //GASTOS DE ADMINISTRACION
                dt.Columns.Add("PAntGastosDeAdministracionContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntGastosDeAdministracionReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActGastosDeAdministracionContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActGastosDeAdministracionReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeGastosDeAdministracionContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeGastosDeAdministracionReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeGastosDeAdministracionContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeGastosDeAdministracionReal", typeof(decimal)).DefaultValue = 0;

                //DEDUCCION POR CUENTAS INCOBRABLES
                dt.Columns.Add("PAntDeduccionPorCuentasIncobrablesContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntDeduccionPorCuentasIncobrablesReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActDeduccionPorCuentasIncobrablesContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActDeduccionPorCuentasIncobrablesReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeDeduccionPorCuentasIncobrablesContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeDeduccionPorCuentasIncobrablesReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeDeduccionPorCuentasIncobrablesContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeDeduccionPorCuentasIncobrablesReal", typeof(decimal)).DefaultValue = 0;

                //GASTOS FINANCIEROS Y OTROS GASTOS
                dt.Columns.Add("PAntGastosFinancierosYOtrosGastosContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntGastosFinancierosYOtrosGastosReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActGastosFinancierosYOtrosGastosContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActGastosFinancierosYOtrosGastosReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeGastosFinancierosYOtrosGastosContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeGastosFinancierosYOtrosGastosReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeGastosFinancierosYOtrosGastosContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeGastosFinancierosYOtrosGastosReal", typeof(decimal)).DefaultValue = 0;


                //OTROS INGRESOS
                dt.Columns.Add("PAntOtrosIngresosContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntOtrosIngresosReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActOtrosIngresosContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActOtrosIngresosReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeOtrosIngresosContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeOtrosIngresosReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeOtrosIngresosContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeOtrosIngresosReal", typeof(decimal)).DefaultValue = 0;


                //UTILIDAD O PERDIDA CAMBIARIA
                dt.Columns.Add("PAntUtilidadOPerdidaCambiariaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntUtilidadOPerdidaCambiariaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActUtilidadOPerdidaCambiariaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActUtilidadOPerdidaCambiariaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeUtilidadOPerdidaCambiariaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeUtilidadOPerdidaCambiariaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeUtilidadOPerdidaCambiariaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeUtilidadOPerdidaCambiariaReal", typeof(decimal)).DefaultValue = 0;

                //IMPUESTOS A LA UTILIDAD-DIFERIDOS
                dt.Columns.Add("PAntImpuestosALaUtilidadContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntImpuestosALaUtilidadReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActImpuestosALaUtilidadContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActImpuestosALaUtilidadReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeImpuestosALaUtilidadContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeImpuestosALaUtilidadReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeImpuestosALaUtilidadContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeImpuestosALaUtilidadReal", typeof(decimal)).DefaultValue = 0;


                //IMPUESTOS RESULTADO DEL EJERCICIO
                dt.Columns.Add("PAntImpuestosResultadoEjercicioContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntImpuestosResultadoEjercicioReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActImpuestosResultadoEjercicioContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActImpuestosResultadoEjercicioReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeImpuestosResultadoEjercicioContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeImpuestosResultadoEjercicioReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeImpuestosResultadoEjercicioContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeImpuestosResultadoEjercicioReal", typeof(decimal)).DefaultValue = 0;

                //PTU DEL EJERCICIO
                dt.Columns.Add("PAntPtuDelEjercicioContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPtuDelEjercicioReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPtuDelEjercicioContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPtuDelEjercicioReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajePtuDelEjercicioContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajePtuDelEjercicioReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajePtuDelEjercicioContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajePtuDelEjercicioReal", typeof(decimal)).DefaultValue = 0;

                //UTILIDAD NETA
                dt.Columns.Add("PAntUtilidadNetaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntUtilidadNetaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActUtilidadNetaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActUtilidadNetaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeUtilidadNetaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeUtilidadNetaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeUtilidadNetaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeUtilidadNetaReal", typeof(decimal)).DefaultValue = 0;

                dt.Columns.Add("PAntUtilidadNetaFacturContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntUtilidadNetaBalorContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntUtilidadNetaTativoContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntUtilidadNetaFacturReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntUtilidadNetaBalorReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntUtilidadNetaTativoReal", typeof(decimal)).DefaultValue = 0;

                dt.Columns.Add("PActUtilidadNetaFacturContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActUtilidadNetaBalorContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActUtilidadNetaTativoContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActUtilidadNetaFacturReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActUtilidadNetaBalorReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActUtilidadNetaTativoReal", typeof(decimal)).DefaultValue = 0;

                dt.Columns.Add("PAntUtilidadNetaTotalContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntUtilidadNetaTotalReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActUtilidadNetaTotalContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActUtilidadNetaTotalReal", typeof(decimal)).DefaultValue = 0;

                //EFECTO CAMBIARIO
                dt.Columns.Add("PAntEfectoCambiarioContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntEfectoCambiarioReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActEfectoCambiarioContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActEfectoCambiarioReal", typeof(decimal)).DefaultValue = 0;


                //UTILIDAD O PERDIDA SIN EFECTO CAMBIARIO
                dt.Columns.Add("PAntUtilidadOPerdidaSinEfectoCambiarioContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntUtilidadOPerdidaSinEfectoCambiarioReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActUtilidadOPerdidaSinEfectoCambiarioContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActUtilidadOPerdidaSinEfectoCambiarioReal", typeof(decimal)).DefaultValue = 0;

                dt.Columns.Add("PAntPorcentajeUtilidadOPerdidaSinEfectoCambiarioContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeUtilidadOPerdidaSinEfectoCambiarioReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeUtilidadOPerdidaSinEfectoCambiarioContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeUtilidadOPerdidaSinEfectoCambiarioReal", typeof(decimal)).DefaultValue = 0;

                //SUELDOS
                dt.Columns.Add("PAntSueldosVentasContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntSueldosVentasReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntSueldosAdministracionContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntSueldosAdministracionReal", typeof(decimal)).DefaultValue = 0;

                dt.Columns.Add("PActSueldosVentasContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActSueldosVentasReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActSueldosAdministracionContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActSueldosAdministracionReal", typeof(decimal)).DefaultValue = 0;

                dt.Columns.Add("PAntPorcentajeSueldosVentasContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeSueldosVentasReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeSueldosAdministracionContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeSueldosAdministracionReal", typeof(decimal)).DefaultValue = 0;

                dt.Columns.Add("PActPorcentajeSueldosVentasContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeSueldosVentasReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeSueldosAdministracionContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeSueldosAdministracionReal", typeof(decimal)).DefaultValue = 0;

                dt.Columns.Add("PAntTotalSueldosContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntTotalSueldosReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActTotalSueldosContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActTotalSueldosReal", typeof(decimal)).DefaultValue = 0;

                dt.Columns.Add("PAntPorcentajeTotalSueldosContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntPorcentajeTotalSueldosReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeTotalSueldosContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActPorcentajeTotalSueldosReal", typeof(decimal)).DefaultValue = 0;

                //GASTOS PERSONALES
                dt.Columns.Add("PAntGastosPersonalesDeVentaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntGastosPersonalesDeAdministracionContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntGastosPersonalesPtuContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntGastosPersonalesDeVentaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntGastosPersonalesDeAdministracionReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntGastosPersonalesPtuReal", typeof(decimal)).DefaultValue = 0;

                dt.Columns.Add("PActGastosPersonalesDeVentaContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActGastosPersonalesDeAdministracionContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActGastosPersonalesPtuContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActGastosPersonalesDeVentaReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActGastosPersonalesDeAdministracionReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActGastosPersonalesPtuReal", typeof(decimal)).DefaultValue = 0;

                dt.Columns.Add("PAntTotalGastosPersonalesContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PAntTotalGastosPersonalesReal", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActTotalGastosPersonalesContable", typeof(decimal)).DefaultValue = 0;
                dt.Columns.Add("PActTotalGastosPersonalesReal", typeof(decimal)).DefaultValue = 0;
                #endregion

                DataRow r = dt.NewRow();


                string _empresaFacturco = "A7D3E5A4-6508-483B-8A3D-0E379FF06755";
                string _empresaBalor = "FA764836-BB07-4EB3-9B30-2B69206174C2";
                int _mesDelPeriodo = fecha.Month;

                tblEmpresaFacturco = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(_empresaFacturco).Tables[0].Copy();
                tblEmpresaBalor = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(_empresaBalor).Tables[0].Copy();

                tblEmpresaFacturco.TableName = "tblEmpresaFacturco";
                tblEmpresaBalor.TableName = "tblEmpresaBalor";



                DataSet dsPeriodoActualFacturco = ReporteResumenRelacionSaldos.ProcesaEstadoDeResultadosDetalle(_empresaFacturco, fecha);
                DataSet dsPeriodoActualBalor = ReporteResumenRelacionSaldos.ProcesaEstadoDeResultadosDetalle(_empresaBalor, fecha);

                DataSet dsPeriodoAnteriorFacturco = ReporteResumenRelacionSaldos.ProcesaEstadoDeResultadosDetalle(_empresaFacturco, fecha.AddYears(-1));
                DataSet dsPeriodoAnteriorBalor = ReporteResumenRelacionSaldos.ProcesaEstadoDeResultadosDetalle(_empresaBalor, fecha.AddYears(-1));
             

                DataRow rPeriodoAnteriorFacturco = dsPeriodoAnteriorFacturco.Tables[0].Rows[0];
                DataRow rPeriodoAnteriorBalor = dsPeriodoAnteriorBalor.Tables[0].Rows[0];               

                DataRow rPeriodoActualFacturco = dsPeriodoActualFacturco.Tables[0].Rows[0];
                DataRow rPeriodoActualBalor = dsPeriodoActualBalor.Tables[0].Rows[0];



                #region Inician Cálculos

                //INGRESOS
                r["PAntIngresosContable"] = ((Convert.ToDouble(rPeriodoAnteriorFacturco["SubVentasTotalesContable"]) + Convert.ToDouble(rPeriodoAnteriorBalor["SubVentasTotalesContable"]) + Convert.ToDouble(rPeriodoAnteriorFacturco["InteresesClientesTativo"])) / 12) * _mesDelPeriodo;
                r["PAntIngresosReal"] = (Convert.ToDouble(rPeriodoAnteriorBalor["SubVentasTotalesReales"]) == 0) ? 0 : ((Convert.ToDouble(rPeriodoAnteriorFacturco["SubVentasTotalesReales"]) + Convert.ToDouble(rPeriodoAnteriorBalor["SubVentasTotalesReales"])) / 12) * _mesDelPeriodo;
                r["PActIngresosContable"] = (Convert.ToDouble(rPeriodoActualFacturco["SubVentasTotalesContable"]) + Convert.ToDouble(rPeriodoActualFacturco["InteresesClientesTativo"]) + Convert.ToDouble(rPeriodoActualBalor["SubVentasTotalesContable"]) + Convert.ToDouble(rPeriodoActualBalor["InteresesClientesTativo"])) ;
                r["PActIngresosReal"] = (Convert.ToDouble(rPeriodoActualFacturco["SubVentasTotalesReales"]) + Convert.ToDouble(rPeriodoActualBalor["SubVentasTotalesReales"])) ;

                //COSTOS DE FINANCIAMIENTO
                r["PAntCostoFinanciamientoContable"] = (Convert.ToDouble(rPeriodoAnteriorFacturco["CostoFinanciamientoRealContable"]) + Convert.ToDouble(rPeriodoAnteriorBalor["CostoFinanciamientoRealContable"]) == 0) ? 0 :((Convert.ToDouble(rPeriodoAnteriorFacturco["CostoFinanciamientoRealContable"]) + Convert.ToDouble(rPeriodoAnteriorBalor["CostoFinanciamientoRealContable"])) / 12) * _mesDelPeriodo;
                r["PAntCostoFinanciamientoReal"] = (Convert.ToDouble(rPeriodoAnteriorFacturco["CostoFinanciamientoRealReales"]) + Convert.ToDouble(rPeriodoAnteriorBalor["CostoFinanciamientoRealReales"])) == 0 ? 0 : ((Convert.ToDouble(rPeriodoAnteriorFacturco["CostoFinanciamientoRealReales"]) + Convert.ToDouble(rPeriodoAnteriorBalor["CostoFinanciamientoRealReales"])) / 12) * _mesDelPeriodo;
                r["PActCostoFinanciamientoContable"] = (Convert.ToDouble(rPeriodoActualFacturco["CostoFinanciamientoRealContable"]) + Convert.ToDouble(rPeriodoActualBalor["CostoFinanciamientoRealContable"]));
                r["PActCostoFinanciamientoReal"] = (Convert.ToDouble(rPeriodoActualFacturco["CostoFinanciamientoRealReales"]) + Convert.ToDouble(rPeriodoActualBalor["CostoFinanciamientoRealReales"]));

                r["PAntPorcentajeCostoFinanciamientoContable"] = Convert.ToDecimal(r["PAntIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntCostoFinanciamientoContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100 ;
                r["PAntPorcentajeCostoFinanciamientoReal"] = Convert.ToDecimal(r["PAntIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntCostoFinanciamientoReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;
                r["PActPorcentajeCostoFinanciamientoContable"] = Convert.ToDecimal(r["PActIngresosContable"]) == 0 ? 0 :(Convert.ToDecimal(r["PActCostoFinanciamientoContable"]) / Convert.ToDecimal(r["PActIngresosContable"])) * 100;
                r["PActPorcentajeCostoFinanciamientoReal"] = Convert.ToDecimal(r["PActIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PActCostoFinanciamientoReal"]) / Convert.ToDecimal(r["PActIngresosReal"])) * 100;

                //UTILIDAD BRUTA
                r["PAntUtilidadBrutaContable"] = Convert.ToDouble(r["PAntIngresosContable"]) - Convert.ToDouble(r["PAntCostoFinanciamientoContable"]);
                r["PAntUtilidadBrutaReal"] = Convert.ToDouble(r["PAntIngresosReal"]) - Convert.ToDouble(r["PAntCostoFinanciamientoReal"]);
                r["PActUtilidadBrutaContable"] = (Convert.ToDouble(r["PActIngresosContable"]) - Convert.ToDouble(r["PActCostoFinanciamientoContable"]));
                r["PActUtilidadBrutaReal"] = (Convert.ToDouble(r["PActIngresosReal"]) - Convert.ToDouble(r["PActCostoFinanciamientoReal"]));

                r["PAntPorcentajeUtilidadBrutaContable"] = Convert.ToDecimal(r["PAntIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntUtilidadBrutaContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                r["PAntPorcentajeUtilidadBrutaReal"] = Convert.ToDecimal(r["PAntIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntUtilidadBrutaReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;
                r["PActPorcentajeUtilidadBrutaContable"] = Convert.ToDecimal(r["PActIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PActUtilidadBrutaContable"]) / Convert.ToDecimal(r["PActIngresosContable"])) * 100;
                r["PActPorcentajeUtilidadBrutaReal"] = Convert.ToDecimal(r["PActIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PActUtilidadBrutaReal"]) / Convert.ToDecimal(r["PActIngresosReal"])) * 100;


                //GASTOS DE VENTA
                double PAntGastosDeVentaContable = (Convert.ToDouble(rPeriodoAnteriorFacturco["GastosDeVentaTotalContable"]) + Convert.ToDouble(rPeriodoAnteriorBalor["GastosDeVentaTotalContable"]) - (Convert.ToDouble(rPeriodoAnteriorFacturco["DeduccionPorCuentasIncobrablesContable"]) + Convert.ToDouble(rPeriodoAnteriorBalor["DeduccionPorCuentasIncobrablesContable"])));
                double PAntGastosDeVentaReal = (Convert.ToDouble(rPeriodoAnteriorFacturco["GastosDeVentaTotalContable"]) + Convert.ToDouble(rPeriodoAnteriorBalor["GastosDeVentaTotalContable"]) - (Convert.ToDouble(rPeriodoAnteriorFacturco["DeduccionPorCuentasIncobrablesContable"]) + Convert.ToDouble(rPeriodoAnteriorBalor["DeduccionPorCuentasIncobrablesContable"])));

                r["PAntGastosDeVentaContable"] = PAntGastosDeVentaContable == 0 ? 0 : (PAntGastosDeVentaContable / 12) * _mesDelPeriodo;
                r["PAntGastosDeVentaReal"] = PAntGastosDeVentaReal == 0 ? 0 : (PAntGastosDeVentaReal / 12) * _mesDelPeriodo;
                r["PActGastosDeVentaContable"] = Convert.ToDouble(rPeriodoActualFacturco["GastosDeVentaTotalContable"]) + Convert.ToDouble(rPeriodoActualBalor["GastosDeVentaTotalContable"]) - (Convert.ToDouble(rPeriodoActualFacturco["DeduccionPorCuentasIncobrablesContable"]) + Convert.ToDouble(rPeriodoActualBalor["DeduccionPorCuentasIncobrablesContable"]));
                //r["PActGastosDeVentaReal"] = (Convert.ToDecimal(rPeriodoActualFacturco["GastosDeVentaTotalReal"]) + Convert.ToDecimal(rPeriodoActualBalor["GastosDeVentaTotalReal"])) - (Convert.ToDecimal(rPeriodoActualFacturco["DeduccionPorCuentasIncobrablesReal"]) + Convert.ToDecimal(rPeriodoActualBalor["DeduccionPorCuentasIncobrablesReal"])) - (dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "640200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "640200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal")));
                r["PActGastosDeVentaReal"] = Convert.ToDecimal(r["PActGastosDeVentaContable"]) - (dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "640200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "640200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal")));
                r["PAntPorcentajeGastosDeVentaContable"] = Convert.ToDecimal(r["PAntIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntGastosDeVentaContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                r["PAntPorcentajeGastosDeVentaReal"] = Convert.ToDecimal(r["PAntIngresosReal"]) == 0 ? 0 :(Convert.ToDecimal(r["PAntGastosDeVentaReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;
                r["PActPorcentajeGastosDeVentaContable"] = Convert.ToDecimal(r["PActIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PActGastosDeVentaContable"]) / Convert.ToDecimal(r["PActIngresosContable"])) * 100;
                r["PActPorcentajeGastosDeVentaReal"] = Convert.ToDecimal(r["PActIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PActGastosDeVentaReal"]) / Convert.ToDecimal(r["PActIngresosReal"])) * 100;


                //GASTOS DE ADMINISTRACION
                r["PAntGastosDeAdministracionContable"] = Convert.ToDouble(rPeriodoAnteriorFacturco["GastosDeAdministracionRealTotalContable"]) + Convert.ToDouble(rPeriodoAnteriorBalor["GastosDeAdministracionRealTotalContable"]);
                r["PAntGastosDeAdministracionReal"] = Convert.ToDouble(rPeriodoAnteriorFacturco["GastosDeAdministracionRealTotalReal"]) + Convert.ToDouble(rPeriodoAnteriorBalor["GastosDeAdministracionRealTotalReal"]);
                r["PActGastosDeAdministracionContable"] = Convert.ToDouble(rPeriodoActualFacturco["GastosDeAdministracionRealTotalContable"]) + Convert.ToDouble(rPeriodoActualBalor["GastosDeAdministracionRealTotalContable"]);
                r["PActGastosDeAdministracionReal"] = Convert.ToDouble(rPeriodoActualFacturco["GastosDeAdministracionRealTotalReal"]) + Convert.ToDouble(rPeriodoActualBalor["GastosDeAdministracionRealTotalReal"]);
                r["PAntPorcentajeGastosDeAdministracionContable"] = Convert.ToDecimal(r["PAntIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntGastosDeAdministracionContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                r["PAntPorcentajeGastosDeAdministracionReal"] = Convert.ToDecimal(r["PAntIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntGastosDeAdministracionReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;
                r["PActPorcentajeGastosDeAdministracionContable"] = Convert.ToDecimal(r["PActIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PActGastosDeAdministracionContable"]) / Convert.ToDecimal(r["PActIngresosContable"])) * 100;
                r["PActPorcentajeGastosDeAdministracionReal"] = Convert.ToDecimal(r["PActIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PActGastosDeAdministracionReal"]) / Convert.ToDecimal(r["PActIngresosReal"])) * 100;



                //DEDUCCION POR CUENTAS INCOBRABLES
                r["PAntDeduccionPorCuentasIncobrablesContable"] = Convert.ToDouble(rPeriodoAnteriorFacturco["DeduccionPorCuentasIncobrablesContable"]) + Convert.ToDouble(rPeriodoAnteriorBalor["DeduccionPorCuentasIncobrablesContable"]);
                r["PAntDeduccionPorCuentasIncobrablesReal"] = Convert.ToDouble(rPeriodoAnteriorFacturco["DeduccionPorCuentasIncobrablesReal"]) + Convert.ToDouble(rPeriodoAnteriorBalor["DeduccionPorCuentasIncobrablesReal"]);
                r["PActDeduccionPorCuentasIncobrablesContable"] = Convert.ToDouble(rPeriodoActualFacturco["DeduccionPorCuentasIncobrablesContable"]) + Convert.ToDouble(rPeriodoActualBalor["DeduccionPorCuentasIncobrablesContable"]);
                r["PActDeduccionPorCuentasIncobrablesReal"] = Convert.ToDouble(rPeriodoActualFacturco["DeduccionPorCuentasIncobrablesReal"]) + Convert.ToDouble(rPeriodoActualBalor["DeduccionPorCuentasIncobrablesReal"]);
                r["PAntPorcentajeDeduccionPorCuentasIncobrablesContable"] = Convert.ToDecimal(r["PAntIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntDeduccionPorCuentasIncobrablesContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                r["PAntPorcentajeDeduccionPorCuentasIncobrablesReal"] = Convert.ToDouble(r["PAntIngresosContable"]) == 0 ? 0 : (Convert.ToDouble(r["PAntDeduccionPorCuentasIncobrablesReal"]) / Convert.ToDouble(r["PAntIngresosContable"])) * 100;
                r["PActPorcentajeDeduccionPorCuentasIncobrablesContable"] = Convert.ToDecimal(r["PActIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PActDeduccionPorCuentasIncobrablesContable"]) / Convert.ToDecimal(r["PActIngresosContable"])) * 100;
                r["PActPorcentajeDeduccionPorCuentasIncobrablesReal"] = Convert.ToDouble(r["PActIngresosContable"]) == 0 ? 0 : (Convert.ToDouble(r["PActDeduccionPorCuentasIncobrablesReal"]) / Convert.ToDouble(r["PActIngresosContable"])) * 100;


                //GASTOS FINANCIEROS Y OTROS GASTOS
                decimal _gastosFinancieros = 0;
                decimal _otrosGastos = 0;
                decimal _perdidaCambiaria = 0;

                _gastosFinancieros = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "750000000000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "750000000000000000000000").Sum(x => x.Field<decimal>("Importe"));
                _otrosGastos = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "775000000000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "775000000000000000000000").Sum(x => x.Field<decimal>("Importe"));
                _perdidaCambiaria = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "750000050000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "750000050000000000000000").Sum(x => x.Field<decimal>("Importe"));
                r["PAntGastosFinancierosYOtrosGastosContable"] = _gastosFinancieros + _otrosGastos - _perdidaCambiaria;
                r["PAntGastosFinancierosYOtrosGastosReal"] = _gastosFinancieros + _otrosGastos - _perdidaCambiaria;


                _gastosFinancieros = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "750000000000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "750000000000000000000000").Sum(x => x.Field<decimal>("Importe"));
                _otrosGastos = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "775000000000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "775000000000000000000000").Sum(x => x.Field<decimal>("Importe"));
                _perdidaCambiaria = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "750000050000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "750000050000000000000000").Sum(x => x.Field<decimal>("Importe"));
                r["PActGastosFinancierosYOtrosGastosContable"] = _gastosFinancieros + _otrosGastos - _perdidaCambiaria;
                r["PActGastosFinancierosYOtrosGastosReal"] = _gastosFinancieros + _otrosGastos - _perdidaCambiaria;

                r["PAntPorcentajeGastosFinancierosYOtrosGastosContable"] = Convert.ToDecimal(r["PActIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntGastosFinancierosYOtrosGastosContable"]) / Convert.ToDecimal(r["PActIngresosContable"])) * 100;
                r["PAntPorcentajeGastosFinancierosYOtrosGastosReal"] = Convert.ToDecimal(r["PActIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntGastosFinancierosYOtrosGastosReal"]) / Convert.ToDecimal(r["PActIngresosReal"])) * 100;
                r["PActPorcentajeGastosFinancierosYOtrosGastosContable"] = Convert.ToDecimal(r["PActIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PActGastosFinancierosYOtrosGastosContable"]) / Convert.ToDecimal(r["PActIngresosContable"])) * 100;
                r["PActPorcentajeGastosFinancierosYOtrosGastosReal"] = Convert.ToDecimal(r["PActIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PActGastosFinancierosYOtrosGastosReal"]) / Convert.ToDecimal(r["PActIngresosReal"])) * 100;


                //OTROS INGRESOS

                decimal _productosFinancieros = 0;
                decimal _otrosProductos = 0;
                decimal _utilidadCambiaria = 0;

                _productosFinancieros = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "710000000000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "710000000000000000000000").Sum(x => x.Field<decimal>("Importe")); ;
                _otrosProductos = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "720000000000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "720000000000000000000000").Sum(x => x.Field<decimal>("Importe"));
                _utilidadCambiaria = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "710000050000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "710000050000000000000000").Sum(x => x.Field<decimal>("Importe"));
                r["PAntOtrosIngresosContable"] = _productosFinancieros + _otrosProductos - _utilidadCambiaria;
                r["PAntOtrosIngresosReal"] = _productosFinancieros + _otrosProductos - _utilidadCambiaria;


                _productosFinancieros = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "710000000000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "710000000000000000000000").Sum(x => x.Field<decimal>("Importe")); ;
                _otrosProductos = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "720000000000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "720000000000000000000000").Sum(x => x.Field<decimal>("Importe"));
                _utilidadCambiaria = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "710000050000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "710000050000000000000000").Sum(x => x.Field<decimal>("Importe"));
                r["PActOtrosIngresosContable"]= _productosFinancieros + _otrosProductos - _utilidadCambiaria;
                r["PActOtrosIngresosReal"] = _productosFinancieros + _otrosProductos - _utilidadCambiaria;

                r["PAntPorcentajeOtrosIngresosContable"] = Convert.ToDecimal(r["PAntIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntOtrosIngresosContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                r["PAntPorcentajeOtrosIngresosReal"] = Convert.ToDecimal(r["PAntIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntOtrosIngresosReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;
                r["PActPorcentajeOtrosIngresosContable"] = Convert.ToDecimal(r["PActIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PActOtrosIngresosContable"]) / Convert.ToDecimal(r["PActIngresosContable"])) * 100;
                r["PActPorcentajeOtrosIngresosReal"] = Convert.ToDecimal(r["PActIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PActOtrosIngresosReal"]) / Convert.ToDecimal(r["PActIngresosReal"])) * 100;


                //UTILIDAD O PERDIDA CAMBIARIA

                _utilidadCambiaria = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "710000050000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "710000050000000000000000").Sum(x => x.Field<decimal>("Importe"));
                _perdidaCambiaria = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "750000050000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "750000050000000000000000").Sum(x => x.Field<decimal>("Importe"));
                r["PAntUtilidadOPerdidaCambiariaContable"] = _utilidadCambiaria;
                r["PAntUtilidadOPerdidaCambiariaReal"] = _utilidadCambiaria;

                _utilidadCambiaria = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "710000050000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "710000050000000000000000").Sum(x => x.Field<decimal>("Importe"));
                _perdidaCambiaria = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "750000050000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "750000050000000000000000").Sum(x => x.Field<decimal>("Importe"));
                r["PActUtilidadOPerdidaCambiariaContable"] = _utilidadCambiaria - _perdidaCambiaria;
                r["PActUtilidadOPerdidaCambiariaReal"] = _utilidadCambiaria - _perdidaCambiaria;

                r["PAntPorcentajeUtilidadOPerdidaCambiariaContable"] = Convert.ToDecimal(r["PAntIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntUtilidadOPerdidaCambiariaContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                r["PAntPorcentajeUtilidadOPerdidaCambiariaReal"] = Convert.ToDecimal(r["PAntIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntUtilidadOPerdidaCambiariaReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;
                r["PActPorcentajeUtilidadOPerdidaCambiariaContable"] = Convert.ToDecimal(r["PActIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PActUtilidadOPerdidaCambiariaContable"]) / Convert.ToDecimal(r["PActIngresosContable"])) * 100;
                r["PActPorcentajeUtilidadOPerdidaCambiariaReal"] = Convert.ToDecimal(r["PActIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PActUtilidadOPerdidaCambiariaReal"]) / Convert.ToDecimal(r["PActIngresosReal"])) * 100;


                //IMPUESTOS A LA UTILIDAD-DIFERIDOS
                r["PAntImpuestosALaUtilidadContable"] = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776000020000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776000020000000000000000").Sum(x => x.Field<decimal>("Importe"));
                r["PAntImpuestosALaUtilidadReal"] = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776000020000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776000020000000000000000").Sum(x => x.Field<decimal>("Importe"));
                r["PActImpuestosALaUtilidadContable"] = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776000020000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776000020000000000000000").Sum(x => x.Field<decimal>("Importe"));
                r["PActImpuestosALaUtilidadReal"] = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776000020000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776000020000000000000000").Sum(x => x.Field<decimal>("Importe"));
                r["PAntPorcentajeImpuestosALaUtilidadContable"] = Convert.ToDecimal(r["PAntIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntImpuestosALaUtilidadContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                r["PAntPorcentajeImpuestosALaUtilidadReal"] = Convert.ToDecimal(r["PAntIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntImpuestosALaUtilidadReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;
                r["PActPorcentajeImpuestosALaUtilidadContable"] = Convert.ToDecimal(r["PActIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PActImpuestosALaUtilidadContable"]) / Convert.ToDecimal(r["PActIngresosContable"])) * 100;
                r["PActPorcentajeImpuestosALaUtilidadReal"] = Convert.ToDecimal(r["PActIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PActImpuestosALaUtilidadReal"]) / Convert.ToDecimal(r["PActIngresosReal"])) * 100;


                //IMPUESTOS RESULTADO DEL EJERCICIO
                r["PAntImpuestosResultadoEjercicioContable"] = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776100010000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776100010000000000000000").Sum(x => x.Field<decimal>("Importe"));
                r["PAntImpuestosResultadoEjercicioReal"] = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776100010000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776100010000000000000000").Sum(x => x.Field<decimal>("Importe"));
                r["PActImpuestosResultadoEjercicioContable"] = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776100010000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776100010000000000000000").Sum(x => x.Field<decimal>("Importe"));
                r["PActImpuestosResultadoEjercicioReal"] = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776100010000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776100010000000000000000").Sum(x => x.Field<decimal>("Importe"));
                r["PAntPorcentajeImpuestosResultadoEjercicioContable"] = Convert.ToDecimal(r["PAntIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntImpuestosResultadoEjercicioContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                r["PAntPorcentajeImpuestosResultadoEjercicioReal"] = Convert.ToDecimal(r["PAntIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntImpuestosResultadoEjercicioReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;
                r["PActPorcentajeImpuestosResultadoEjercicioContable"] = Convert.ToDecimal(r["PActIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PActImpuestosResultadoEjercicioContable"]) / Convert.ToDecimal(r["PActIngresosContable"])) * 100;
                r["PActPorcentajeImpuestosResultadoEjercicioReal"] = Convert.ToDecimal(r["PActIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PActImpuestosResultadoEjercicioReal"]) / Convert.ToDecimal(r["PActIngresosReal"])) * 100;


                //PTU DEL EJERCICIO
                r["PAntPtuDelEjercicioContable"] = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776200010000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776200010000000000000000").Sum(x => x.Field<decimal>("Importe"));
                r["PAntPtuDelEjercicioReal"] = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776200010000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776200010000000000000000").Sum(x => x.Field<decimal>("Importe"));
                r["PActPtuDelEjercicioContable"] = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776200010000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776200010000000000000000").Sum(x => x.Field<decimal>("Importe"));
                r["PActPtuDelEjercicioReal"] = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776200010000000000000000").Sum(x => x.Field<decimal>("Importe")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "776200010000000000000000").Sum(x => x.Field<decimal>("Importe"));
                r["PAntPorcentajePtuDelEjercicioContable"] = Convert.ToDecimal(r["PAntIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntPtuDelEjercicioContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                r["PAntPorcentajePtuDelEjercicioReal"] = Convert.ToDecimal(r["PAntIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntPtuDelEjercicioReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;
                r["PActPorcentajePtuDelEjercicioContable"] = Convert.ToDecimal(r["PActIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PActPtuDelEjercicioContable"]) / Convert.ToDecimal(r["PActIngresosContable"])) * 100;
                r["PActPorcentajePtuDelEjercicioReal"] = Convert.ToDecimal(r["PActIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PActPtuDelEjercicioReal"]) / Convert.ToDecimal(r["PActIngresosReal"])) * 100;


                //UTILIDAD NETA
                decimal _utilidadNetaContableAnterior = 0;
                decimal _utilidadNetaRealAnterior = 0;
                decimal _utilidadNetaContableActual = 0;
                decimal _utilidadNetaRealActual = 0;

                _utilidadNetaContableAnterior = Convert.ToDecimal(r["PAntUtilidadBrutaContable"]);
                _utilidadNetaContableAnterior = _utilidadNetaContableAnterior - (Convert.ToDecimal(r["PAntGastosDeVentaContable"]) + Convert.ToDecimal(r["PAntGastosDeAdministracionContable"]) + Convert.ToDecimal(r["PAntDeduccionPorCuentasIncobrablesContable"]) + Convert.ToDecimal(r["PAntGastosFinancierosYOtrosGastosContable"]));
                _utilidadNetaContableAnterior = _utilidadNetaContableAnterior + (Convert.ToDecimal(r["PAntOtrosIngresosContable"]) + Convert.ToDecimal(r["PAntUtilidadOPerdidaCambiariaContable"]) + Convert.ToDecimal(r["PAntImpuestosALaUtilidadContable"]) + Convert.ToDecimal(r["PAntImpuestosResultadoEjercicioContable"]) + Convert.ToDecimal(r["PAntPtuDelEjercicioContable"]));

                _utilidadNetaRealAnterior = Convert.ToDecimal(r["PAntUtilidadBrutaReal"]);
                _utilidadNetaRealAnterior = _utilidadNetaRealAnterior - (Convert.ToDecimal(r["PAntGastosDeVentaReal"]) + Convert.ToDecimal(r["PAntGastosDeAdministracionReal"]) + Convert.ToDecimal(r["PAntDeduccionPorCuentasIncobrablesReal"]) + Convert.ToDecimal(r["PAntGastosFinancierosYOtrosGastosReal"]));
                _utilidadNetaRealAnterior = _utilidadNetaRealAnterior + (Convert.ToDecimal(r["PAntOtrosIngresosReal"]) + Convert.ToDecimal(r["PAntUtilidadOPerdidaCambiariaReal"]) + Convert.ToDecimal(r["PAntImpuestosALaUtilidadReal"]) + Convert.ToDecimal(r["PAntImpuestosResultadoEjercicioReal"]) + Convert.ToDecimal(r["PAntPtuDelEjercicioReal"]));

                _utilidadNetaContableActual = Convert.ToDecimal(r["PActUtilidadBrutaContable"]);
                _utilidadNetaContableActual = _utilidadNetaContableActual - (Convert.ToDecimal(r["PActGastosDeVentaContable"]) + Convert.ToDecimal(r["PActGastosDeAdministracionContable"]) + Convert.ToDecimal(r["PActDeduccionPorCuentasIncobrablesContable"]) + Convert.ToDecimal(r["PActGastosFinancierosYOtrosGastosContable"]));
                _utilidadNetaContableActual = _utilidadNetaContableActual + (Convert.ToDecimal(r["PActOtrosIngresosContable"]) + Convert.ToDecimal(r["PActUtilidadOPerdidaCambiariaContable"]) + Convert.ToDecimal(r["PActImpuestosALaUtilidadContable"]) + Convert.ToDecimal(r["PActImpuestosResultadoEjercicioContable"]) + Convert.ToDecimal(r["PActPtuDelEjercicioContable"]));

                _utilidadNetaRealActual = Convert.ToDecimal(r["PActUtilidadBrutaReal"]);
                _utilidadNetaRealActual = _utilidadNetaRealActual - (Convert.ToDecimal(r["PActGastosDeVentaReal"]) + Convert.ToDecimal(r["PActGastosDeAdministracionReal"]) + Convert.ToDecimal(r["PActDeduccionPorCuentasIncobrablesReal"]) + Convert.ToDecimal(r["PActGastosFinancierosYOtrosGastosReal"]));
                _utilidadNetaRealActual = _utilidadNetaRealActual + (Convert.ToDecimal(r["PActOtrosIngresosReal"]) + Convert.ToDecimal(r["PActUtilidadOPerdidaCambiariaReal"]) + Convert.ToDecimal(r["PActImpuestosALaUtilidadReal"]) + Convert.ToDecimal(r["PActImpuestosResultadoEjercicioReal"]) + Convert.ToDecimal(r["PActPtuDelEjercicioReal"]));

                r["PAntUtilidadNetaContable"] = _utilidadNetaContableAnterior;
                r["PAntUtilidadNetaReal"] = _utilidadNetaRealAnterior;
                r["PActUtilidadNetaContable"] = _utilidadNetaContableActual;
                r["PActUtilidadNetaReal"] = _utilidadNetaRealActual;
                r["PAntPorcentajeUtilidadNetaContable"] = Convert.ToDecimal(r["PAntIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntUtilidadNetaContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                r["PAntPorcentajeUtilidadNetaReal"] = Convert.ToDecimal(r["PAntIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntUtilidadNetaReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;
                r["PActPorcentajeUtilidadNetaContable"] = Convert.ToDecimal(r["PActIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PActUtilidadNetaContable"]) / Convert.ToDecimal(r["PActIngresosContable"])) * 100;
                r["PActPorcentajeUtilidadNetaReal"] = Convert.ToDecimal(r["PActIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PActUtilidadNetaReal"]) / Convert.ToDecimal(r["PActIngresosReal"])) * 100;




                //UTILIDAD NETA POR EMPRESA
                //Anterior
                r["PAntUtilidadNetaFacturContable"] = Convert.ToDouble(rPeriodoAnteriorFacturco["UtilidadOPerdidaContableContable"]);
                r["PAntUtilidadNetaBalorContable"] = Convert.ToDouble(rPeriodoAnteriorBalor["UtilidadOPerdidaContableContable"]);
                r["PAntUtilidadNetaTativoContable"] = Convert.ToDouble(rPeriodoAnteriorFacturco["InteresesClientesTativo"]);
                r["PAntUtilidadNetaFacturReal"] = Convert.ToDouble(rPeriodoAnteriorFacturco["UtilidadOPerdidaContableReal"]);
                r["PAntUtilidadNetaBalorReal"] = Convert.ToDouble(rPeriodoAnteriorBalor["UtilidadOPerdidaContableReal"]);
                r["PAntUtilidadNetaTativoReal"] = Convert.ToDouble(rPeriodoAnteriorFacturco["InteresesClientesTativo"]);

                r["PAntUtilidadNetaTotalContable"] = Convert.ToDouble(r["PAntUtilidadNetaFacturContable"]) + Convert.ToDouble(r["PAntUtilidadNetaBalorContable"]) + Convert.ToDouble(r["PAntUtilidadNetaTativoContable"]);
                r["PAntUtilidadNetaTotalReal"] = Convert.ToDouble(r["PAntUtilidadNetaFacturReal"]) + Convert.ToDouble(r["PAntUtilidadNetaBalorReal"]) + Convert.ToDouble(r["PAntUtilidadNetaTativoReal"]);


                //Actual
                decimal _gastosPersonalesFacturco = 0;
                decimal _gastosPersonalesBalor = 0;
                decimal _gastosPersonalesTativo = 0;

                decimal _gastosPersonalesDeVenta = 0;
                decimal _gastosPersonalesDeAdministracion = 0;

                decimal _gastosFucturDeBalor = 0;

                _gastosPersonalesFacturco = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "640200000000000000000000" || x.Field<string>("Cuenta") == "660200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal") + x.Field<decimal>("ImporteBalor") + x.Field<decimal>("ImporteTativo") + x.Field<decimal>("ImportePersonalesFacturDeBalor"));
                //_gastosPersonalesFacturco = _gastosPersonalesFacturco - Convert.ToDecimal(rPeriodoActualFacturco["InteresesClientesTativo"]);
                _gastosPersonalesBalor = dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "640200000000000000000000" || x.Field<string>("Cuenta") == "660200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal"));
                _gastosPersonalesTativo = Convert.ToDecimal(rPeriodoActualFacturco["InteresesClientesTativo"]) - (820 * _mesDelPeriodo);

                //_gastosPersonalesDeVenta = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "640200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal") + x.Field<decimal>("ImporteBalor") + x.Field<decimal>("ImporteTativo") + x.Field<decimal>("ImportePersonalesFacturDeBalor"));
                //_gastosPersonalesDeAdministracion = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "660200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal") + x.Field<decimal>("ImporteBalor") + x.Field<decimal>("ImporteTativo") + x.Field<decimal>("ImportePersonalesFacturDeBalor"));

                //_gastosFucturDeBalor = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "640200000000000000000000" || x.Field<string>("Cuenta") == "660200000000000000000000").Sum(x => x.Field<decimal>("ImporteFacturDeBalor") + x.Field<decimal>("ImportePersonalesFacturDeBalor"));
                _gastosFucturDeBalor = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "640200000000000000000000" || x.Field<string>("Cuenta") == "660200000000000000000000").Sum(x => x.Field<decimal>("ImporteFacturDeBalor"));


                r["PActUtilidadNetaFacturContable"] = Convert.ToDouble(rPeriodoActualFacturco["UtilidadOPerdidaContableContable"]);
                r["PActUtilidadNetaBalorContable"] = Convert.ToDouble(rPeriodoActualBalor["UtilidadOPerdidaContableContable"]); 
                r["PActUtilidadNetaTativoContable"] = Convert.ToDouble(rPeriodoActualFacturco["InteresesClientesTativo"]);
                //r["PActUtilidadNetaFacturReal"] = Convert.ToDecimal(rPeriodoActualFacturco["UtilidadOPerdidaContableReal"]) + _gastosPersonalesFacturco;
                r["PActUtilidadNetaFacturReal"] = Convert.ToDecimal(r["PActUtilidadNetaFacturContable"]) + _gastosPersonalesFacturco;
                r["PActUtilidadNetaBalorReal"] = Convert.ToDecimal(r["PActUtilidadNetaBalorContable"]) + _gastosPersonalesBalor - _gastosFucturDeBalor;
                r["PActUtilidadNetaTativoReal"] = _gastosPersonalesTativo;


                r["PActUtilidadNetaTotalContable"] = Convert.ToDouble(r["PActUtilidadNetaFacturContable"]) + Convert.ToDouble(r["PActUtilidadNetaBalorContable"]) + Convert.ToDouble(r["PActUtilidadNetaTativoContable"]);
                r["PActUtilidadNetaTotalReal"] = Convert.ToDouble(r["PActUtilidadNetaFacturReal"]) + Convert.ToDouble(r["PActUtilidadNetaBalorReal"]) + Convert.ToDouble(r["PActUtilidadNetaTativoReal"]);


                //EFECTO CAMBIARIO
                r["PAntEfectoCambiarioContable"] = Convert.ToDouble(r["PAntUtilidadOPerdidaCambiariaContable"]);
                r["PAntEfectoCambiarioReal"] = Convert.ToDouble(r["PAntUtilidadOPerdidaCambiariaReal"]);
                r["PActEfectoCambiarioContable"] = Convert.ToDouble(r["PActUtilidadOPerdidaCambiariaContable"]);
                r["PActEfectoCambiarioReal"] = Convert.ToDouble(r["PActUtilidadOPerdidaCambiariaReal"]);

                //UTILIDAD O PERDIDA SIN EFECTO CAMBIARIO
                r["PAntUtilidadOPerdidaSinEfectoCambiarioContable"] = Convert.ToDouble(r["PAntUtilidadNetaContable"]) - Convert.ToDouble(r["PAntEfectoCambiarioContable"]);
                r["PAntUtilidadOPerdidaSinEfectoCambiarioReal"] = Convert.ToDouble(r["PAntUtilidadNetaReal"]) - Convert.ToDouble(r["PAntEfectoCambiarioReal"]);
                r["PActUtilidadOPerdidaSinEfectoCambiarioContable"] = Convert.ToDouble(r["PActUtilidadNetaContable"]) - Convert.ToDouble(r["PActEfectoCambiarioContable"]);
                r["PActUtilidadOPerdidaSinEfectoCambiarioReal"] = Convert.ToDouble(r["PActUtilidadNetaReal"]) - Convert.ToDouble(r["PActEfectoCambiarioReal"]);

                r["PAntPorcentajeUtilidadOPerdidaSinEfectoCambiarioContable"] = Convert.ToDecimal(r["PAntIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntUtilidadOPerdidaSinEfectoCambiarioContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                r["PAntPorcentajeUtilidadOPerdidaSinEfectoCambiarioReal"] = Convert.ToDecimal(r["PAntIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntUtilidadOPerdidaSinEfectoCambiarioReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;
                r["PActPorcentajeUtilidadOPerdidaSinEfectoCambiarioContable"] = Convert.ToDecimal(r["PActIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PActUtilidadOPerdidaSinEfectoCambiarioContable"]) / Convert.ToDecimal(r["PActIngresosContable"])) * 100;
                r["PActPorcentajeUtilidadOPerdidaSinEfectoCambiarioReal"] = Convert.ToDecimal(r["PActIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PActUtilidadOPerdidaSinEfectoCambiarioReal"]) / Convert.ToDecimal(r["PActIngresosReal"])) * 100;


                //SUELDOS

                //Habilitar cuando se habilite de nuevo la consulta del periodo anterior y comentar las dos lineas siguientes a estas
                //decimal _pantSueldosVentasContable = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6402" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("Importe"));
                //decimal _pantSueldosVentasReal = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6402" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("ImporteReal"));

                decimal _pantSueldosVentasContable =  dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0,4) == "6402" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("Importe") - x.Field<decimal>("ImportePersonal") - x.Field<decimal>("ImporteBalor")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6402" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("Importe") - x.Field<decimal>("ImportePersonal") - x.Field<decimal>("ImporteBalor"));
                //decimal _pantSueldosVentasReal = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6402" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("ImporteReal")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6402" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("ImporteReal"));
                decimal _pantSueldosVentasReal = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6402" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("Importe") - x.Field<decimal>("ImportePersonal") - x.Field<decimal>("ImporteBalor")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6402" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("Importe") - x.Field<decimal>("ImportePersonal") - x.Field<decimal>("ImporteBalor"));

                decimal _pantSueldosAdministracionContable = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6602" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("Importe") - x.Field<decimal>("ImportePersonal") - x.Field<decimal>("ImporteBalor")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6602" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("Importe") - x.Field<decimal>("ImportePersonal") - x.Field<decimal>("ImporteBalor"));
                //decimal _pantSueldosAdministracionReal = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6602" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("ImporteReal")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6602" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("ImporteReal"));
                decimal _pantSueldosAdministracionReal = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6602" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("Importe") - x.Field<decimal>("ImportePersonal") - x.Field<decimal>("ImporteBalor")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6602" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("Importe") - x.Field<decimal>("ImportePersonal") - x.Field<decimal>("ImporteBalor"));

                decimal _pactSueldosVentasContable = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6402" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("Importe") - x.Field<decimal>("ImportePersonal") - x.Field<decimal>("ImporteBalor")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6402" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("Importe") - x.Field<decimal>("ImportePersonal") - x.Field<decimal>("ImporteBalor"));
                //decimal _pactSueldosVentasReal = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6402" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("ImporteReal")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6402" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("ImporteReal"));
                decimal _pactSueldosVentasReal = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6402" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("Importe") - x.Field<decimal>("ImportePersonal") - x.Field<decimal>("ImporteBalor")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6402" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("Importe") - x.Field<decimal>("ImportePersonal") - x.Field<decimal>("ImporteBalor"));

                decimal _pactSueldosAdministracionContable = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6602" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("Importe") - x.Field<decimal>("ImportePersonal") - x.Field<decimal>("ImporteBalor")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6602" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("Importe") - x.Field<decimal>("ImportePersonal") - x.Field<decimal>("ImporteBalor"));
                //decimal _pactSueldosAdministracionReal = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6602" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("ImporteReal")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6602" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("ImporteReal"));
                decimal _pactSueldosAdministracionReal = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6602" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("Importe") - x.Field<decimal>("ImportePersonal") - x.Field<decimal>("ImporteBalor")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta").Substring(0, 4) == "6602" && x.Field<string>("Descripcion") == "SUELDOS").Sum(x => x.Field<decimal>("Importe") - x.Field<decimal>("ImportePersonal") - x.Field<decimal>("ImporteBalor"));

                r["PAntSueldosVentasContable"] = _pantSueldosVentasContable;
                r["PAntSueldosVentasReal"] = _pantSueldosVentasReal;
                r["PAntSueldosAdministracionContable"] = _pantSueldosAdministracionContable;
                r["PAntSueldosAdministracionReal"] = _pantSueldosAdministracionReal;

                r["PActSueldosVentasContable"] = _pactSueldosVentasContable;
                r["PActSueldosVentasReal"] = _pactSueldosVentasReal;
                r["PActSueldosAdministracionContable"] = _pactSueldosAdministracionContable;
                r["PActSueldosAdministracionReal"] = _pactSueldosAdministracionReal;

                r["PAntPorcentajeSueldosVentasContable"] = Convert.ToDecimal(r["PAntIngresosContable"]) == 0 ? 0 : (_pantSueldosVentasContable / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                r["PAntPorcentajeSueldosVentasReal"] = Convert.ToDecimal(r["PAntIngresosReal"]) == 0 ? 0 : (_pantSueldosVentasReal / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;
                r["PAntPorcentajeSueldosAdministracionContable"] = Convert.ToDecimal(r["PAntIngresosContable"]) == 0 ? 0 : (_pantSueldosAdministracionContable / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                r["PAntPorcentajeSueldosAdministracionReal"] = Convert.ToDecimal(r["PAntIngresosReal"]) == 0 ? 0 : (_pantSueldosAdministracionReal / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;

                r["PActPorcentajeSueldosVentasContable"] = Convert.ToDecimal(r["PActIngresosContable"]) == 0 ? 0 : (_pactSueldosVentasContable / Convert.ToDecimal(r["PActIngresosContable"])) * 100;
                r["PActPorcentajeSueldosVentasReal"] = Convert.ToDecimal(r["PActIngresosReal"]) == 0 ? 0 : (_pactSueldosVentasReal / Convert.ToDecimal(r["PActIngresosReal"])) * 100;
                r["PActPorcentajeSueldosAdministracionContable"] = Convert.ToDecimal(r["PActIngresosContable"]) == 0 ? 0 : (_pactSueldosAdministracionContable / Convert.ToDecimal(r["PActIngresosContable"])) * 100;
                r["PActPorcentajeSueldosAdministracionReal"] = Convert.ToDecimal(r["PActIngresosReal"]) == 0 ? 0 : (_pactSueldosAdministracionReal / Convert.ToDecimal(r["PActIngresosReal"])) * 100;


                r["PAntTotalSueldosContable"] = _pantSueldosVentasContable + _pantSueldosAdministracionContable;
                r["PAntTotalSueldosReal"] = _pantSueldosVentasReal + _pantSueldosAdministracionReal;
                r["PActTotalSueldosContable"] = _pactSueldosVentasContable + _pactSueldosAdministracionContable;
                r["PActTotalSueldosReal"] = _pactSueldosVentasReal + _pactSueldosAdministracionReal;

                r["PAntPorcentajeTotalSueldosContable"] = Convert.ToDecimal(r["PAntIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntTotalSueldosContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                r["PAntPorcentajeTotalSueldosReal"] = Convert.ToDecimal(r["PAntIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntTotalSueldosReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;
                r["PActPorcentajeTotalSueldosContable"] = Convert.ToDecimal(r["PActIngresosContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PActTotalSueldosContable"]) / Convert.ToDecimal(r["PActIngresosContable"])) * 100;
                r["PActPorcentajeTotalSueldosReal"] = Convert.ToDecimal(r["PActIngresosReal"]) == 0 ? 0 : (Convert.ToDecimal(r["PActTotalSueldosReal"]) / Convert.ToDecimal(r["PActIngresosReal"])) * 100;


                //GASTOS PERSONALES
                r["PAntGastosPersonalesDeVentaContable"] = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "640200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "640200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal"));
                r["PAntGastosPersonalesDeAdministracionContable"] = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "660200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "660200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal"));
                r["PAntGastosPersonalesPtuContable"] = 0;
                r["PAntGastosPersonalesDeVentaReal"] = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "640200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "640200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal"));
                r["PAntGastosPersonalesDeAdministracionReal"] = dsPeriodoAnteriorFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "660200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal")) + dsPeriodoAnteriorBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "660200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal"));
                r["PAntGastosPersonalesPtuReal"] = 0;

                r["PAntGastosPersonalesDeVentaContable"] = Convert.ToDecimal(r["PAntGastosPersonalesDeVentaContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntGastosPersonalesDeVentaContable"])/ 12) * _mesDelPeriodo;
                r["PAntGastosPersonalesDeAdministracionContable"] = Convert.ToDecimal(r["PAntGastosPersonalesDeAdministracionContable"]) == 0 ? 0 : (Convert.ToDecimal(r["PAntGastosPersonalesDeAdministracionContable"]) / 12) * _mesDelPeriodo;
                r["PAntGastosPersonalesPtuContable"] = 0;
                //r["PAntGastosPersonalesDeVentaReal"] = (Convert.ToDecimal(r["PAntGastosPersonalesDeVentaReal"]) / 12) * _mesDelPeriodo;
                //r["PAntGastosPersonalesDeAdministracionReal"] = (Convert.ToDecimal(r["PAntGastosPersonalesDeAdministracionReal"]) / 12) * _mesDelPeriodo;
                r["PAntGastosPersonalesPtuReal"] = 0;

                r["PActGastosPersonalesDeVentaContable"] = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "640200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "640200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal"));
                r["PActGastosPersonalesDeAdministracionContable"] = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "660200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "660200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal") + x.Field<decimal>("ImportePersonalesFacturDeBalor"));
                //r["PActGastosPersonalesDeAdministracionContable"] = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "660200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal") + x.Field<decimal>("ImportePersonalesFacturDeBalor")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "660200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal") + x.Field<decimal>("ImportePersonalesFacturDeBalor")) ;
                r["PActGastosPersonalesPtuContable"] = 0;
                //r["PActGastosPersonalesDeVentaReal"] = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "640200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "640200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal"));
                //r["PActGastosPersonalesDeAdministracionReal"] = dsPeriodoActualFacturco.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "660200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal")) + dsPeriodoActualBalor.Tables["tblGastos1"].AsEnumerable().Where(x => x.Field<string>("Cuenta") == "660200000000000000000000").Sum(x => x.Field<decimal>("ImportePersonal"));
                r["PActGastosPersonalesPtuReal"] = 0;


                r["PAntTotalGastosPersonalesContable"] = Convert.ToDecimal(r["PAntGastosPersonalesDeVentaContable"]) + Convert.ToDecimal(r["PAntGastosPersonalesDeAdministracionContable"]) + Convert.ToDecimal(r["PAntGastosPersonalesPtuContable"]);
                r["PAntTotalGastosPersonalesReal"] = Convert.ToDecimal(r["PAntGastosPersonalesDeVentaReal"]) + Convert.ToDecimal(r["PAntGastosPersonalesDeAdministracionReal"]) + Convert.ToDecimal(r["PAntGastosPersonalesPtuReal"]);
                r["PActTotalGastosPersonalesContable"] = Convert.ToDecimal(r["PActGastosPersonalesDeVentaContable"]) + Convert.ToDecimal(r["PActGastosPersonalesDeAdministracionContable"]) + Convert.ToDecimal(r["PActGastosPersonalesPtuContable"]);
                r["PActTotalGastosPersonalesReal"] =  Convert.ToDecimal(r["PActGastosPersonalesDeVentaReal"]) + Convert.ToDecimal(r["PActGastosPersonalesDeAdministracionReal"]) + Convert.ToDecimal(r["PActGastosPersonalesPtuReal"]);
                #endregion

                if (fecha.AddYears(-1).Year == 2020)
                {


                    DataRow dr = dsPeriodoActualFacturco.Tables["TblEstadoResultados2020"].Rows[0];

                    ////INGRESOS
                    //r["PAntIngresosContable"] = Convert.ToDouble(dr["IngresosContable"]);
                    //r["PAntIngresosReal"] = Convert.ToDouble(dr["IngresosReal"]);

                    ////COSTO DE FINANCIAMIENTO
                    //r["PAntCostoFinanciamientoContable"] = Convert.ToDouble(dr["CostosDeFinanciamientoContable"]);
                    //r["PAntCostoFinanciamientoReal"] = Convert.ToDouble(dr["CostosDeFinanciamientoReal"]);
                    //r["PAntPorcentajeCostoFinanciamientoContable"] = (Convert.ToDecimal(r["PAntCostoFinanciamientoContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    //r["PAntPorcentajeCostoFinanciamientoReal"] = (Convert.ToDecimal(r["PAntCostoFinanciamientoReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;

                    ////UTILIDAD BRUTA
                    //r["PAntUtilidadBrutaContable"] = Convert.ToDouble(dr["UtilidadBrutaContable"]);
                    //r["PAntUtilidadBrutaReal"] = Convert.ToDouble(dr["UtilidadBrutaReal"]);
                    //r["PAntPorcentajeUtilidadBrutaContable"] = (Convert.ToDecimal(r["PAntUtilidadBrutaContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    //r["PAntPorcentajeUtilidadBrutaReal"] = (Convert.ToDecimal(r["PAntUtilidadBrutaReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;

                    ////GASTOS DE VENTA
                    //r["PAntGastosDeVentaContable"] = Convert.ToDouble(dr["GastosDeVentaContable"]);
                    //r["PAntGastosDeVentaReal"] = Convert.ToDouble(dr["GastosDeVentaReal"]);
                    //r["PAntPorcentajeGastosDeVentaContable"] = (Convert.ToDecimal(r["PAntGastosDeVentaContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    //r["PAntPorcentajeGastosDeVentaReal"] = (Convert.ToDecimal(r["PAntGastosDeVentaReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;

                    ////GASTOS DE ADMINISTRACION
                    //r["PAntGastosDeAdministracionContable"] = Convert.ToDouble(dr["GastosDeAdministracionContable"]);
                    //r["PAntGastosDeAdministracionReal"] = Convert.ToDouble(dr["GastosDeAdministracionReal"]);
                    //r["PAntPorcentajeGastosDeAdministracionContable"] = (Convert.ToDecimal(r["PAntGastosDeAdministracionContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    //r["PAntPorcentajeGastosDeAdministracionReal"] = (Convert.ToDecimal(r["PAntGastosDeAdministracionReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;

                    ////DEDUCCION POR CUENTAS INCOBRABLES
                    //r["PAntDeduccionPorCuentasIncobrablesContable"] = Convert.ToDouble(dr["DeduccionPorCuentasIncobrablesContable"]); 
                    //r["PAntDeduccionPorCuentasIncobrablesReal"] = Convert.ToDouble(dr["DeduccionPorCuentasIncobrablesReal"]);
                    //r["PAntPorcentajeDeduccionPorCuentasIncobrablesContable"] = (Convert.ToDecimal(r["PAntDeduccionPorCuentasIncobrablesContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    //r["PAntPorcentajeDeduccionPorCuentasIncobrablesReal"] = (Convert.ToDouble(r["PAntDeduccionPorCuentasIncobrablesReal"]) / Convert.ToDouble(r["PAntIngresosContable"])) * 100;


                    ////GASTOS FINANCIEROS Y OTROS GASTOS
                    //r["PAntGastosFinancierosYOtrosGastosContable"] = Convert.ToDouble(dr["GastosFinancierosYOtrosGastosContable"]);
                    //r["PAntGastosFinancierosYOtrosGastosReal"] = Convert.ToDouble(dr["GastosFinancierosYOtrosGastosReal"]);
                    //r["PAntPorcentajeGastosFinancierosYOtrosGastosContable"] = (Convert.ToDecimal(r["PAntGastosFinancierosYOtrosGastosContable"]) / Convert.ToDecimal(r["PActIngresosContable"])) * 100;
                    //r["PAntPorcentajeGastosFinancierosYOtrosGastosReal"] = (Convert.ToDecimal(r["PAntGastosFinancierosYOtrosGastosReal"]) / Convert.ToDecimal(r["PActIngresosReal"])) * 100;


                    ////OTROS INGRESOS
                    //r["PAntOtrosIngresosContable"] = Convert.ToDouble(dr["OtrosIngresosContable"]);
                    //r["PAntOtrosIngresosReal"] = Convert.ToDouble(dr["OtrosIngresosReal"]);
                    //r["PAntPorcentajeOtrosIngresosContable"] = (Convert.ToDecimal(r["PAntOtrosIngresosContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    //r["PAntPorcentajeOtrosIngresosReal"] = (Convert.ToDecimal(r["PAntOtrosIngresosReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;


                    ////UTILIDAD O PERDIDA CAMBIARIA
                    //r["PAntUtilidadOPerdidaCambiariaContable"] = Convert.ToDouble(dr["UtilidadOPerdidaCambiariaContable"]);
                    //r["PAntUtilidadOPerdidaCambiariaReal"] = Convert.ToDouble(dr["UtilidadOPerdidaCambiariaReal"]);
                    //r["PAntPorcentajeUtilidadOPerdidaCambiariaContable"] = (Convert.ToDecimal(r["PAntUtilidadOPerdidaCambiariaContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    //r["PAntPorcentajeUtilidadOPerdidaCambiariaReal"] = (Convert.ToDecimal(r["PAntUtilidadOPerdidaCambiariaReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;


                    ////IMPUESTOS A LA UTILIDAD DIFERIDOS
                    //r["PAntImpuestosALaUtilidadContable"] = Convert.ToDouble(dr["ImpuestosAlaUtilidadDiferidosContable"]);
                    //r["PAntImpuestosALaUtilidadReal"] = Convert.ToDouble(dr["ImpuestosAlaUtilidadDiferidosReal"]);
                    //r["PAntPorcentajeImpuestosALaUtilidadContable"] = (Convert.ToDecimal(r["PAntImpuestosALaUtilidadContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    //r["PAntPorcentajeImpuestosALaUtilidadReal"] = (Convert.ToDecimal(r["PAntImpuestosALaUtilidadReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;


                    ////IMPUESTOS RESULTADO DEL EJERCICIO
                    //r["PAntImpuestosResultadoEjercicioContable"] = Convert.ToDouble(dr["ImpuestosResultadoDelEjercicioContable"]);
                    //r["PAntImpuestosResultadoEjercicioReal"] = Convert.ToDouble(dr["ImpuestosResultadoDelEjercicioReal"]);
                    //r["PAntPorcentajeImpuestosResultadoEjercicioContable"] = (Convert.ToDecimal(r["PAntImpuestosResultadoEjercicioContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    //r["PAntPorcentajeImpuestosResultadoEjercicioReal"] = (Convert.ToDecimal(r["PAntImpuestosResultadoEjercicioReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;


                    ////PTU DEL EJERCICIO
                    //r["PAntPtuDelEjercicioContable"] = Convert.ToDouble(dr["PtuDelEjercicioContable"]);
                    //r["PAntPtuDelEjercicioReal"] = Convert.ToDouble(dr["PtuDelEjercicioReal"]);
                    //r["PAntPorcentajePtuDelEjercicioContable"] = (Convert.ToDecimal(r["PAntPtuDelEjercicioContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    //r["PAntPorcentajePtuDelEjercicioReal"] = (Convert.ToDecimal(r["PAntPtuDelEjercicioReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;


                    ////UTILIDAD NETA
                    //r["PAntUtilidadNetaContable"] = Convert.ToDouble(dr["UtilidadNetaContable"]);
                    //r["PAntUtilidadNetaReal"] = Convert.ToDouble(dr["UtilidadNetaReal"]);
                    //r["PAntPorcentajeUtilidadNetaContable"] = (Convert.ToDecimal(r["PAntUtilidadNetaContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    //r["PAntPorcentajeUtilidadNetaReal"] = (Convert.ToDecimal(r["PAntUtilidadNetaReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;

                    //r["PAntUtilidadNetaFacturContable"] = Convert.ToDouble(dr["UtilidadNetaFacturcoContable"]);
                    //r["PAntUtilidadNetaBalorContable"] = Convert.ToDouble(dr["UtilidadNetaBalorContable"]);
                    //r["PAntUtilidadNetaTativoContable"] = Convert.ToDouble(dr["UtilidadNetaTativoContable"]);
                    //r["PAntUtilidadNetaFacturReal"] = Convert.ToDouble(dr["UtilidadNetaFacturcoReal"]);
                    //r["PAntUtilidadNetaBalorReal"] = Convert.ToDouble(dr["UtilidadNetaBalorReal"]);
                    //r["PAntUtilidadNetaTativoReal"] = Convert.ToDouble(dr["UtilidadNetaTativoReal"]);


                    //r["PAntUtilidadNetaTotalContable"] = Convert.ToDouble(r["PAntUtilidadNetaFacturContable"]) + Convert.ToDouble(r["PAntUtilidadNetaBalorContable"]) + Convert.ToDouble(r["PAntUtilidadNetaTativoContable"]);
                    //r["PAntUtilidadNetaTotalReal"] = Convert.ToDouble(r["PAntUtilidadNetaFacturReal"]) + Convert.ToDouble(r["PAntUtilidadNetaBalorReal"]) + Convert.ToDouble(r["PAntUtilidadNetaTativoReal"]);

                    //r["PAntEfectoCambiarioContable"] = Convert.ToDouble(dr["EfectoCambiarioContable"]);
                    //r["PAntEfectoCambiarioReal"] = Convert.ToDouble(dr["EfectoCambiarioReal"]);


                    ////UTILIDAD O PERDIDA SIN EFECTO CAMBIARIO
                    //r["PAntUtilidadOPerdidaSinEfectoCambiarioContable"] = Convert.ToDouble(dr["UtilidadOPerdidaSinEfectoCambiarioContable"]);
                    //r["PAntUtilidadOPerdidaSinEfectoCambiarioReal"] = Convert.ToDouble(dr["UtilidadOPerdidaSinEfectoCambiarioReal"]);
                    //r["PAntPorcentajeUtilidadOPerdidaSinEfectoCambiarioContable"] = (Convert.ToDecimal(r["PAntUtilidadOPerdidaSinEfectoCambiarioContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    //r["PAntPorcentajeUtilidadOPerdidaSinEfectoCambiarioReal"] = (Convert.ToDecimal(r["PAntUtilidadOPerdidaSinEfectoCambiarioReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100 ;


                    //r["PAntSueldosVentasContable"] =  Convert.ToDouble(dr["SueldosDeVentasContable"]);
                    //r["PAntSueldosVentasReal"] = Convert.ToDouble(dr["SueldosDeVentasReal"]);                                    
                    //r["PAntSueldosAdministracionContable"] =  Convert.ToDouble(dr["SueldosDeAdministracionContable"]);
                    //r["PAntSueldosAdministracionReal"] = Convert.ToDouble(dr["SueldosDeAdministracionReal"]);
                    //r["PAntPorcentajeSueldosVentasContable"] = (Convert.ToDecimal(r["PAntSueldosVentasContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    //r["PAntPorcentajeSueldosVentasReal"] = (Convert.ToDecimal(r["PAntSueldosVentasReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;
                    //r["PAntPorcentajeSueldosAdministracionContable"] = (Convert.ToDecimal(r["PAntSueldosAdministracionContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    //r["PAntPorcentajeSueldosAdministracionReal"] = (Convert.ToDecimal(r["PAntSueldosAdministracionReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;
                    //r["PAntTotalSueldosContable"] = Convert.ToDecimal(r["PAntSueldosVentasContable"]) + Convert.ToDecimal(r["PAntSueldosAdministracionContable"]);
                    //r["PAntTotalSueldosReal"] = Convert.ToDecimal(r["PAntSueldosVentasReal"]) + Convert.ToDecimal(r["PAntSueldosAdministracionReal"]);
                    //r["PAntPorcentajeTotalSueldosContable"] = (Convert.ToDecimal(r["PAntTotalSueldosContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    //r["PAntPorcentajeTotalSueldosReal"] = (Convert.ToDecimal(r["PAntTotalSueldosReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;

                    ////GASTOS PERSONALES
                    //r["PAntGastosPersonalesDeVentaContable"] = Convert.ToDouble(dr["GastosPersonalesDeVentasContable"]);
                    //r["PAntGastosPersonalesDeAdministracionContable"] = Convert.ToDouble(dr["GastosPersonalesDeAdministracionContable"]);
                    //r["PAntGastosPersonalesPtuContable"] = Convert.ToDouble(dr["GastosPersonalesPtuContable"]);
                    //r["PAntGastosPersonalesDeVentaReal"] = Convert.ToDouble(dr["GastosPersonalesDeVentasReal"]);
                    //r["PAntGastosPersonalesDeAdministracionReal"] = Convert.ToDouble(dr["GastosPersonalesDeAdministracionReal"]);
                    //r["PAntGastosPersonalesPtuReal"] = Convert.ToDouble(dr["GastosPersonalesPtuReal"]);

                    //r["PAntTotalGastosPersonalesContable"] = Convert.ToDecimal(r["PAntGastosPersonalesDeVentaContable"]) + Convert.ToDecimal(r["PAntGastosPersonalesDeAdministracionContable"]) + Convert.ToDecimal(r["PAntGastosPersonalesPtuContable"]);
                    //r["PAntTotalGastosPersonalesReal"] = Convert.ToDecimal(r["PAntGastosPersonalesDeVentaReal"]) + Convert.ToDecimal(r["PAntGastosPersonalesDeAdministracionReal"]) + Convert.ToDecimal(r["PAntGastosPersonalesPtuReal"]);





                    //-------------------------------------------------------------------------------------------------------



                    //INGRESOS
                    r["PAntIngresosContable"] = Convert.ToDouble(dr["IngresosContable"]) != 0 ? (Convert.ToDouble(dr["IngresosContable"]) / 12) * _mesDelPeriodo : 0;
                    r["PAntIngresosReal"] = Convert.ToDouble(dr["IngresosReal"]) != 0 ? (Convert.ToDouble(dr["IngresosReal"]) / 12) * _mesDelPeriodo : 0;

                    //COSTO DE FINANCIAMIENTO
                    r["PAntCostoFinanciamientoContable"] = Convert.ToDouble(dr["CostosDeFinanciamientoContable"]) != 0 ? (Convert.ToDouble(dr["CostosDeFinanciamientoContable"]) / 12) * _mesDelPeriodo : 0;
                    r["PAntCostoFinanciamientoReal"] = Convert.ToDouble(dr["CostosDeFinanciamientoReal"]) != 0 ? (Convert.ToDouble(dr["CostosDeFinanciamientoReal"]) / 12) * _mesDelPeriodo : 0;
                    r["PAntPorcentajeCostoFinanciamientoContable"] = (Convert.ToDecimal(r["PAntCostoFinanciamientoContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    r["PAntPorcentajeCostoFinanciamientoReal"] = (Convert.ToDecimal(r["PAntCostoFinanciamientoReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;

                    //UTILIDAD BRUTA
                    r["PAntUtilidadBrutaContable"] = Convert.ToDouble(r["PAntIngresosContable"]) - Convert.ToDouble(r["PAntCostoFinanciamientoContable"]);
                    r["PAntUtilidadBrutaReal"] = Convert.ToDouble(r["PAntIngresosReal"]) - Convert.ToDouble(r["PAntCostoFinanciamientoReal"]);
                    r["PAntPorcentajeUtilidadBrutaContable"] = (Convert.ToDecimal(r["PAntUtilidadBrutaContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    r["PAntPorcentajeUtilidadBrutaReal"] = (Convert.ToDecimal(r["PAntUtilidadBrutaReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;

                    //GASTOS DE VENTA
                    r["PAntGastosDeVentaContable"] = Convert.ToDouble(dr["GastosDeVentaContable"]) != 0 ? (Convert.ToDouble(dr["GastosDeVentaContable"]) / 12) * _mesDelPeriodo : 0;
                    r["PAntGastosDeVentaReal"] = Convert.ToDouble(r["PAntGastosDeVentaContable"]) - Convert.ToDouble(dr["GastosPersonalesDeVentasContable"]) != 0 ? Convert.ToDouble(r["PAntGastosDeVentaContable"]) - ((Convert.ToDouble(dr["GastosPersonalesDeVentasContable"]) / 12) * _mesDelPeriodo) : 0; // Se omite datos de empresas internas como fintac docvision, etc, porque ya no se manejan segun depto de contabilidad
                    r["PAntPorcentajeGastosDeVentaContable"] = (Convert.ToDecimal(r["PAntGastosDeVentaContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    r["PAntPorcentajeGastosDeVentaReal"] = (Convert.ToDecimal(r["PAntGastosDeVentaReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;

                    //GASTOS DE ADMINISTRACION
                    r["PAntGastosDeAdministracionContable"] = Convert.ToDouble(dr["GastosDeAdministracionContable"]) != 0 ? (Convert.ToDouble(dr["GastosDeAdministracionContable"])/12)* _mesDelPeriodo : 0;
                    r["PAntGastosDeAdministracionReal"] = Convert.ToDouble(r["PAntGastosDeAdministracionContable"]) - Convert.ToDouble(dr["GastosPersonalesDeAdministracionContable"]) != 0 ? Convert.ToDouble(r["PAntGastosDeAdministracionContable"]) - ((Convert.ToDouble(dr["GastosPersonalesDeAdministracionContable"]) / 12) * _mesDelPeriodo) : 0; // Se omite datos de empresas internas como fintac docvision, etc, porque ya no se manejan segun depto de contabilidad;
                    r["PAntPorcentajeGastosDeAdministracionContable"] = (Convert.ToDecimal(r["PAntGastosDeAdministracionContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    r["PAntPorcentajeGastosDeAdministracionReal"] = (Convert.ToDecimal(r["PAntGastosDeAdministracionReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;

                    //DEDUCCION POR CUENTAS INCOBRABLES
                    r["PAntDeduccionPorCuentasIncobrablesContable"] = Convert.ToDouble(dr["DeduccionPorCuentasIncobrablesContable"]) != 0 ? (Convert.ToDouble(dr["DeduccionPorCuentasIncobrablesContable"]) / 12) * _mesDelPeriodo : 0;
                    r["PAntDeduccionPorCuentasIncobrablesReal"] = Convert.ToDouble(dr["DeduccionPorCuentasIncobrablesReal"]) != 0 ? (Convert.ToDouble(dr["DeduccionPorCuentasIncobrablesReal"]) / 12) * _mesDelPeriodo : 0;
                    r["PAntPorcentajeDeduccionPorCuentasIncobrablesContable"] = (Convert.ToDecimal(r["PAntDeduccionPorCuentasIncobrablesContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    r["PAntPorcentajeDeduccionPorCuentasIncobrablesReal"] = (Convert.ToDouble(r["PAntDeduccionPorCuentasIncobrablesReal"]) / Convert.ToDouble(r["PAntIngresosContable"])) * 100;


                    //GASTOS FINANCIEROS Y OTROS GASTOS
                    r["PAntGastosFinancierosYOtrosGastosContable"] = Convert.ToDouble(dr["GastosFinancierosYOtrosGastosContable"]) != 0 ? (Convert.ToDouble(dr["GastosFinancierosYOtrosGastosContable"]) / 12) * _mesDelPeriodo : 0;
                    r["PAntGastosFinancierosYOtrosGastosReal"] = Convert.ToDouble(dr["GastosFinancierosYOtrosGastosReal"]) != 0 ? (Convert.ToDouble(dr["GastosFinancierosYOtrosGastosReal"]) / 12) * _mesDelPeriodo : 0;
                    r["PAntPorcentajeGastosFinancierosYOtrosGastosContable"] = (Convert.ToDecimal(r["PAntGastosFinancierosYOtrosGastosContable"]) / Convert.ToDecimal(r["PActIngresosContable"])) * 100;
                    r["PAntPorcentajeGastosFinancierosYOtrosGastosReal"] = (Convert.ToDecimal(r["PAntGastosFinancierosYOtrosGastosReal"]) / Convert.ToDecimal(r["PActIngresosReal"])) * 100;


                    //OTROS INGRESOS
                    r["PAntOtrosIngresosContable"] = Convert.ToDouble(dr["OtrosIngresosContable"]) != 0 ? (Convert.ToDouble(dr["OtrosIngresosContable"]) / 12) * _mesDelPeriodo : 0;
                    r["PAntOtrosIngresosReal"] = Convert.ToDouble(dr["OtrosIngresosReal"]) != 0 ? (Convert.ToDouble(dr["OtrosIngresosReal"]) / 12) * _mesDelPeriodo : 0;
                    r["PAntPorcentajeOtrosIngresosContable"] = (Convert.ToDecimal(r["PAntOtrosIngresosContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    r["PAntPorcentajeOtrosIngresosReal"] = (Convert.ToDecimal(r["PAntOtrosIngresosReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;


                    //UTILIDAD O PERDIDA CAMBIARIA
                    r["PAntUtilidadOPerdidaCambiariaContable"] = Convert.ToDouble(dr["UtilidadOPerdidaCambiariaContable"]) != 0 ? (Convert.ToDouble(dr["UtilidadOPerdidaCambiariaContable"]) / 12) * _mesDelPeriodo : 0;
                    r["PAntUtilidadOPerdidaCambiariaReal"] = Convert.ToDouble(dr["UtilidadOPerdidaCambiariaReal"]) != 0 ? (Convert.ToDouble(dr["UtilidadOPerdidaCambiariaReal"]) / 12) * _mesDelPeriodo : 0;
                    r["PAntPorcentajeUtilidadOPerdidaCambiariaContable"] = (Convert.ToDecimal(r["PAntUtilidadOPerdidaCambiariaContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    r["PAntPorcentajeUtilidadOPerdidaCambiariaReal"] = (Convert.ToDecimal(r["PAntUtilidadOPerdidaCambiariaReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;


                    //IMPUESTOS A LA UTILIDAD DIFERIDOS
                    r["PAntImpuestosALaUtilidadContable"] = Convert.ToDouble(dr["ImpuestosAlaUtilidadDiferidosContable"]) != 0 ? (Convert.ToDouble(dr["ImpuestosAlaUtilidadDiferidosContable"])) * _mesDelPeriodo : 0;
                    r["PAntImpuestosALaUtilidadReal"] = Convert.ToDouble(dr["ImpuestosAlaUtilidadDiferidosReal"]) != 0 ? (Convert.ToDouble(dr["ImpuestosAlaUtilidadDiferidosReal"])) * _mesDelPeriodo : 0;
                    r["PAntPorcentajeImpuestosALaUtilidadContable"] = (Convert.ToDecimal(r["PAntImpuestosALaUtilidadContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    r["PAntPorcentajeImpuestosALaUtilidadReal"] = (Convert.ToDecimal(r["PAntImpuestosALaUtilidadReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;


                    //IMPUESTOS RESULTADO DEL EJERCICIO
                    r["PAntImpuestosResultadoEjercicioContable"] = Convert.ToDouble(dr["ImpuestosResultadoDelEjercicioContable"]) != 0 ? (Convert.ToDouble(dr["ImpuestosResultadoDelEjercicioContable"])) * _mesDelPeriodo : 0;
                    r["PAntImpuestosResultadoEjercicioReal"] = Convert.ToDouble(dr["ImpuestosResultadoDelEjercicioReal"]) != 0 ? (Convert.ToDouble(dr["ImpuestosResultadoDelEjercicioReal"])) * _mesDelPeriodo : 0;
                    r["PAntPorcentajeImpuestosResultadoEjercicioContable"] = (Convert.ToDecimal(r["PAntImpuestosResultadoEjercicioContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    r["PAntPorcentajeImpuestosResultadoEjercicioReal"] = (Convert.ToDecimal(r["PAntImpuestosResultadoEjercicioReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;


                    //PTU DEL EJERCICIO
                    r["PAntPtuDelEjercicioContable"] = Convert.ToDouble(dr["PtuDelEjercicioContable"]) != 0 ? (Convert.ToDouble(dr["PtuDelEjercicioContable"])) * _mesDelPeriodo : 0;
                    r["PAntPtuDelEjercicioReal"] = Convert.ToDouble(dr["PtuDelEjercicioReal"]) != 0 ? (Convert.ToDouble(dr["PtuDelEjercicioReal"])) * _mesDelPeriodo : 0;
                    r["PAntPorcentajePtuDelEjercicioContable"] = (Convert.ToDecimal(r["PAntPtuDelEjercicioContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    r["PAntPorcentajePtuDelEjercicioReal"] = (Convert.ToDecimal(r["PAntPtuDelEjercicioReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;


                    //UTILIDAD NETA
                    r["PAntUtilidadNetaContable"] =
                        Convert.ToDouble(r["PAntUtilidadBrutaContable"])
                        -
                        (Convert.ToDouble(r["PAntGastosDeVentaContable"]) + Convert.ToDouble(r["PAntGastosDeAdministracionContable"]) + Convert.ToDouble(r["PAntDeduccionPorCuentasIncobrablesContable"]) + Convert.ToDouble(r["PAntGastosFinancierosYOtrosGastosContable"]))
                        +                    
                        (Convert.ToDouble(r["PAntOtrosIngresosContable"]) + Convert.ToDouble(r["PAntUtilidadOPerdidaCambiariaContable"]) - Convert.ToDouble(r["PAntImpuestosResultadoEjercicioContable"]) - Convert.ToDouble(r["PAntPtuDelEjercicioContable"]) + Convert.ToDouble(r["PAntImpuestosALaUtilidadContable"]));


                    r["PAntUtilidadNetaReal"] =
                        Convert.ToDouble(r["PAntUtilidadBrutaReal"])
                        -
                        (Convert.ToDouble(r["PAntGastosDeVentaReal"]) + Convert.ToDouble(r["PAntGastosDeAdministracionReal"]) + Convert.ToDouble(r["PAntDeduccionPorCuentasIncobrablesReal"]) + Convert.ToDouble(r["PAntGastosFinancierosYOtrosGastosReal"]))
                        +
                        (Convert.ToDouble(r["PAntOtrosIngresosReal"]) + Convert.ToDouble(r["PAntUtilidadOPerdidaCambiariaReal"]) - Convert.ToDouble(r["PAntImpuestosResultadoEjercicioReal"]) - Convert.ToDouble(r["PAntPtuDelEjercicioReal"]) + Convert.ToDouble(r["PAntImpuestosALaUtilidadReal"]));


                    r["PAntPorcentajeUtilidadNetaContable"] = (Convert.ToDecimal(r["PAntUtilidadNetaContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    r["PAntPorcentajeUtilidadNetaReal"] = (Convert.ToDecimal(r["PAntUtilidadNetaReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;


                    //Gastos de venta y administrativos por empresa (Se requiere calcularlo aquí para sacar la utilidad neta por empresa)
                    double _gastoDeVentaPersonalFacturContable = Convert.ToDouble(dr["GastoDeVentaPersonalFacturContable"]);
                    double _gastoDeVentaPersonalFacturReal = Convert.ToDouble(dr["GastoDeVentaPersonalFacturContable"]) != 0 ? (Convert.ToDouble(dr["GastoDeVentaPersonalFacturContable"]) / 12) * _mesDelPeriodo : 0;

                    double _gastoDeVentaPersonalBalorContable = Convert.ToDouble(dr["GastoDeVentaPersonalBalorContable"]);
                    double _gastoDeVentaPersonalBalorReal = Convert.ToDouble(dr["GastoDeVentaPersonalBalorContable"]) != 0 ? (Convert.ToDouble(dr["GastoDeVentaPersonalBalorContable"]) / 12) * _mesDelPeriodo : 0;

                    double _gastoDeAdministracionPersonalFacturContable = Convert.ToDouble(dr["GastoDeAdministracionPersonalFacturContable"]);
                    double _gastoDeAdministracionPersonalFacturReal = Convert.ToDouble(dr["GastoDeAdministracionPersonalFacturContable"]) != 0 ? (Convert.ToDouble(dr["GastoDeAdministracionPersonalFacturContable"]) / 12) * _mesDelPeriodo : 0;

                    double _gastoDeAdministracionPersonalBalorContable = Convert.ToDouble(dr["GastoDeAdministracionPersonalBalorContable"]);
                    double _gastoDeAdministracionPersonalBalorReal = Convert.ToDouble(dr["GastoDeAdministracionPersonalBalorContable"]) != 0 ? (Convert.ToDouble(dr["GastoDeAdministracionPersonalBalorContable"]) / 12) * _mesDelPeriodo : 0;


                    r["PAntUtilidadNetaFacturContable"] = Convert.ToDouble(dr["UtilidadNetaFacturcoContable"]) != 0 ? (Convert.ToDouble(dr["UtilidadNetaFacturcoContable"]) / 12) * _mesDelPeriodo : 0;
                    r["PAntUtilidadNetaBalorContable"] = Convert.ToDouble(dr["UtilidadNetaBalorContable"]) != 0 ? (Convert.ToDouble(dr["UtilidadNetaBalorContable"]) / 12) * _mesDelPeriodo : 0;
                    r["PAntUtilidadNetaTativoContable"] = Convert.ToDouble(dr["UtilidadNetaTativoContable"]) != 0 ? (Convert.ToDouble(dr["UtilidadNetaTativoContable"]) / 12) * _mesDelPeriodo : 0;
                    r["PAntUtilidadNetaFacturReal"] = Convert.ToDouble(r["PAntUtilidadNetaFacturContable"]) + _gastoDeVentaPersonalFacturReal + _gastoDeAdministracionPersonalFacturReal;
                    r["PAntUtilidadNetaBalorReal"] = Convert.ToDouble(r["PAntUtilidadNetaBalorContable"]) + _gastoDeVentaPersonalBalorReal + _gastoDeAdministracionPersonalBalorReal;
                    r["PAntUtilidadNetaTativoReal"] = Convert.ToDouble(dr["UtilidadNetaTativoContable"]) != 0 ? ((Convert.ToDouble(dr["UtilidadNetaTativoContable"]) / 12) * _mesDelPeriodo) + 1 : 0;


                    r["PAntUtilidadNetaTotalContable"] = Convert.ToDouble(r["PAntUtilidadNetaFacturContable"]) + Convert.ToDouble(r["PAntUtilidadNetaBalorContable"]) + Convert.ToDouble(r["PAntUtilidadNetaTativoContable"]);
                    r["PAntUtilidadNetaTotalReal"] = Convert.ToDouble(r["PAntUtilidadNetaFacturReal"]) + Convert.ToDouble(r["PAntUtilidadNetaBalorReal"]) + Convert.ToDouble(r["PAntUtilidadNetaTativoReal"]);

                    r["PAntEfectoCambiarioContable"] = Convert.ToDouble(dr["UtilidadOPerdidaCambiariaContable"]) != 0 ? (Convert.ToDouble(dr["UtilidadOPerdidaCambiariaContable"]) / 12) * _mesDelPeriodo : 0;
                    r["PAntEfectoCambiarioReal"] = Convert.ToDouble(dr["UtilidadOPerdidaCambiariaReal"]) != 0 ? (Convert.ToDouble(dr["UtilidadOPerdidaCambiariaReal"]) / 12) * _mesDelPeriodo : 0;


                    //UTILIDAD O PERDIDA SIN EFECTO CAMBIARIO
                    //r["PAntUtilidadOPerdidaSinEfectoCambiarioContable"] = Convert.ToDouble(dr["UtilidadOPerdidaSinEfectoCambiarioContable"]);
                    //r["PAntUtilidadOPerdidaSinEfectoCambiarioReal"] = Convert.ToDouble(dr["UtilidadOPerdidaSinEfectoCambiarioReal"]);

                    r["PAntUtilidadOPerdidaSinEfectoCambiarioContable"] =  Convert.ToDouble(r["PAntUtilidadNetaContable"])- Convert.ToDouble(r["PAntUtilidadOPerdidaCambiariaContable"]);
                    r["PAntUtilidadOPerdidaSinEfectoCambiarioReal"] = Convert.ToDouble(r["PAntUtilidadNetaReal"]) - Convert.ToDouble(r["PAntUtilidadOPerdidaCambiariaReal"]);
                    r["PAntPorcentajeUtilidadOPerdidaSinEfectoCambiarioContable"] = (Convert.ToDecimal(r["PAntUtilidadOPerdidaSinEfectoCambiarioContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    r["PAntPorcentajeUtilidadOPerdidaSinEfectoCambiarioReal"] = (Convert.ToDecimal(r["PAntUtilidadOPerdidaSinEfectoCambiarioReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;


                    r["PAntSueldosVentasContable"] = Convert.ToDouble(dr["SueldosDeVentasContable"]) != 0 ? (Convert.ToDouble(dr["SueldosDeVentasContable"]) / 12) * _mesDelPeriodo : 0;
                    r["PAntSueldosVentasReal"] = Convert.ToDouble(dr["SueldosDeVentasReal"]) != 0 ? (Convert.ToDouble(dr["SueldosDeVentasReal"]) / 12) * _mesDelPeriodo : 0;
                    r["PAntSueldosAdministracionContable"] = Convert.ToDouble(dr["SueldosDeAdministracionContable"]) !=0 ? (Convert.ToDouble(dr["SueldosDeAdministracionContable"])/12) * _mesDelPeriodo : 0;
                    r["PAntSueldosAdministracionReal"] = Convert.ToDouble(dr["SueldosDeAdministracionReal"]) != 0 ? (Convert.ToDouble(dr["SueldosDeAdministracionReal"])/12) * _mesDelPeriodo : 0;
                    r["PAntPorcentajeSueldosVentasContable"] = (Convert.ToDecimal(r["PAntSueldosVentasContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    r["PAntPorcentajeSueldosVentasReal"] = (Convert.ToDecimal(r["PAntSueldosVentasReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;
                    r["PAntPorcentajeSueldosAdministracionContable"] = (Convert.ToDecimal(r["PAntSueldosAdministracionContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    r["PAntPorcentajeSueldosAdministracionReal"] = (Convert.ToDecimal(r["PAntSueldosAdministracionReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;
                    r["PAntTotalSueldosContable"] = Convert.ToDecimal(r["PAntSueldosVentasContable"]) + Convert.ToDecimal(r["PAntSueldosAdministracionContable"]);
                    r["PAntTotalSueldosReal"] = Convert.ToDecimal(r["PAntSueldosVentasReal"]) + Convert.ToDecimal(r["PAntSueldosAdministracionReal"]);
                    r["PAntPorcentajeTotalSueldosContable"] = (Convert.ToDecimal(r["PAntTotalSueldosContable"]) / Convert.ToDecimal(r["PAntIngresosContable"])) * 100;
                    r["PAntPorcentajeTotalSueldosReal"] = (Convert.ToDecimal(r["PAntTotalSueldosReal"]) / Convert.ToDecimal(r["PAntIngresosReal"])) * 100;

                    //GASTOS PERSONALES
                    r["PAntGastosPersonalesDeVentaContable"] = Convert.ToDouble(dr["GastosPersonalesDeVentasContable"]) != 0 ? (Convert.ToDouble(dr["GastosPersonalesDeVentasContable"])/12) * _mesDelPeriodo : 0;
                    r["PAntGastosPersonalesDeAdministracionContable"] = Convert.ToDouble(dr["GastosPersonalesDeAdministracionContable"]) != 0 ? (Convert.ToDouble(dr["GastosPersonalesDeAdministracionContable"]) / 12) * _mesDelPeriodo : 0;
                    r["PAntGastosPersonalesPtuContable"] = Convert.ToDouble(dr["GastosPersonalesPtuContable"]) != 0 ? (Convert.ToDouble(dr["GastosPersonalesPtuContable"]) / 12) * _mesDelPeriodo : 0; 
                    r["PAntGastosPersonalesDeVentaReal"] = Convert.ToDouble(dr["GastosPersonalesDeVentasReal"]) != 0? (Convert.ToDouble(dr["GastosPersonalesDeVentasReal"])/12)* _mesDelPeriodo : 0;
                    r["PAntGastosPersonalesDeAdministracionReal"] = Convert.ToDouble(dr["GastosPersonalesDeAdministracionReal"]) != 0 ? (Convert.ToDouble(dr["GastosPersonalesDeAdministracionReal"])/12) * _mesDelPeriodo : 0;
                    r["PAntGastosPersonalesPtuReal"] = Convert.ToDouble(dr["GastosPersonalesPtuReal"]) != 0 ? (Convert.ToDouble(dr["GastosPersonalesPtuReal"]) / 12) * _mesDelPeriodo : 0;

                    r["PAntTotalGastosPersonalesContable"] = Convert.ToDecimal(r["PAntGastosPersonalesDeVentaContable"]) + Convert.ToDecimal(r["PAntGastosPersonalesDeAdministracionContable"]) + Convert.ToDecimal(r["PAntGastosPersonalesPtuContable"]);
                    r["PAntTotalGastosPersonalesReal"] = Convert.ToDecimal(r["PAntGastosPersonalesDeVentaReal"]) + Convert.ToDecimal(r["PAntGastosPersonalesDeAdministracionReal"]) + Convert.ToDecimal(r["PAntGastosPersonalesPtuReal"]);




                }
                dt.Rows.Add(r);

                ds.Tables.Add(tblEmpresaFacturco.Copy());
                ds.Tables.Add(tblEmpresaBalor.Copy());
                ds.Tables.Add(dt);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            

            return ds;
        }
    }

    public class ControladorReporteResumenRelacionSaldos : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            //OBTENEMOS LOS PARAMETROS DEL REPORTE
            string empresaid = parametros.Get("empresaid");
            DateTime fecha = DateTime.Parse(parametros.Get("fecha"));
            int mostrarComprobacion = int.Parse(parametros.Get("mostrarcomprobacion"));
            int mostrarComprobacionADetalle = int.Parse(parametros.Get("mostrarcomprobacionadetalle"));




            bool incluirDemandasCaducadas = true;
            bool ajustecontable = true;
            bool incluirDemandasPagadas = true;

            Entity.Configuracion.Catempresa _empresa = MobileBO.ControlConfiguracion.TraerCatempresas(empresaid);

            if (empresaid != null)
            {
                if (empresaid.Trim() == "" || empresaid.Trim() == "*")
                {
                    empresaid = null;
                }
            }

                

            try
            {

                Entity.Configuracion.Catempresa empresa = MobileBO.ControlConfiguracion.TraerCatempresas(empresaid);
                //DataSet ds = MobileBO.ControlCartera.ResumenRelacionSaldos(fecha, empresaid);
                DataSet ds = ReporteResumenRelacionSaldos.ProcesaResumenRelacionDeSaldos(empresaid, fecha);

                DataSet dsEmpresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(empresa.Empresaid);
                dsEmpresa.Tables[0].TableName = "tblEmpresa";
                ds.Tables.Add(dsEmpresa.Tables[0].Copy());

                DataTable DatosReporte = new DataTable("DatosReporte");
                DatosReporte.Columns.Add("Fecha", typeof(DateTime));
                DatosReporte.Columns.Add("MostrarComprobacion", typeof(int)); //0=NO, 1=SI
                DatosReporte.Columns.Add("MostrarComprobacionADetalle", typeof(int)); //0=NO, 1=SI
                DataRow rd = DatosReporte.NewRow();
                rd["Fecha"] = fecha;
                rd["MostrarComprobacion"] = mostrarComprobacion;
                rd["MostrarComprobacionADetalle"] = mostrarComprobacion == 1 ? mostrarComprobacionADetalle : 0;
                DatosReporte.Rows.Add(rd);
                ds.Tables.Add(DatosReporte);


                if (_empresa.Empresa == 1)
                {
                    base.NombreReporte = "ReporteResumenRelacionSaldosF";
                }
                else
                {
                    base.NombreReporte = "ReporteResumenRelacionSaldos";
                }
                base.FormatoReporte = 3;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\ResumenRelacionSaldos.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        #endregion
    }

    public class ControladorReporteGastos : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            try
            {
                string empresaid = parametros.Get("empresaid");
                DateTime fecha = DateTime.Parse(parametros.Get("fecha"));

                Entity.Configuracion.Catempresa empresa = MobileBO.ControlConfiguracion.TraerCatempresas(empresaid);
                DataSet ds = MobileBO.ControlContabilidad.TraeGastos(fecha, empresaid);

                foreach(DataRow r in ds.Tables[0].Rows)
                {
                    if(r["Cuenta"].ToString().StartsWith("6402") || r["Cuenta"].ToString().StartsWith("6602"))
                    {
                        if (empresaid.ToUpper() == "FA764836-BB07-4EB3-9B30-2B69206174C2")
                        {
                            r["ImporteReal"] = Convert.ToInt32(r["CuentaSinMovimientoBalor"]) == 1 ?
                                    Convert.ToDecimal(r["Importe"]) - Convert.ToDecimal(r["ImportePersonal"]) - Convert.ToDecimal(r["ImporteTativo"]) :
                                    Convert.ToDecimal(r["Importe"]) - Convert.ToDecimal(r["ImportePersonal"]) + (Convert.ToDecimal(r["ImporteFacturDeBalor"]) + Convert.ToDecimal(r["ImportePersonalesFacturDeBalor"])) - Convert.ToDecimal(r["ImporteTativo"]);
                        }
                        else
                        {
                            r["ImporteReal"] = Convert.ToDecimal(r["Importe"]) - Convert.ToDecimal(r["ImportePersonal"]) - Convert.ToDecimal(r["ImporteFacturDeBalor"]) - Convert.ToDecimal(r["ImportePersonalesFacturDeBalor"]) - Convert.ToDecimal(r["ImporteTativo"]);
                        }

                    }

                }
                ds.Tables[0].AcceptChanges();

                DataSet dsEmpresa = MobileBO.ControlConfiguracion.TraerDsEmpresasFull(empresa.Empresaid);
                dsEmpresa.Tables[0].TableName = "tblEmpresa";
                ds.Tables.Add(dsEmpresa.Tables[0].Copy());

                DataTable DatosReporte = new DataTable("DatosReporte");
                DatosReporte.Columns.Add("Fecha", typeof(DateTime));                
                DataRow rd = DatosReporte.NewRow();
                rd["Fecha"] = fecha;                
                DatosReporte.Rows.Add(rd);
                ds.Tables.Add(DatosReporte);

                base.NombreReporte = "ReporteGastos";
                base.FormatoReporte = 3;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\ReporteGastos.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        #endregion
    }

    public class ControladorReporteEstadoDeResultadosDetalle : Base.Clases.BaseReportes
    {
        #region Inicializa Reporte
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            try
            {
                //DataSet ds = new DataSet();
                string empresaid = parametros.Get("empresaid");
                DateTime fecha = DateTime.Parse(parametros.Get("fecha"));

                DataSet ds = ReporteResumenRelacionSaldos.ProcesaEstadoDeResultadosDetalle(empresaid, fecha);

                base.NombreReporte = "EstadoDeResultadosDetalle";
                base.FormatoReporte = 3;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\ReporteEstadoDeResultadosDetalle.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        #endregion
    }

    public class ControladorReporteEstadoDeResultadosDetalleConsolidado : Base.Clases.BaseReportes
    {        
        #region Inicializa Reporte
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            try
            {
                //DataSet ds = new DataSet();
                string empresaid = parametros.Get("empresaid");
                DateTime fecha = DateTime.Parse(parametros.Get("fecha"));

                DataSet ds = ReporteResumenRelacionSaldos.ProcesaEstadoDeResultadosConsolidado(fecha);

                DataTable DatosReporte = new DataTable("DatosReporte");
                DatosReporte.Columns.Add("Fecha", typeof(DateTime));
                DataRow rd = DatosReporte.NewRow();
                rd["Fecha"] = fecha;
                DatosReporte.Rows.Add(rd);
                ds.Tables.Add(DatosReporte);

                base.NombreReporte = "EstadoDeResultadosDetalleConsolidado";
                base.FormatoReporte = 3;
                base.RutaReporte = "Contabilidad\\Reportes";
                base.DataSource = ds;
                base.DataSource.WriteXml("c:\\Reportes\\ReporteEstadoDeResultadosDetalleConsolidado.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        #endregion
    }
}