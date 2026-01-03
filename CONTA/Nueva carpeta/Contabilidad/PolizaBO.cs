using System;
using Entity;
using System.Transactions;
using System.Collections.Generic;
using System.Text;

namespace MobileBO.Contabilidad
{

    /// <summary>
    /// Clase para manejar todas las reglas de negocios de Poliza
    /// </summary>
    internal class PolizaBO
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public PolizaBO() { }
        #endregion //Constructor

        #region Métodos Públicos

        public void GuardarPolizaCierre(ListaDeEntidades<Entity.Contabilidad.Poliza> listaPoliza)
        {
            MobileDAL.Contabilidad.Poliza.Guardar(ref listaPoliza);
            GeneraPolizaCapturaPoliza(listaPoliza);
        }

        public void GuardarPoliza(ListaDeEntidades<Entity.Contabilidad.Poliza> listaPoliza)
        {
            //using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 10, 0)))
            //{
            MobileDAL.Contabilidad.Poliza.Guardar(ref listaPoliza);
            GeneraPolizaCapturaPoliza(listaPoliza);
            //scope.Complete();
            //}
        }
        public void GuardarPolizaC(ListaDeEntidades<Entity.Contabilidad.Poliza> listaPoliza)
        {
            //using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 10, 0)))
            //{
            MobileDAL.Contabilidad.Poliza.GuardarC(ref listaPoliza);
            GeneraPolizaCapturaPoliza(listaPoliza);
            //scope.Complete();
            //}
        }

        public void GuardarPolizaFactoraje(ListaDeEntidades<Entity.Contabilidad.Poliza> listaPoliza)
        {
            MobileDAL.Contabilidad.Poliza.Guardar(ref listaPoliza);
            GeneraPolizaCapturaPoliza(listaPoliza);
        }
        
        public Entity.Contabilidad.Poliza TraerPolizas(string polizaid)
        {
            return MobileDAL.Contabilidad.Poliza.TraerPolizas(polizaid);
        }

        public List<Entity.Contabilidad.Poliza> TraerPolizasPorFiltros(string EmpresaID, int? año, int? mes, bool? pendiente, string tip_pol)
        {
            return MobileDAL.Contabilidad.Poliza.TraerPolizasPorFiltros(EmpresaID, año, mes, pendiente, tip_pol);
        }

        public Entity.Contabilidad.Poliza TraerPolizaPorFolio(string folio, string tippol, string empresaid, DateTime fechapol)
        {
            return MobileDAL.Contabilidad.Poliza.TraerPolizaPorFolio(folio, tippol, empresaid, fechapol);
        }

        public ListaDeEntidades<Entity.Contabilidad.Poliza> TraerPolizasPorDescripcion(string descripcion, string tippol, string sucursalid, DateTime fechapol, bool Pendiente)
        {
            return MobileDAL.Contabilidad.Poliza.TraerPolizasPorDescripcion(descripcion, tippol, sucursalid, fechapol, Pendiente);
        }

        public System.Data.DataSet TraerPolizasDS()
        {
            return MobileDAL.Contabilidad.Poliza.TraerPolizasDS();
        }

    
        
