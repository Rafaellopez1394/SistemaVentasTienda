using CapaModelo;
using iTextSharp.text;
using iTextSharp.text.pdf;
using QRCoder;
using System;
using System.IO;

namespace VentasWeb.Utilidades
{
    public class PDFFacturaGenerator
    {
        // Colores corporativos
        private static readonly BaseColor ColorPrimario = new BaseColor(41, 128, 185); // Azul
        private static readonly BaseColor ColorSecundario = new BaseColor(52, 73, 94); // Gris oscuro
        private static readonly BaseColor ColorFondo = new BaseColor(236, 240, 241); // Gris claro

        public static byte[] GenerarPDF(Factura factura)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Crear documento (tamaño carta)
                Document document = new Document(PageSize.LETTER, 40, 40, 60, 60);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                // ===== ENCABEZADO =====
                AgregarEncabezado(document, factura);

                // Línea separadora
                document.Add(new Paragraph(" "));
                LineSeparator line = new LineSeparator(1f, 100f, ColorPrimario, Element.ALIGN_CENTER, -5);
                document.Add(new Chunk(line));
                document.Add(new Paragraph(" "));

                // ===== DATOS EMISOR Y RECEPTOR =====
                AgregarDatosEmisorReceptor(document, factura);

                document.Add(new Paragraph(" "));

                // ===== CONCEPTOS =====
                AgregarConceptos(document, factura);

                document.Add(new Paragraph(" "));

                // ===== TOTALES Y QR =====
                AgregarTotalesYQR(document, factura);

                document.Add(new Paragraph(" "));

                // ===== INFORMACIÓN FISCAL =====
                AgregarInformacionFiscal(document, factura);

                // ===== PIE DE PÁGINA =====
                AgregarPiePagina(document, factura);

                document.Close();

