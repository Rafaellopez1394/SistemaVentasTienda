using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using QRCoder;
using CapaModelo;

namespace CapaDatos.Generadores
{
    /// <summary>
    /// Generador de PDF para facturas CFDI 4.0
    /// Compatible con .NET Framework 4.6
    /// </summary>
    public class PDFFacturaGenerator
    {
        private readonly Font fuenteTitulo = new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD, BaseColor.BLACK);
        private readonly Font fuenteSubtitulo = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.DARK_GRAY);
        private readonly Font fuenteNormal = new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL, BaseColor.BLACK);
        private readonly Font fuenteNormalBold = new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD, BaseColor.BLACK);
        private readonly Font fuentePequena = new Font(Font.FontFamily.HELVETICA, 7, Font.NORMAL, BaseColor.GRAY);

        public byte[] GenerarPDF(Factura factura)
        {
            if (factura == null)
                throw new ArgumentNullException(nameof(factura));

            using (var ms = new MemoryStream())
            {
                // Crear documento PDF (tamaño carta)
                var documento = new Document(PageSize.LETTER, 30f, 30f, 40f, 40f);
                var writer = PdfWriter.GetInstance(documento, ms);
                
                documento.Open();

                // Encabezado
                AgregarEncabezado(documento, factura);

                // Datos del emisor y receptor
                AgregarDatosEmisorReceptor(documento, factura);

                // Conceptos/Productos
                AgregarConceptos(documento, factura);

                // Totales
                AgregarTotales(documento, factura);

                // Datos fiscales (UUID, sellos, certificados)
                AgregarDatosFiscales(documento, factura);

                // QR Code
                if (!string.IsNullOrEmpty(factura.UUID))
                {
                    AgregarCodigoQR(documento, factura);
                }

                // Pie de página
                AgregarPiePagina(documento, factura);

                documento.Close();
                return ms.ToArray();
            }
        }

        private void AgregarEncabezado(Document doc, Factura factura)
        {
            var tabla = new PdfPTable(2) { WidthPercentage = 100 };
            tabla.SetWidths(new float[] { 60f, 40f });

            // Columna izquierda: Datos del emisor
            var celdaEmisor = new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                PaddingBottom = 10f
            };

            celdaEmisor.AddElement(new Paragraph(factura.EmisorNombre ?? "SIN NOMBRE", fuenteTitulo));
            celdaEmisor.AddElement(new Paragraph($"RFC: {factura.EmisorRFC}", fuenteNormal));
            celdaEmisor.AddElement(new Paragraph($"Régimen Fiscal: {factura.EmisorRegimenFiscal}", fuentePequena));
            
            tabla.AddCell(celdaEmisor);

            // Columna derecha: Folio fiscal
            var celdaFolio = new PdfPCell
            {
                Border = Rectangle.BOX,
                BorderColor = BaseColor.GRAY,
                Padding = 8f,
                HorizontalAlignment = Element.ALIGN_RIGHT
            };

            celdaFolio.AddElement(new Paragraph("FACTURA ELECTRÓNICA", fuenteSubtitulo) { Alignment = Element.ALIGN_RIGHT });
            celdaFolio.AddElement(new Paragraph($"Serie-Folio: {factura.Serie}-{factura.Folio}", fuenteNormal) { Alignment = Element.ALIGN_RIGHT });
            celdaFolio.AddElement(new Paragraph($"Fecha: {factura.FechaEmision:dd/MM/yyyy HH:mm:ss}", fuentePequena) { Alignment = Element.ALIGN_RIGHT });
            
            if (!string.IsNullOrEmpty(factura.UUID))
            {
                celdaFolio.AddElement(new Paragraph(" ", fuentePequena));
                celdaFolio.AddElement(new Paragraph("UUID (Folio Fiscal)", fuenteNormalBold) { Alignment = Element.ALIGN_RIGHT });
                celdaFolio.AddElement(new Paragraph(factura.UUID, new Font(Font.FontFamily.COURIER, 7, Font.NORMAL, BaseColor.BLUE)) { Alignment = Element.ALIGN_RIGHT });
            }

            tabla.AddCell(celdaFolio);
            doc.Add(tabla);
            doc.Add(new Paragraph(" "));
        }

        private void AgregarDatosEmisorReceptor(Document doc, Factura factura)
        {
            var tabla = new PdfPTable(2) { WidthPercentage = 100 };
            tabla.SetWidths(new float[] { 50f, 50f });

            // EMISOR
            var celdaEmisor = new PdfPCell
            {
                Border = Rectangle.BOX,
                BorderColor = BaseColor.LIGHT_GRAY,
                Padding = 8f,
                BackgroundColor = new BaseColor(240, 248, 255)
            };
            celdaEmisor.AddElement(new Paragraph("EMISOR", fuenteSubtitulo));
            celdaEmisor.AddElement(new Paragraph(factura.EmisorNombre ?? "N/A", fuenteNormal));
            celdaEmisor.AddElement(new Paragraph($"RFC: {factura.EmisorRFC}", fuenteNormal));
            celdaEmisor.AddElement(new Paragraph($"Régimen Fiscal: {factura.EmisorRegimenFiscal}", fuentePequena));
            if (!string.IsNullOrEmpty(factura.CodigoPostalEmisor))
            {
                celdaEmisor.AddElement(new Paragraph($"Código Postal: {factura.CodigoPostalEmisor}", fuentePequena));
            }
            tabla.AddCell(celdaEmisor);

            // RECEPTOR
            var celdaReceptor = new PdfPCell
            {
                Border = Rectangle.BOX,
                BorderColor = BaseColor.LIGHT_GRAY,
                Padding = 8f,
                BackgroundColor = new BaseColor(255, 250, 240)
            };
            celdaReceptor.AddElement(new Paragraph("RECEPTOR (CLIENTE)", fuenteSubtitulo));
            celdaReceptor.AddElement(new Paragraph(factura.ReceptorNombre ?? "N/A", fuenteNormal));
            celdaReceptor.AddElement(new Paragraph($"RFC: {factura.ReceptorRFC}", fuenteNormal));
            celdaReceptor.AddElement(new Paragraph($"Uso CFDI: {factura.ReceptorUsoCFDI}", fuentePequena));
            if (!string.IsNullOrEmpty(factura.ReceptorDomicilioFiscalCP))
            {
                celdaReceptor.AddElement(new Paragraph($"Código Postal: {factura.ReceptorDomicilioFiscalCP}", fuentePequena));
            }
            tabla.AddCell(celdaReceptor);

            doc.Add(tabla);
            doc.Add(new Paragraph(" "));
        }

        private void AgregarConceptos(Document doc, Factura factura)
        {
            doc.Add(new Paragraph("CONCEPTOS", fuenteSubtitulo));
            doc.Add(new Paragraph(" "));

            var tabla = new PdfPTable(5) { WidthPercentage = 100 };
            tabla.SetWidths(new float[] { 10f, 40f, 15f, 15f, 20f });

            // Encabezados
            var headerFont = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.WHITE);
            AgregarCeldaHeader(tabla, "Cant.", headerFont);
            AgregarCeldaHeader(tabla, "Descripción", headerFont);
            AgregarCeldaHeader(tabla, "Clave SAT", headerFont);
            AgregarCeldaHeader(tabla, "Precio Unit.", headerFont);
            AgregarCeldaHeader(tabla, "Importe", headerFont);

            // Conceptos
            if (factura.Conceptos != null && factura.Conceptos.Any())
            {
                foreach (var concepto in factura.Conceptos)
                {
                    AgregarCeldaDato(tabla, concepto.Cantidad.ToString("N2"), Element.ALIGN_CENTER);
                    AgregarCeldaDato(tabla, concepto.Descripcion);
                    AgregarCeldaDato(tabla, concepto.ClaveProdServ, Element.ALIGN_CENTER);
                    AgregarCeldaDato(tabla, concepto.ValorUnitario.ToString("C2"), Element.ALIGN_RIGHT);
                    AgregarCeldaDato(tabla, concepto.Importe.ToString("C2"), Element.ALIGN_RIGHT);
                }
            }

            doc.Add(tabla);
            doc.Add(new Paragraph(" "));
        }

        private void AgregarTotales(Document doc, Factura factura)
        {
            var tabla = new PdfPTable(2) { WidthPercentage = 50, HorizontalAlignment = Element.ALIGN_RIGHT };
            tabla.SetWidths(new float[] { 60f, 40f });

            // Subtotal
            AgregarFilaTotal(tabla, "Subtotal:", factura.Subtotal.ToString("C2"));

            // Descuento (si aplica)
            if (factura.Descuento > 0)
            {
                AgregarFilaTotal(tabla, "Descuento:", factura.Descuento.ToString("C2"));
            }

            // IVA Trasladado
            if (factura.TotalImpuestosTrasladados > 0)
            {
                AgregarFilaTotal(tabla, "IVA (16%):", factura.TotalImpuestosTrasladados.ToString("C2"));
            }

            // Total
            var celdaLabelTotal = new PdfPCell(new Phrase("TOTAL:", new Font(Font.FontFamily.HELVETICA, 11, Font.BOLD)))
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                PaddingTop = 5f,
                PaddingBottom = 5f
            };
            var celdaValorTotal = new PdfPCell(new Phrase(factura.Total.ToString("C2"), new Font(Font.FontFamily.HELVETICA, 11, Font.BOLD)))
            {
                Border = Rectangle.TOP_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                PaddingTop = 5f,
                PaddingBottom = 5f
            };
            tabla.AddCell(celdaLabelTotal);
            tabla.AddCell(celdaValorTotal);

            // Forma de pago y método de pago
            doc.Add(tabla);
            doc.Add(new Paragraph(" "));
            
            var infoPago = new Paragraph($"Forma de Pago: {factura.FormaPago} | Método de Pago: {factura.MetodoPago}", fuentePequena)
            {
                Alignment = Element.ALIGN_RIGHT
            };
            doc.Add(infoPago);
            doc.Add(new Paragraph(" "));
        }

        private void AgregarDatosFiscales(Document doc, Factura factura)
        {
            if (string.IsNullOrEmpty(factura.UUID))
                return;

            doc.Add(new Paragraph("DATOS FISCALES DEL CFDI", fuenteSubtitulo));
            doc.Add(new Paragraph(" "));

            var tabla = new PdfPTable(1) { WidthPercentage = 100 };
            
            var celda = new PdfPCell
            {
                Border = Rectangle.BOX,
                BorderColor = BaseColor.LIGHT_GRAY,
                Padding = 8f,
                BackgroundColor = new BaseColor(250, 250, 250)
            };

            celda.AddElement(new Paragraph($"Folio Fiscal (UUID): {factura.UUID}", fuenteNormal));
            
            if (factura.FechaTimbrado.HasValue)
            {
                celda.AddElement(new Paragraph($"Fecha de Certificación: {factura.FechaTimbrado.Value:dd/MM/yyyy HH:mm:ss}", fuentePequena));
            }

            if (!string.IsNullOrEmpty(factura.NoCertificadoSAT))
            {
                celda.AddElement(new Paragraph($"No. Certificado SAT: {factura.NoCertificadoSAT}", fuentePequena));
            }

            celda.AddElement(new Paragraph($"Proveedor PAC: {factura.ProveedorPAC ?? "N/A"}", fuentePequena));

            // Sellos (truncados para PDF)
            if (!string.IsNullOrEmpty(factura.SelloCFD))
            {
                var selloCorto = factura.SelloCFD.Length > 60 ? factura.SelloCFD.Substring(0, 60) + "..." : factura.SelloCFD;
                celda.AddElement(new Paragraph($"Sello Digital CFD: {selloCorto}", new Font(Font.FontFamily.COURIER, 6, Font.NORMAL, BaseColor.GRAY)));
            }

            if (!string.IsNullOrEmpty(factura.SelloSAT))
            {
                var selloSATCorto = factura.SelloSAT.Length > 60 ? factura.SelloSAT.Substring(0, 60) + "..." : factura.SelloSAT;
                celda.AddElement(new Paragraph($"Sello Digital SAT: {selloSATCorto}", new Font(Font.FontFamily.COURIER, 6, Font.NORMAL, BaseColor.GRAY)));
            }

            tabla.AddCell(celda);
            doc.Add(tabla);
            doc.Add(new Paragraph(" "));
        }

        private void AgregarCodigoQR(Document doc, Factura factura)
        {
            try
            {
                // Generar cadena para QR según especificación SAT
                // https://emisorrfc.com?re=RFC_EMISOR&rr=RFC_RECEPTOR&tt=TOTAL&id=UUID
                var cadenaQR = $"https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx?id={factura.UUID}&re={factura.EmisorRFC}&rr={factura.ReceptorRFC}&tt={factura.Total:F6}";

                // Generar QR con QRCoder
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(cadenaQR, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);
                var qrBitmap = qrCode.GetGraphic(4);

                // Convertir bitmap a imagen iTextSharp
                using (var ms = new MemoryStream())
                {
                    qrBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;
                    var qrImage = Image.GetInstance(ms);
                    qrImage.ScaleToFit(120f, 120f);
                    qrImage.Alignment = Element.ALIGN_CENTER;
                    
                    doc.Add(qrImage);
                }

                doc.Add(new Paragraph("Código QR para verificación en el SAT", fuentePequena) { Alignment = Element.ALIGN_CENTER });
            }
            catch (Exception ex)
            {
                doc.Add(new Paragraph($"Error al generar código QR: {ex.Message}", fuentePequena));
            }
        }

        private void AgregarPiePagina(Document doc, Factura factura)
        {
            doc.Add(new Paragraph(" "));
            var lineaSeparadora = new Paragraph("_________________________________________________________________________")
            {
                Alignment = Element.ALIGN_CENTER,
                Font = new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.LIGHT_GRAY)
            };
            doc.Add(lineaSeparadora);

            var parrafoLegal = new Paragraph(
                "Este documento es una representación impresa de un CFDI versión 4.0. " +
                "La validez de este documento debe verificarse en el portal del SAT con el UUID mostrado arriba.",
                fuentePequena
            )
            {
                Alignment = Element.ALIGN_CENTER
            };
            doc.Add(parrafoLegal);
        }

        // Métodos auxiliares
        private void AgregarCeldaHeader(PdfPTable tabla, string texto, Font fuente)
        {
            var celda = new PdfPCell(new Phrase(texto, fuente))
            {
                BackgroundColor = new BaseColor(70, 130, 180),
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 5f,
                Border = Rectangle.BOX,
                BorderColor = BaseColor.WHITE
            };
            tabla.AddCell(celda);
        }

        private void AgregarCeldaDato(PdfPTable tabla, string texto, int alineacion = Element.ALIGN_LEFT)
        {
            var celda = new PdfPCell(new Phrase(texto ?? "", fuenteNormal))
            {
                HorizontalAlignment = alineacion,
                Padding = 4f,
                Border = Rectangle.BOTTOM_BORDER,
                BorderColor = BaseColor.LIGHT_GRAY
            };
            tabla.AddCell(celda);
        }

        private void AgregarFilaTotal(PdfPTable tabla, string label, string valor)
        {
            var celdaLabel = new PdfPCell(new Phrase(label, fuenteNormal))
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                PaddingRight = 10f
            };
            var celdaValor = new PdfPCell(new Phrase(valor, fuenteNormal))
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT
            };
            tabla.AddCell(celdaLabel);
            tabla.AddCell(celdaValor);
        }
    }
}