        public void GeneraPolizaCapturaPoliza(ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizas)
        {
            ControlContabilidad controlContabilidad = new ControlContabilidad();
            ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizasPendientes = new ListaDeEntidades<Entity.Contabilidad.Poliza>();
            ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizasNormales = new ListaDeEntidades<Entity.Contabilidad.Poliza>();

            foreach (Entity.Contabilidad.Poliza poliza in listaPolizas)
            {
                // buscar si se encuentra en acvgral o acvgralpdte
                bool enAcvGral = MobileDAL.Contabilidad.Acvgral.PolizaEnAcvgral(poliza.Polizaid);
                bool enAcvGralpdte = MobileDAL.Contabilidad.Acvgralpdte.PolizaEnAcvgralpdte(poliza.Polizaid);

                if (enAcvGral)
                {
                    listaPolizasNormales.Add(poliza);
                }
                else if (enAcvGralpdte)
                {
                    listaPolizasPendientes.Add(poliza);
                }
                else
                {
                    if (poliza.Pendiente)
                    {
                        listaPolizasPendientes.Add(poliza);
                    }
                    else
                    {
                        listaPolizasNormales.Add(poliza);
                    }
                }
            }

            //ListaDeEntidades<Entity.Contabilidad.Acvgral> listaAcvGral = GeneraAcvgral(listaPolizas);
            ListaDeEntidades<Entity.Contabilidad.Acvgral> listaAcvGral = GeneraAcvgral(listaPolizasNormales);

            if (listaAcvGral.Count > 0)
            {
                controlContabilidad.GuardarPolizaContable(listaAcvGral);
            }

            // Guardar polizas pendientes
            ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> listaAcvGralPdte = GeneraAcvgralPdte(listaPolizasPendientes);
            GuardarPolizaContablePendientes(listaAcvGralPdte);

        }

        public ListaDeEntidades<Entity.Contabilidad.Acvgral> GeneraAcvgral(ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizas)
        {
            ListaDeEntidades<Entity.Contabilidad.Acvgral> listaAcvGral = new ListaDeEntidades<Entity.Contabilidad.Acvgral>();
            foreach (Entity.Contabilidad.Poliza elemento in listaPolizas)
            {
                Entity.Contabilidad.Catcuenta cuenta;
                Entity.Configuracion.Catempresa empresa = new Entity.Configuracion.Catempresa();
                string empresaid = elemento.EmpresaId;

                empresa = MobileDAL.Configuracion.Catempresa.TraerCatempresas(empresaid);

                // AcvGral
                Entity.Contabilidad.Acvgral acvGral = new Entity.Contabilidad.Acvgral();
                acvGral.EmpresaId = empresaid;
                acvGral.ReferenciaId = elemento.Polizaid;
                acvGral.CodEmpresa = empresa.Empresa.ToString();
                acvGral.Anomes = elemento.Fechapol.ToString("yyyyMM");  // Fecha del recibo del anticipo
                acvGral.TipPol = elemento.TipPol;  // Catalgo de tipos de polizas
                acvGral.TipoMov = 2; // Aqui debe ir el tipo de movimiento segun el sistema por donde se hizo 1- Anticipos 2-Monitor de documentos
                acvGral.NumPol = elemento.Folio.ToString();
                acvGral.FecPol = elemento.Fechapol;
                acvGral.Concepto = elemento.Concepto;
                acvGral.Importe = elemento.Importe;
                acvGral.Usuario = elemento.Usuario;
                acvGral.Estatus = elemento.Estatus;
                acvGral.Fecha = DateTime.Now;

                // AcvMov
                if (elemento.Estatus != 2) {
                    int numRenglon = 0;

                    List<Entity.Contabilidad.Catcuenta> CatalogoCuentas = MobileDAL.Contabilidad.Catcuenta.TraerCatcuentasPorEmpresa(empresaid);

                    foreach (Entity.Contabilidad.Polizasdetalle detallePoliza in elemento.ListaPolizaDetalle)
                    {
                        cuenta = CatalogoCuentas.Find(x => x.Cuentaid.ToUpper().Equals(detallePoliza.Cuentaid.ToUpper())); //MobileDAL.Contabilidad.Catcuenta.TraerCatcuentas(detallePoliza.Cuentaid, null, null);

                        // acvmov
                        numRenglon++;
                        Entity.Contabilidad.Acvmov acvmov = new Entity.Contabilidad.Acvmov();
                        acvmov.EmpresaId = acvGral.EmpresaId;
                        acvmov.CodEmpresa = acvGral.CodEmpresa;
                        acvmov.Acvgralid = acvGral.Acvgralid;
                        acvmov.Anomes = acvGral.Anomes;                      // Fecha del recibo del anticipo
                        acvmov.FecPol = acvGral.FecPol;
                        acvmov.TipPol = acvGral.TipPol;                      // Catalgo de tipos de polizas
                        acvmov.NumPol = acvGral.NumPol;
                        acvmov.NumRenglon = numRenglon;
                        acvmov.TipMov = detallePoliza.TipMov;                // Cargo-Abono
                        acvmov.Cuenta = cuenta.Cuenta;                       // Cuenta contable
                        acvmov.Concepto = detallePoliza.Concepto;

                        //Se cambia el dato a guardar en la columna Refer por el folio de la cesion en caso de que aplique
                        if(detallePoliza.Referencia.Trim() != "")
                        {
                            acvmov.Refer = detallePoliza.Referencia;
                        }
                        else
                        {
                            acvmov.Refer = acvGral.NumPol;
                        }
                        
                        acvmov.ClaseConta = "F";
                        acvmov.Importe = detallePoliza.Importe;
                        acvmov.TasaIva = 0;
                        acvmov.Iva = 0;
                        acvmov.RetencionIva = 0;
                        acvmov.Pendiente = false;
                        acvmov.CodFlujo = detallePoliza.TipMov == "1" ? cuenta.FlujoCar : cuenta.FlujoAbo;         // Flujo_Car o Flujo_Abo de la tabla Cat_Cuentas
                        acvmov.CodProveedor = "";
                        acvmov.FechaFiscal = acvGral.FecPol;
                        acvmov.Ctaaux = cuenta.Cuenta;                         // Cuenta contable
                        acvmov.Usuario = acvGral.Usuario;
                        acvmov.Estatus = 1;
                        acvmov.Fecha = DateTime.Now;
                        

                        acvGral.ListaAcvmov.Add(acvmov);
                    }
                }

               
                listaAcvGral.Add(acvGral);
            }

            return listaAcvGral;
        }

