using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class Pruebas : Page
    {
        // Modelo para los datos de crédito
        public class Credito
        {
            public int IDCredito { get; set; }
            public int NumCredito { get; set; }
            public int IDCliente { get; set; }
            public string RFC { get; set; }
            public string TipoCredito { get; set; }
            public DateTime FechaInicio { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Configurar la licencia de EPPlus (para uso no comercial)
            // Nota: En EPPlus 4.x, no se requiere esta configuración de licencia
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        protected void btnProcess_Click(object sender, EventArgs e)
        {
            // Validar que se haya subido un archivo
            if (!fileUpload.HasFile)
            {
                ShowMessage("Por favor, selecciona un archivo.", true);
                return;
            }

            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;
            string filePath = string.Empty;

            try
            {
                // Lista para almacenar los datos procesados
                List<Credito> creditos = new List<Credito>();

                // Guardar el archivo temporalmente
                filePath = Server.MapPath("~/Temp/" + fileUpload.FileName);
                fileUpload.SaveAs(filePath);

                // Iniciar Excel
                excelApp = new Excel.Application();
                workbook = excelApp.Workbooks.Open(filePath);
                worksheet = (Excel.Worksheet)workbook.Sheets[1]; // Obtener la primera hoja

                Excel.Range usedRange = worksheet.UsedRange;
                int rowCount = usedRange.Rows.Count;

                // Validar si hay datos
                if (rowCount < 7) // 1 para encabezados + al menos 1 fila de datos
                {
                    ShowMessage("El archivo está vacío o no tiene datos válidos.", true);
                    return;
                }

                // Iterar sobre todas las filas, empezando desde la fila 2 (la fila 1 tiene los encabezados)
                for (int row = 7; row <= rowCount; row++)
                {
                    try
                    {
                        // Declarar las variables antes de usarlas en TryParse
                        int idCredito;
                        int numCredito;
                        int idCliente;
                        DateTime fechaInicio;

                        // Extraer los datos de las celdas
                        string idCreditoText = ((Excel.Range)worksheet.Cells[row, 1])?.Value?.ToString(); // ID Credito
                        string numCreditoText = ((Excel.Range)worksheet.Cells[row, 2])?.Value?.ToString(); // NumCredito
                        string idClienteText = ((Excel.Range)worksheet.Cells[row, 3])?.Value?.ToString(); // ID Cliente
                        string rfc = ((Excel.Range)worksheet.Cells[row, 23])?.Value?.ToString(); // RFC
                        string tipoCredito = ((Excel.Range)worksheet.Cells[row, 29])?.Value?.ToString(); // TipoCredito
                        string fechaInicioText = ((Excel.Range)worksheet.Cells[row, 30])?.Value?.ToString(); // Fecha de Inicio

                        // Validar y convertir los datos
                        if (!int.TryParse(idCreditoText, out idCredito) ||
                            !int.TryParse(numCreditoText, out numCredito) ||
                            !int.TryParse(idClienteText, out idCliente) ||
                            string.IsNullOrEmpty(rfc) ||
                            string.IsNullOrEmpty(tipoCredito) ||
                            !DateTime.TryParse(fechaInicioText, out fechaInicio))
                        {
                            continue; // Si hay error en cualquier campo, saltar esta fila
                        }

                        // Crear el objeto Credito y agregarlo a la lista
                        Credito credito = new Credito
                        {
                            IDCredito = idCredito,
                            NumCredito = numCredito,
                            IDCliente = idCliente,
                            RFC = rfc,
                            TipoCredito = tipoCredito,
                            FechaInicio = fechaInicio
                        };
                        creditos.Add(credito);
                    }
                    catch
                    {
                        continue; // Si algo falla en esta fila, continuar con la siguiente
                    }
                }

                // Validar si se encontraron registros
                if (creditos.Count == 0)
                {
                    ShowMessage("No se encontraron registros válidos en el archivo.", true);
                    return;
                }

                // Insertar los datos en la base de datos
                int insertedRows = InsertIntoDatabase(creditos);
                //int insertedRows = 0;

                // Mostrar mensaje de éxito
                ShowMessage("Se insertaron " + insertedRows + " registros en la tabla tmppld2.", false);
            }
            catch (Exception ex)
            {
                ShowMessage("Ocurrió un error al procesar el archivo: " + ex.Message, true);
            }
            finally
            {
                // Liberar recursos
                if (workbook != null)
                {
                    workbook.Close(false);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                }
                if (excelApp != null)
                {
                    excelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                }

                // Limpiar objetos COM
                GC.Collect();
                GC.WaitForPendingFinalizers();

                // Eliminar el archivo temporal
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        // Método para insertar los datos en la tabla tmppld2 usando SqlBulkCopy
        private int InsertIntoDatabase(List<Credito> creditos)
        {
            int insertedRows = 0;
            //string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            // Crear un DataTable para almacenar los datos
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("ID Credito", typeof(int));
            dataTable.Columns.Add("NumCredito", typeof(int));
            dataTable.Columns.Add("ID Cliente", typeof(int));
            dataTable.Columns.Add("RFC", typeof(string));
            dataTable.Columns.Add("TipoCredito", typeof(string));
            dataTable.Columns.Add("Fecha de Inicio", typeof(DateTime));

            // Llenar el DataTable con los datos
            foreach (Credito credito in creditos)
            {
                dataTable.Rows.Add(
                    credito.IDCredito,
                    credito.NumCredito,
                    credito.IDCliente,
                    credito.RFC,
                    credito.TipoCredito,
                    credito.FechaInicio
                );
            }

            // Usar SqlBulkCopy para insertar los datos de forma masiva
            /*using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "tmppld2";

                    // Mapear las columnas del DataTable a las columnas de la tabla
                    bulkCopy.ColumnMappings.Add("ID Credito", "ID Credito");
                    bulkCopy.ColumnMappings.Add("NumCredito", "NumCredito");
                    bulkCopy.ColumnMappings.Add("ID Cliente", "ID Cliente");
                    bulkCopy.ColumnMappings.Add("RFC", "RFC");
                    bulkCopy.ColumnMappings.Add("TipoCredito", "TipoCredito");
                    bulkCopy.ColumnMappings.Add("Fecha de Inicio", "Fecha de Inicio");

                    try
                    {
                        // Insertar todos los datos de una vez
                        bulkCopy.WriteToServer(dataTable);
                        insertedRows = dataTable.Rows.Count;
                    }
                    catch (Exception ex)
                    {
                        ShowMessage("Error al insertar los datos en la base de datos: " + ex.Message, true);
                        return 0;
                    }
                }

                connection.Close();
            }*/

            return insertedRows;
        }

        // Método para mostrar mensajes de éxito o error
        private void ShowMessage(string message, bool isError)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "message " + (isError ? "error" : "success");
            lblMessage.Visible = true;
        }
    }
}