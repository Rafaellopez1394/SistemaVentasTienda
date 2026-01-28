using System;
using System.Web.Mvc;
using CapaDatos;
using CapaModelo;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.IO;

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
                System.Diagnostics.Debug.WriteLine("=== Iniciando ImprimirTicketDirecto ===");
                
                if (string.IsNullOrEmpty(contenidoTicket))
                {
                    return Json(new { success = false, mensaje = "No hay contenido para imprimir" });
                }

                ConfiguracionImpresora impresora = CD_Configuracion.Instancia.ObtenerImpresoraPorTipo("TICKET");
                
                if (impresora == null || string.IsNullOrEmpty(impresora.NombreImpresora))
                {
                    return Json(new { success = false, mensaje = "No hay impresora configurada" });
                }

                // Obtener configuracion del logo
                var config = CD_Configuracion.Instancia.ObtenerConfiguracionGeneral();
                byte[] logoBytes = new byte[0];
                
                if (config != null && config.MostrarLogoEnTicket && !string.IsNullOrEmpty(config.LogoPath))
                {
                    try
                    {
                        string rutaCompleta = Server.MapPath(config.LogoPath);
                        if (System.IO.File.Exists(rutaCompleta))
                        {
                            using (var imagen = System.Drawing.Image.FromFile(rutaCompleta))
                            {
                                // Ancho reducido para mejor centrado visual (4.5 en lugar de 6)
                                int anchoLogo = impresora.AnchoPapel > 0 ? (int)(impresora.AnchoPapel * 4.5) : 260;
                                logoBytes = ImageToEscPosBitmap(imagen, anchoLogo);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error cargando logo: " + ex.Message);
                    }
                }

                // Convertir HTML a texto plano
                string textoPlano = ConvertirHtmlATextoTicket(contenidoTicket);
                
                // Convertir texto a CP850 para ESC/POS
                System.Text.Encoding cp850 = System.Text.Encoding.GetEncoding(850);
                byte[] textoBytes = cp850.GetBytes(textoPlano);
                
                // Crear documento ESC/POS unificado
                using (var ms = new System.IO.MemoryStream())
                {
                    // Inicializar impresora (sin salto de línea inicial)
                    ms.Write(new byte[] { 0x1B, 0x40 }, 0, 2); // ESC @ - inicializar
                    
                    // Agregar logo si existe (sin espacio adicional arriba)
                    if (logoBytes.Length > 0)
                    {
                        ms.Write(logoBytes, 0, logoBytes.Length);
                    }
                    
                    // Agregar texto
                    ms.Write(textoBytes, 0, textoBytes.Length);
                    
                    // Avanzar y cortar
                    ms.Write(new byte[] { 0x0A, 0x0A, 0x0A, 0x0A }, 0, 4); // Saltos de linea
                    ms.Write(new byte[] { 0x1D, 0x56, 0x01 }, 0, 3); // GS V 1 - corte parcial
                    
                    // Enviar a impresora
                    byte[] documento = ms.ToArray();
                    bool exito = SendBytesToPrinter(impresora.NombreImpresora, documento);
                    
                    if (!exito)
                    {
                        return Json(new { success = false, mensaje = "Error al enviar datos a la impresora" });
                    }
                }
                
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
                // NO hacer Trim para preservar espacios de centrado
                string lineaLimpia = linea;
                // Solo eliminar espacios al final, NO al inicio
                lineaLimpia = System.Text.RegularExpressions.Regex.Replace(lineaLimpia, @"\s+$", "");
                
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

        // ===== MÉTODOS PARA IMPRESIÓN RAW ESC/POS =====
        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)] public string pDataType;
        }

        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        private static bool SendBytesToPrinter(string printerName, byte[] bytes)
        {
            IntPtr pUnmanagedBytes = Marshal.AllocCoTaskMem(bytes.Length);
            Marshal.Copy(bytes, 0, pUnmanagedBytes, bytes.Length);
            
            Int32 dwWritten = 0;
            IntPtr hPrinter = IntPtr.Zero;
            DOCINFOA di = new DOCINFOA { pDocName = "Ticket", pDataType = "RAW" };
            bool success = false;

            if (OpenPrinter(printerName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    if (StartPagePrinter(hPrinter))
                    {
                        success = WritePrinter(hPrinter, pUnmanagedBytes, bytes.Length, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return success;
        }

        private static Bitmap InvertBitmap(Bitmap src)
        {
            Bitmap bmp = new Bitmap(src.Width, src.Height);

            for (int y = 0; y < src.Height; y++)
            {
                for (int x = 0; x < src.Width; x++)
                {
                    Color p = src.GetPixel(x, y);
                    Color inv = Color.FromArgb(
                        255 - p.R,
                        255 - p.G,
                        255 - p.B
                    );
                    bmp.SetPixel(x, y, inv);
                }
            }

            return bmp;
        }

        private byte[] ImageToEscPosBitmap(System.Drawing.Image imagen, int maxWidth)
        {
            try
            {
                int newWidth = Math.Min(imagen.Width, maxWidth);
                int newHeight = (int)(imagen.Height * ((float)newWidth / imagen.Width));

                using (Bitmap bmp = new Bitmap(newWidth, newHeight))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(imagen, 0, 0, newWidth, newHeight);
                    }

                    // Invertir colores del logo
                    using (Bitmap bmpInvertido = InvertBitmap(bmp))
                    {
                        int width = bmpInvertido.Width;
                        int height = bmpInvertido.Height;
                        int widthBytes = (width + 7) / 8;
                        byte[] imageBytes = new byte[widthBytes * height];

                        int index = 0;
                        for (int y = 0; y < height; y++)
                        {
                            for (int xByte = 0; xByte < widthBytes; xByte++)
                            {
                                byte b = 0;
                                for (int bit = 0; bit < 8; bit++)
                                {
                                    int x = xByte * 8 + bit;
                                    if (x < width)
                                    {
                                        Color pixel = bmpInvertido.GetPixel(x, y);
                                        int luminance = (pixel.R + pixel.G + pixel.B) / 3;
                                        if (luminance < 128) b |= (byte)(1 << (7 - bit));
                                    }
                                }
                                imageBytes[index++] = b;
                            }
                        }

                        using (var ms = new System.IO.MemoryStream())
                        {
                            // Activar centrado ANTES del bitmap
                            ms.Write(new byte[] { 0x1B, 0x61, 0x01 }, 0, 3); // ESC a 1 - centrar
                            // GS v 0 modo normal (0x00)
                            ms.Write(new byte[] { 0x1D, 0x76, 0x30, 0x00 }, 0, 4); // GS v 0 0
                            ms.WriteByte((byte)(widthBytes & 0xFF));
                            ms.WriteByte((byte)((widthBytes >> 8) & 0xFF));
                            ms.WriteByte((byte)(height & 0xFF));
                            ms.WriteByte((byte)((height >> 8) & 0xFF));
                            ms.Write(imageBytes, 0, imageBytes.Length);
                            // Mantener centrado y agregar salto
                            ms.Write(new byte[] { 0x0A }, 0, 1); // Salto de línea
                            // Volver alineación a izquierda para el resto del ticket
                            ms.Write(new byte[] { 0x1B, 0x61, 0x00 }, 0, 3); // ESC a 0 - izquierda
                            return ms.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error convirtiendo imagen: " + ex.Message);
                return new byte[0];
            }
        }
    }
}