        public ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> GeneraAcvgralPdte(ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizas)
        {
            ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> listaAcvGral = new ListaDeEntidades<Entity.Contabilidad.Acvgralpdte>();
            foreach (Entity.Contabilidad.Poliza elemento in listaPolizas)
            {
                Entity.Contabilidad.Catcuenta cuenta;
                Entity.Configuracion.Catempresa empresa = new Entity.Configuracion.Catempresa();
                string empresaid = elemento.EmpresaId;
                empresa = MobileDAL.Configuracion.Catempresa.TraerCatempresas(empresaid);

                // Acvgralpdte
                Entity.Contabilidad.Acvgralpdte acvGral = new Entity.Contabilidad.Acvgralpdte();
                acvGral.EmpresaId = empresaid;
                acvGral.ReferenciaId = elemento.Polizaid;
                acvGral.CodEmpresa = empresa.Empresa.ToString();
                acvGral.Anomes = elemento.Fechapol.ToString("yyyyMM");  // Fecha del recibo del anticipo
                acvGral.TipPol = elemento.TipPol;  // Catalgo de tipos de polizas
                acvGral.TipoMov = 2; // Aqui debe ir el tipo de movimiento segun el sistema por donde se hizo 1- Anticipos 2-Monitor de documentos
                acvGral.NumPol = elemento.Folio.ToString();
                acvGral.FecPol = elemento.Fechapol;
                acvGral.Concepto = elemento.Concepto;
                acvGral.Importe = elemento.Importe;
                acvGral.Usuario = elemento.Usuario;
                acvGral.Estatus = elemento.Estatus;
                acvGral.Fecha = DateTime.Now;

                // AcvMov
                if (elemento.Estatus != 2)
                {
                    int numRenglon = 0;
                    List<Entity.Contabilidad.Catcuenta> CatalogoCuentas = MobileDAL.Contabilidad.Catcuenta.TraerCatcuentasPorEmpresa(empresaid);
                    foreach (Entity.Contabilidad.Polizasdetalle detallePoliza in elemento.ListaPolizaDetalle)
                    {
                        cuenta = CatalogoCuentas.Find(x => x.Cuentaid.ToUpper().Equals(detallePoliza.Cuentaid.ToUpper()));

                        // Acvpdte
                        numRenglon++;
                        Entity.Contabilidad.Acvpdte acvmov = new Entity.Contabilidad.Acvpdte();
                        acvmov.EmpresaId = acvGral.EmpresaId;
                        acvmov.CodEmpresa = acvGral.CodEmpresa;
                        acvmov.Acvgralid = acvGral.Acvgralid;
                        acvmov.Anomes = acvGral.Anomes;                      // Fecha del recibo del anticipo
                        acvmov.FecPol = acvGral.FecPol;
                        acvmov.TipPol = acvGral.TipPol;                      // Catalgo de tipos de polizas
                        acvmov.NumPol = acvGral.NumPol;
                        acvmov.NumRenglon = numRenglon;
                        acvmov.TipMov = detallePoliza.TipMov;                // Cargo-Abono
                        acvmov.Cuenta = cuenta.Cuenta;                       // Cuenta contable
                        acvmov.Concepto = detallePoliza.Concepto;
                        acvmov.Refer = acvGral.NumPol;
                        acvmov.ClaseConta = "F";
                        acvmov.Importe = detallePoliza.Importe;
                        acvmov.TasaIva = 0;
                        acvmov.Iva = 0;
                        acvmov.RetencionIva = 0;
                        acvmov.Pendiente = false;
                        acvmov.CodFlujo = detallePoliza.TipMov == "1" ? cuenta.FlujoCar : cuenta.FlujoAbo;         // Flujo_Car o Flujo_Abo de la tabla Cat_Cuentas
                        acvmov.CodProveedor = "";
                        acvmov.FechaFiscal = acvGral.FecPol;
                        acvmov.Ctaaux = cuenta.Cuenta;                         // Cuenta contable
                        acvmov.Usuario = acvGral.Usuario;
                        acvmov.Estatus = 1;
                        acvmov.Fecha = DateTime.Now;

                        acvGral.ListaAcvpdte.Add(acvmov);
                    }
                }


                listaAcvGral.Add(acvGral);
            }

            return listaAcvGral;
        }

