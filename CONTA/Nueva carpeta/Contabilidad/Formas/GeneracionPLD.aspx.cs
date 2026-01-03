using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Data;
using System.Reflection;
using System.Text;

namespace BalorFinanciera.Contabilidad.Formas
{
    public partial class GeneraPLD : System.Web.UI.Page  
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Entity.Response<Entity.Contabilidad.ArchivoGenerado> TraerDatosCuenta(string fechaInicio, string fechaFin)
        {

            string empresaid = "FA764836-BB07-4EB3-9B30-2B69206174C2";
            try
            {

                fechaInicio = fechaInicio.Replace("-", "");

                fechaFin = fechaFin.Replace("-", ""); 

                Entity.ListaDeEntidades<Entity.Contabilidad.GeneracionPLD> listaClientesCreditoPLD =
                MobileBO.GeneracioPldBO.GenerarListaClientesPLD(fechaInicio, fechaFin);

                var clientesInvalidos = new List<string>();
                DataSet dsCliente = null;
                foreach (var item in listaClientesCreditoPLD)
                {
                    var cliente = item as Entity.Contabilidad.GeneracionPLD;

                    if (cliente != null && string.IsNullOrWhiteSpace(cliente.Id_cliente))
                    {
                        int? folio = Convert.ToInt32( cliente.No_credito.ToString());
                        
                        Entity.Operacion.Cesion _cesion =
                        MobileBO.ControlOperacion.TraerCesionesPorEmpresaConFiltros(empresaid, folio, null, null, null, null)
                        .Where(y => y.Folio == folio).FirstOrDefault();
                        //dsCliente = MobileBO.ControlAnalisis.TraerCatclientesDS(_cesion.Clienteid.ToString());

                        if (!clientesInvalidos.Contains(_cesion.Cliente.ToString()))
                        {
                            clientesInvalidos.Add(_cesion.Cliente.ToString());
                        }
                    }
                }

                string creditosSinCliente = string.Join(", ", clientesInvalidos);
                if (clientesInvalidos.Count > 0)
                {
                    return Entity.Response<Entity.Contabilidad.ArchivoGenerado>
                        .CrearResponse<Entity.Contabilidad.ArchivoGenerado>(
                            false,
                            null,
                            $"Se encontraron Clientes no registrados {creditosSinCliente}. Favor de revisar su existencia en sicartera posteriormente solicitarle a sistemas el alta de el o los mismos."
                        );
                }
              
                StringBuilder sb = new StringBuilder();
                try
                {

                    // Convertir el contenido en un array de bytes
                    byte[] fileBytes = Encoding.UTF8.GetBytes(sb.ToString());

                    // Crear y escribir en el archivo
                    using (MemoryStream memoryStream = new MemoryStream(fileBytes))
                    {

                        if (listaClientesCreditoPLD != null)
                        {


                            // Nombre de la propiedad que quieres excluir
                            string columnaExcluida = "UltimaAct";

                            // Obtiene todas las propiedades excepto la excluida
                            PropertyInfo[] propiedades = typeof(Entity.Contabilidad.GeneracionPLD)
                                .GetProperties()
                                .Where(p => p.Name != columnaExcluida)
                                .ToArray();
                            // Encabezados (nombres de las columnas)
                            sb.AppendLine(string.Join(",", propiedades.Select(p => p.Name.ToLower())));

                            // Datos (valores de los objetos)
                            foreach (var item in listaClientesCreditoPLD)
                            {
                                sb.AppendLine(string.Join(",", propiedades.Select(p => p.GetValue(item, null))));
                            }

                        }

                    }
                    fileBytes = Encoding.UTF8.GetBytes(sb.ToString());

                    string prueba = sb.ToString();
                    // Convertir a bytes y luego a base64
                    string fileBase64 = Convert.ToBase64String(fileBytes);
                    string fileName = $"Creditos {fechaInicio}-{fechaFin}.csv";
                    // Crear la respuesta con el archivo en base64
                    var ArchivoGenerado = new Entity.Contabilidad.ArchivoGenerado
                    {
                        NombreArchivo = fileName,
                        ContenidoBase64 = fileBase64
                    };


                    return Entity.Response<Entity.Contabilidad.ArchivoGenerado>.CrearResponse(true, ArchivoGenerado);
                }
                catch (Exception ex)
                {
                    return Entity.Response<Entity.Contabilidad.ArchivoGenerado>.CrearResponse<Entity.Contabilidad.ArchivoGenerado>(false, null, ex.Message);
                }


            }
            catch (Exception ex)
            {
                return Entity.Response<Entity.Contabilidad.ArchivoGenerado>.CrearResponse<Entity.Contabilidad.ArchivoGenerado>(false, null, ex.Message);
            }
        }
        //// 🔹 Lista estática para almacenar los créditos en memoria
        //private static List<Credito> listaCreditos = new List<Credito>();