                return ms.ToArray();
            }
        }

        private static void AgregarEncabezado(Document document, Factura factura)
        {
            PdfPTable tableHeader = new PdfPTable(2) { WidthPercentage = 100 };
            tableHeader.SetWidths(new float[] { 60f, 40f });

            // Logo y datos empresa (izquierda)
            PdfPCell cellLogo = new PdfPCell();
            cellLogo.Border = Rectangle.NO_BORDER;
            cellLogo.PaddingBottom = 10;

            Font fontEmpresa = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, ColorPrimario);
            Font fontDatos = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.DARK_GRAY);

            Paragraph pEmpresa = new Paragraph("MI EMPRESA S.A. DE C.V.", fontEmpresa);
            cellLogo.AddElement(pEmpresa);
            cellLogo.AddElement(new Paragraph($"RFC: {factura.RFCEmisor}", fontDatos));
            cellLogo.AddElement(new Paragraph("Calle Principal #123, Col. Centro", fontDatos));
            cellLogo.AddElement(new Paragraph("C.P. 12345, Ciudad, Estado", fontDatos));
            cellLogo.AddElement(new Paragraph("Tel: (555) 123-4567", fontDatos));
            cellLogo.AddElement(new Paragraph("Email: contacto@miempresa.com", fontDatos));

            tableHeader.AddCell(cellLogo);

            // Información factura (derecha)
            PdfPCell cellFactura = new PdfPCell();
            cellFactura.Border = Rectangle.BOX;
            cellFactura.BackgroundColor = ColorFondo;
            cellFactura.Padding = 10;
            cellFactura.HorizontalAlignment = Element.ALIGN_RIGHT;

            Font fontTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, ColorSecundario);
            Font fontFolio = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);

            Paragraph pTitulo = new Paragraph("FACTURA", fontTitulo);
            pTitulo.Alignment = Element.ALIGN_RIGHT;
            cellFactura.AddElement(pTitulo);

            cellFactura.AddElement(new Paragraph($"Folio Fiscal:", fontDatos));
            cellFactura.AddElement(new Paragraph($"{factura.UUID}", fontFolio));
            cellFactura.AddElement(new Paragraph(" "));
            cellFactura.AddElement(new Paragraph($"Serie: {factura.Serie}", fontDatos));
            cellFactura.AddElement(new Paragraph($"Folio: {factura.Folio}", fontDatos));
            cellFactura.AddElement(new Paragraph($"Fecha: {factura.FechaTimbrado:dd/MM/yyyy HH:mm:ss}", fontDatos));

            tableHeader.AddCell(cellFactura);

            document.Add(tableHeader);
        }

        private static void AgregarDatosEmisorReceptor(Document document, Factura factura)
        {
            PdfPTable table = new PdfPTable(2) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 50f, 50f });

            Font fontTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.WHITE);
            Font fontDatos = FontFactory.GetFont(FontFactory.HELVETICA, 9);

            // EMISOR
            PdfPCell cellEmisorTitle = new PdfPCell(new Phrase("EMISOR", fontTitulo));
            cellEmisorTitle.BackgroundColor = ColorPrimario;
            cellEmisorTitle.HorizontalAlignment = Element.ALIGN_CENTER;
            cellEmisorTitle.Padding = 5;
            table.AddCell(cellEmisorTitle);

            // RECEPTOR
            PdfPCell cellReceptorTitle = new PdfPCell(new Phrase("RECEPTOR", fontTitulo));
            cellReceptorTitle.BackgroundColor = ColorPrimario;
            cellReceptorTitle.HorizontalAlignment = Element.ALIGN_CENTER;
            cellReceptorTitle.Padding = 5;
            table.AddCell(cellReceptorTitle);

            // Datos Emisor
            PdfPCell cellEmisor = new PdfPCell();
            cellEmisor.Padding = 8;
            cellEmisor.AddElement(new Paragraph($"RFC: {factura.RFCEmisor}", fontDatos));
            cellEmisor.AddElement(new Paragraph($"Nombre: MI EMPRESA S.A. DE C.V.", fontDatos));
            cellEmisor.AddElement(new Paragraph($"Régimen Fiscal: 601 - General de Ley Personas Morales", fontDatos));
            table.AddCell(cellEmisor);

            // Datos Receptor
            PdfPCell cellReceptor = new PdfPCell();
            cellReceptor.Padding = 8;
            cellReceptor.AddElement(new Paragraph($"RFC: {factura.RFCReceptor}", fontDatos));
            cellReceptor.AddElement(new Paragraph($"Nombre: {factura.NombreReceptor}", fontDatos));
            cellReceptor.AddElement(new Paragraph($"Uso CFDI: {factura.UsoCFDI}", fontDatos));
            cellReceptor.AddElement(new Paragraph($"Método de Pago: {factura.MetodoPago}", fontDatos));
            cellReceptor.AddElement(new Paragraph($"Forma de Pago: {factura.FormaPago}", fontDatos));
            table.AddCell(cellReceptor);

            document.Add(table);
        }

        private static void AgregarConceptos(Document document, Factura factura)
        {
            Font fontTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.WHITE);
            Font fontHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8);
            Font fontDatos = FontFactory.GetFont(FontFactory.HELVETICA, 8);

            Paragraph pConceptos = new Paragraph("CONCEPTOS", fontTitulo);
            pConceptos.Alignment = Element.ALIGN_CENTER;
            PdfPCell cellTitle = new PdfPCell(pConceptos);
            cellTitle.BackgroundColor = ColorSecundario;
            cellTitle.HorizontalAlignment = Element.ALIGN_CENTER;
            cellTitle.Padding = 5;
            cellTitle.Colspan = 6;

            PdfPTable table = new PdfPTable(6) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 10f, 35f, 12f, 12f, 12f, 15f });

            table.AddCell(cellTitle);

            // Headers
            string[] headers = { "Clave", "Descripción", "Cantidad", "Precio Unit.", "Importe", "IVA" };
            foreach (string header in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, fontHeader));
                cell.BackgroundColor = ColorFondo;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Padding = 5;
                table.AddCell(cell);
            }

            // Conceptos
            foreach (var concepto in factura.Conceptos)
            {
                table.AddCell(new PdfPCell(new Phrase(concepto.ClaveProdServ, fontDatos)) { Padding = 5 });
                table.AddCell(new PdfPCell(new Phrase(concepto.Descripcion, fontDatos)) { Padding = 5 });
                table.AddCell(new PdfPCell(new Phrase(concepto.Cantidad.ToString("N2"), fontDatos)) 
                    { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                table.AddCell(new PdfPCell(new Phrase(concepto.ValorUnitario.ToString("C2"), fontDatos)) 
                    { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                table.AddCell(new PdfPCell(new Phrase(concepto.Importe.ToString("C2"), fontDatos)) 
                    { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                
                decimal iva = concepto.Importe * (concepto.TasaIVA / 100m);
                table.AddCell(new PdfPCell(new Phrase(iva.ToString("C2"), fontDatos)) 
                    { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
            }

            document.Add(table);
        }

        private static void AgregarTotalesYQR(Document document, Factura factura)
        {
            PdfPTable table = new PdfPTable(2) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 40f, 60f });

            Font fontDatos = FontFactory.GetFont(FontFactory.HELVETICA, 9);
            Font fontTotal = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, ColorPrimario);

            // QR Code (izquierda)
            PdfPCell cellQR = new PdfPCell();
            cellQR.Border = Rectangle.NO_BORDER;
            cellQR.HorizontalAlignment = Element.ALIGN_CENTER;
            cellQR.VerticalAlignment = Element.ALIGN_MIDDLE;

            try
            {
                // Generar QR con URL del SAT para verificación
                string qrContent = $"https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx?&id={factura.UUID}&re={factura.RFCEmisor}&rr={factura.RFCReceptor}&tt={factura.Total:F6}&fe={factura.SelloSAT.Substring(factura.SelloSAT.Length - 8)}";
                
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                
                using (System.Drawing.Bitmap qrBitmap = qrCode.GetGraphic(20))
                using (MemoryStream qrMs = new MemoryStream())
                {
                    qrBitmap.Save(qrMs, System.Drawing.Imaging.ImageFormat.Png);
                    Image qrImage = Image.GetInstance(qrMs.ToArray());
                    qrImage.ScaleToFit(120f, 120f);
                    cellQR.AddElement(qrImage);
                }
            }
            catch
            {
                cellQR.AddElement(new Paragraph("QR Code", fontDatos));
            }

            table.AddCell(cellQR);

            // Totales (derecha)
            PdfPCell cellTotales = new PdfPCell();
            cellTotales.Border = Rectangle.NO_BORDER;
            cellTotales.PaddingLeft = 20;

            PdfPTable tableTotales = new PdfPTable(2);
            tableTotales.WidthPercentage = 100;
            tableTotales.SetWidths(new float[] { 60f, 40f });

            tableTotales.AddCell(new PdfPCell(new Phrase("Subtotal:", fontDatos)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 3 });
            tableTotales.AddCell(new PdfPCell(new Phrase(factura.Subtotal.ToString("C2"), fontDatos)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 3 });

            tableTotales.AddCell(new PdfPCell(new Phrase("IVA:", fontDatos)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 3 });
            tableTotales.AddCell(new PdfPCell(new Phrase(factura.IVA.ToString("C2"), fontDatos)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 3 });

            if (factura.Descuento > 0)
            {
                tableTotales.AddCell(new PdfPCell(new Phrase("Descuento:", fontDatos)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 3 });
                tableTotales.AddCell(new PdfPCell(new Phrase(factura.Descuento.ToString("C2"), fontDatos)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 3 });
            }

            PdfPCell cellTotalLabel = new PdfPCell(new Phrase("TOTAL:", fontTotal));
            cellTotalLabel.Border = Rectangle.TOP;
            cellTotalLabel.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellTotalLabel.Padding = 8;
            cellTotalLabel.BackgroundColor = ColorFondo;
            tableTotales.AddCell(cellTotalLabel);

            PdfPCell cellTotalValue = new PdfPCell(new Phrase(factura.Total.ToString("C2"), fontTotal));
            cellTotalValue.Border = Rectangle.TOP;
            cellTotalValue.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellTotalValue.Padding = 8;
            cellTotalValue.BackgroundColor = ColorFondo;
            tableTotales.AddCell(cellTotalValue);

            cellTotales.AddElement(tableTotales);
            table.AddCell(cellTotales);

            document.Add(table);
        }

        private static void AgregarInformacionFiscal(Document document, Factura factura)
        {
            Font fontTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9);
            Font fontDatos = FontFactory.GetFont(FontFactory.HELVETICA, 7, BaseColor.DARK_GRAY);

            document.Add(new Paragraph("INFORMACIÓN FISCAL", fontTitulo));
            document.Add(new Paragraph(" "));

            document.Add(new Paragraph($"Certificado Digital del Emisor: {factura.NoCertificado}", fontDatos));
            document.Add(new Paragraph($"Certificado Digital del SAT: {factura.NoCertificadoSAT}", fontDatos));
            document.Add(new Paragraph(" "));

            document.Add(new Paragraph("Cadena Original del Complemento de Certificación Digital del SAT:", fontTitulo));
            Paragraph pCadena = new Paragraph(factura.CadenaOriginal, fontDatos);
            pCadena.Alignment = Element.ALIGN_JUSTIFIED;
            document.Add(pCadena);
            document.Add(new Paragraph(" "));

            document.Add(new Paragraph("Sello Digital del CFDI:", fontTitulo));
            Paragraph pSelloCFDI = new Paragraph(factura.SelloCFDI, fontDatos);
            pSelloCFDI.Alignment = Element.ALIGN_JUSTIFIED;
            document.Add(pSelloCFDI);
            document.Add(new Paragraph(" "));

            document.Add(new Paragraph("Sello Digital del SAT:", fontTitulo));
            Paragraph pSelloSAT = new Paragraph(factura.SelloSAT, fontDatos);
            pSelloSAT.Alignment = Element.ALIGN_JUSTIFIED;
            document.Add(pSelloSAT);
        }

        private static void AgregarPiePagina(Document document, Factura factura)
        {
            Font fontPie = FontFactory.GetFont(FontFactory.HELVETICA, 7, BaseColor.GRAY);

            document.Add(new Paragraph(" "));
            LineSeparator line = new LineSeparator(0.5f, 100f, BaseColor.LIGHT_GRAY, Element.ALIGN_CENTER, -5);
            document.Add(new Chunk(line));

            Paragraph pPie = new Paragraph(
                $"Este documento es una representación impresa de un CFDI | " +
                $"UUID: {factura.UUID} | " +
                $"Fecha de Certificación: {factura.FechaTimbrado:dd/MM/yyyy HH:mm:ss}",
                fontPie
            );
            pPie.Alignment = Element.ALIGN_CENTER;
            document.Add(pPie);
        }
    }
}
