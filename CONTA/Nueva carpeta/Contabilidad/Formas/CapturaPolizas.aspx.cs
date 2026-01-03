using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Xml;
using System.IO;
using System.Web.Script.Serialization;
using Entity;
using System.Text.RegularExpressions;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class CapturaPolizas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (this.Request.QueryString.Get("Polizaid") != null)
            {
                hiddenPolizaForaneaID.Value = this.Request.QueryString.Get("Polizaid");
            }
        }

        [WebMethod]
        public static Entity.Response<object> GuardarCambioNumeroPoliza(string numpol, string polizaid,string usuario)
        {
            try
            {
                Entity.Contabilidad.Poliza Poliza = new MobileBO.ControlContabilidad().TraerPolizas(polizaid);
                if (Poliza == null)
                    return Entity.Response<object>.CrearResponse<object>(true, new { Guardo = false, msg = "Inconsistencia de datos, comunicate a sistemas" });

                Entity.Contabilidad.Cierrecontabilidad cierre = MobileBO.ControlContabilidad.TraerCierrecontabilidad(Poliza.EmpresaId);
                if (cierre != null)
                {
                    DateTime fechaPol = Poliza.Fechapol;
                    if (fechaPol <= cierre.Fechacierre)
                    {
                        throw new Exception("Contabilidad cerrada al " + cierre.Fechacierre.ToShortDateString());
                    }
                }


                DataSet ds = MobileBO.ControlContabilidad.VerificarFolioPoliza(numpol, Poliza.TipPol, Poliza.Fechapol, Poliza.EmpresaId, Poliza.Pendiente);
                if (ds.Tables[0].Rows.Count > 0)
                    return Entity.Response<object>.CrearResponse<object>(true, new { Guardo = false, msg = "El numero de poliza que intentas registrar ya existe en el sistema" });

                Poliza.Usuario = usuario;
                Poliza.Folio = numpol;
                Poliza.Fecha = DateTime.Now;

                new MobileBO.ControlContabilidad().GuardarPoliza(new Entity.ListaDeEntidades<Entity.Contabilidad.Poliza>() { Poliza });
                
                return Entity.Response<object>.CrearResponse<object>(true, new { Guardo = true });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }


        [WebMethod]
        public static Entity.Response<object> GuardarCopiaDePoliza(string value)
        {
            MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();
            Entity.Contabilidad.Poliza Poliza = new Entity.Contabilidad.Poliza();
            Entity.ModeloPoliza modeloPoliza;
            try
            {
                modeloPoliza = MobileBO.Utilerias.Deserializar<Entity.ModeloPoliza>(value);
                Poliza = ModeloAEntidad(modeloPoliza);
                Entity.Contabilidad.Poliza PolizaOriginal = new MobileBO.ControlContabilidad().TraerPolizas(Poliza.Polizaid);

                if (PolizaOriginal.Pendiente)
                {
                    //Entity.Contabilidad.Poliza PolizaNoPendiente = new MobileBO.ControlContabilidad().TraerPolizaPorFolio(Poliza.Folio, Poliza.TipPol, Poliza.EmpresaId, Poliza.Fechapol);
                    return Entity.Response<object>.CrearResponse<object>(true, new { Guardo = false, msg = "Solo se pueden copiar polizas normales a pendientes, la poliza que intentar copiar está marcada como pendiente" });
                }

                Entity.Contabilidad.Cierrecontabilidad cierre = MobileBO.ControlContabilidad.TraerCierrecontabilidad(Poliza.EmpresaId);
                if (cierre != null)
                 {
                    DateTime fechaPol = Poliza.Fechapol;
                    if (fechaPol <= cierre.Fechacierre)
                    {
                        throw new Exception("Contabilidad cerrada al " + cierre.Fechacierre.ToShortDateString());
                    }
                }
                
                //Obtenemos el estado de la contabilidad
                bool _modoFiscal = MobileBO.ControlContabilidad.ValidaCuentasFiscales(Poliza.EmpresaId, Convert.ToDateTime(Poliza.Fechapol).Year);

                if (PolizaOriginal.Folio.Substring(0, 1) == "0")
                {
                    Poliza.Folio = PolizaOriginal.Folio.Substring(1, (PolizaOriginal.Folio.Length - 1));
                }
                else
                {
                    Poliza.Folio = "0" + PolizaOriginal.Folio;
                }



                //if (_modoFiscal)
                //{
                //    if (Poliza.Folio.Substring(0, 1) != "0")
                //    {
                //        Poliza.Folio = "0" + Poliza.Folio;
                //    }
                //    else
                //    {
                //        return Entity.Response<object>.CrearResponse<object>(false, new { Guardo = false, msg = "No se puede copiar un poliza normal a una pendiente cuando el número de folio inicia con '0' y el sistema se encuentra en 'FISCAL'" });
                //    }                                                  
                //}
                //else
                //{
                //    if (Poliza.Folio.Substring(0, 1) == "0")
                //    {
                //        Poliza.Folio = Poliza.Folio.Substring(1, (Poliza.Folio.Length - 1));
                //    }
                   
                //}

                Poliza.Polizaid = Guid.Empty.ToString();
                Poliza.Pendiente = true;
                Poliza.Fecha = DateTime.Now;

                foreach (Entity.Contabilidad.Polizasdetalle pd in Poliza.ListaPolizaDetalle)
                {
                    pd.Polizadetalleid = Guid.Empty.ToString();
                    pd.Polizaid = Guid.Empty.ToString();
                    pd.Fecha = DateTime.Now;
                }

                DataSet ds = MobileBO.ControlContabilidad.VerificarFolioPoliza(Poliza.Folio, Poliza.TipPol, Poliza.Fechapol, Poliza.EmpresaId, Poliza.Pendiente);
                if (ds.Tables[0].Rows.Count > 0)
                    return Entity.Response<object>.CrearResponse<object>(true, new { Guardo = false, msg = "El numero de poliza que intenta registrar como pendiente ya existe en el sistema" });

                using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope())
                {
                    //Guardar la póliza pendiente

                    
                    bool intercambiaPolizas = false;
                    new MobileBO.ControlContabilidad().GuardarPoliza(new Entity.ListaDeEntidades<Entity.Contabilidad.Poliza>() { Poliza });

                    if (_modoFiscal && PolizaOriginal.Folio.Substring(0, 1) == "0")
                    {
                        intercambiaPolizas = true;
                    }
                    if(!_modoFiscal && PolizaOriginal.Folio.Substring(0, 1) != "0")
                    {
                        intercambiaPolizas = true;
                    }

                    if (intercambiaPolizas)
                    {
                        //intercambiamos polizas                        
                        Entity.Contabilidad.Acvgral _acvGralOriginal = MobileBO.ControlContabilidad.TraerAcvgralPorReferenciaId(PolizaOriginal.Polizaid);
                        Entity.Contabilidad.Acvgralpdte _acvGralPdte = MobileBO.ControlContabilidad.TraerAcvgralpdtePorReferenciaId(Poliza.Polizaid);
                                                
                        MobileBO.ControlContabilidad.IntercambiaPolizasContablesFiscales(_acvGralOriginal.Acvgralid, _acvGralPdte.Acvgralid);                        
                    }
                    ts.Complete();
                }

                return Entity.Response<object>.CrearResponse<object>(true, new { Guardo = true });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }


        [WebMethod]
        public static Entity.Response<object> GuardarCambioFechaPoliza(string fecha, string polizaid, string usuario)
        {
            try
            {
                Entity.Contabilidad.Poliza Poliza = new MobileBO.ControlContabilidad().TraerPolizas(polizaid);
                if (Poliza == null)
                    return Entity.Response<object>.CrearResponse<object>(true, new { Guardo = false, msg = "Inconsistencia de datos, comunicate a sistemas" });


                Entity.Contabilidad.Cierrecontabilidad cierre = MobileBO.ControlContabilidad.TraerCierrecontabilidad(Poliza.EmpresaId);
                if (cierre != null)
                {
                    DateTime fechaPol = DateTime.Parse(fecha);
                    if (fechaPol <= cierre.Fechacierre)
                    {
                        throw new Exception("Contabilidad cerrada al " + cierre.Fechacierre.ToShortDateString());
                    }
                }


                Poliza.Usuario = usuario;
                Poliza.Fechapol = DateTime.Parse(fecha);
                Poliza.Fecha = DateTime.Now;

                new MobileBO.ControlContabilidad().GuardarPoliza(new Entity.ListaDeEntidades<Entity.Contabilidad.Poliza>() { Poliza });

                return Entity.Response<object>.CrearResponse<object>(true, new { Guardo = true });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

        


        [WebMethod]
        public static Entity.Response<object> VerificarFolioPoliza(string NumPol, string TipPol, string FecPol, string EmpresaID, bool Pendiente)
        {
            try
            {
                bool existe = false;
                DataSet ds = MobileBO.ControlContabilidad.VerificarFolioPoliza(NumPol, TipPol, DateTime.Parse(FecPol), EmpresaID, Pendiente);
                if (ds.Tables[0].Rows.Count > 0)
                    existe = true;

                return Entity.Response<object>.CrearResponse<object>(true, new { Existe = existe });
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
            catch {
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

        [WebMethod]
        public static Entity.Response<bool> exportaPoliza(string polizaid)
        {
            try
            {
                Entity.Contabilidad.Poliza Poliza = new MobileBO.ControlContabilidad().TraerPolizas(polizaid);
                Entity.Configuracion.Catempresa Empresa = MobileBO.ControlConfiguracion.TraerCatempresas(Poliza.EmpresaId);
                /*
                string stringconexion = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\\sistemas\\ConWin\\Balor\\Conta.MDB";
                if (Empresa.Empresa == 1)
                    stringconexion = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\\sistemas\\ConWin\\Factur\\Conta.MDB";
                */


                string stringconexion = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\\\192.168.10.251\\D\\sistema\\ConWin\\Balor\\Conta.MDB";
                if (Empresa.Empresa == 1)
                    stringconexion = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\\\192.168.10.251\\D\\sistema\\ConWin\\Factur\\Conta.MDB";
                
                OleDbConnection Conexion = new OleDbConnection(stringconexion);
                Conexion.Open();
                if (Conexion.State == ConnectionState.Open)
                {
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = Conexion;
                    if (!Poliza.Pendiente)
                    {
                        Entity.Contabilidad.Acvgral acvgral = MobileBO.ControlContabilidad.TraerAcvgralPorReferenciaId(Poliza.Polizaid);
                        Entity.ListaDeEntidades<Entity.Contabilidad.Acvmov> ListaAcvMov = MobileBO.ControlContabilidad.TraerAcvmovPorAcvGral(acvgral.Acvgralid);


                        DataTable TablaPoliza = new DataTable();
                        string sql = "Select * from acvmov where tip_pol='" + acvgral.TipPol + "' and ltrim(num_pol)='" + acvgral.NumPol.Trim() + "' and fec_pol = DateValue('" + acvgral.FecPol.ToShortDateString() + "')";
                        OleDbDataAdapter da = new OleDbDataAdapter(sql, Conexion);
                        da.Fill(TablaPoliza);

                        if (TablaPoliza.Rows.Count == 0) {
                            string numpol = "          " + acvgral.NumPol.Trim();
                            numpol = numpol.Substring(numpol.Length - 9, 9);

                            string insertacvgral = "insert into acvgral (AnoMes,TIP_POL,NUM_POL,FEC_POL,CONCEPTO,IMPORTE,USUARIO,AFECTA_ADMVA) VALUES(";
                            insertacvgral += "'" + acvgral.Anomes + "',";
                            insertacvgral += "'" + acvgral.TipPol + "',";
                            insertacvgral += "'" + numpol + "',";
                            insertacvgral += "datevalue('" + acvgral.FecPol.ToShortDateString() + "'),";
                            insertacvgral += "'" + acvgral.Concepto + "',";
                            insertacvgral += "" + acvgral.Importe + ",";
                            insertacvgral += "'" + acvgral.Usuario + "',";
                            insertacvgral += "0)";
                            cmd.CommandText = insertacvgral;
                            cmd.ExecuteNonQuery();

                            foreach (Entity.Contabilidad.Acvmov acvmov in ListaAcvMov)
                            {
                                string refer = "          " + acvmov.Refer.Trim();
                                refer = refer.Substring(refer.Length - 9, 9);
                                string cuenta = acvmov.Cuenta.Substring(0, 4) + acvmov.Cuenta.Substring(5, 3) + acvmov.Cuenta.Substring(9, 3) + acvmov.Cuenta.Substring(13, 3);

                                string insertacvmov = "insert into acvmov(ANOMES,TIP_POL,NUM_POL,FEC_POL,TIP_MOV,CUENTA,CONCEPTO,REFEREN,IMPORTE,CLASE_CONT,AFECTA_ADMVA,CPTO_FIS,TASA_IVA,IMP_IVA,CPTO_IVA,FLUJO) VALUES(";
                                insertacvmov += "'" + acvmov.Anomes + "',";
                                insertacvmov += "'" + acvmov.TipPol + "',";
                                insertacvmov += "'" + numpol + "',";
                                insertacvmov += "datevalue('" + acvmov.FecPol.ToShortDateString() + "'),";
                                insertacvmov += "'" + acvmov.TipMov + "',";
                                insertacvmov += "'" + cuenta + "',";
                                insertacvmov += "'" + acvmov.Concepto + "',";
                                insertacvmov += "'" + refer + "',";
                                insertacvmov += "" + acvmov.Importe + ",";
                                insertacvmov += "'F',";
                                insertacvmov += "0,";
                                insertacvmov += "'',";
                                insertacvmov += "" + acvmov.TasaIva + ",";
                                insertacvmov += "0,";
                                insertacvmov += "'',";
                                insertacvmov += "'')";
                                cmd.CommandText = insertacvmov;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            return Entity.Response<bool>.CrearResponseVacio<bool>(false, "Esta poliza ya existe en el sistema viejito");
                        } 
                    }
                    else {
                        Entity.Contabilidad.Acvgralpdte acvgral = MobileBO.ControlContabilidad.TraerAcvgralpdtePorReferenciaId(Poliza.Polizaid);
                        Entity.ListaDeEntidades<Entity.Contabilidad.Acvpdte> ListaAcvMov = MobileBO.ControlContabilidad.TraerAcvpdtePorAcvGralPdte(acvgral.Acvgralid);


                        DataTable TablaPoliza = new DataTable();
                        string sql = "Select * from acvpdte where tip_pol='" + acvgral.TipPol + "' and ltrim(num_pol)='" + acvgral.NumPol.Trim() + "' and fec_pol = DateValue('" + acvgral.FecPol.ToShortDateString() + "')";
                        OleDbDataAdapter da = new OleDbDataAdapter(sql, Conexion);
                        da.Fill(TablaPoliza);

                        if (TablaPoliza.Rows.Count == 0)
                        {

                            string numpol = "          " + acvgral.NumPol.Trim();
                            numpol = numpol.Substring(numpol.Length - 9, 9);

                            string insertacvgral = "insert into acvgral (AnoMes,TIP_POL,NUM_POL,FEC_POL,CONCEPTO,IMPORTE,USUARIO,AFECTA_ADMVA) VALUES(";
                            insertacvgral += "'" + acvgral.Anomes + "',";
                            insertacvgral += "'" + acvgral.TipPol + "',";
                            insertacvgral += "'" + numpol + "',";
                            insertacvgral += "datevalue('" + acvgral.FecPol.ToShortDateString() + "'),";
                            insertacvgral += "'" + acvgral.Concepto + "',";
                            insertacvgral += "" + acvgral.Importe + ",";
                            insertacvgral += "'" + acvgral.Usuario + "',";
                            insertacvgral += "0)";
                            cmd.CommandText = insertacvgral;
                            cmd.ExecuteNonQuery();

                            foreach (Entity.Contabilidad.Acvmov acvmov in ListaAcvMov)
                            {
                                string refer = "          " + acvmov.Refer.Trim();
                                refer = refer.Substring(refer.Length - 9, 9);
                                string cuenta = acvmov.Cuenta.Substring(0, 4) + acvmov.Cuenta.Substring(5, 3) + acvmov.Cuenta.Substring(9, 3) + acvmov.Cuenta.Substring(13, 3);

                                string insertacvmov = "insert into ACVPDTE(ANOMES,TIP_POL,NUM_POL,FEC_POL,TIP_MOV,CUENTA,CONCEPTO,REFEREN,IMPORTE,CLASE_CONT,AFECTA_ADMVA,CPTO_FIS,TASA_IVA,IMP_IVA,CPTO_IVA,FLUJO) VALUES(";
                                insertacvmov += "'" + acvmov.Anomes + "',";
                                insertacvmov += "'" + acvmov.TipPol + "',";
                                insertacvmov += "'" + numpol + "',";
                                insertacvmov += "datevalue('" + acvmov.FecPol.ToShortDateString() + "'),";
                                insertacvmov += "'" + acvmov.TipMov + "',";
                                insertacvmov += "'" + cuenta + "',";
                                insertacvmov += "'" + acvmov.Concepto + "',";
                                insertacvmov += "'" + refer + "',";
                                insertacvmov += "" + acvmov.Importe + ",";
                                insertacvmov += "'F',";
                                insertacvmov += "0,";
                                insertacvmov += "'',";
                                insertacvmov += "" + acvmov.TasaIva + ",";
                                insertacvmov += "0,";
                                insertacvmov += "'',";
                                insertacvmov += "'')";
                                cmd.CommandText = insertacvmov;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else {
                            return Entity.Response<bool>.CrearResponseVacio<bool>(false, "Esta poliza ya existe en el sistema viejito");
                        }    

                    }


                    cmd = null;
                    Conexion.Close();
                }
                else
                {
                    return Entity.Response<bool>.CrearResponseVacio<bool>(false, "No se pudo establecer la conexion con el servidor de contabilidad del sistema viejo");
                }
                return Entity.Response<bool>.CrearResponse<bool>(true, true);
            }
            catch (Exception ex)
            {
                return Entity.Response<bool>.CrearResponseVacio<bool>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<Entity.Contabilidad.Poliza> TraerDatosPolizaForanea(string Polizaid)
        {
            try
            {               
                Entity.Contabilidad.Poliza Poliza = new MobileBO.ControlContabilidad().TraerPolizas(Polizaid);
                if (Poliza == null)
                    Poliza = new Entity.Contabilidad.Poliza();
                return Entity.Response<Entity.Contabilidad.Poliza>.CrearResponse<Entity.Contabilidad.Poliza>(true, Poliza);
            }
            catch (Exception ex)
            {
                return Entity.Response<Entity.Contabilidad.Poliza>.CrearResponseVacio<Entity.Contabilidad.Poliza>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<Entity.HelpField.Values>> AyudaPoliza_FindByCode(string folio, string tippol, string EmpresaId, string fechapol)
        {
            MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();
            Entity.Contabilidad.Poliza poliza = new Entity.Contabilidad.Poliza();
            List<Entity.HelpField.Values> ListaElementos;
            DateTime fechapoliza;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();                
                //if (EmpresaId == string.Empty)
                //{
                //    EmpresaId = "28497FE4-D426-4585-AC9A-ED1F9D7B2CDD";
                //}

                fechapoliza = fechapol == string.Empty || fechapol == null ? DateTime.Now : Convert.ToDateTime(fechapol);
                poliza = controlContabilidad.TraerPolizaPorFolio(folio, tippol, EmpresaId, fechapoliza);

                if (poliza != null)
                {
                    Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = poliza.Polizaid, Codigo = poliza.Folio.ToString(), Descripcion = poliza.Concepto };
                    ListaElementos.Add(elemento);
                }
                else
                {
                    return Entity.Response<List<Entity.HelpField.Values>>.CrearResponseVacio<List<Entity.HelpField.Values>>(false, "No se encontró resultado.");
                }
                return Entity.Response<List<Entity.HelpField.Values>>.CrearResponse<List<Entity.HelpField.Values>>(true, ListaElementos);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.HelpField.Values>>.CrearResponseVacio<List<Entity.HelpField.Values>>(false, ex.Message);
            }

        }

        [WebMethod]
        public static Entity.Response<List<Entity.HelpField.ValuesPolizas>> AyudaPoliza_FindByPopUp(string descripcion, string tippol, string EmpresaId, string fechapol, bool Pendiente)
        {
            MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();
            Entity.ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizas;
            List<Entity.HelpField.ValuesPolizas> ListaElementos;
            DateTime fechapoliza;
            try
            {
                ListaElementos = new List<Entity.HelpField.ValuesPolizas>();
                //Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                //if (EmpresaId == string.Empty)
                //{
                //    EmpresaId = "28497FE4-D426-4585-AC9A-ED1F9D7B2CDD";
                //}

                fechapoliza = fechapol == string.Empty || fechapol == null ? DateTime.Now : Convert.ToDateTime(fechapol);
                listaPolizas = controlContabilidad.TraerPolizasPorDescripcion(descripcion, tippol, EmpresaId, fechapoliza, Pendiente);
                if (listaPolizas != null)
                {
                    foreach (Entity.Contabilidad.Poliza poliza in listaPolizas)
                    {
                        Entity.HelpField.ValuesPolizas elemento = new Entity.HelpField.ValuesPolizas() { ID = poliza.Polizaid, Fecha = poliza.Fechapol.ToString("dd/MM/yyyy"), Folio = poliza.Folio.ToString(), Tipo = poliza.TipPol, Descripcion = poliza.Concepto };
                        ListaElementos.Add(elemento);
                    }
                }
                return Entity.Response<List<Entity.HelpField.ValuesPolizas>>.CrearResponse<List<Entity.HelpField.ValuesPolizas>>(true, ListaElementos);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.HelpField.ValuesPolizas>>.CrearResponseVacio<List<Entity.HelpField.ValuesPolizas>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<Entity.HelpField.Values>> AyudaCuenta_FindByCode(string value)
        {
            MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();
            
            Entity.Contabilidad.Catcuenta cuenta = new Entity.Contabilidad.Catcuenta();
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                cuenta = controlContabilidad.TraerCatCuentasPorCuentaAfectable(values.Codigo, values.ID);

                if (cuenta != null)
                {
                    Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = cuenta.Cuentaid, Codigo = cuenta.Cuenta, Descripcion = cuenta.Descripcion };
                    ListaElementos.Add(elemento);
                }
                else
                {
                    return Entity.Response<List<Entity.HelpField.Values>>.CrearResponseVacio<List<Entity.HelpField.Values>>(false, "No se encontró resultado.");
                }
                return Entity.Response<List<Entity.HelpField.Values>>.CrearResponse<List<Entity.HelpField.Values>>(true, ListaElementos);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.HelpField.Values>>.CrearResponseVacio<List<Entity.HelpField.Values>>(false, ex.Message);
            }

        }

        [WebMethod]
        public static Entity.Response<List<Entity.HelpField.Values>> AyudaCuenta_FindByPopUp(string value)
        {
            MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();
            Entity.ListaDeEntidades<Entity.Contabilidad.Catcuenta> listaCatCuentas;
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                listaCatCuentas = controlContabilidad.TraerCatCuentasPorDescripcionAfectable(values.Descripcion, values.ID);
                if (listaCatCuentas != null)
                {
                    foreach (Entity.Contabilidad.Catcuenta catCuenta in listaCatCuentas)
                    {
                        Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = catCuenta.Cuentaid, Codigo = catCuenta.Cuenta, Descripcion = catCuenta.Descripcion };
                        ListaElementos.Add(elemento);
                    }
                }
                return Entity.Response<List<Entity.HelpField.Values>>.CrearResponse<List<Entity.HelpField.Values>>(true, ListaElementos);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.HelpField.Values>>.CrearResponseVacio<List<Entity.HelpField.Values>>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<object>> TraerTipPol()
        {
            MobileBO.ControlContabilidad controlContabilidad;
            Entity.ListaDeEntidades<Entity.Contabilidad.Acvtip> listaAcvTip;
            List<object> listaElementos = new List<object>();
            try
            {
                controlContabilidad = new MobileBO.ControlContabilidad();
                listaAcvTip = controlContabilidad.TraerAcvtip();
                if (listaAcvTip != null)
                {
                    foreach (Entity.Contabilidad.Acvtip acvtip in listaAcvTip)
                    {
                        object elemento = new { id = acvtip.TipPol, nombre = acvtip.Descripcion };
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
        public static Entity.Response<Entity.ModeloPoliza> ConsultarPoliza(string polizaid)
        {
            MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();
            Entity.Contabilidad.Poliza poliza = new Entity.Contabilidad.Poliza();
            Entity.ModeloPoliza modeloPoliza = new Entity.ModeloPoliza();
            try
            {
                poliza = controlContabilidad.TraerPolizas(polizaid);

                if (poliza != null)
                {
                    modeloPoliza = EntidadAModelo(poliza);
                }
                else
                {
                    return Entity.Response<Entity.ModeloPoliza>.CrearResponseVacio<Entity.ModeloPoliza>(false, "No se encontró resultado.");
                }

                //modeloPoliza.FacturasProveedor = MobileBO.ControlContabilidad.TraerCatfacturasproveedorPorCompraID(polizaid);
                modeloPoliza.FacturasProveedor = MobileBO.DIOT.TraerFacturasProveedoresPorPolizaID(polizaid);
                modeloPoliza.ListaPolizasNomina = MobileBO.ControlContabilidad.TraerPolizasnominaPorPolizaID(polizaid);

                foreach(Entity.Contabilidad.Polizasnomina nom in modeloPoliza.ListaPolizasNomina)
                {
                    Entity.ModeloRespuestaNomina n = new Entity.ModeloRespuestaNomina();
                    n.Polizaid = nom.Polizaid;
                    n.Polizanominaid = nom.Polizanominaid;
                    n.Emisornombre = nom.Nombreemisor;
                    n.Emisorrfc = nom.Rfcemisor;
                    n.Estatus = nom.Estatus.ToString();
                    n.Factura = nom.Folio;
                    n.Sueldo = nom.Sueldo;
                    n.Total = (nom.Sueldo + nom.Premioasistencia + nom.Premiopuntualidad + nom.Vacaciones + nom.Primavacacional + nom.Aguinaldo + nom.Gastosmedicosmayores + nom.Segurodevida + nom.Indemnizacion + nom.Primadeantiguedad + nom.Ptu) - (nom.Isrretenido + nom.Imss + nom.Infonavit + nom.Subsidioalempleo + nom.Prestamoinfonavitcf + nom.Fonacot + nom.Primaspagadaspatron + nom.Isrart174); //(nom.Sueldo + nom.Premioasistencia + nom.Premiopuntualidad) - (nom.Isrretenido + nom.Imss + nom.Infonavit);
                    n.TotalPercepciones = (nom.Sueldo + nom.Premioasistencia + nom.Premiopuntualidad + nom.Vacaciones + nom.Primavacacional + nom.Aguinaldo + nom.Gastosmedicosmayores + nom.Segurodevida + nom.Indemnizacion + nom.Primadeantiguedad + nom.Ptu);
                    n.TotalDeducciones = (nom.Isrretenido + nom.Imss + nom.Infonavit + nom.Subsidioalempleo + nom.Prestamoinfonavitcf + nom.Fonacot + nom.Primaspagadaspatron + nom.Isrart174);
                    n.UUID = nom.Uuid;
                    n.UltimaAct = nom.UltimaAct;
                    n.Subtotal = n.TotalPercepciones;
                    n.Serie = nom.Serie;
                    n.Receptornombre = nom.Nombrereceptor;
                    n.Receptorrfc = nom.Rfcreceptor;
                    n.PremioPorAsistencia = nom.Premioasistencia;
                    n.PremioPorPuntualidad = nom.Premiopuntualidad;
                    n.Vacaciones = nom.Vacaciones;
                    n.PrimaVacacional = nom.Primavacacional;
                    n.Aguinaldo = nom.Aguinaldo;
                    n.GastosMedicosMayores = nom.Gastosmedicosmayores;
                    n.SeguroDeVida = nom.Segurodevida;
                    n.Indemnizacion = nom.Indemnizacion;
                    n.PrimaDeAntiguedad = nom.Primadeantiguedad;
                    n.PTU = nom.Ptu;
                    n.SubsidioAlEmpleo = nom.Subsidioalempleo;
                    n.Imss = nom.Imss;
                    n.Infonavit = nom.Infonavit;
                    n.IsrMensual = nom.Isrretenido;
                    n.Fonacot = nom.Fonacot;
                    n.PrimasPagadasPatron = nom.Primaspagadaspatron;
                    n.IsrArt174 = nom.Isrart174;
                    n.PrestamoInfonavitCF = nom.Prestamoinfonavitcf;
                    XmlDocument xml = new XmlDocument();
                    string xmlstring = nom.Nominaxml.ToLower();
                    xml.LoadXml(xmlstring);
                    System.Xml.XmlNamespaceManager nm = new System.Xml.XmlNamespaceManager(xml.NameTable);

                    string cadenaversion = xmlstring.Substring(xmlstring.IndexOf("<cfdi:comprobante"), xmlstring.Length - xmlstring.IndexOf("<cfdi:comprobante"));
                    string version = cadenaversion.Substring(cadenaversion.IndexOf("version=\"") + 8, 4);
                    version = version.Replace("\"", "");

                    switch (version)
                    {
                        case "3.3":
                            nm.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/3");
                            break;
                        case "4.0":
                            nm.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/4");
                            break;
                    }
                    nm.AddNamespace("tfd", "http://www.sat.gob.mx/timbrefiscaldigital");
                    nm.AddNamespace("nomina12", "http://www.sat.gob.mx/nomina12");

                    if (xml.DocumentElement.SelectSingleNode("/cfdi:comprobante/cfdi:complemento/tfd:timbrefiscaldigital", nm) != null)
                    {
                        XmlNode _nodoTFD = xml.DocumentElement.SelectSingleNode("/cfdi:comprobante/cfdi:complemento/tfd:timbrefiscaldigital", nm);
                        n.Fechatimbrado = Convert.ToDateTime(_nodoTFD.Attributes["fechatimbrado"].Value);
                    }
                    modeloPoliza.ListaModeloRespuestaNomina.Add(n);
                }
                modeloPoliza.ListaModeloRespuestaNomina = modeloPoliza.ListaModeloRespuestaNomina.OrderBy(x => x.Fechatimbrado).ToList();

                return Entity.Response<Entity.ModeloPoliza>.CrearResponse<Entity.ModeloPoliza>(true, modeloPoliza);
            }
            catch (Exception ex)
            {
                return Entity.Response<Entity.ModeloPoliza>.CrearResponseVacio<Entity.ModeloPoliza>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<Entity.ModeloFacturaPoliza> ConsultarFacturasPorPoliza(string Polizaid)
        {
            try
            {
                Entity.ModeloFacturaPoliza mp = MobileBO.ControlOperacion.TraerFacturasPorPoliza(Polizaid);
                //List<Entity.Operacion.Catfacturasproveedor> lstMP = MobileBO.DIOT.TraerFacturasProveedoresPorPolizaID(Polizaid);

                return Entity.Response<Entity.ModeloFacturaPoliza>.CrearResponse<Entity.ModeloFacturaPoliza>(true, mp);
            }
            catch (Exception ex)
            {
                return Entity.Response<Entity.ModeloFacturaPoliza>.CrearResponseVacio<Entity.ModeloFacturaPoliza>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<Entity.Contabilidad.DocumentosAdicionalesPolizas>> ConsultarDocumentosPorPoliza(string Polizaid)
        {
            try
            {
                List<Entity.Contabilidad.DocumentosAdicionalesPolizas> dp = MobileBO.ControlContabilidad.ConsultarDocumentosAdicionalesPolizas(Polizaid);

                return Entity.Response<List<Entity.Contabilidad.DocumentosAdicionalesPolizas>>.CrearResponse<List<Entity.Contabilidad.DocumentosAdicionalesPolizas>>(true, dp);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.Contabilidad.DocumentosAdicionalesPolizas>>.CrearResponseVacio<List<Entity.Contabilidad.DocumentosAdicionalesPolizas>>(false, ex.Message);
            }
        }

        //[WebMethod(EnableSession = true)]
        [WebMethod]
        public static Entity.Response<Entity.ModeloPoliza> Guardar(string value)
        {
            MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();
            Entity.Contabilidad.Poliza poliza = new Entity.Contabilidad.Poliza();
            Entity.ModeloPoliza modeloPoliza;
            //Entity.Operacion.Catempresasbanco

            Entity.ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizas;

            try
            {
                listaPolizas = new Entity.ListaDeEntidades<Entity.Contabilidad.Poliza>();
                modeloPoliza = MobileBO.Utilerias.Deserializar<Entity.ModeloPoliza>(value);
                poliza = ModeloAEntidad(modeloPoliza);

                Entity.Contabilidad.Cierrecontabilidad cierre = MobileBO.ControlContabilidad.TraerCierrecontabilidad(poliza.EmpresaId);
                if (cierre != null)
                {
                    DateTime fechaPol = poliza.Fechapol;
                    if (fechaPol <= cierre.Fechacierre)
                    {
                        throw new Exception("Contabilidad cerrada al " + cierre.Fechacierre.ToShortDateString());
                    }
                }

                if (poliza.Polizaid != string.Empty && poliza.Pendiente == true && poliza.Polizaid != Guid.Empty.ToString())
                {
                    //Verificamos si esa poliza ya estaba guardada en el sistema como pendiente anteriormente y la cancelamos
                    Entity.Contabilidad.Poliza polizaAnt = new MobileBO.ControlContabilidad().TraerPolizas(poliza.Polizaid);
                    if (!polizaAnt.Pendiente)
                    {
                        //Si la poliza anterior no estaba pendiente
                        int estatusaux = poliza.Estatus;
                        poliza.Estatus = 2;
                        Entity.ListaDeEntidades<Entity.Contabilidad.Poliza> lPolizaCancelar = new Entity.ListaDeEntidades<Entity.Contabilidad.Poliza>();
                        lPolizaCancelar.Add(poliza);
                        controlContabilidad.GuardarPoliza(lPolizaCancelar);
                        poliza = lPolizaCancelar[0];
                        poliza.Estatus = estatusaux;
                    }
                }

                if (poliza.Polizaid != string.Empty && poliza.Pendiente != true && poliza.Polizaid != Guid.Empty.ToString())
                {
                    //Verificamos si esa poliza ya estaba guardada en el sistema como pendiente anteriormente y la cancelamos
                    Entity.Contabilidad.Poliza polizaAnt = new MobileBO.ControlContabilidad().TraerPolizas(poliza.Polizaid);
                    if (polizaAnt.Pendiente)
                    {
                        //Si la poliza anterior no estaba pendiente
                        int estatusaux = poliza.Estatus;
                        poliza.Estatus = 2;
                        Entity.ListaDeEntidades<Entity.Contabilidad.Poliza> lPolizaCancelar = new Entity.ListaDeEntidades<Entity.Contabilidad.Poliza>();
                        lPolizaCancelar.Add(poliza);
                        controlContabilidad.GuardarPoliza(lPolizaCancelar);
                        poliza = lPolizaCancelar[0];
                        poliza.Estatus = estatusaux;
                    }
                }
                /*Este candado valida que para poder guardar una poliza la contabilidad debe estar FISCAS(que las polizas con CERO esten en )
                if (modeloPoliza.Polizaid == string.Empty && modeloPoliza.Pendiente == true)
                {
                    bool puedeProcesar = controlContabilidad.PuedeProcesar(Convert.ToDateTime(modeloPoliza.Fechapol).Year, modeloPoliza.EmpresaId);
                    if (!puedeProcesar)
                    {
                        return Entity.Response<Entity.ModeloPoliza>.CrearResponseVacio<Entity.ModeloPoliza>(false, "Favor de comunicarse con el área de sistemas.");
                    }
                }
                */



                listaPolizas.Add(poliza);
                controlContabilidad.GuardarPoliza(listaPolizas);
                Entity.ModeloPoliza resultado = EntidadAModelo(poliza);

                //Codigo para guardar los xmls de las compras
                try
                {
                    //List<Entity.Contabilidad.Catfacturasproveedor> FacturasEliminar = MobileBO.ControlContabilidad.TraerCatfacturasproveedorPorCompraID(poliza.Polizaid);
                    //foreach (Entity.Operacion.Catfacturasproveedor fact in FacturasEliminar)
                    //{
                    //    Entity.Contabilidad.Catproveedor proveedor = MobileBO.ControlContabilidad.TraerCatproveedores(fact.Proveedorid, null, fact.Empresaid);
                    //    string Location = pathXML(proveedor.Rfc, fact.Rutaxml, fact.Empresaid, poliza.Fechapol);
                    //    if (File.Exists(Location))
                    //        File.Delete(Location);
                    //}

                    List<string> lstArchivos;
                    Entity.ModeloDIOT modeloDIOT;
                    Entity.ModeloPolizaDIOT polizaDIOT;
                    List<Entity.Operacion.Catfacturasproveedor> lstFacturas;

                    polizaDIOT = new Entity.ModeloPolizaDIOT();
                    polizaDIOT.Fec_Pol = poliza.Fechapol;
                    polizaDIOT.Importe = poliza.Importe;
                    polizaDIOT.Num_Pol = poliza.Folio;
                    polizaDIOT.Polizaid = poliza.Polizaid;
                    polizaDIOT.Tip_Pol = poliza.TipPol;

                    foreach (Entity.Operacion.Catfacturasproveedor factura in modeloPoliza.FacturasProveedor)
                    {
                        lstArchivos = new List<string>();
                        Entity.Contabilidad.Catproveedor proveedor = MobileBO.ControlContabilidad.TraerCatproveedores(factura.Proveedorid, null, factura.Empresaid);
                        string Location = pathXML(proveedor.Rfc, factura.NomArchivo, factura.Empresaid, poliza.Fechapol);
                        lstArchivos.Add(Location);

                        modeloDIOT = new Entity.ModeloDIOT();
                        modeloDIOT.Cod_Empresa = factura.CodEmpresa;// "0";
                        modeloDIOT.Cod_Proveedor = factura.CodProveedor;// "0";
                        modeloDIOT.Factura = factura.Factura;
                        modeloDIOT.Empresaid = poliza.EmpresaId;
                        modeloDIOT.Proveedorid = proveedor.Proveedorid;
                        modeloDIOT.usuario = poliza.Usuario;
                        modeloDIOT.listaArchivos = lstArchivos;
                        modeloDIOT.ultimaAct = factura.UltimaAct;
                        modeloDIOT.Facturaproveedorid = factura.Facturaproveedorid;

                        lstFacturas = MobileBO.DIOT.ProcesarXMLFactura(modeloDIOT, polizaDIOT);
                        resultado.FacturasProveedor.AddRange(lstFacturas);
                    }
                    //MobileBO.ControlContabilidad.Catfacturasproveedor_Delete(poliza.Polizaid);
                    //foreach (Entity.Operacion.Catfacturasproveedor factura in modeloPoliza.FacturasProveedor)
                    //{
                    //    Entity.Contabilidad.Catproveedor proveedor = MobileBO.ControlContabilidad.TraerCatproveedores(factura.Proveedorid, null, factura.Empresaid);
                    //    string Location = pathXML(proveedor.Rfc, factura.NomArchivo, factura.Empresaid, poliza.Fechapol);
                    //    if (File.Exists(Location))
                    //        factura.Xml = GetXmlString(Location);
                    //    factura.Fecha = poliza.Fechapol;
                    //    //factura.Facturaproveedorid = (factura.Facturaproveedorid == null || factura.Facturaproveedorid == string.Empty || factura.Facturaproveedorid == "null" ? Guid.Empty.ToString() : factura.Facturaproveedorid);
                    //    //factura.Compraid = poliza.Polizaid;
                    //}
                    //Guardamos la factura del proveedor
                    //MobileBO.ControlContabilidad.GuardarCatfacturasproveedor(modeloPoliza.FacturasProveedor);
                }
                catch(Exception ex)
                {
                    Console.Write(ex.Message);
                }

                //Código para guardar los xml's de Nómina
                try
                {
                    foreach(Entity.Contabilidad.Polizasnomina nom in modeloPoliza.ListaPolizasNomina)
                    {
                        nom.Polizanominaid = (nom.Polizanominaid == null || nom.Polizanominaid.Trim() == "null" || nom.Polizanominaid.Trim() == "") ? null : nom.Polizanominaid;
                        Entity.Contabilidad.Polizasnomina nomBD = MobileBO.ControlContabilidad.TraerPolizasnomina(nom.Polizanominaid);

                        if(nomBD == null)
                        {
                            //string rutaArchivo = pathXMLNomina(nom.NombreArchivo, poliza.EmpresaId, DateTime.Now, nom.Rfcreceptor);
                            string rutaArchivo = pathXMLNomina(nom.NombreArchivo, poliza.EmpresaId, poliza.Fechapol, nom.Rfcreceptor);
                            if (rutaArchivo != null && (nom.Polizanominaid == null || nom.Polizanominaid == Guid.Empty.ToString()))
                            {
                                StreamReader stream = new StreamReader(rutaArchivo);
                                nom.Nominaxml = stream.ReadToEnd();
                                nom.Polizanominaid = Guid.Empty.ToString();
                                nom.Serie = (nom.Serie == "null" ? null : nom.Serie);
                                nom.Polizaid = poliza.Polizaid;
                                nom.Fecha = DateTime.Now;
                                nom.Estatus = 1;
                                nom.Usuario = poliza.Usuario;
                                nom.Empresaid = poliza.EmpresaId;

                            }
                            MobileBO.ControlContabilidad.GuardarPolizasnomina(new List<Entity.Contabilidad.Polizasnomina> { nom });
                        }                        
                    }
                }
                catch (Exception ex)
                {
                    return Entity.Response<Entity.ModeloPoliza>.CrearResponseVacio<Entity.ModeloPoliza>(false, ex.Message);
                }
                return Entity.Response<Entity.ModeloPoliza>.CrearResponse<Entity.ModeloPoliza>(true, resultado);
            }
            catch (Exception ex)
            {
                return Entity.Response<Entity.ModeloPoliza>.CrearResponseVacio<Entity.ModeloPoliza>(false, ex.Message);
            }
        }

        private static Entity.Contabilidad.Poliza ModeloAEntidad(Entity.ModeloPoliza modeloPoliza)
        {
            Entity.Contabilidad.Poliza poliza = new Entity.Contabilidad.Poliza();
            //decimal importePoliza = 0;

            poliza.Polizaid = modeloPoliza.Polizaid == string.Empty ? Guid.Empty.ToString() : modeloPoliza.Polizaid;
            poliza.EmpresaId = modeloPoliza.EmpresaId;
            poliza.Folio = modeloPoliza.Folio;
            poliza.TipPol = modeloPoliza.TipPol;
            poliza.Fechapol = modeloPoliza.Fechapol == string.Empty || modeloPoliza.Fechapol == null ? DateTime.Now : Convert.ToDateTime(modeloPoliza.Fechapol);
            poliza.Concepto = modeloPoliza.Concepto;
            poliza.Importe = modeloPoliza.Importe;
            poliza.Estatus = modeloPoliza.Estatus;
            poliza.Fecha = modeloPoliza.Fecha == string.Empty || modeloPoliza.Fecha == null ? DateTime.Now : Convert.ToDateTime(modeloPoliza.Fecha);
            poliza.Usuario = modeloPoliza.Usuario == string.Empty ? "sistema" : modeloPoliza.Usuario;
            poliza.UltimaAct = modeloPoliza.UltimaAct;
            poliza.Pendiente = modeloPoliza.Pendiente;
            poliza.Pagoprogramado = modeloPoliza.Pagoprogramado;

            // Llenar detalle
            foreach (Entity.ModeloPolizaDetalle modeloDetalle in modeloPoliza.ListaPolizaDetalle)
            {
                Entity.Contabilidad.Polizasdetalle polizadetalle = new Entity.Contabilidad.Polizasdetalle();

                polizadetalle.Polizadetalleid = modeloDetalle.Polizadetalleid == string.Empty ? Guid.Empty.ToString() : modeloDetalle.Polizadetalleid;
                polizadetalle.Polizaid = poliza.Polizaid;
                polizadetalle.Cuentaid = modeloDetalle.Cuentaid;
                polizadetalle.TipMov = modeloDetalle.TipMov;
                polizadetalle.Concepto = modeloDetalle.Concepto;
                polizadetalle.Cantidad = modeloDetalle.Cantidad;
                polizadetalle.Importe = modeloDetalle.Importe;
                polizadetalle.Usuario = poliza.Usuario;
                polizadetalle.Estatus = modeloDetalle.Estatus;
                polizadetalle.Fecha = modeloDetalle.Fecha == string.Empty || modeloDetalle.Fecha == null ? DateTime.Now : Convert.ToDateTime(modeloDetalle.Fecha);
                polizadetalle.UltimaAct = modeloDetalle.UltimaAct;
                polizadetalle.PresupuestodetalleId = modeloDetalle.PresupuestodetalleId == string.Empty ? Guid.Empty.ToString() : modeloDetalle.PresupuestodetalleId;
                polizadetalle.Inventariocostoid = modeloDetalle.Inventariocostoid == string.Empty ? Guid.Empty.ToString() : modeloDetalle.Inventariocostoid;

                //importePoliza += polizadetalle.Importe;

                poliza.ListaPolizaDetalle.Add(polizadetalle);
            }

            //poliza.Importe = importePoliza;

            return poliza;
        }

        private static Entity.ModeloPoliza EntidadAModelo(Entity.Contabilidad.Poliza poliza)
        {
            MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();
            Entity.Contabilidad.Catcuenta cuenta;
            Entity.ModeloPoliza modeloPoliza = new Entity.ModeloPoliza();
            //decimal importePoliza = 0;

            modeloPoliza.Polizaid = poliza.Polizaid;
            modeloPoliza.EmpresaId = poliza.EmpresaId;
            modeloPoliza.Folio = poliza.Folio;
            modeloPoliza.TipPol = poliza.TipPol;
            modeloPoliza.Fechapol = poliza.Fechapol.ToString("dd/MM/yyyy");
            modeloPoliza.Concepto = poliza.Concepto;
            modeloPoliza.Importe = poliza.Importe;
            modeloPoliza.Estatus = poliza.Estatus;
            modeloPoliza.Fecha = poliza.Fecha.ToString();
            modeloPoliza.Usuario = poliza.Usuario;
            modeloPoliza.UltimaAct = poliza.UltimaAct;
            modeloPoliza.Pendiente = poliza.Pendiente;
            modeloPoliza.Pagoprogramado = poliza.Pagoprogramado;

            foreach (Entity.Contabilidad.Polizasdetalle polizadetalle in poliza.ListaPolizaDetalle)
            {
                Entity.ModeloPolizaDetalle modeloPolizaDetalle = new Entity.ModeloPolizaDetalle();
                modeloPolizaDetalle.Polizadetalleid = polizadetalle.Polizadetalleid;
                modeloPolizaDetalle.Polizaid = polizadetalle.Polizaid;
                modeloPolizaDetalle.Cuentaid = polizadetalle.Cuentaid;
                modeloPolizaDetalle.TipMov = polizadetalle.TipMov;
                modeloPolizaDetalle.Concepto = polizadetalle.Concepto;
                modeloPolizaDetalle.Cantidad = polizadetalle.Cantidad;
                modeloPolizaDetalle.Importe = polizadetalle.Importe;
                modeloPolizaDetalle.Usuario = polizadetalle.Usuario;
                modeloPolizaDetalle.Estatus = polizadetalle.Estatus;
                modeloPolizaDetalle.Fecha = polizadetalle.Fecha.ToString();
                modeloPolizaDetalle.UltimaAct = polizadetalle.UltimaAct;
                modeloPolizaDetalle.PresupuestodetalleId = polizadetalle.PresupuestodetalleId;
                modeloPolizaDetalle.Inventariocostoid = polizadetalle.Inventariocostoid;

                cuenta = controlContabilidad.TraerCatCuentas(polizadetalle.Cuentaid);
                if (cuenta != null)
                {
                    modeloPolizaDetalle.Cuenta = cuenta.Cuenta;
                    modeloPolizaDetalle.CuentaDesc = cuenta.Descripcion;
                }

                //importePoliza += modeloPolizaDetalle.Importe;

                modeloPoliza.ListaPolizaDetalle.Add(modeloPolizaDetalle);
            }

            //modeloPoliza.Importe = importePoliza;

            return modeloPoliza;
        }

        #region Facturas de compras
        [WebMethod]
        public static Entity.Response<object> ExisteUUID(string uuid)
        {
            try
            {
                Entity.Contabilidad.Catfacturasproveedor factura = MobileBO.ControlContabilidad.TraerCatfacturasproveedor(null, uuid);
                if (factura != null)
                    return Entity.Response<object>.CrearResponse<object>(true, new { Existe = true });
                return Entity.Response<object>.CrearResponse<object>(true, new { Existe = false });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }
        [WebMethod]
        public static Entity.Response<object> ExisteUUIDNomina(string uuid)
        {
            try
            {
                Entity.Contabilidad.Polizasnomina nomina = MobileBO.ControlContabilidad.TraerPolizasnominaPorUUID(uuid);
                if (nomina != null)
                    return Entity.Response<object>.CrearResponse<object>(true, new { Existe = true });
                return Entity.Response<object>.CrearResponse<object>(true, new { Existe = false });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }
        [WebMethod]
        public static Entity.Response<object> ontenerRFCPorEmpresa(string empresaid)
        {
            try
            {
                Entity.Configuracion.Catempresa empresa = MobileBO.ControlConfiguracion.TraerCatempresas(empresaid);
                if (empresa != null)
                {
                    return Entity.Response<object>.CrearResponse<object>(true, new { Existe = true, RFC = empresa.Rfc.ToUpper() });
                }
                return Entity.Response<object>.CrearResponse<object>(true, new { Existe = false });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<Entity.Contabilidad.Catfacturasproveedor>> ProcesarXmlFacturas(string value,string fecha)
        {
            try
            {
                List<Entity.Contabilidad.Catfacturasproveedor> FacturasProveedor = new List<Entity.Contabilidad.Catfacturasproveedor>();
                List<ModeloXmlFacProv> Xml = MobileBO.Utilerias.Deserializar<List<ModeloXmlFacProv>>(value);

                if (Xml.Count == 0)
                    return Entity.Response<List<Entity.Contabilidad.Catfacturasproveedor>>.CrearResponseVacio<List<Entity.Contabilidad.Catfacturasproveedor>>(false, "Error al procesar los archivos, lista de xml vacia");

                foreach (ModeloXmlFacProv xml in Xml)
                {
                    string Location = pathXML(xml.RFC, xml.File, xml.EmpresaID, DateTime.Parse(fecha));
                    if (!File.Exists(Location))
                        return Entity.Response<List<Entity.Contabilidad.Catfacturasproveedor>>.CrearResponseVacio<List<Entity.Contabilidad.Catfacturasproveedor>>(false, "Error al obtener el archivo xml de la factura: " + xml.File);
                    Entity.Contabilidad.Catfacturasproveedor Factura = ProcesarXML(Location);

                    Entity.Contabilidad.Catproveedor proveedor = MobileBO.ControlContabilidad.TraerCatproveedores(null, xml.RFC, xml.EmpresaID);
                    Factura.Proveedorid = proveedor.Proveedorid;
                    Factura.Rutaxml = xml.File;
                    FacturasProveedor.Add(Factura);
                }
                return Entity.Response<List<Entity.Contabilidad.Catfacturasproveedor>>.CrearResponse<List<Entity.Contabilidad.Catfacturasproveedor>>(true, FacturasProveedor);
            }
            catch (Exception ex)
            {
                return Entity.Response<List<Entity.Contabilidad.Catfacturasproveedor>>.CrearResponseVacio<List<Entity.Contabilidad.Catfacturasproveedor>>(false, ex.Message);
            }
        }

        public static string pathXML(string rfc, string File,string EmpresaID,DateTime Fecha)
        {
            Entity.Contabilidad.Catproveedor proveedor = MobileBO.ControlContabilidad.TraerCatproveedores(null, rfc, EmpresaID);
            if (proveedor == null)
                throw new Exception("Error al obtener el proveedor de la factura: " + File);
            Entity.Configuracion.Catempresa Empresa = MobileBO.ControlConfiguracion.TraerCatempresas(proveedor.Empresaid);
            string directorio = System.AppDomain.CurrentDomain.RelativeSearchPath;
            directorio += "\\Proveedores\\" + (Empresa.Empresa == 1 ? "Factur" : "Balor") + " \\" + Fecha.Year.ToString() + "\\" + Fecha.Month + "\\" + proveedor.Rfc;
            string Location = directorio + "\\" + File;
            return Location;
        }

        public static string pathXMLNomina(string File, string EmpresaID, DateTime Fecha, string rfcReceptor)
        {
            
            Entity.Configuracion.Catempresa Empresa = MobileBO.ControlConfiguracion.TraerCatempresas(EmpresaID);
            string directorio = System.AppDomain.CurrentDomain.RelativeSearchPath;
            string nombreCarpetaEmpresa = "";

            directorio += "\\XmlNomina\\" + (Empresa.Empresa == 1 ? "Factur" : "Balor") + " \\" + Fecha.Year.ToString() + "\\" + Fecha.Month + "\\" + rfcReceptor;
            string Location = directorio + "\\" + File;
            return Location;
        }
        public static string GetXmlString(string Ruta)
        {
            // Read the file and display it line by line.
            string xml = "";
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(Ruta);
            while ((line = file.ReadLine()) != null)
            {
                xml += line;
            }
            file.Close();

            if (xml.Substring(0, 1) == "?")
                xml = xml.Substring(1, xml.Length - 1);

            xml = xml.Replace("ꨩ", "").Replace("�", "");
            return xml;
        }
        public static Entity.Contabilidad.Catfacturasproveedor ProcesarXML(string Ruta)
        {
            string[] total = new string[0];
            Entity.Contabilidad.Catfacturasproveedor Factura = new Entity.Contabilidad.Catfacturasproveedor();
            Factura.Facturaproveedorid = Guid.Empty.ToString();
            Factura.Estatus = 1;
            //Factura.Cuentagasto = "";


            // Read the file and display it line by line.
            string xml = "";
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(Ruta);
            while ((line = file.ReadLine()) != null)
            {
                xml += line;
            }
            file.Close();

            if (xml.Substring(0, 1) == "?")
                xml = xml.Substring(1, xml.Length - 1);

            xml = xml.Replace("ꨩ", "").Replace("�", "");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(xml);
            using (System.IO.StreamWriter outfile = new System.IO.StreamWriter(Ruta))
            {
                outfile.Write(sb.ToString());
            }
            using (XmlReader reader = XmlReader.Create(Ruta))
            {
                while (reader.Read())
                {
                    if (reader.Name.ToUpper().Trim().Contains("COMPROBANTE") && reader.AttributeCount > 0)
                    {
                        Factura.Total = decimal.Parse(reader.GetAttribute("total").ToString());
                        Factura.Subtotal = decimal.Parse(reader.GetAttribute("subtotal").ToString());
                        if (reader.GetAttribute("folio") != null)
                            Factura.Factura = reader.GetAttribute("folio").ToString().Trim();
                        else
                            Factura.Factura = "";

                        if (reader.GetAttribute("serie") != null)
                            Factura.Factura += "-" + reader.GetAttribute("serie").ToString().Trim().ToUpper();
                    }
                    if (reader.Name.ToUpper().Trim().Contains("TIMBREFISCAL") && reader.AttributeCount > 0)
                    {
                        Factura.Uuid = reader.GetAttribute("uuid").ToString().Trim().ToUpper();
                        Factura.Fechatimbre = DateTime.Parse(reader.GetAttribute("fechatimbrado").ToString().Trim()).ToShortDateString();
                    }

                    if (reader.Name.ToUpper().Trim().Contains("TRASLADO") && reader.AttributeCount > 0)
                    {
                        if (reader.GetAttribute("impuesto") != null)
                        {
                            if (reader.GetAttribute("impuesto").ToString().Trim().ToUpper() == "IVA" || reader.GetAttribute("impuesto").ToString().Trim().ToUpper() == "002")
                                Factura.Iva += decimal.Parse(reader.GetAttribute("importe").ToString());
                        }
                    }

                    if (reader.Name.ToUpper().Trim().Contains("RETENCION") && reader.AttributeCount > 0)
                    {
                        if (reader.GetAttribute("impuesto") != null)
                        {
                            if (reader.GetAttribute("impuesto").ToString().Trim().ToUpper() == "ISR" || reader.GetAttribute("impuesto").ToString().Trim().ToUpper() == "001")
                                Factura.RetIsr += decimal.Parse(reader.GetAttribute("importe").ToString());

                            if (reader.GetAttribute("impuesto").ToString().Trim().ToUpper() == "IVA" || reader.GetAttribute("impuesto").ToString().Trim().ToUpper() == "002")
                                Factura.RetIva += decimal.Parse(reader.GetAttribute("importe").ToString());
                        }
                    }
                }
                reader.Close();
            }
            return Factura;

        }


        [WebMethod]
        public static Entity.Response<object> EliminarFacturaProveedor(int FacturaProveedorid, string usuario)
        {
            try
            {
                Entity.Operacion.Catfacturasproveedor factura = MobileBO.DIOT.TraerCatFacturaProveedor(FacturaProveedorid);
                List<Entity.Operacion.Catfacturasproveedor> lstFacturas = new List<Entity.Operacion.Catfacturasproveedor>();
                List<Entity.Operacion.Catfacturasproveedorespoliza> lstFacPolizas;
                if (factura != null)
                {
                    factura.Estatus = 2;
                    factura.Fecha = DateTime.Today;
                    factura.Usuario = usuario;
                    lstFacturas.Add(factura);
                    MobileBO.DIOT.GuardarFacturasProveedores(ref lstFacturas);

                    lstFacPolizas = MobileBO.DIOT.TraerCatfacturasproveedorespolizasPorFacturaProveedorID(FacturaProveedorid);

                    foreach (Entity.Operacion.Catfacturasproveedorespoliza p in lstFacPolizas)
                    {
                        p.Estatus = 2;
                        p.Fecha = DateTime.Today;
                        p.Usuario = usuario;
                    }
                    MobileBO.DIOT.GuardarFacturasProveedoresPolizas(ref lstFacPolizas);

                }
                else
                    throw new Exception("Factura no encontrada...");

                


                return Entity.Response<object>.CrearResponse<object>(true, new { Existe = false });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<object> EliminarFacturasProveedores(string polizaid, string usuario)
        {
            List<Entity.Operacion.Catfacturasproveedor> lstFacturas = new List<Entity.Operacion.Catfacturasproveedor>();
            List<Entity.Operacion.Catfacturasproveedorespoliza> lstFacPolizas = new List<Entity.Operacion.Catfacturasproveedorespoliza>();

            try
            {
                List<Entity.Operacion.Catfacturasproveedor> lstFacturasEliminar = MobileBO.DIOT.TraerFacturasProveedoresPorPolizaID(polizaid);

                if (lstFacturasEliminar != null && lstFacturasEliminar.Count > 0)
                {
                    foreach (var factura in lstFacturasEliminar)
                    {
                        factura.Estatus = 2;
                        factura.Fecha = DateTime.Today;
                        factura.Usuario = usuario;
                        lstFacturas.Add(factura);

                        var relaciones = MobileBO.DIOT.TraerCatfacturasproveedorespolizasPorFacturaProveedorID(factura.Facturaproveedorid);
                        if (relaciones != null && relaciones.Count > 0)
                        {
                            foreach (var rel in relaciones)
                            {
                                rel.Estatus = 2;
                                rel.Fecha = DateTime.Today;
                                rel.Usuario = usuario;
                                lstFacPolizas.Add(rel);
                            }
                        }
                    }

                    if (lstFacturas.Count > 0)
                        MobileBO.DIOT.GuardarFacturasProveedores(ref lstFacturas);

                    if (lstFacPolizas.Count > 0)
                        MobileBO.DIOT.GuardarFacturasProveedoresPolizas(ref lstFacPolizas);
                }

                return Entity.Response<object>.CrearResponse<object>(true, new { Eliminadas = lstFacturas.Count });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }


        [WebMethod]
        public static Entity.Response<object> EliminarPolizaNomina(string polizanominaid, string usuario)
        {
            try
            {
                polizanominaid = (polizanominaid == null || polizanominaid == "null" || polizanominaid.Trim() == "") ? null : polizanominaid;
                Entity.Contabilidad.Polizasnomina nom = MobileBO.ControlContabilidad.TraerPolizasnomina(polizanominaid);
                if (nom != null)
                {
                    nom.Estatus = 2;
                    nom.Fecha = DateTime.Today;
                    nom.Usuario = usuario;
                    MobileBO.ControlContabilidad.GuardarPolizasnomina(new List<Entity.Contabilidad.Polizasnomina> { nom });
                }               

                return Entity.Response<object>.CrearResponse<object>(true, new { Existe = false });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

        #endregion


        [WebMethod]
        public static Entity.Response<Entity.Contabilidad.Programacionpago> ConsultaPagoProgramado(string polizaid)
        {
            try
            {
                MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();
                Entity.Contabilidad.Poliza poliza = new Entity.Contabilidad.Poliza();
                poliza = controlContabilidad.TraerPolizas(polizaid);

                Entity.Contabilidad.Programacionpago pago = MobileBO.ControlContabilidad.TraerProgramacionpagosPorPolizaID(poliza.Polizaid);


                return Entity.Response<Entity.Contabilidad.Programacionpago>.CrearResponse<Entity.Contabilidad.Programacionpago>(true, pago);
            }
            catch (Exception ex)
            {
                return Entity.Response<Entity.Contabilidad.Programacionpago>.CrearResponseVacio<Entity.Contabilidad.Programacionpago>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<List<object>> traerBancosEmpresas(string empresaid)
        {
            List<object> _datos = new List<object>();
            List<Entity.Operacion.Catempresasbanco> _cuentas;
            try
            {
                _cuentas = MobileBO.ControlOperacion.TraerCatempresasbancosPorEmpresa(empresaid);
                foreach(Entity.Operacion.Catempresasbanco _cuenta in _cuentas)
                {
                    DataSet ds = MobileBO.ControlOperacion.Catbancoscontable_Select(_cuenta.Bancoid);
                    string nombreBanco = string.Empty;
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        nombreBanco = ds.Tables[0].Rows[0]["Nombre"].ToString();
                    }
                    var o = new
                    {

                        BanatBancosContableoid = _cuenta.Bancoid,
                        Nombre = nombreBanco,
                        Consecutivo = _cuenta.Consecutivo,
                        CtaCheq = _cuenta.CtaCheq,
                        Cuenta = _cuenta.Cuenta,
                        Empresabancoid = _cuenta.Empresabancoid,
                        Empresaid = _cuenta.Empresaid,
                        Estatus = _cuenta.Estatus,
                        Moneda = _cuenta.Moneda,
                        NumCheq = _cuenta.NumCheq ,
                        UltimaAct = _cuenta.UltimaAct ,
                        Usuario = _cuenta.Usuario,
                        Fecha = string.Format("yyyyMMdd hh:MM:ss", _cuenta.Fecha) 
                    };
                    _datos.Add(o);
                }
                return Entity.Response<List<object>>.CrearResponse<List<object>>(true, _datos);
            }
            catch(Exception ex)
            {
                return Entity.Response<List<object>>.CrearResponseVacio<List<object>>(false, ex.Message);
            }
        }
        [WebMethod]
        public static Entity.Response<object> ConsultarCatDocumentosPolizas()
        {
            try
            {
                List<object> _datos = new List<object>();
                DataSet ds = MobileBO.ControlContabilidad.ConsultarCatDocumentosPolizas();
                if (ds.Tables[0].Rows.Count > 0) {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        int DocumentoID = Convert.ToInt32(ds.Tables[0].Rows[i]["DocumentoID"]);
                        string NombreDocumento = Convert.ToString(ds.Tables[0].Rows[i]["NombreDocumento"]);

                        var o = new
                        {
                            DocumentoID = DocumentoID,
                            NombreDocumento = NombreDocumento
                        };

                        _datos.Add(o);
                    }
                }
                return Entity.Response<object>.CrearResponse<object>(true, _datos);
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }
        
        [WebMethod]
        public static Entity.Response<Entity.ModeloPoliza> GuardarCopiaPolizaFecha(string value,string othersvalue)
        {

            MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();
            Entity.Contabilidad.Poliza poliza = new Entity.Contabilidad.Poliza();
            Entity.ModeloPoliza modeloPoliza;
            //Entity.Operacion.Catempresasbanco
            Entity.Contabilidad.Acvgralpdte acvgralpdte = null;
            Entity.Contabilidad.Acvgral acvgral = null;

            Entity.ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizas;

            try
            {
                listaPolizas = new Entity.ListaDeEntidades<Entity.Contabilidad.Poliza>();
                modeloPoliza = MobileBO.Utilerias.Deserializar<Entity.ModeloPoliza>(value);
                var serializer = new JavaScriptSerializer();
                OtrosValues otrosValuesObj = serializer.Deserialize<OtrosValues>(othersvalue);
                poliza = ModeloAEntidad(modeloPoliza);

                Entity.Contabilidad.Cierrecontabilidad cierre = MobileBO.ControlContabilidad.TraerCierrecontabilidad(poliza.EmpresaId);
                if (cierre != null)
                {
                    DateTime fechaPol = poliza.Fechapol;
                    if (fechaPol <= cierre.Fechacierre)
                    {
                        throw new Exception("Contabilidad cerrada al " + cierre.Fechacierre.ToShortDateString());
                    }
                }

                poliza.Folio = poliza.Folio.TrimStart();

                bool _modoFiscal = MobileBO.ControlContabilidad.ValidaCuentasFiscales(poliza.EmpresaId, Convert.ToDateTime(poliza.Fechapol).Year);
                if (_modoFiscal)
                {
                    if (poliza.Folio.Substring(0, 1) == "0")
                    {
                        poliza.Folio = poliza.Folio.Substring(1, (poliza.Folio.Length - 1));
                    }
                    //poliza.Pendiente = false;
                }
                else
                {
                    ListaDeEntidades<Entity.Contabilidad.Poliza> polizag = controlContabilidad.TraerPolizasGenerales(poliza.TipPol, poliza.EmpresaId, DateTime.Parse(otrosValuesObj.FechapolOt), poliza.Folio, poliza.Pendiente);
                    if (polizag != null && polizag.Count > 0)
                    {
                        acvgral = MobileBO.ControlContabilidad.TraerAcvgralPorReferenciaId(polizag[0].Polizaid);
                    }
                    ListaDeEntidades<Entity.Contabilidad.Poliza> polizagp = controlContabilidad.TraerPolizasGenerales(poliza.TipPol, poliza.EmpresaId, DateTime.Parse(otrosValuesObj.FechapolOt), (poliza.Folio.Substring(0, 1) != "0" ? "0" + poliza.Folio : poliza.Folio.Substring(1, (poliza.Folio.Length - 1))), !poliza.Pendiente);
                    if (polizagp != null && polizagp.Count > 0)
                    {
                        acvgralpdte = MobileBO.ControlContabilidad.TraerAcvgralpdtePorReferenciaId(polizagp[0].Polizaid);
                    }

                    if (acvgralpdte != null && acvgral != null)
                    {
                        if (poliza.Folio.Substring(0, 1) != "0")
                        {
                            poliza.Folio = "0" + poliza.Folio;
                        }
                    }else if (poliza.Folio.Substring(0, 1) == "0")
                    {
                        poliza.Folio = poliza.Folio.Substring(1, (poliza.Folio.Length - 1));
                    }
                    //poliza.Pendiente = true;
                }


                listaPolizas.Add(poliza);
                controlContabilidad.GuardarPolizaC(listaPolizas); 
                Entity.ModeloPoliza resultado = EntidadAModelo(poliza);

                
                string foliopolOt = otrosValuesObj.foliopolOt;
                string tipPolOt = otrosValuesObj.TipPolOt;
                DateTime fechapolOt = DateTime.Parse(otrosValuesObj.FechapolOt);
                bool checkgrab = otrosValuesObj.checkgrab;
                ListaDeEntidades <Entity.Contabilidad.Poliza> otpoliza = controlContabilidad.TraerPolizasGenerales(tipPolOt, poliza.EmpresaId, fechapolOt, foliopolOt, !checkgrab);
                if(otpoliza.Count > 0)
                {
                    Entity.ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizasAux;
                    ListaDeEntidades<Entity.Contabilidad.Polizasdetalle> listdetalles = controlContabilidad.TraerPolizaDetalles(otpoliza[0].Polizaid);
                    listaPolizasAux = otpoliza;

                    poliza.Folio = poliza.Folio.TrimStart();

                    if (listaPolizasAux[0].EmpresaId.ToUpper() != "A7D3E5A4-6508-483B-8A3D-0E379FF06755") { 
                    
                        listaPolizasAux[0].Concepto = poliza.Concepto;
                        listaPolizasAux[0].Polizaid= "00000000-0000-0000-0000-000000000000";
                        listaPolizasAux[0].Folio = poliza.Folio.StartsWith("0") ? poliza.Folio.Substring(1) : "0" + poliza.Folio;
                        listaPolizasAux[0].Fechapol = poliza.Fechapol;
                        listaPolizasAux[0].Usuario = poliza.Usuario;
                        listaPolizasAux[0].Fecha = DateTime.Now;
                        listaPolizasAux[0].Estatus = 1;
                        foreach (Entity.Contabilidad.Polizasdetalle p in listdetalles)
                        {
                            p.Polizaid = "00000000-0000-0000-0000-000000000000";
                            p.Polizadetalleid = "00000000-0000-0000-0000-000000000000";
                            p.Estatus = 1;
                            p.Concepto = poliza.ListaPolizaDetalle[0].Concepto;
                            p.Fecha = DateTime.Now;
                        }
                    }
                    else
                    {
                        //listaPolizasAux[0].Concepto = listaPolizasAux[0].Concepto;
                        //listaPolizasAux[0].Concepto = poliza.Concepto;
                        listaPolizasAux[0].Concepto = ModificarMesYAnio(otpoliza[0].Concepto, otrosValuesObj.mesnuevo, otrosValuesObj.anonuevo);
                        listaPolizasAux[0].Polizaid = "00000000-0000-0000-0000-000000000000";
                        listaPolizasAux[0].Folio = poliza.Folio.StartsWith("0") ? poliza.Folio.Substring(1) : "0" + poliza.Folio;
                        listaPolizasAux[0].Fechapol = poliza.Fechapol;
                        listaPolizasAux[0].Usuario = poliza.Usuario;
                        listaPolizasAux[0].Fecha = DateTime.Now;
                        listaPolizasAux[0].Estatus = 1;
                        foreach (Entity.Contabilidad.Polizasdetalle p in listdetalles)
                        {
                            p.Polizaid = "00000000-0000-0000-0000-000000000000";
                            p.Polizadetalleid = "00000000-0000-0000-0000-000000000000";
                            p.Estatus = 1;
                            p.Concepto = ModificarMesYAnio(p.Concepto, otrosValuesObj.mesnuevo, otrosValuesObj.anonuevo);
                            p.Fecha = DateTime.Now;
                        }
                    }
                    listaPolizasAux[0].ListaPolizaDetalle = listdetalles;
                    controlContabilidad.GuardarPoliza(listaPolizasAux);
                }
                
                return Entity.Response<Entity.ModeloPoliza>.CrearResponse<Entity.ModeloPoliza>(true, resultado);
            }
            catch (Exception ex)
            {
                return Entity.Response<Entity.ModeloPoliza>.CrearResponseVacio<Entity.ModeloPoliza>(false, ex.Message);
            }
        }

        public class OtrosValues
        {
            public string foliopolOt { get; set; }
            public string TipPolOt { get; set; }
            public string FechapolOt { get; set; }
            public bool checkgrab { get; set; }
            public string mesnuevo { get; set; }
            public string anonuevo { get; set; }
        }
        public static string ModificarMesYAnio(string texto, string mesNuevo, string anioNuevo)
        {
            //string patron = @"\b(enero|febrero|marzo|abril|mayo|junio|julio|agosto|septiembre|octubre|noviembre|diciembre)\b (\d{4})";
            string patron = @"\b(enero|febrero|marzo|abril|mayo|junio|julio|agosto|septiembre|octubre|noviembre|diciembre)\b\s+(\d{4})";

            Regex regex = new Regex(patron, RegexOptions.IgnoreCase);

            string textoModificado = regex.Replace(texto, $"{mesNuevo} {anioNuevo}");

            return textoModificado;
        }
    }

    public class ControladorReporteReportePoliza : Base.Clases.BaseReportes
    {
        public override void InicializaReporte(System.Collections.Specialized.NameValueCollection parametros)
        {
            DataSet ds = new DataSet();
            try
            {
                base.NombreReporte = "ReportePoliza";
                base.FormatoReporte = 3;
                base.RutaReporte = "Contabilidad\\Reportes";

                string polizaid = parametros.Get("polizaid");
                int Ingles = int.Parse(parametros.Get("Ingles"));
                MobileBO.ControlContabilidad controlContabilidad = new MobileBO.ControlContabilidad();
                ds = controlContabilidad.TraerDatosReportePolizas(polizaid, Ingles);
                base.DataSource = ds;
                //base.DataSource.WriteXml("c:\\Reportes\\ReportePolizaAnticipo.xml", System.Data.XmlWriteMode.WriteSchema);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class ModeloXmlFacProv
    {
        public string Proveedorid { get; set; }
        public string File { get; set; }
        public string EmpresaID { get; set; }
        public string RFC { get; set; }
    }

}