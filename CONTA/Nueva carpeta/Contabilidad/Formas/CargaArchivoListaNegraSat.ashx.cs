using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web;
using System.Text;

namespace BalorFinanciera.Contabilidad.Formas
{
    /// <summary>
    /// Summary description for CargaArchivoListaNegraSat
    /// </summary>
    public class CargaArchivoListaNegraSat : IHttpHandler
    {
        private static string[] SplitCsv(string line)
        {
            List<string> result = new List<string>();
            StringBuilder currentStr = new StringBuilder("");
            bool inQuotes = false;
            for (int i = 0; i < line.Length; i++) // For each character
            {
                if (line[i] == '\"') // Quotes are closing or opening
                    inQuotes = !inQuotes;
                else if (line[i] == ',') // Comma
                {
                    if (!inQuotes) // If not in quotes, end of current string, add it to result
                    {
                        result.Add(currentStr.ToString());
                        currentStr.Clear();
                    }
                    else
                        currentStr.Append(line[i]); // If in quotes, just add it 
                }
                else // Add any other character to current string
                    currentStr.Append(line[i]);
            }
            result.Add(currentStr.ToString());
            return result.ToArray(); // Return array of all strings
        }

        public static string ReadToString(StreamReader sr, string splitString)
        {
            char nextChar;
            StringBuilder line = new StringBuilder();
            int matchIndex = 0;

            while (sr.Peek() > 0)
            {
                nextChar = (char)sr.Read();
                line.Append(nextChar);
                if (nextChar == splitString[matchIndex])
                {
                    if (matchIndex == splitString.Length - 1)
                    {
                        return line.ToString().Substring(0, line.Length - splitString.Length);
                    }
                    matchIndex++;
                }
                else
                {
                    matchIndex = 0;
                }
            }

            return line.Length == 0 ? null : line.ToString();
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                if (context.Request.QueryString["upload"] != null)
                {
                    string usuario = context.Request.QueryString["usuario"];
                    string sesionid = context.Request.QueryString["sesionid"];
                    bool errorArchivo = false;
                    var postedFile = context.Request.Files[0];

                    int index = 1;

                    List<Entity.Contabilidad.Catsatlistanegracontribuyente> listaContribuyentes = new List<Entity.Contabilidad.Catsatlistanegracontribuyente>();
                    Entity.Contabilidad.Catsatlistanegracontribuyente contribuyente = new Entity.Contabilidad.Catsatlistanegracontribuyente();

                    //borrar tabla lista sat
                    //MobileBO.ControlContabilidad.EliminarTodoCatsatlistanegracontribuyentes();
                    MobileBO.ControlContabilidad.EliminarTodoCatsatlistanegracontribuyentesTEMP(usuario, sesionid);

                    using (StreamReader reader = new StreamReader(postedFile.InputStream, System.Text.Encoding.Default, true))
                    {
                        string line;
                        string[] row;
                        while ((line = ReadToString(reader, "\r\n")) != null)
                        {
                            if (index < 4)
                            {
                                index++;
                                continue;//ignorar primeras 3 lineas
                            }

                            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                            row = SplitCsv(line);

                            //20 columnas
                            if (row.Length != 20)
                            {
                                context.Response.Write("Error en el formato de archivo - Renglon " + index.ToString());
                                errorArchivo = true;
                                break;

                                //si hay error en la linea se ignora
                                //continue;
                            }


                            string fechaParse = string.Empty;
                            int fechaMes = 0;
                            int fechaDia = 0;
                            int fechaAnio = 0;

                            DateTime fechaActual;
                            DateTime fechaIgnorar = new DateTime(1900, 1, 1);

                            //leer columnas
                            contribuyente = new Entity.Contabilidad.Catsatlistanegracontribuyente();
                            //0 Numero
                            contribuyente.Numero = int.Parse(row[0].ToString());

                            if (contribuyente.Numero > 11651)
                            {
                                contribuyente.Numero = contribuyente.Numero;
                            }

                            //1 RFC	
                            contribuyente.Rfc = row[1].ToString();
                            //2 Nombre
                            contribuyente.Nombre = row[2].ToString();
                            //3 Situacion
                            contribuyente.Situacion = row[3].ToString();
                            //4 NumeroFechaPresuncionSat
                            contribuyente.Numerofechapresuncionsat = row[4].ToString();
                            //5 PublicacionSatPresuntos  
                            fechaParse = row[5].ToString();
                            if (fechaParse.Length > 0)
                            {
                                fechaActual = ProcesarFecha(fechaParse);
                                if (DateTime.Compare(fechaActual, fechaIgnorar) != 0)
                                {
                                    contribuyente.Publicacionsatpresuntos = fechaActual;
                                }
                            }

                            //6 NumeroFechaPresuncionDof
                            contribuyente.Numerofechapresunciondof = row[6].ToString();
                            //7 PublicacionDofPresuntos	 
                            fechaParse = row[7].ToString();
                            if (fechaParse.Length > 0)
                            {

                                fechaActual = ProcesarFecha(fechaParse);
                                if (DateTime.Compare(fechaActual, fechaIgnorar) != 0)
                                {
                                    contribuyente.Publicaciondofpresuntos = fechaActual;
                                }
                            }
                            //--------------------------------------------------------------------
                            //8 NumeroFechaDesvirtuaron	SAT **************************
                            contribuyente.NumerofechadesvirtuaronSat = row[8].ToString();

                            //9 PublicacionSatDesvirtuados 
                            fechaParse = row[9].ToString();
                            if (fechaParse.Length > 0)
                            {

                                fechaActual = ProcesarFecha(fechaParse);
                                if (DateTime.Compare(fechaActual, fechaIgnorar) != 0)
                                {
                                    contribuyente.Publicacionsatdesvirtuados = fechaActual;
                                }
                            }

                            //10 NumeroFechaDesvirtuaron	DOF ********************
                            contribuyente.NumerofechadesvirtuaronDof = row[10].ToString();

                            //11 PublicacionDofDesvirtuados	
                            fechaParse = row[11].ToString();
                            if (fechaParse.Length > 0)
                            {

                                fechaActual = ProcesarFecha(fechaParse);
                                if (DateTime.Compare(fechaActual, fechaIgnorar) != 0)
                                {
                                    contribuyente.Publicaciondofdesvirtuados = fechaActual;
                                }
                            }

                            //12 NumeroFechaDefinitivos	SAT****************
                            contribuyente.NumerofechadefinitivosSat = row[12].ToString();
                            //13 PublicacionSatDefinitivos	
                            fechaParse = row[13].ToString();
                            if (fechaParse.Length > 0)
                            {
                                
                                fechaActual = ProcesarFecha(fechaParse);
                                if (DateTime.Compare(fechaActual, fechaIgnorar) != 0)
                                {
                                    contribuyente.Publicacionsatdefinitivos = fechaActual;
                                }
                            }

                            //14 NumeroFechaDefinitivos	DOF******************************
                            contribuyente.NumerofechadefinitivosDof = row[14].ToString();

                            //15 PublicacionDofDefinitivos	
                            fechaParse = row[15].ToString();
                            if (fechaParse.Length > 0)
                            {
                                
                                fechaActual = ProcesarFecha(fechaParse);
                                if (DateTime.Compare(fechaActual, fechaIgnorar) != 0)
                                {
                                    contribuyente.Publicaciondofdefinitivos = fechaActual;
                                }
                            }

                            //16 NumeroFechaSentenciaFavorableSat	
                            contribuyente.Numerofechasentenciafavorablesat = row[16].ToString();
                            //17 PublicacionSatSentenciaFavorable
                            fechaParse = row[17].ToString();
                            if (fechaParse.Length > 0)
                            {
                                
                                fechaActual = ProcesarFecha(fechaParse);
                                if (DateTime.Compare(fechaActual, fechaIgnorar) != 0)
                                {
                                    contribuyente.Publicacionsatsentenciafavorable = fechaActual;
                                }
                            }

                            //18 NumeroFechaSentenciaFavorableDof	
                            contribuyente.Numerofechasentenciafavorabledof = row[18].ToString();
                            //19 PublicacionDofSentenciaFavorable
                            fechaParse = row[19].ToString();
                            if (fechaParse.Length > 0)
                            {

                                fechaActual = ProcesarFecha(fechaParse);
                                if (DateTime.Compare(fechaActual, fechaIgnorar) != 0)
                                {
                                    contribuyente.Publicaciondofsentenciafavorable = fechaActual;
                                }
                                    
                            }

                            contribuyente.Fecha = DateTime.Now;
                            contribuyente.Estatus = 1;
                            contribuyente.Usuario = usuario;
                            contribuyente.SesionId = sesionid;

                            using (TransactionScope scope = new TransactionScope())
                            {
                                //MobileBO.ControlContabilidad.GuardarCatsatlistanegracontribuyente(new List<Entity.Contabilidad.Catsatlistanegracontribuyente>() { contribuyente });
                                MobileBO.ControlContabilidad.GuardarCatsatlistanegracontribuyenteTEMP(new List<Entity.Contabilidad.Catsatlistanegracontribuyente>() { contribuyente });

                                scope.Complete();
                            }
                        }
                    }

                    //using (StreamReader readFile = new StreamReader(postedFile.InputStream, System.Text.Encoding.Default, true))
                    //{
                    //    string line;
                    //    string[] row;


                    //    while ((line = readFile.ReadLine()) != null)
                    //    {

                            


                            

                    //    }


                    //}

                    context.Response.AddHeader("Vary", "Accept");
                    try
                    {
                        if (context.Request["HTTP_ACCEPT"].Contains("application/json"))
                            context.Response.ContentType = "application/json";
                        else
                            context.Response.ContentType = "text/plain";
                    }
                    catch
                    {
                        context.Response.ContentType = "text/plain";
                    }

                    if (!errorArchivo)
                    {
                        //borra tabla datos actuales
                        MobileBO.ControlContabilidad.EliminarTodoCatsatlistanegracontribuyentes();
                        //copiar desde temp
                        MobileBO.ControlContabilidad.CopiarTodoCatsatlistanegracontribuyentesTEMP(usuario, sesionid);
                        //borra de temp
                        MobileBO.ControlContabilidad.EliminarTodoCatsatlistanegracontribuyentesTEMP(usuario, sesionid);
                        context.Response.Write("Success");
                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.Write(ex.Message.ToString());
            }
            finally
            {
                context.Response.Flush();
            }
        }

        private DateTime ProcesarFecha(string valor)
        {
            DateTime fecha = new DateTime(1900, 1, 1);
            int fechaMes = 0;
            int fechaDia = 0;
            int fechaAnio = 0;

            try
            {
                // se vam ignorar formatos invalidos
                if (valor.Length == 10)
                {
                    fechaDia = int.Parse(valor.Substring(0, 2));
                    fechaMes = int.Parse(valor.Substring(3, 2));
                    fechaAnio = int.Parse(valor.Substring(6, 4));
                    fecha = new DateTime(fechaAnio, fechaMes, fechaDia);
                }

                if (valor.Length == 7)
                {
                    fechaDia = int.Parse(valor.Substring(0, 1));
                    fechaMes = int.Parse(valor.Substring(2, 2));
                    fechaAnio = int.Parse(valor.Substring(5, 2));
                    fechaAnio = fechaAnio + 2000;
                    fecha = new DateTime(fechaAnio, fechaMes, fechaDia);
                }

                if (valor.Length == 8)
                {
                    fechaDia = int.Parse(valor.Substring(0, 2));
                    fechaMes = int.Parse(valor.Substring(3, 2));
                    fechaAnio = int.Parse(valor.Substring(6, 2));
                    fechaAnio = fechaAnio + 2000;
                    fecha = new DateTime(fechaAnio, fechaMes, fechaDia);
                }
            }
            catch (Exception ex)
            {
                //regresa 01/01/1900
            }
            
            return fecha;
            
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}