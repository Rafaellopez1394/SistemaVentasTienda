using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CapaDatos;
using CapaModelo;

namespace VentasWeb.Controllers
{
    [Authorize]
    public class InventarioInicialController : Controller
    {
        private readonly CD_InventarioInicial cdInventario = new CD_InventarioInicial();
        private const long MAX_FILE_SIZE = 10 * 1024 * 1024; // 10 MB

        /// <summary>
        /// Valida la sesión del usuario y verifica permisos
        /// </summary>
        private bool ValidarSesionYPermisos(out string mensaje)
        {
            mensaje = string.Empty;

            // Validar que la sesión existe
            if (Session["Usuario"] == null || Session["SucursalID"] == null)
            {
                mensaje = "Sesión expirada. Por favor, inicie sesión nuevamente.";
                return false;
            }

            // Validar rol (solo Admin y Gerente pueden usar este módulo)
            var rolID = Session["RolID"] != null ? Convert.ToInt32(Session["RolID"]) : 0;
            if (rolID != 1 && rolID != 2) // 1=Admin, 2=Gerente
            {
                mensaje = "No tiene permisos para acceder a este módulo. Solo Administradores y Gerentes.";
                return false;
            }

            return true;
        }

        // GET: InventarioInicial
        public ActionResult Index()
        {
            try
            {
                // DEBUG: Verificar valores de sesión
                System.Diagnostics.Debug.WriteLine($"Session[Usuario]: {Session["Usuario"]}");
                System.Diagnostics.Debug.WriteLine($"Session[RolID]: {Session["RolID"]}");
                System.Diagnostics.Debug.WriteLine($"Session[SucursalID]: {Session["SucursalID"]}");
                
                // Validar sesión y permisos
                if (!ValidarSesionYPermisos(out string mensaje))
                {
                    // Mostrar el error de forma visible
                    var mensajeDebug = $"{mensaje}<br><br>DEBUG:<br>Usuario: {Session["Usuario"]}<br>RolID: {Session["RolID"]}<br>SucursalID: {Session["SucursalID"]}";
                    return Content($"<script>alert('{mensaje}\\n\\nDEBUG:\\nUsuario: {Session["Usuario"]}\\nRolID: {Session["RolID"]}\\nSucursalID: {Session["SucursalID"]}'); window.location.href='{Url.Action("Index", "Home")}';</script>", "text/html");
                }

                int sucursalID = Convert.ToInt32(Session["SucursalID"]);
                
                // Verificar si hay una carga activa
                var cargaActiva = cdInventario.ObtenerCargaActiva(sucursalID);
                
                if (cargaActiva != null)
                {
                    // Redirigir a continuar con la carga activa
                    return RedirectToAction("Cargar", new { id = cargaActiva.CargaID });
                }

                // Mostrar historial de cargas
                var historial = cdInventario.ObtenerHistorial();
                return View(historial);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar página: " + ex.Message;
                return View(new List<InventarioInicial>());
            }
        }