        [WebMethod] // Permite que el método sea llamado desde JavaScript
        public static string btnProcess_Click(List<Entity.Contabilidad.Creditos> datosExtraidos)
        {

            Entity.Contabilidad.Creditos datosguardado = new Entity.Contabilidad.Creditos();


            // Procesa los datos recibidos
            if (datosExtraidos == null || datosExtraidos.Count == 0)
            {
                return "No se recibieron datos.";
            }
            else
            {
                MobileBO.GeneracioPldBO.guardarlistaPagos(( datosExtraidos ));
            }

            // Simulación de procesamiento
            return $"Se procesaron {datosExtraidos.Count} registros.";
            //return Entity.Response<Entity.Contabilidad.ArchivoGenerado>.CrearResponse(true,null, "Se procesaron {datosExtraidos.Count} registros.");
        }


        [WebMethod]
        public static Entity.Response<Entity.Contabilidad.ArchivoGenerado> TraerDatosPagos(string fechaInicio, string fechaFin)
        {
            try
            {

                fechaInicio = fechaInicio.Replace("-", "");

                fechaFin = fechaFin.Replace("-", "");

                Entity.ListaDeEntidades<Entity.Contabilidad.PagosCreditosPLD> listaClientesPagosCreditoPLD = MobileBO.GeneracioPldBO.GenerarListapagosClientesPLD(fechaInicio, fechaFin);

                foreach (Entity.Contabilidad.PagosCreditosPLD cliente in listaClientesPagosCreditoPLD)
                {
                    if (cliente.Id_credito.Equals(""))
                    {
                        return Entity.Response<Entity.Contabilidad.ArchivoGenerado>.CrearResponse<Entity.Contabilidad.ArchivoGenerado>(false, null, "Favor de pedirle a sistemas que actualice la lista de clientes de Sicartera");
                    }
                }




                StringBuilder sb = new StringBuilder();
                try
                {

                    // Convertir el contenido en un array de bytes
                    byte[] fileBytes = Encoding.UTF8.GetBytes(sb.ToString());

                    // Crear y escribir en el archivo
                    using (MemoryStream memoryStream = new MemoryStream(fileBytes))
                    {

                        if (listaClientesPagosCreditoPLD != null)
                        {
                            // Nombres de las propiedades que quieres excluir
                            string[] columnasExcluidas = { "UltimaAct", "pagosCredito" };

                            // Obtiene todas las propiedades excepto las excluidas
                            PropertyInfo[] propiedades = typeof(Entity.Contabilidad.PagosCreditosPLD)
                                .GetProperties()
                                .Where(p => !columnasExcluidas.Contains(p.Name))
                                .ToArray();

                            // Encabezados (nombres de las columnas)
                            sb.AppendLine(string.Join(",", propiedades.Select(p => p.Name.ToLower())));

                            // Datos (valores de los objetos)
                            foreach (var item in listaClientesPagosCreditoPLD)
                            {
                                sb.AppendLine(string.Join(",", propiedades.Select(p => p.GetValue(item, null))));
                            }

                        }

                    }
                    fileBytes = Encoding.UTF8.GetBytes(sb.ToString());

                    string prueba = sb.ToString();
                    // Convertir a bytes y luego a base64
                    string fileBase64 = Convert.ToBase64String(fileBytes);
                    string fileName = $"Pagos {fechaInicio}-{fechaFin}.csv";
                    // Crear la respuesta con el archivo en base64
                    var ArchivoGenerado = new Entity.Contabilidad.ArchivoGenerado
                    {
                        NombreArchivo = fileName,
                        ContenidoBase64 = fileBase64
                    };


                    return Entity.Response<Entity.Contabilidad.ArchivoGenerado>.CrearResponse(true, ArchivoGenerado);
                }
                catch (Exception ex)
                {
                    return Entity.Response<Entity.Contabilidad.ArchivoGenerado>.CrearResponse<Entity.Contabilidad.ArchivoGenerado>(false, null, ex.Message);
                }


            }
            catch (Exception ex)
            {
                return Entity.Response<Entity.Contabilidad.ArchivoGenerado>.CrearResponse<Entity.Contabilidad.ArchivoGenerado>(false, null, ex.Message);
            }
        }
    }
}