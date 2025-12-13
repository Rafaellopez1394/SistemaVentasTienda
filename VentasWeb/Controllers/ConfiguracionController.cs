using System;
using System.Web.Mvc;
using CapaDatos;
using CapaModelo;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;

namespace VentasWeb.Controllers
{
    public class ConfiguracionController : Controller
    {
        // GET: Configuracion
        public ActionResult Index()
        {
            return View();
        }

        // POST: Imprimir ticket directamente a la impresora configurada
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ImprimirTicketDirecto(string contenidoTicket)
        {
            try
            {
                // Log para debug
                System.Diagnostics.Debug.WriteLine("=== Iniciando ImprimirTicketDirecto ===");
                
                if (string.IsNullOrEmpty(contenidoTicket))
                {
                    System.Diagnostics.Debug.WriteLine("Contenido vacio");
                    return Json(new { success = false, mensaje = "No hay contenido para imprimir" });
                }

                System.Diagnostics.Debug.WriteLine("Obteniendo impresora configurada...");
                ConfiguracionImpresora impresora = null;
                
                try
                {
                    impresora = CD_Configuracion.Instancia.ObtenerImpresoraPorTipo("TICKET");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error al obtener impresora: " + ex.Message);
                    return Json(new { success = false, mensaje = "Error al obtener configuracion de impresora: " + ex.Message });
                }
                
                if (impresora == null)
                {
                    System.Diagnostics.Debug.WriteLine("Impresora es null");
                    return Json(new { success = false, mensaje = "No hay impresora configurada. Ve a Configuracion > Impresoras y selecciona una impresora." });
                }
                
                if (string.IsNullOrEmpty(impresora.NombreImpresora))
                {
                    System.Diagnostics.Debug.WriteLine("Nombre de impresora vacio");
                    return Json(new { success = false, mensaje = "El nombre de la impresora esta vacio" });
                }

                System.Diagnostics.Debug.WriteLine("Impresora encontrada: " + impresora.NombreImpresora);

                // Crear documento de impresion
                PrintDocument printDoc = new PrintDocument();
                printDoc.PrinterSettings.PrinterName = impresora.NombreImpresora;

                if (!printDoc.PrinterSettings.IsValid)
                {
                    System.Diagnostics.Debug.WriteLine("Impresora no valida: " + impresora.NombreImpresora);
                    return Json(new { success = false, mensaje = "La impresora '" + impresora.NombreImpresora + "' no esta disponible. Verifica que este instalada y encendida." });
                }

                // Convertir HTML a texto plano
                string textoPlano = ConvertirHtmlATextoTicket(contenidoTicket);
                System.Diagnostics.Debug.WriteLine("Texto convertido, lineas: " + textoPlano.Split('\n').Length);
                
                // Variables para el evento PrintPage
                string[] lineasTexto = textoPlano.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                int anchoMM = impresora.AnchoPapel > 0 ? impresora.AnchoPapel : 58;
                
                // Convertir mm a pixeles (aprox 3.78 px por mm a 96 DPI)
                float anchoPapelPx = anchoMM * 3.78f;
                
                printDoc.PrintPage += (sender, e) =>
                {
                    try
                    {
                        // Configurar DPI para impresora termica
                        e.Graphics.PageUnit = System.Drawing.GraphicsUnit.Millimeter;
                        
                        // Fuentes en puntos (optimizadas para 58mm)
                        Font fuentePequena = new Font("Arial", 6f, FontStyle.Regular);
                        Font fuenteNormal = new Font("Arial", 7f, FontStyle.Regular);
                        Font fuenteNegrita = new Font("Arial", 8f, FontStyle.Bold);
                        Font fuenteTitulo = new Font("Arial", 9f, FontStyle.Bold);
                        Font fuenteGrande = new Font("Arial", 10f, FontStyle.Bold);
                        
                        float y = 2f; // Posicion Y en mm
                        float margenIzq = 2f; // Margen izquierdo en mm
                        float anchoUtil = anchoMM - (margenIzq * 2); // Ancho util en mm
                        
                        foreach (string linea in lineasTexto)
                        {
                            string lineaTrim = linea.Trim();
                            
                            if (string.IsNullOrWhiteSpace(lineaTrim))
                            {
                                y += 2f; // 2mm de espacio
                                continue;
                            }
                            
                            // Seleccionar fuente segun contenido
                            Font fuenteActual = fuenteNormal;
                            bool centrar = false;
                            float espacioExtra = 0f;
                            
                            // SEPARADORES
                            if (lineaTrim.Contains("---") || lineaTrim.Contains("==="))
                            {
                                fuenteActual = fuentePequena;
                                centrar = true;
                            }
                            // NOMBRE DEL NEGOCIO (suele ser la linea mas larga al inicio)
                            else if (!lineaTrim.Contains(":") && lineaTrim.Length > 15 && y < 15f)
                            {
                                fuenteActual = fuenteTitulo;
                                centrar = true;
                                espacioExtra = 0.5f;
                            }
                            // RFC, TEL, DIRECCION
                            else if (lineaTrim.StartsWith("RFC:") || lineaTrim.StartsWith("Tel:"))
                            {
                                fuenteActual = fuentePequena;
                                centrar = true;
                            }
                            // TICKET DE VENTA
                            else if (lineaTrim.Contains("TICKET DE VENTA"))
                            {
                                fuenteActual = fuenteNegrita;
                                centrar = true;
                                y += 2f; // Espacio antes
                                espacioExtra = 1f;
                            }
                            // TOTAL
                            else if (lineaTrim.StartsWith("TOTAL:") || lineaTrim.Contains("TOTAL:"))
                            {
                                fuenteActual = fuenteGrande;
                                y += 2f; // Espacio antes del total
                                espacioExtra = 1f;
                            }
                            // SUBTOTAL, IVA
                            else if (lineaTrim.Contains("Subtotal:") || lineaTrim.Contains("IVA"))
                            {
                                fuenteActual = fuenteNormal;
                            }
                            // MENSAJES FINALES
                            else if (lineaTrim.Contains("Gracias") || lineaTrim.Contains("Conserve") || lineaTrim.Contains("***"))
                            {
                                fuenteActual = fuentePequena;
                                centrar = true;
                                espacioExtra = 1f;
                            }
                            
                            // Calcular el rectangulo para el texto
                            RectangleF rect = new RectangleF(margenIzq, y, anchoUtil, 20f);
                            
                            // Formato de alineacion
                            StringFormat formato = new StringFormat();
                            formato.Alignment = centrar ? StringAlignment.Center : StringAlignment.Near;
                            formato.LineAlignment = StringAlignment.Near;
                            formato.FormatFlags = StringFormatFlags.NoWrap;
                            formato.Trimming = StringTrimming.Character;
                            
                            // Dibujar el texto
                            e.Graphics.DrawString(lineaTrim, fuenteActual, Brushes.Black, rect, formato);
                            
                            // Medir el alto real del texto para calcular la siguiente posicion Y
                            SizeF medida = e.Graphics.MeasureString(lineaTrim, fuenteActual, (int)anchoUtil);
                            float altoMM = medida.Height / e.Graphics.DpiY * 25.4f; // Convertir a mm
                            
                            // Avanzar Y con el alto del texto + espaciado minimo
                            y += Math.Max(altoMM, fuenteActual.Size * 0.35f) + 0.8f + espacioExtra;
                        }
                        
                        // Espacio final para corte
                        y += 10f;
                        
                        e.HasMorePages = false;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error en PrintPage: " + ex.Message);
                    }
                };

                // Imprimir el documento
                System.Diagnostics.Debug.WriteLine("Enviando a imprimir...");
                printDoc.Print();
                System.Diagnostics.Debug.WriteLine("Impresion completada");
                
                return Json(new { success = true, mensaje = "Ticket enviado a impresora " + impresora.NombreImpresora });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR GENERAL: " + ex.ToString());
                string mensajeCompleto = ex.Message;
                if (ex.InnerException != null)
                {
                    mensajeCompleto += " | Inner: " + ex.InnerException.Message;
                }
                return Json(new { success = false, mensaje = "Error: " + mensajeCompleto });
            }
        }