        // GET: Iniciar nueva carga
        public ActionResult NuevaCarga()
        {
            // Validar sesión y permisos
            if (!ValidarSesionYPermisos(out string mensaje))
            {
                TempData["Error"] = mensaje;
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: Iniciar nueva carga
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NuevaCarga(string comentarios)
        {
            try
            {
                string usuario = Session["Usuario"]?.ToString() ?? "admin";
                int sucursalID = Convert.ToInt32(Session["SucursalID"]);

                // Verificar si ya hay una carga activa
                var cargaActiva = cdInventario.ObtenerCargaActiva(sucursalID);
                if (cargaActiva != null)
                {
                    TempData["Warning"] = "Ya existe una carga en proceso. Finalizala antes de crear una nueva.";
                    return RedirectToAction("Cargar", new { id = cargaActiva.CargaID });
                }

                var resultado = cdInventario.IniciarCarga(usuario, sucursalID, comentarios);

                if ((bool)resultado["Resultado"])
                {
                    int cargaID = (int)resultado["CargaID"];
                    TempData["Success"] = "Carga iniciada correctamente. Ahora agrega productos.";
                    return RedirectToAction("Cargar", new { id = cargaID });
                }
                else
                {
                    TempData["Error"] = resultado["Mensaje"].ToString();
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al iniciar carga: " + ex.Message;
                return View();
            }
        }

        // POST: Importar desde CSV
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportarCSV(System.Web.HttpPostedFileBase archivo, string comentarios)
        {
            try
            {
                // Validar sesión y permisos
                if (!ValidarSesionYPermisos(out string mensaje))
                {
                    TempData["Error"] = mensaje;
                    return RedirectToAction("Index", "Home");
                }

                if (archivo == null || archivo.ContentLength == 0)
                {
                    TempData["Error"] = "Selecciona un archivo CSV válido.";
                    return RedirectToAction("Index");
                }

                // Validar tamaño de archivo (máximo 10 MB)
                if (archivo.ContentLength > MAX_FILE_SIZE)
                {
                    TempData["Error"] = string.Format("El archivo es demasiado grande. Máximo permitido: {0} MB. Tamaño del archivo: {1:N2} MB",
                        MAX_FILE_SIZE / 1024 / 1024,
                        archivo.ContentLength / 1024.0 / 1024.0);
                    return RedirectToAction("Index");
                }

                // Validar extensión de archivo
                var extension = System.IO.Path.GetExtension(archivo.FileName)?.ToLower();
                if (extension != ".csv")
                {
                    TempData["Error"] = "Solo se permiten archivos CSV (.csv). Archivo recibido: " + extension;
                    return RedirectToAction("Index");
                }

                string usuario = Session["Usuario"]?.ToString() ?? "admin";
                int sucursalID = Convert.ToInt32(Session["SucursalID"]);
                string nombreArchivo = System.IO.Path.GetFileName(archivo.FileName);

                // Verificar si ya hay una carga activa
                var cargaActiva = cdInventario.ObtenerCargaActiva(sucursalID);
                if (cargaActiva != null)
                {
                    TempData["Warning"] = "Ya existe una carga en proceso. Finalizala antes de importar.";
                    return RedirectToAction("Cargar", new { id = cargaActiva.CargaID });
                }

                // Leer archivo CSV
                var lineas = new List<string>();
                using (var reader = new System.IO.StreamReader(archivo.InputStream, System.Text.Encoding.UTF8))
                {
                    string linea;
                    while ((linea = reader.ReadLine()) != null)
                    {
                        lineas.Add(linea);
                    }
                }

                if (lineas.Count < 2) // Al menos encabezado + 1 fila
                {
                    TempData["Error"] = "El archivo CSV está vacío o no tiene productos.";
                    return RedirectToAction("Index");
                }

                // Validar límite de filas (máximo 5000 productos)
                const int MAX_FILAS = 5000;
                if (lineas.Count > MAX_FILAS + 1) // +1 por el encabezado
                {
                    TempData["Error"] = string.Format("El archivo CSV tiene demasiadas filas. Máximo permitido: {0} productos. El archivo tiene: {1} filas.", 
                        MAX_FILAS, lineas.Count - 1);
                    return RedirectToAction("Index");
                }

                // Validar formato CSV (encabezado con columnas requeridas)
                var encabezado = lineas[0].ToLower();
                var columnasRequeridas = new[] { "codigo", "producto", "cantidad", "costo" };
                var columnasFaltantes = columnasRequeridas.Where(col => !encabezado.Contains(col)).ToList();
                
                if (columnasFaltantes.Any())
                {
                    TempData["Error"] = "Formato CSV inválido. Faltan columnas requeridas: " + string.Join(", ", columnasFaltantes) + 
                        ". Descarga la plantilla para ver el formato correcto.";
                    return RedirectToAction("Index");
                }

                // Log de auditoría en comentarios
                string comentariosConAuditoria = string.Format(
                    "[IMPORTACIÓN CSV] Archivo: {0} | Tamaño: {1:N2} KB | Usuario: {2} | Fecha: {3:dd/MM/yyyy HH:mm:ss} | Comentarios: {4}",
                    nombreArchivo,
                    archivo.ContentLength / 1024.0,
                    usuario,
                    DateTime.Now,
                    comentarios ?? "Sin comentarios"
                );

                // Iniciar nueva carga con auditoría
                var resultadoCarga = cdInventario.IniciarCarga(usuario, sucursalID, comentariosConAuditoria);
                if (!(bool)resultadoCarga["Resultado"])
                {
                    TempData["Error"] = "Error al iniciar carga: " + resultadoCarga["Mensaje"];
                    return RedirectToAction("Index");
                }

                int cargaID = (int)resultadoCarga["CargaID"];

                // Procesar líneas (saltar encabezado)
                int productosAgregados = 0;
                int errores = 0;
                var mensajesError = new List<string>();

                for (int i = 1; i < lineas.Count; i++)
                {
                    try
                    {
                        var linea = lineas[i];
                        if (string.IsNullOrWhiteSpace(linea)) continue;

                        // Parsear CSV (simple, maneja comillas básicas)
                        var campos = ParsearLineaCSV(linea);

                        if (campos.Length < 7) continue;

                        int productoID = int.Parse(campos[0]);
                        decimal cantidad = decimal.Parse(campos[4]);
                        decimal costo = decimal.Parse(campos[5]);
                        decimal precio = decimal.Parse(campos[6]);
                        string comentario = campos.Length > 7 ? campos[7] : "";

                        // Solo agregar si tiene cantidad > 0
                        if (cantidad > 0 && costo > 0 && precio > 0)
                        {
                            var resultado = cdInventario.AgregarProducto(
                                cargaID, productoID, cantidad, costo, precio, null, comentario);

                            if ((bool)resultado["Resultado"])
                            {
                                productosAgregados++;
                            }
                            else
                            {
                                errores++;
                                mensajesError.Add($"Línea {i + 1}: {resultado["Mensaje"]}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errores++;
                        mensajesError.Add($"Línea {i + 1}: {ex.Message}");
                    }
                }

                if (productosAgregados == 0)
                {
                    TempData["Error"] = "No se pudo agregar ningún producto. Verifica que el archivo tenga cantidades > 0.";
                    return RedirectToAction("Cargar", new { id = cargaID });
                }

                var mensajeResultado = $"Importación completada: {productosAgregados} productos agregados";
                if (errores > 0)
                {
                    mensajeResultado += $", {errores} errores. ";
                    if (mensajesError.Count <= 5)
                    {
                        mensajeResultado += string.Join("; ", mensajesError);
                    }
                }

                TempData["Success"] = mensajeResultado;
                return RedirectToAction("Cargar", new { id = cargaID });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al importar: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // Método auxiliar para parsear líneas CSV
        private string[] ParsearLineaCSV(string linea)
        {
            var campos = new List<string>();
            bool dentroComillas = false;
            var campoActual = new System.Text.StringBuilder();

            for (int i = 0; i < linea.Length; i++)
            {
                char c = linea[i];

                if (c == '"')
                {
                    dentroComillas = !dentroComillas;
                }
                else if (c == ',' && !dentroComillas)
                {
                    campos.Add(campoActual.ToString().Trim());
                    campoActual.Clear();
                }
                else
                {
                    campoActual.Append(c);
                }
            }

            campos.Add(campoActual.ToString().Trim());
            return campos.ToArray();
        }

        // GET: Cargar productos
        public ActionResult Cargar(int id)
        {
            try
            {
                int sucursalID = Convert.ToInt32(Session["SucursalID"]);
                
                // Verificar que la carga existe y pertenece a la sucursal
                var carga = cdInventario.ObtenerCargaActiva(sucursalID);
                
                if (carga == null || carga.CargaID != id)
                {
                    TempData["Error"] = "La carga no existe o ya fue finalizada.";
                    return RedirectToAction("Index");
                }

                ViewBag.CargaID = id;
                ViewBag.Carga = carga;

                // Obtener productos ya agregados a esta carga
                var productosEnCarga = cdInventario.ObtenerProductos(id);
                
                return View(productosEnCarga);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar datos: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: Buscar productos para agregar
        [HttpGet]
        public JsonResult BuscarProductos(string termino)
        {
            try
            {
                var todosProductos = cdInventario.ObtenerProductos(null);
                
                var productosFiltrados = todosProductos
                    .Cast<ProductoInventarioInicial>()
                    .Where(p => 
                        p.NombreProducto.ToLower().Contains(termino.ToLower()) ||
                        p.CodigoInterno.ToLower().Contains(termino.ToLower()))
                    .Take(20)
                    .Select(p => new
                    {
                        ProductoID = p.ProductoID,
                        NombreProducto = p.NombreProducto,
                        CodigoInterno = p.CodigoInterno,
                        StockActual = p.StockActual
                    })
                    .ToList();

                return Json(productosFiltrados, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Agregar producto a la carga
        [HttpPost]
        public JsonResult AgregarProducto(
            int cargaID, 
            int productoID, 
            decimal cantidad, 
            decimal costoUnitario, 
            decimal precioVenta,
            string fechaCaducidad = null,
            string comentarios = null)
        {
            try
            {
                // Validar sesión y permisos
                if (!ValidarSesionYPermisos(out string mensaje))
                {
                    return Json(new { Resultado = false, Mensaje = mensaje });
                }

                // Validar parámetros
                if (cargaID <= 0 || productoID <= 0)
                {
                    return Json(new { Resultado = false, Mensaje = "Parámetros inválidos: CargaID y ProductoID deben ser mayores a 0." });
                }

                if (cantidad <= 0)
                {
                    return Json(new { Resultado = false, Mensaje = "La cantidad debe ser mayor a 0." });
                }

                if (costoUnitario < 0 || precioVenta < 0)
                {
                    return Json(new { Resultado = false, Mensaje = "El costo y precio deben ser mayores o iguales a 0." });
                }

                // Validar que el producto no esté duplicado en esta carga
                var productosEnCarga = cdInventario.ObtenerProductos(cargaID);
                var productoDuplicado = productosEnCarga.Cast<ProductoInventarioInicial>()
                    .FirstOrDefault(p => p.ProductoID == productoID);
                
                if (productoDuplicado != null)
                {
                    return Json(new { 
                        Resultado = false, 
                        Mensaje = "Este producto ya está agregado en la carga actual. Edita la cantidad existente o elimínalo antes de agregarlo nuevamente." 
                    });
                }

                DateTime? fechaCad = null;
                if (!string.IsNullOrEmpty(fechaCaducidad))
                {
                    fechaCad = DateTime.Parse(fechaCaducidad);
                }

                var resultado = cdInventario.AgregarProducto(
                    cargaID, 
                    productoID, 
                    cantidad, 
                    costoUnitario, 
                    precioVenta, 
                    fechaCad, 
                    comentarios);

                return Json(resultado);
            }
            catch (Exception ex)
            {
                return Json(new 
                { 
                    Resultado = false, 
                    Mensaje = "Error: " + ex.Message 
                });
            }
        }

        // POST: Eliminar producto de la carga
        [HttpPost]
        public JsonResult EliminarProducto(int detalleID)
        {
            try
            {
                // Validar sesión y permisos
                if (!ValidarSesionYPermisos(out string mensaje))
                {
                    return Json(new { Resultado = false, Mensaje = mensaje });
                }

                if (detalleID <= 0)
                {
                    return Json(new { Resultado = false, Mensaje = "DetalleID inválido." });
                }

                var resultado = cdInventario.EliminarProducto(detalleID);
                return Json(resultado);
            }
            catch (Exception ex)
            {
                return Json(new 
                { 
                    Resultado = false, 
                    Mensaje = "Error: " + ex.Message 
                });
            }
        }

        // GET: Confirmar finalización
        public ActionResult ConfirmarFinalizacion(int id)
        {
            try
            {
                // Validar sesión y permisos
                if (!ValidarSesionYPermisos(out string mensaje))
                {
                    TempData["Error"] = mensaje;
                    return RedirectToAction("Index", "Home");
                }

                int sucursalID = Convert.ToInt32(Session["SucursalID"]);
                var carga = cdInventario.ObtenerCargaActiva(sucursalID);
                
                if (carga == null || carga.CargaID != id)
                {
                    TempData["Error"] = "La carga no existe o ya fue finalizada.";
                    return RedirectToAction("Index");
                }

                ViewBag.Carga = carga;
                var productos = cdInventario.ObtenerProductos(id);
                
                return View(productos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: Finalizar carga y aplicar al inventario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinalizarCarga(int cargaID)
        {
            try
            {
                // Validar sesión y permisos
                if (!ValidarSesionYPermisos(out string mensaje))
                {
                    TempData["Error"] = mensaje;
                    return RedirectToAction("Index", "Home");
                }

                string usuario = Session["Usuario"]?.ToString() ?? "admin";
                int sucursalID = Convert.ToInt32(Session["SucursalID"]);
                
                // PASO 1: Limpiar inventario existente (solo de la sucursal actual)
                var resultadoLimpieza = cdInventario.LimpiarInventarioSucursal(sucursalID);
                
                // PASO 2: Aplicar el nuevo inventario inicial
                var resultado = cdInventario.FinalizarCarga(cargaID, usuario);

                if ((bool)resultado["Resultado"])
                {
                    TempData["Success"] = string.Format(
                        "✓ Inventario anterior eliminado: {0} lotes<br>" +
                        "✓ Inventario inicial aplicado correctamente<br>" +
                        "Productos: {1}, Unidades: {2:N2}, Valor Total: ${3:N2}",
                        resultadoLimpieza["LotesEliminados"],
                        resultado["ProductosAplicados"],
                        resultado["UnidadesAplicadas"],
                        resultado["ValorTotal"]);
                        
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = resultado["Mensaje"].ToString();
                    return RedirectToAction("Cargar", new { id = cargaID });
                }
            }
            catch (Exception ex)
            {
                // Log del error para diagnóstico
                System.Diagnostics.Debug.WriteLine($"[ERROR] FinalizarCarga - Usuario: {Session["Usuario"]}, CargaID: {cargaID}, Error: {ex.Message}");
                
                TempData["Error"] = "Error al finalizar: " + ex.Message;
                return RedirectToAction("Cargar", new { id = cargaID });
            }
        }

        // GET: Ver detalle de carga finalizada
        public ActionResult Detalle(int id)
        {
            try
            {
                // Validar sesión y permisos
                if (!ValidarSesionYPermisos(out string mensaje))
                {
                    TempData["Error"] = mensaje;
                    return RedirectToAction("Index", "Home");
                }

                var productos = cdInventario.ObtenerProductos(id);
                
                if (productos.Count == 0)
                {
                    TempData["Error"] = "La carga no existe o no tiene productos.";
                    return RedirectToAction("Index");
                }

                ViewBag.CargaID = id;
                
                return View(productos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: Cancelar carga activa (elimina la carga sin aplicar al inventario)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelarCarga(int cargaID)
        {
            try
            {
                // Validar sesión y permisos
                if (!ValidarSesionYPermisos(out string mensaje))
                {
                    TempData["Error"] = mensaje;
                    return RedirectToAction("Index", "Home");
                }

                int sucursalID = Convert.ToInt32(Session["SucursalID"]);
                var carga = cdInventario.ObtenerCargaActiva(sucursalID);

                // Validar que la carga existe y está activa
                if (carga == null || carga.CargaID != cargaID)
                {
                    TempData["Error"] = "La carga no existe o ya fue finalizada.";
                    return RedirectToAction("Index");
                }

                // Validar que la carga no está finalizada
                if (!carga.Activo)
                {
                    TempData["Error"] = "No se puede cancelar una carga ya finalizada.";
                    return RedirectToAction("Index");
                }

                // Intentar cancelar la carga (eliminar de BD)
                var resultado = cdInventario.CancelarCarga(cargaID);

                if ((bool)resultado["Resultado"])
                {
                    TempData["Success"] = "Carga cancelada correctamente. Todos los productos de esta carga fueron eliminados.";
                }
                else
                {
                    TempData["Error"] = resultado["Mensaje"].ToString();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log del error para diagnóstico
                System.Diagnostics.Debug.WriteLine($"[ERROR] CancelarCarga - Usuario: {Session["Usuario"]}, CargaID: {cargaID}, Error: {ex.Message}");
                
                TempData["Error"] = "Error al cancelar la carga: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: Exportar plantilla CSV
        public ActionResult DescargarPlantilla()
        {
            try
            {                // Validar sesión y permisos
                if (!ValidarSesionYPermisos(out string mensaje))
                {
                    TempData["Error"] = mensaje;
                    return RedirectToAction("Index", "Home");
                }                var productos = cdInventario.ObtenerProductos(null);
                
                if (productos == null || productos.Count == 0)
                {
                    TempData["Warning"] = "No hay productos en el catálogo para exportar.";
                    return RedirectToAction("Index");
                }
                
                // Crear CSV con UTF-8 BOM para Excel
                var csv = new System.Text.StringBuilder();
                
                // Encabezados
                csv.AppendLine("ProductoID,CodigoInterno,NombreProducto,StockActual,CantidadNueva,CostoUnitario,PrecioVenta,Comentarios");
                
                // Agregar productos
                foreach (var item in productos)
                {
                    var producto = item as ProductoInventarioInicial;
                    if (producto != null)
                    {
                        // Escapar comillas en el nombre
                        var nombreLimpio = producto.NombreProducto.Replace("\"", "\"\"");
                        csv.AppendLine($"{producto.ProductoID},{producto.CodigoInterno},\"{nombreLimpio}\",{producto.StockActual},0,0.00,0.00,");
                    }
                }
                
                // Agregar BOM UTF-8 para que Excel lo reconozca correctamente
                var preamble = System.Text.Encoding.UTF8.GetPreamble();
                var content = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                var bytesWithBOM = new byte[preamble.Length + content.Length];
                System.Buffer.BlockCopy(preamble, 0, bytesWithBOM, 0, preamble.Length);
                System.Buffer.BlockCopy(content, 0, bytesWithBOM, preamble.Length, content.Length);
                
                // Descargar archivo
                var fileName = $"PlantillaInventarioInicial_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                return File(bytesWithBOM, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al generar plantilla: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