        public void GuardarPolizaContablePendientes(ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> listaAcvGral)
        {
            ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> ListaacvgralBorrar = new ListaDeEntidades<Entity.Contabilidad.Acvgralpdte>();

            foreach (Entity.Contabilidad.Acvgralpdte elemento in listaAcvGral)
            {
                // Borrar poliza anterior
                Entity.Contabilidad.Acvgralpdte acvgralBorrar = MobileDAL.Contabilidad.Acvgralpdte.TraerAcvgralpdtePorReferenciaId(elemento.ReferenciaId);
                if (acvgralBorrar != null)
                {
                    ListaacvgralBorrar.Add(acvgralBorrar);
                }
            }

            MobileDAL.Contabilidad.Acvgralpdte.GuardarPoliza(ref listaAcvGral, ListaacvgralBorrar);
        }

        
        

        public bool ProcesarPolizasPendientes(int ejercicio,string empresaid)
        {
            StringBuilder elXml = new StringBuilder();
            //using (TransactionScope scope = new TransactionScope())
            //{
            // Obtener acvgralpdte por ejercicio
            ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> listaAcvgalpdte = MobileDAL.Contabilidad.Acvgralpdte.TraerAcvgralpdtePorEjercicio(ejercicio, empresaid);
            Entity.ListaDeEntidades<Entity.Contabilidad.Acvgral> listaAcvGral = new ListaDeEntidades<Entity.Contabilidad.Acvgral>();
            //Dictionary<string, Entity.Contabilidad.Saldo> dictSaldos = new Dictionary<string, Entity.Contabilidad.Saldo>();

            foreach (Entity.Contabilidad.Acvgralpdte acvgralpendiente in listaAcvgalpdte)
            {
                elXml.Append("<polizas>");
                elXml.AppendFormat("<AcvGralPdte>{0}</AcvGralPdte>", acvgralpendiente.Acvgralid);

                // Obtener el numero de poliza
                string numpoliza = acvgralpendiente.NumPol.Substring(0, 1) == "0" ? acvgralpendiente.NumPol.Substring(1) : "0" + acvgralpendiente.NumPol;
                Entity.Contabilidad.Acvgral acvgral = MobileDAL.Contabilidad.Acvgral.TraerAcvgral(numpoliza, acvgralpendiente.TipPol, acvgralpendiente.FecPol, empresaid);                
                if (acvgral == null)
                {
                    // No existe la poliza equivalente en acvgral, detener el proceso.                    
                    throw new System.ArgumentException("Poliza: " + numpoliza + ", Tipo: " + acvgralpendiente.TipPol + ", Fecha: " + acvgralpendiente.FecPol.ToShortDateString() + " no encontrada");
                }
                else
                {
                    elXml.AppendFormat("<AcvGral>{0}</AcvGral>", acvgral.Acvgralid);
                    elXml.Append("</polizas>");
                    listaAcvGral.Add(acvgral);

                    //// Afectar saldos que se van a eliminar
                    //new MobileBO.ControlContabilidad().AfectaSaldosParaEliminarPoliza(acvgral, ref dictSaldos);

                    //// Afectar saldos que se van a agregar
                    //new MobileBO.Contabilidad.AcvgralpdteBO().AfectaSaldosParaAgregarPoliza(acvgralpendiente, ref dictSaldos);
                }
            }

            if (listaAcvgalpdte.Count > 0 && listaAcvGral.Count > 0)
            {
                //ListaDeEntidades<Entity.Contabilidad.Saldo> listaSaldosDict = new ListaDeEntidades<Entity.Contabilidad.Saldo>();
                ////foreach (Entity.Contabilidad.Saldo s in dictSaldos.Values)
                ////{
                ////    listaSaldosDict.Add(s);
                ////}

                //MobileDAL.Contabilidad.Acvgralpdte.ProcesarPolizasPendientes(ref listaAcvgalpdte, ref listaAcvGral, ref listaSaldosDict);
                MobileDAL.Contabilidad.Acvgralpdte.ProcesarPolizasPdtes(elXml.ToString());
            }
            else
            {
                return false;
            }

            //    scope.Complete();
            //}
            return true;
        }


