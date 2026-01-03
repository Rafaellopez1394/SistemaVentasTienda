using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;
using Entity;
using System.IO;
using System.Text;
using System.Xml;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class CapturaCompras : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        #region Ayudas
        [WebMethod]
        public static Entity.Response<List<Entity.HelpField.Values>> AyudaCompra_FindByCode(string value)
        {
            Entity.Contabilidad.Compra compra;
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                compra = MobileBO.ControlContabilidad.TraerComprasPorCodigo(values.ID, int.Parse(values.Codigo));
                if (compra != null)
                {
                    Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = compra.Compraid, Codigo = compra.Folio.ToString(), Descripcion = "" };
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
        public static Entity.Response<List<Entity.HelpField.Values>> AyudaCompra_FindByPopUp(string value)
        {
            List<Entity.Contabilidad.Compra> listaCompras;
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                listaCompras = MobileBO.ControlContabilidad.TraerComprasPorProveedor(values.ID, values.Descripcion);
                if (listaCompras != null)
                {
                    foreach (Entity.Contabilidad.Compra compra in listaCompras)
                    {
                        Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = compra.Compraid, Codigo = compra.Folio.ToString(), Descripcion = compra.Nombreproveedor };
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
        public static Entity.Response<List<Entity.HelpField.Values>> AyudaProveedor_FindByCode(string value)
        {
            Entity.Contabilidad.Catproveedor proveedores;
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                proveedores = MobileBO.ControlContabilidad.TraerCatproveedoresPorCodigo(int.Parse(values.Codigo), values.ID);
                if (proveedores != null)
                {
                    Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = proveedores.Proveedorid, Codigo = proveedores.Codigo.ToString(), Descripcion = proveedores.Nombre };
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
        public static Entity.Response<List<Entity.HelpField.Values>> AyudaProveedor_FindByPopUp(string value)
        {
            System.Collections.Generic.List<Entity.Contabilidad.Catproveedor> listaProveedores;
            List<Entity.HelpField.Values> ListaElementos;
            try
            {
                ListaElementos = new List<Entity.HelpField.Values>();
                Entity.HelpField.Values values = MobileBO.Utilerias.Deserializar<Entity.HelpField.Values>(value);
                listaProveedores = MobileBO.ControlContabilidad.TraerCatproveedoresPorNombre(values.Descripcion, values.ID);
                if (listaProveedores != null)
                {
                    foreach (Entity.Contabilidad.Catproveedor proveedor in listaProveedores)
                    {
                        Entity.HelpField.Values elemento = new Entity.HelpField.Values() { ID = proveedor.Proveedorid, Codigo = proveedor.Codigo.ToString(), Descripcion = proveedor.Nombre };
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
        #endregion
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
        public static Entity.Response<List<Entity.Contabilidad.Catfacturasproveedor>> ProcesarXmlFacturas(string value)
        {
            try
            {
                List<Entity.Contabilidad.Catfacturasproveedor> FacturasProveedor = new List<Entity.Contabilidad.Catfacturasproveedor>();
                List<ModeloXmlFacProv> Xml = MobileBO.Utilerias.Deserializar<List<ModeloXmlFacProv>>(value);

                if (Xml.Count == 0)
                    return Entity.Response<List<Entity.Contabilidad.Catfacturasproveedor>>.CrearResponseVacio<List<Entity.Contabilidad.Catfacturasproveedor>>(false, "Error al procesar los archivos, lista de xml vacia");

                foreach (ModeloXmlFacProv xml in Xml)
                {
                    string Location = pathXML(xml.Proveedorid, xml.File);
                    if (!File.Exists(Location))
                        return Entity.Response<List<Entity.Contabilidad.Catfacturasproveedor>>.CrearResponseVacio<List<Entity.Contabilidad.Catfacturasproveedor>>(false, "Error al obtener el archivo xml de la factura: " + xml.File);
                    Entity.Contabilidad.Catfacturasproveedor Factura = ProcesarXML(Location);
                    Factura.Proveedorid = xml.Proveedorid;
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

        public static string pathXML(string Proveedorid,string File) {
            Entity.Contabilidad.Catproveedor proveedor = MobileBO.ControlContabilidad.TraerCatproveedores(Proveedorid, null, null);
            if (proveedor == null)
                throw new Exception("Error al obtener el proveedor de la factura: " + File);
            Entity.Configuracion.Catempresa Empresa = MobileBO.ControlConfiguracion.TraerCatempresas(proveedor.Empresaid);
            string directorio = System.AppDomain.CurrentDomain.RelativeSearchPath;
            directorio += "\\Proveedores\\" + (Empresa.Empresa == 1 ? "Factur" : "Balor") + " \\" + DateTime.Now.Year.ToString() + "\\" + DateTime.Now.Month + "\\" + proveedor.Rfc;
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
                        Factura.Subtotal = decimal.Parse(reader.GetAttribute("subTotal").ToString());
                        Factura.Factura = reader.GetAttribute("folio").ToString().Trim();
                        if (reader.GetAttribute("serie") != null)
                            Factura.Factura += "-" + reader.GetAttribute("serie").ToString().Trim();
                    }
                    if (reader.Name.ToUpper().Trim().Contains("TIMBREFISCAL") && reader.AttributeCount > 0)
                    {
                        Factura.Uuid = reader.GetAttribute("UUID").ToString().Trim();
                        Factura.Fechatimbre = DateTime.Parse(reader.GetAttribute("FechaTimbrado").ToString().Trim()).ToShortDateString();
                    }

                    if (reader.Name.ToUpper().Trim().Contains("TRASLADO") && reader.AttributeCount > 0)
                    {
                        if (reader.GetAttribute("impuesto").ToString().Trim().ToUpper() == "IVA")
                            Factura.Iva = decimal.Parse(reader.GetAttribute("importe").ToString());
                    }
                }
                reader.Close();
            }                        
            return Factura;
            
        }



        [WebMethod]
        public static Entity.Response<ModeloCompraSave> GuardarComprasProveedor(string value,string fecha) {
            try
            {
                ModeloCompraSave respuesta = new ModeloCompraSave();

                List<Entity.Contabilidad.Catfacturasproveedor> FacturasProveedor = MobileBO.Utilerias.Deserializar<List<Entity.Contabilidad.Catfacturasproveedor>>(value);
                if (FacturasProveedor.Count == 0)
                    return Entity.Response<ModeloCompraSave>.CrearResponseVacio<ModeloCompraSave>(false, "Error al procesar los elementos a guardar");


                Entity.Contabilidad.Cierrecontabilidad cierre = MobileBO.ControlContabilidad.TraerCierrecontabilidad(FacturasProveedor[0].Empresaid);
                if (cierre != null)
                {
                    DateTime fechaPol = DateTime.Today;
                    if (fechaPol <= cierre.Fechacierre)
                    {
                        throw new Exception("Contabilidad cerrada al " + cierre.Fechacierre.ToShortDateString());
                    }
                }


                decimal importe = 0m;
                //Obtenemos el XML para guardarlo en la base de datos
                foreach (Entity.Contabilidad.Catfacturasproveedor factura in FacturasProveedor)
                {
                    string Location = pathXML(factura.Proveedorid, factura.Rutaxml);
                    if (!File.Exists(Location))
                        return Entity.Response<ModeloCompraSave>.CrearResponseVacio<ModeloCompraSave>(false, "Error al obtener el archivo xml de la factura: " + factura.Rutaxml);
                    factura.Xml = GetXmlString(Location);
                    factura.Fecha = DateTime.Now;
                    factura.Facturaproveedorid = (factura.Facturaproveedorid == null || factura.Facturaproveedorid == string.Empty || factura.Facturaproveedorid == "null" ? Guid.Empty.ToString() : factura.Facturaproveedorid);
                    factura.Compraid = (factura.Compraid == null || factura.Compraid == string.Empty || factura.Compraid == "null" ? Guid.Empty.ToString() : factura.Compraid);
                    importe += factura.Total;
                }

                //Generamos la entidad del cabecero de la compra
                Entity.Contabilidad.Compra Compra = MobileBO.ControlContabilidad.TraerCompras(FacturasProveedor[0].Compraid);
                if (Compra == null)
                {
                    Compra = new Entity.Contabilidad.Compra();
                    Compra.Compraid = Guid.Empty.ToString();
                    Compra.Empresaid = FacturasProveedor[0].Empresaid;
                    Compra.Proveedorid = FacturasProveedor[0].Proveedorid;
                    Compra.Folio = 0;
                    Compra.Importe = importe;
                    Compra.Estatus = 1;
                    Compra.Fecha = DateTime.Parse(fecha).ToShortDateString();
                    Compra.Usuario = FacturasProveedor[0].Usuario;
                }
                else
                {
                    Compra.Importe = importe;
                    Compra.Fecha = DateTime.Parse(fecha).ToShortDateString();
                    Compra.Usuario = FacturasProveedor[0].Usuario;
                }

                //Guardamos la compra
                MobileBO.ControlContabilidad.GuardarCompra(new List<Entity.Contabilidad.Compra>() { Compra });

                //Actualizamos el campo compraid
                foreach (Entity.Contabilidad.Catfacturasproveedor factura in FacturasProveedor)
                    factura.Compraid = Compra.Compraid;

                //Guardamos la factura del proveedor
                MobileBO.ControlContabilidad.GuardarCatfacturasproveedor(FacturasProveedor);

                //Actualizamos el detalle de las facturas de proveedor
                foreach (Entity.Contabilidad.Catfacturasproveedor factura in FacturasProveedor)
                {
                    MobileBO.ControlContabilidad.EliminarCatfacturasproveedordet(factura.Facturaproveedorid);
                    foreach (Entity.Contabilidad.Catfacturasproveedordet detalle in factura.Detalle)
                    {
                        detalle.Facturaproveedorid = factura.Facturaproveedorid;
                        detalle.Facturaproveedordetid = Guid.Empty.ToString();
                    }
                    MobileBO.ControlContabilidad.GuardarCatfacturasproveedordet(factura.Detalle);
                }

                //Guardamos en contabilidad las facturas que Generan Pasivo
                Entity.Contabilidad.Poliza Poliza = new Entity.Contabilidad.Poliza();
                if (FacturasProveedor.FindAll(x => x.Generapasivo).Count > 0)
                {                    
                    Poliza.Polizaid = Guid.Empty.ToString();
                    Poliza.ListaPolizaDetalle = new ListaDeEntidades<Entity.Contabilidad.Polizasdetalle>();
                    decimal importePoliza = 0m;
                    foreach (Entity.Contabilidad.Catfacturasproveedor factura in FacturasProveedor)
                    {
                        importePoliza += factura.Total;

                        Entity.Contabilidad.Catproveedor proveedor = MobileBO.ControlContabilidad.TraerCatproveedores(factura.Proveedorid, null, null);
                        if (proveedor == null)
                            return Entity.Response<ModeloCompraSave>.CrearResponseVacio<ModeloCompraSave>(false, "Inconsistencia al obtener la cuenta contable del proveedor");

                        //Realizamos el cargo y el abono de cada uno de los gastos capturados en el detalle de la factura
                        foreach (Entity.Contabilidad.Catfacturasproveedordet detalle in factura.Detalle)
                        {
                            //Cargo a la cuenta del GASTO
                            Entity.Contabilidad.Catcuenta CuentaCargo = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta(detalle.Cuenta.Replace("-", ""), factura.Empresaid);
                            Entity.Contabilidad.Polizasdetalle Cargo = new Entity.Contabilidad.Polizasdetalle();
                            Cargo.Polizadetalleid = Guid.Empty.ToString();
                            Cargo.Polizaid = Poliza.Polizaid;
                            Cargo.Cuentaid = CuentaCargo.Cuentaid;
                            Cargo.TipMov = "1";
                            Cargo.Concepto = detalle.Concepto;
                            Cargo.Cantidad = 1;
                            Cargo.Importe = detalle.Importe;
                            Cargo.Usuario = factura.Usuario;
                            Cargo.Estatus = 1;
                            Cargo.Fecha = DateTime.Now;

                            ////Abono a la cuenta del proveedor
                            //Entity.Operacion.Catempresasbanco empresaBanco = MobileBO.ControlOperacion.TraerCatempresasbancos(transferencia.Empresabancoid);
                            Entity.Contabilidad.Catcuenta CuentaAbono = new MobileBO.ControlContabilidad().TraerCatCuentasPorCuenta(proveedor.Cuentacontable, factura.Empresaid);
                            Entity.Contabilidad.Polizasdetalle Abono = new Entity.Contabilidad.Polizasdetalle();
                            Abono.Polizadetalleid = Guid.Empty.ToString();
                            Abono.Polizaid = Poliza.Polizaid;
                            Abono.Cuentaid = CuentaAbono.Cuentaid;
                            Abono.TipMov = "2";
                            Abono.Concepto = detalle.Concepto;
                            Abono.Cantidad = 1;
                            Abono.Importe = detalle.Importe;
                            Abono.Usuario = factura.Usuario;
                            Abono.Estatus = 1;
                            Abono.Fecha = DateTime.Now;
                            Poliza.ListaPolizaDetalle.Add(Cargo);
                            Poliza.ListaPolizaDetalle.Add(Abono);
                        }
                    }

                    Poliza.EmpresaId = FacturasProveedor[0].Empresaid;
                    Poliza.Folio = "0";
                    Poliza.TipPol = "DR";
                    Poliza.Fechapol = DateTime.Today;
                    Poliza.Concepto = "POLIZA DE COMPRAS";
                    Poliza.Importe = importePoliza;
                    Poliza.Estatus = 1;
                    Poliza.Fecha = DateTime.Now;
                    Poliza.Usuario = FacturasProveedor[0].Usuario;
                    Poliza.Pendiente = false;
                    new MobileBO.ControlContabilidad().GuardarPoliza(new Entity.ListaDeEntidades<Entity.Contabilidad.Poliza>() { Poliza });


                }

                //Preparamos los datos que vamos a regresar en la respues
                respuesta.Compra = Compra;
                respuesta.Poliza = Poliza;
                foreach (Entity.Contabilidad.Catfacturasproveedor factura in FacturasProveedor)
                    factura.Xml = "";
                respuesta.ListaCatfacturasproveedor = FacturasProveedor;
                return Entity.Response<ModeloCompraSave>.CrearResponse<ModeloCompraSave>(true, respuesta);
            }
            catch (Exception ex)
            {
                return Entity.Response<ModeloCompraSave>.CrearResponseVacio<ModeloCompraSave>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<ModeloCompraSave> TraerCompra(string CompraID) {
            try
            {
                ModeloCompraSave respuesta = new ModeloCompraSave();
                respuesta.Compra = MobileBO.ControlContabilidad.TraerCompras(CompraID);
                respuesta.ListaCatfacturasproveedor = MobileBO.ControlContabilidad.TraerCatfacturasproveedorPorCompraID(CompraID);
                respuesta.Proveedor = MobileBO.ControlContabilidad.TraerCatproveedores(respuesta.Compra.Proveedorid, null, null);
                respuesta.Poliza = new Entity.Contabilidad.Poliza();
                //if (respuesta.ListaCatfacturasproveedor.FindAll(x => x.Generapasivo == true).Count > 0)
                    //respuesta.Poliza = new MobileBO.ControlContabilidad().TraerPolizas(respuesta.ListaCatfacturasproveedor.FindAll(x => x.Generapasivo == true).First().Polizaid);
                foreach (Entity.Contabilidad.Catfacturasproveedor factura in respuesta.ListaCatfacturasproveedor)
                    factura.Xml = "";
                return Entity.Response<ModeloCompraSave>.CrearResponse<ModeloCompraSave>(true, respuesta);
            }
            catch (Exception ex)
            {
                return Entity.Response<ModeloCompraSave>.CrearResponseVacio<ModeloCompraSave>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<object> CancelaCompra(string CompraID,string Usuario)
        {
            try
            {   
                Entity.Contabilidad.Compra Compra = MobileBO.ControlContabilidad.TraerCompras(CompraID);
                List<Entity.Contabilidad.Catfacturasproveedor> ListaCatfacturasproveedor = MobileBO.ControlContabilidad.TraerCatfacturasproveedorPorCompraID(CompraID);
                Entity.Contabilidad.Poliza Poliza = new Entity.Contabilidad.Poliza();
                //if (ListaCatfacturasproveedor.FindAll(x => x.Generapasivo == true).Count > 0)
                //    Poliza = new MobileBO.ControlContabilidad().TraerPolizas(ListaCatfacturasproveedor.FindAll(x => x.Generapasivo == true).First().Polizaid);

                Compra.Estatus = 2;
                Compra.Usuario = Usuario;
                foreach (Entity.Contabilidad.Catfacturasproveedor factura in ListaCatfacturasproveedor) {
                    factura.Usuario = Usuario;
                    factura.Estatus = 2;
                }

                Entity.Contabilidad.Cierrecontabilidad cierre = MobileBO.ControlContabilidad.TraerCierrecontabilidad(Compra.Empresaid);
                if (cierre != null)
                {
                    DateTime fechaPol = DateTime.Parse(Compra.Fecha);
                    if (fechaPol <= cierre.Fechacierre)
                    {
                        throw new Exception("Contabilidad cerrada al " + cierre.Fechacierre.ToShortDateString());
                    }
                }

                //Cancelamos la poliza
                if (Poliza.UltimaAct != 0) {
                    Poliza.Estatus = 2;
                    Poliza.Usuario = Usuario;
                    foreach (Entity.Contabilidad.Polizasdetalle detalle in Poliza.ListaPolizaDetalle)
                    {
                        detalle.Estatus = 2;
                        detalle.Usuario = Usuario;
                    }
                    Poliza.ListaPolizaDetalle.AcceptChanges();
                    new MobileBO.ControlContabilidad().GuardarPoliza(new ListaDeEntidades<Entity.Contabilidad.Poliza>() { Poliza });
                }
                MobileBO.ControlContabilidad.GuardarCompra(new List<Entity.Contabilidad.Compra>() { Compra });
                MobileBO.ControlContabilidad.GuardarCatfacturasproveedor(ListaCatfacturasproveedor);
                return Entity.Response<object>.CrearResponse<object>(true, new { Cancelo = true });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

        [WebMethod]
        public static Entity.Response<object> TieneCuentaContable(string proveedorid)
        {
            try
            {
                Entity.Contabilidad.Catproveedor prov = MobileBO.ControlContabilidad.TraerCatproveedores(proveedorid, null, null);
                if (prov != null) {
                    if (prov.Cuentacontable.Trim().Length == 24)
                        return Entity.Response<object>.CrearResponse<object>(true, new { Existe = true });
                    else
                        return Entity.Response<object>.CrearResponse<object>(true, new { Existe = false });
                }
                    
                return Entity.Response<object>.CrearResponse<object>(true, new { Existe = false });
            }
            catch (Exception ex)
            {
                return Entity.Response<object>.CrearResponseVacio<object>(false, ex.Message);
            }
        }

    }
   
    public class ModeloCompraSave {
        public Entity.Contabilidad.Compra Compra { get; set; }
        public Entity.Contabilidad.Catproveedor Proveedor { get; set; }
        public Entity.Contabilidad.Poliza Poliza { get; set; }
        public List<Entity.Contabilidad.Catfacturasproveedor> ListaCatfacturasproveedor { get; set; }



    }
   

}