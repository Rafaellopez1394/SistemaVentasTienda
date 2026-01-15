// Controllers/CompraController.cs
using System;
using System.Web;
using System.Web.Mvc;
using CapaDatos;
using CapaDatos.Utilidades;
using CapaModelo;
using System.Collections.Generic;
using System.Linq;

namespace VentasWeb.Controllers
{
    public class CompraController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        // Obtener todas las compras (para reporte)
        [HttpGet]
        public JsonResult Obtener()
        {
            try
            {
                var lista = CD_Compra.Instancia.ObtenerTodas();
                return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { data = new List<Compra>(), error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Buscar proveedor por RFC o Razón Social
        [HttpGet]
        public JsonResult BuscarProveedor(string texto)
        {
            var proveedores = CD_Proveedor.Instancia.BuscarPorNombreORFC(texto);
            return Json(proveedores, JsonRequestBehavior.AllowGet);
        }

        // Registrar compra completa con lotes
        [HttpPost]
        public JsonResult RegistrarCompra(Compra compra)
        {
            compra.Usuario = User.Identity.Name ?? "system";
            
            // Asignar sucursal activa a la compra
            if (Session["SucursalActiva"] != null)
            {
                compra.SucursalID = (int)Session["SucursalActiva"];
            }

            bool resultado = false;
            string mensaje = "";

            try
            {
                resultado = CD_Compra.Instancia.RegistrarCompraConLotes(compra, compra.Usuario);
                mensaje = resultado ? "Compra registrada correctamente" : "Error al registrar la compra";
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = ex.Message.Contains("RAISERROR") ? ex.Message : "Error interno del servidor";
            }

            return Json(new { resultado, mensaje }, JsonRequestBehavior.AllowGet);
        }

        // Obtener productos para autocompletado
        [HttpGet]
        public JsonResult ObtenerProductos(string termino = "")
        {
            // Obtener sucursal activa
            int sucursalID = Session["SucursalActiva"] != null 
                ? (int)Session["SucursalActiva"] 
                : 1;
                
            var productos = CD_Producto.Instancia.BuscarProductosPorSucursal(termino, sucursalID);
            return Json(productos, JsonRequestBehavior.AllowGet);
        }

        // ===== NUEVOS MÉTODOS PARA XML =====

        /// <summary>
        /// Vista de prueba para diagnóstico de carga XML
        /// </summary>
        public ActionResult TestXML()
        {
            return View();
        }

        /// <summary>
        /// Vista para cargar y procesar facturas XML
        /// </summary>
        public ActionResult CargarXML()
        {
            return View();
        }

        /// <summary>
        /// Procesa el archivo XML cargado y extrae los datos
        /// </summary>
        [HttpPost]
        public JsonResult ProcesarXML()
        {
            try
            {
                // Log 1: Verificar cuántos archivos llegaron
                System.Diagnostics.Debug.WriteLine($"Request.Files.Count: {Request.Files.Count}");
                
                if (Request.Files.Count == 0)
                {
                    return Json(new { success = false, mensaje = "No se recibió ningún archivo" });
                }

                // Log 2: Listar todos los nombres de archivos
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var key = Request.Files.AllKeys[i];
                    var file = Request.Files[i];
                    System.Diagnostics.Debug.WriteLine($"File[{i}]: Key={key}, FileName={file?.FileName}, Length={file?.ContentLength}");
                }

                HttpPostedFileBase archivo = Request.Files["file"];
                
                if (archivo == null)
                {
                    // Intentar obtener el primer archivo si no está con el nombre "file"
                    archivo = Request.Files[0];
                    System.Diagnostics.Debug.WriteLine($"Archivo obtenido por índice[0]: {archivo?.FileName}");
                }
                
                if (archivo == null)
                {
                    return Json(new { success = false, mensaje = "No se pudo obtener el archivo del formulario. Keys disponibles: " + string.Join(", ", Request.Files.AllKeys) });
                }

                System.Diagnostics.Debug.WriteLine($"Archivo recibido: {archivo.FileName}, ContentLength: {archivo.ContentLength}");

                // FUNCIONALIDAD DE IMPORTACIÓN DE CFDI ELIMINADA
                return Json(new { 
                    success = false, 
                    mensaje = "Funcionalidad de importación de CFDI de compras eliminada del sistema" 
                });

                /* CÓDIGO ELIMINADO - Parseador de CFDI de compras
                if (archivo.ContentLength == 0)
                {
                    return Json(new { success = false, mensaje = $"El archivo '{archivo.FileName}' está vacío (0 bytes)" });
                }

                // Validar extensión
                string extension = System.IO.Path.GetExtension(archivo.FileName).ToLower();
                if (extension != ".xml")
                {
                    return Json(new { success = false, mensaje = "El archivo debe ser un XML" });
                }

                // Guardar temporalmente
                string carpetaTemp = Server.MapPath("~/App_Data/TempXML");
                if (!System.IO.Directory.Exists(carpetaTemp))
                {
                    System.IO.Directory.CreateDirectory(carpetaTemp);
                }

                string rutaTemporal = System.IO.Path.Combine(carpetaTemp, Guid.NewGuid().ToString() + ".xml");
                System.Diagnostics.Debug.WriteLine($"Guardando archivo en: {rutaTemporal}");
                
                archivo.SaveAs(rutaTemporal);

                // Verificar que el archivo se guardó correctamente
                if (!System.IO.File.Exists(rutaTemporal))
                {
                    return Json(new { success = false, mensaje = "Error al guardar el archivo temporalmente" });
                }

                var fileInfo = new System.IO.FileInfo(rutaTemporal);
                System.Diagnostics.Debug.WriteLine($"Archivo guardado: {fileInfo.Length} bytes");
                
                if (fileInfo.Length == 0)
                {
                    System.IO.File.Delete(rutaTemporal);
                    return Json(new { success = false, mensaje = "El archivo guardado está vacío (0 bytes guardados)" });
                }

                // Validar estructura
                string mensajeValidacion;
                bool esValido = CFDICompraParser.ValidarEstructura(rutaTemporal, out mensajeValidacion);

                if (!esValido)
                {
                    System.IO.File.Delete(rutaTemporal);
                    return Json(new { success = false, mensaje = mensajeValidacion });
                }

                // Parsear XML
                var datosFactura = CFDICompraParser.ParsearXML(rutaTemporal);

                // Preparar datos para la vista
                var resultado = new
                {
                    success = true,
                    rutaTemporal = rutaTemporal,
                    datos = new
                    {
                        // Datos del comprobante
                        serie = datosFactura.Serie,
                        folio = datosFactura.Folio,
                        fecha = datosFactura.Fecha.ToString("yyyy-MM-dd HH:mm:ss"),
                        formaPago = datosFactura.FormaPago,
                        metodoPago = datosFactura.MetodoPago,
                        moneda = datosFactura.Moneda,
                        subTotal = datosFactura.SubTotal,
                        descuento = datosFactura.Descuento,
                        total = datosFactura.Total,
                        uuid = datosFactura.UUID,

                        // Emisor (Proveedor)
                        proveedorRFC = datosFactura.EmisorRFC,
                        proveedorNombre = datosFactura.EmisorNombre,

                        // Conceptos
                        conceptos = datosFactura.Conceptos.Select(c => new
                        {
                            claveProdServ = c.ClaveProdServ,
                            noIdentificacion = c.NoIdentificacion,
                            cantidad = c.Cantidad,
                            claveUnidad = c.ClaveUnidad,
                            unidad = c.Unidad,
                            descripcion = c.Descripcion,
                            valorUnitario = c.ValorUnitario,
                            importe = c.Importe,
                            descuento = c.Descuento,
                            
                            // Impuestos
                            tieneIVA = c.ImpuestosTrasladados != null && c.ImpuestosTrasladados.Count > 0,
                            tasaIVA = c.ImpuestosTrasladados != null && c.ImpuestosTrasladados.Count > 0 
                                ? c.ImpuestosTrasladados[0].TasaOCuota * 100 
                                : 0,
                            
                            // Desglose (inicializado con valores por defecto)
                            factorConversion = 1,
                            cantidadDesglosada = c.Cantidad,
                            precioUnitarioDesglosado = c.ValorUnitario
                        }).ToList()
                    }
                };

                return Json(resultado);
                */
            }
            catch (System.Xml.XmlException xmlEx)
            {
                return Json(new { success = false, mensaje = "Error al leer el archivo XML: " + xmlEx.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error al procesar XML: " + ex.Message + " | Tipo: " + ex.GetType().Name });
            }
        }

        /// <summary>
        /// Registra la compra desde el XML procesado
        /// </summary>
        [HttpPost]
        public JsonResult RegistrarCompraDesdeXML(string rutaXML, List<MapeoProductoXML> mapeos, int sucursalID)
        {
            try
            {
                // Validar sucursal
                if (sucursalID <= 0)
                {
                    return Json(new { success = false, mensaje = "Debe seleccionar una sucursal" });
                }

                // Convertir mapeos a diccionario
                var diccionarioMapeos = new Dictionary<int, ProductoCompraXML>();
                int index = 0;
                foreach (var mapeo in mapeos)
                {
                    diccionarioMapeos[index++] = new ProductoCompraXML
                    {
                        ProductoID = mapeo.ProductoID,
                        NoIdentificacionXML = mapeo.NoIdentificacion,
                        DescripcionXML = mapeo.Descripcion,
                        FactorConversion = mapeo.FactorConversion,
                        PrecioVentaSugerido = mapeo.PrecioVentaSugerido
                    };
                }

                string usuario = User.Identity.Name ?? "system";
                string mensaje;
                
                bool resultado = CD_Compra.Instancia.RegistrarCompraDesdeXML(
                    rutaXML, 
                    diccionarioMapeos,
                    sucursalID,
                    usuario, 
                    out mensaje
                );

                // Eliminar archivo temporal
                if (System.IO.File.Exists(rutaXML))
                {
                    try { System.IO.File.Delete(rutaXML); } catch { }
                }

                return Json(new { success = resultado, mensaje = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error: " + ex.Message });
            }
        }

        /// <summary>
        /// Busca productos por código o descripción para mapear
        /// </summary>
        [HttpGet]
        public JsonResult BuscarProductoParaMapeo(string termino)
        {
            try
            {
                var productos = CD_Producto.Instancia.BuscarProductos(termino);
                
                var resultado = productos.Select(p => new
                {
                    productoID = p.ProductoID,
                    nombreProducto = p.Nombre,
                    codigoProducto = p.CodigoInterno ?? "SIN-CODIGO",
                    precioVenta = 0m, // Se calculará del costo con margen
                    unidadMedida = p.UnidadMedidaBase ?? p.UnidadSAT ?? "PZA"
                }).ToList();

                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }

    /// <summary>
    /// Clase auxiliar para recibir mapeos desde el frontend
    /// </summary>
    public class MapeoProductoXML
    {
        public int ProductoID { get; set; }
        public string NoIdentificacion { get; set; }
        public string Descripcion { get; set; }
        public decimal FactorConversion { get; set; }
        public decimal PrecioVentaSugerido { get; set; }
    }
}