        public System.Data.DataSet TraerTipoContabilidad(string empresaid)
        {
            return MobileDAL.Contabilidad.Poliza.TraerTipoContabilidad(empresaid);
        }
        public System.Data.DataSet TraerDatosReportePolizas(string polizaid, int Ingles)
        {
            return MobileDAL.Contabilidad.Poliza.TraerDatosReportePolizas(polizaid, Ingles);
        }

        public static System.Data.DataSet ReporteCapturaPolizasMasivo(string EmpresaID, DateTime FechaInicial, DateTime FechaFinal, string TipPol, int Ingles, string folioInicial, string folioFinal)
        {
            return MobileDAL.Contabilidad.Poliza.ReporteCapturaPolizasMasivo(EmpresaID, FechaInicial, FechaFinal, TipPol, Ingles, folioInicial, folioFinal);
        }
        public void GuardarPolizaBLT(ListaDeEntidades<Entity.Contabilidad.Poliza> listaPoliza)
        {
            MobileDAL.Contabilidad.Poliza.GuardarBLT(ref listaPoliza);
            GeneraPolizaCapturaPolizaBLT(listaPoliza);
        }
        public void GeneraPolizaCapturaPolizaBLT(ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizas)
        {
            ControlContabilidad controlContabilidad = new ControlContabilidad();
            ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizasPendientes = new ListaDeEntidades<Entity.Contabilidad.Poliza>();
            ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizasNormales = new ListaDeEntidades<Entity.Contabilidad.Poliza>();

            foreach (Entity.Contabilidad.Poliza poliza in listaPolizas)
            {
                // buscar si se encuentra en acvgral o acvgralpdte
                bool enAcvGral = MobileDAL.Contabilidad.Acvgral.PolizaEnAcvgralBLT(poliza.Polizaid);
                bool enAcvGralpdte = MobileDAL.Contabilidad.Acvgralpdte.PolizaEnAcvgralpdteBLT(poliza.Polizaid);

                if (enAcvGral)
                {
                    listaPolizasNormales.Add(poliza);
                }
                else if (enAcvGralpdte)
                {
                    listaPolizasPendientes.Add(poliza);
                }
                else
                {
                    if (poliza.Pendiente)
                    {
                        listaPolizasPendientes.Add(poliza);
                    }
                    else
                    {
                        listaPolizasNormales.Add(poliza);
                    }
                }
            }
            
            ListaDeEntidades<Entity.Contabilidad.Acvgral> listaAcvGral = GeneraAcvgralBLT(listaPolizasNormales);

            if (listaAcvGral.Count > 0)
            {
                controlContabilidad.GuardarPolizaContableBLT(listaAcvGral);
            }

            // Guardar polizas pendientes
            ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> listaAcvGralPdte = GeneraAcvgralPdteBLT(listaPolizasPendientes);
            GuardarPolizaContablePendientesBLT(listaAcvGralPdte);

        }
        public ListaDeEntidades<Entity.Contabilidad.Acvgral> GeneraAcvgralBLT(ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizas)
        {
            ListaDeEntidades<Entity.Contabilidad.Acvgral> listaAcvGral = new ListaDeEntidades<Entity.Contabilidad.Acvgral>();
            foreach (Entity.Contabilidad.Poliza elemento in listaPolizas)
            {
                Entity.Contabilidad.Catcuenta cuenta;
                Entity.Configuracion.Catempresa empresa = new Entity.Configuracion.Catempresa();
                string empresaid = elemento.EmpresaId;

                empresa = MobileDAL.Configuracion.Catempresa.TraerCatempresasBLT(empresaid);

                // AcvGral
                Entity.Contabilidad.Acvgral acvGral = new Entity.Contabilidad.Acvgral();
                acvGral.EmpresaId = empresaid;
                acvGral.ReferenciaId = elemento.Polizaid;
                acvGral.CodEmpresa = empresa.Empresa.ToString();
                acvGral.Anomes = elemento.Fechapol.ToString("yyyyMM");  // Fecha del recibo del anticipo
                acvGral.TipPol = elemento.TipPol;  // Catalgo de tipos de polizas
                acvGral.TipoMov = 2; // Aqui debe ir el tipo de movimiento segun el sistema por donde se hizo 1- Anticipos 2-Monitor de documentos
                acvGral.NumPol = elemento.Folio.ToString();
                acvGral.FecPol = elemento.Fechapol;
                acvGral.Concepto = elemento.Concepto;
                acvGral.Importe = elemento.Importe;
                acvGral.Usuario = elemento.Usuario;
                acvGral.Estatus = elemento.Estatus;
                acvGral.Fecha = DateTime.Now;

                // AcvMov
                if (elemento.Estatus != 2)
                {
                    int numRenglon = 0;

                    List<Entity.Contabilidad.Catcuenta> CatalogoCuentas = MobileDAL.Contabilidad.Catcuenta.TraerCatcuentasPorEmpresaBLT(empresaid);

                    foreach (Entity.Contabilidad.Polizasdetalle detallePoliza in elemento.ListaPolizaDetalle)
                    {
                        cuenta = CatalogoCuentas.Find(x => x.Cuentaid.ToUpper().Equals(detallePoliza.Cuentaid.ToUpper())); //MobileDAL.Contabilidad.Catcuenta.TraerCatcuentas(detallePoliza.Cuentaid, null, null);

                        // acvmov
                        numRenglon++;
                        Entity.Contabilidad.Acvmov acvmov = new Entity.Contabilidad.Acvmov();
                        acvmov.EmpresaId = acvGral.EmpresaId;
                        acvmov.CodEmpresa = acvGral.CodEmpresa;
                        acvmov.Acvgralid = acvGral.Acvgralid;
                        acvmov.Anomes = acvGral.Anomes;                      // Fecha del recibo del anticipo
                        acvmov.FecPol = acvGral.FecPol;
                        acvmov.TipPol = acvGral.TipPol;                      // Catalgo de tipos de polizas
                        acvmov.NumPol = acvGral.NumPol;
                        acvmov.NumRenglon = numRenglon;
                        acvmov.TipMov = detallePoliza.TipMov;                // Cargo-Abono
                        acvmov.Cuenta = cuenta.Cuenta;                       // Cuenta contable
                        acvmov.Concepto = detallePoliza.Concepto;

                        //Se cambia el dato a guardar en la columna Refer por el folio de la cesion en caso de que aplique
                        if (detallePoliza.Referencia.Trim() != "")
                        {
                            acvmov.Refer = detallePoliza.Referencia;
                        }
                        else
                        {
                            acvmov.Refer = acvGral.NumPol;
                        }

                        acvmov.ClaseConta = "F";
                        acvmov.Importe = detallePoliza.Importe;
                        acvmov.TasaIva = 0;
                        acvmov.Iva = 0;
                        acvmov.RetencionIva = 0;
                        acvmov.Pendiente = false;
                        acvmov.CodFlujo = detallePoliza.TipMov == "1" ? cuenta.FlujoCar : cuenta.FlujoAbo;         // Flujo_Car o Flujo_Abo de la tabla Cat_Cuentas
                        acvmov.CodProveedor = "";
                        acvmov.FechaFiscal = acvGral.FecPol;
                        acvmov.Ctaaux = cuenta.Cuenta;                         // Cuenta contable
                        acvmov.Usuario = acvGral.Usuario;
                        acvmov.Estatus = 1;
                        acvmov.Fecha = DateTime.Now;


                        acvGral.ListaAcvmov.Add(acvmov);
                    }
                }


                listaAcvGral.Add(acvGral);
            }

            return listaAcvGral;
        }
        public ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> GeneraAcvgralPdteBLT(ListaDeEntidades<Entity.Contabilidad.Poliza> listaPolizas)
        {
            ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> listaAcvGral = new ListaDeEntidades<Entity.Contabilidad.Acvgralpdte>();
            foreach (Entity.Contabilidad.Poliza elemento in listaPolizas)
            {
                Entity.Contabilidad.Catcuenta cuenta;
                Entity.Configuracion.Catempresa empresa = new Entity.Configuracion.Catempresa();
                string empresaid = elemento.EmpresaId;
                empresa = MobileDAL.Configuracion.Catempresa.TraerCatempresasBLT(empresaid);

                // Acvgralpdte
                Entity.Contabilidad.Acvgralpdte acvGral = new Entity.Contabilidad.Acvgralpdte();
                acvGral.EmpresaId = empresaid;
                acvGral.ReferenciaId = elemento.Polizaid;
                acvGral.CodEmpresa = empresa.Empresa.ToString();
                acvGral.Anomes = elemento.Fechapol.ToString("yyyyMM");  // Fecha del recibo del anticipo
                acvGral.TipPol = elemento.TipPol;  // Catalgo de tipos de polizas
                acvGral.TipoMov = 2; // Aqui debe ir el tipo de movimiento segun el sistema por donde se hizo 1- Anticipos 2-Monitor de documentos
                acvGral.NumPol = elemento.Folio.ToString();
                acvGral.FecPol = elemento.Fechapol;
                acvGral.Concepto = elemento.Concepto;
                acvGral.Importe = elemento.Importe;
                acvGral.Usuario = elemento.Usuario;
                acvGral.Estatus = elemento.Estatus;
                acvGral.Fecha = DateTime.Now;

                // AcvMov
                if (elemento.Estatus != 2)
                {
                    int numRenglon = 0;
                    List<Entity.Contabilidad.Catcuenta> CatalogoCuentas = MobileDAL.Contabilidad.Catcuenta.TraerCatcuentasPorEmpresaBLT(empresaid);
                    foreach (Entity.Contabilidad.Polizasdetalle detallePoliza in elemento.ListaPolizaDetalle)
                    {
                        cuenta = CatalogoCuentas.Find(x => x.Cuentaid.ToUpper().Equals(detallePoliza.Cuentaid.ToUpper()));

                        // Acvpdte
                        numRenglon++;
                        Entity.Contabilidad.Acvpdte acvmov = new Entity.Contabilidad.Acvpdte();
                        acvmov.EmpresaId = acvGral.EmpresaId;
                        acvmov.CodEmpresa = acvGral.CodEmpresa;
                        acvmov.Acvgralid = acvGral.Acvgralid;
                        acvmov.Anomes = acvGral.Anomes;                      // Fecha del recibo del anticipo
                        acvmov.FecPol = acvGral.FecPol;
                        acvmov.TipPol = acvGral.TipPol;                      // Catalgo de tipos de polizas
                        acvmov.NumPol = acvGral.NumPol;
                        acvmov.NumRenglon = numRenglon;
                        acvmov.TipMov = detallePoliza.TipMov;                // Cargo-Abono
                        acvmov.Cuenta = cuenta.Cuenta;                       // Cuenta contable
                        acvmov.Concepto = detallePoliza.Concepto;
                        acvmov.Refer = acvGral.NumPol;
                        acvmov.ClaseConta = "F";
                        acvmov.Importe = detallePoliza.Importe;
                        acvmov.TasaIva = 0;
                        acvmov.Iva = 0;
                        acvmov.RetencionIva = 0;
                        acvmov.Pendiente = false;
                        acvmov.CodFlujo = detallePoliza.TipMov == "1" ? cuenta.FlujoCar : cuenta.FlujoAbo;         // Flujo_Car o Flujo_Abo de la tabla Cat_Cuentas
                        acvmov.CodProveedor = "";
                        acvmov.FechaFiscal = acvGral.FecPol;
                        acvmov.Ctaaux = cuenta.Cuenta;                         // Cuenta contable
                        acvmov.Usuario = acvGral.Usuario;
                        acvmov.Estatus = 1;
                        acvmov.Fecha = DateTime.Now;

                        acvGral.ListaAcvpdte.Add(acvmov);
                    }
                }


                listaAcvGral.Add(acvGral);
            }

            return listaAcvGral;
        }
        public void GuardarPolizaContablePendientesBLT(ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> listaAcvGral)
        {
            ListaDeEntidades<Entity.Contabilidad.Acvgralpdte> ListaacvgralBorrar = new ListaDeEntidades<Entity.Contabilidad.Acvgralpdte>();

            foreach (Entity.Contabilidad.Acvgralpdte elemento in listaAcvGral)
            {
                // Borrar poliza anterior
                Entity.Contabilidad.Acvgralpdte acvgralBorrar = MobileDAL.Contabilidad.Acvgralpdte.TraerAcvgralpdtePorReferenciaIdBLT(elemento.ReferenciaId);
                if (acvgralBorrar != null)
                {
                    ListaacvgralBorrar.Add(acvgralBorrar);
                }
            }

            MobileDAL.Contabilidad.Acvgralpdte.GuardarPolizaBLT(ref listaAcvGral, ListaacvgralBorrar);
        }
        public System.Data.DataSet TraerPolizasPorUUID(string UUID)
        {
            return MobileDAL.Contabilidad.Poliza.TraerPolizasPorUUID(UUID);
        }
        #endregion //Métodos Públicos

    }
}