        // Convertir HTML del ticket a texto plano para impresora termica con formato
        private string ConvertirHtmlATextoTicket(string html)
        {
            if (string.IsNullOrEmpty(html))
                return "";

            string texto = html;
            
            // Reemplazar divs y breaks por saltos de linea
            texto = System.Text.RegularExpressions.Regex.Replace(texto, @"</div>", "\n", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            texto = System.Text.RegularExpressions.Regex.Replace(texto, @"<br\s*/?>", "\n", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            texto = System.Text.RegularExpressions.Regex.Replace(texto, @"</p>", "\n", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
            // Remover todos los tags HTML
            texto = System.Text.RegularExpressions.Regex.Replace(texto, @"<[^>]+>", "");
            
            // Decodificar entidades HTML
            texto = System.Net.WebUtility.HtmlDecode(texto);
            
            // Limpiar multiples espacios en blanco pero mantener estructura
            var lineas = texto.Split(new[] { '\n' }, StringSplitOptions.None);
            var lineasLimpias = new List<string>();
            
            foreach (var linea in lineas)
            {
                string lineaLimpia = linea.Trim();
                // Mantener lineas vacias para separadores visuales
                if (!string.IsNullOrWhiteSpace(lineaLimpia) || lineasLimpias.Count > 0)
                {
                    lineasLimpias.Add(lineaLimpia);
                }
            }
            
            return string.Join("\n", lineasLimpias);
        }

        // GET: Configuracion de Impresoras
        public ActionResult Impresoras()
        {
            var impresoras = CD_Configuracion.Instancia.ObtenerImpresoras();
            return View(impresoras);
        }

        // POST: Guardar configuracion de impresora
        [HttpPost]
        public JsonResult GuardarImpresora(ConfiguracionImpresora config)
        {
            try
            {
                config.Usuario = User.Identity.Name ?? "system";
                bool resultado = CD_Configuracion.Instancia.GuardarImpresora(config);
                return Json(new { success = resultado, mensaje = resultado ? "Configuracion guardada" : "Error al guardar" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

        // GET: Obtener impresora por defecto para tickets
        [HttpGet]
        public JsonResult ObtenerImpresoraTickets()
        {
            try
            {
                var impresora = CD_Configuracion.Instancia.ObtenerImpresoraPorTipo("TICKET");
                return Json(new { success = true, data = impresora }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Listar impresoras del sistema
        [HttpGet]
        public JsonResult ListarImpresorasSistema()
        {
            try
            {
                var impresoras = new List<string>();
                foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                {
                    impresoras.Add(printer);
                }
                return Json(new { success = true, data = impresoras }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Configuracion General
        public ActionResult General()
        {
            var config = CD_Configuracion.Instancia.ObtenerConfiguracionGeneral();
            return View(config);
        }

        // POST: Guardar configuracion general
        [HttpPost]
        public JsonResult GuardarConfigGeneral(ConfiguracionGeneral config)
        {
            try
            {
                config.Usuario = User.Identity.Name ?? "system";
                bool resultado = CD_Configuracion.Instancia.GuardarConfigGeneral(config);
                return Json(new { success = resultado, mensaje = resultado ? "Configuracion guardada" : "Error al guardar" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

        // GET: Obtener configuracion del negocio para ticket
        [HttpGet]
        public JsonResult ObtenerDatosNegocio()
        {
            try
            {
                var datos = CD_Configuracion.Instancia.ObtenerDatosNegocio();
                return Json(new { success = true, data = datos }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Obtener configuracion general completa
        [HttpGet]
        public JsonResult ObtenerConfigGeneralJSON()
        {
            try
            {
                var config = CD_Configuracion.Instancia.ObtenerConfiguracionGeneral();
                return Json(new { success = true, data = config }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Obtener configuracion de tickets
        [HttpGet]
        public JsonResult ObtenerConfigTickets()
        {
            try
            {
                var config = CD_Configuracion.Instancia.ObtenerConfiguracionGeneral();
                return Json(new { 
                    success = true, 
                    data = new {
                        config.MensajeTicket,
                        config.ImprimirTicketAutomatico,
                        config.MostrarLogoEnTicket
                    } 
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Guardar configuracion de tickets
        [HttpPost]
        public JsonResult GuardarConfigTickets(string mensajeTicket, bool imprimirAuto, bool mostrarLogo)
        {
            try
            {
                bool resultado = CD_Configuracion.Instancia.ActualizarConfigTickets(mensajeTicket, imprimirAuto, mostrarLogo, User.Identity.Name ?? "system");
                return Json(new { success = resultado, mensaje = resultado ? "Configuracion de tickets guardada" : "Error al guardar" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }
    }
}
