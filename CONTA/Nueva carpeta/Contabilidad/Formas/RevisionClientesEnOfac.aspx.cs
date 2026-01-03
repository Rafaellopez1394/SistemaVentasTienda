using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Xml;

namespace BalorFinanciera.Contabilidad.Formas
{
    // Clase auxiliar para guardar nombre y apellido OFAC
    public class OfacEntry
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string NombreCompleto { get; set; }
    }

    public partial class RevisionClientesEnOfac : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) btnProcess_Click(null, null);

            // 🔹 Obtener clientes desde BO
            var clientes = new List<Entity.Contabilidad.Clientes>();
            try
            {
                clientes = ObtenerBloqueados();
            }
            catch (Exception exCli)
            {
                lblResultado.ForeColor = System.Drawing.Color.Red;
                lblResultado.Text = $"❌ Error al obtener clientes: {exCli.Message}";
                return;
            }
        }

        protected void btnProcess_Click(object sender, EventArgs e)
        {
            try
            {
                lblResultado.ForeColor = System.Drawing.Color.Blue;
                lblResultado.Text = "⏳ Procesando archivo desde red... Por favor espere.";

                // 🔹 Leer archivo XML OFAC desde ruta compartida
                var listaOfac = new List<OfacEntry>();
                var dedup = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                var settings = new XmlReaderSettings
                {
                    IgnoreWhitespace = true,
                    DtdProcessing = DtdProcessing.Ignore
                };

                // Ruta del archivo XML en red
                string rutaArchivo = @"\\Win-jbuuav9rjmh\ofac\sdn_enhanced.xml";

                // Validar existencia
                if (!System.IO.File.Exists(rutaArchivo))
                {
                    lblResultado.Text = $"❌ No se encontró el archivo en la ruta: {rutaArchivo}";
                    lblResultado.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                using (var reader = XmlReader.Create(rutaArchivo, settings))
                {
                    bool inTranslation = false;
                    string currentFirst = null;
                    string currentLast = null;
                    string currentFull = null;

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            switch (reader.LocalName)
                            {
                                case "translation":
                                    inTranslation = true;
                                    currentFirst = currentLast = currentFull = null;
                                    break;
                                case "formattedFirstName":
                                    if (inTranslation)
                                        currentFirst = reader.ReadElementContentAsString();
                                    break;
                                case "formattedLastName":
                                    if (inTranslation)
                                        currentLast = reader.ReadElementContentAsString();
                                    break;
                                case "formattedFullName":
                                    if (inTranslation)
                                        currentFull = reader.ReadElementContentAsString();
                                    break;
                            }
                        }
                        else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "translation")
                        {
                            inTranslation = false;

                            string nombre = currentFirst?.Trim() ?? "";
                            string apellido = currentLast?.Trim() ?? "";
                            string full = currentFull?.Trim();

                            if (string.IsNullOrWhiteSpace(full))
                                full = $"{nombre} {apellido}".Trim();

                            if (!string.IsNullOrWhiteSpace(full))
                            {
                                var key = $"{nombre}|{apellido}|{full}";
                                if (dedup.Add(key))
                                {
                                    listaOfac.Add(new OfacEntry
                                    {
                                        Nombre = nombre,
                                        Apellido = apellido,
                                        NombreCompleto = full
                                    });
                                }
                            }
                        }
                    }
                }

                // 🔹 Obtener clientes desde BO
                var clientes = new List<Entity.Contabilidad.Clientes>();
                try
                {
                    clientes = ObtenerClientes();
                }
                catch (Exception exCli)
                {
                    lblResultado.ForeColor = System.Drawing.Color.Red;
                    lblResultado.Text = $"❌ Error al obtener clientes: {exCli.Message}";
                    return;
                }

                // 🔹 Preprocesar tokens OFAC para mejorar rendimiento
                var ofacTokens = listaOfac.Select(o => new Tuple<OfacEntry, HashSet<string>>(
                    o,
                    new HashSet<string>(
                        Normalizar(o.NombreCompleto)
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
                        StringComparer.OrdinalIgnoreCase)
                )).ToList();

                // 🔹 Coincidencias
                var coincidencias = new List<object>();

                foreach (var cli in clientes)
                {
                    var partesCli = Normalizar(cli.NombreCompleto)
                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (partesCli.Length == 0)
                        continue;

                    // Extraer posibles nombres y apellidos del cliente
                    string cliNombre1 = partesCli.ElementAtOrDefault(0) ?? "";
                    string cliNombre2 = partesCli.ElementAtOrDefault(1) ?? "";
                    string cliApePat = partesCli.ElementAtOrDefault(partesCli.Length - 2) ?? "";
                    string cliApeMat = partesCli.ElementAtOrDefault(partesCli.Length - 1) ?? "";

                    bool tieneDosNombres = !string.IsNullOrEmpty(cliNombre2);
                    bool tieneDosApellidos = !string.IsNullOrEmpty(cliApeMat) && cliApeMat != cliApePat;

                    foreach (var tuple in ofacTokens)
                    {
                        var ofac = tuple.Item1;
                        var tokens = tuple.Item2;

                        int score = 0;

                        if (tokens.Contains(cliApePat)) score++;
                        if (tokens.Contains(cliApeMat)) score++;
                        if (tokens.Contains(cliNombre1)) score++;
                        if (!string.IsNullOrEmpty(cliNombre2) && tokens.Contains(cliNombre2)) score++;

                        string nivel = null;

                        // 🔴 Alta: 4/4 coincidencias o 2/2 si solo tiene 1 nombre y 1 apellido
                        if ((tieneDosNombres && tieneDosApellidos && score == 4) ||
                            (!tieneDosNombres && !tieneDosApellidos && score == 2))
                        {
                            nivel = "🔴 Alta coincidencia (nombres y apellidos completos)";
                        }
                        // 🟠 Media: 3 coincidencias
                        else if (score == 3)
                        {
                            nivel = "🟠 Coincidencia media (3 coincidencias parciales)";
                        }
                        else
                        {
                            continue; // Se omiten bajas
                        }

                        coincidencias.Add(new
                        {
                            Cliente = cli.NombreCompleto,
                            Match = ofac.NombreCompleto,
                            Nivel = nivel
                        });
                    }
                }

                // 🔹 Mostrar resultados finales
                lblResultado.ForeColor = System.Drawing.Color.Green;
                lblResultado.Text = $"✅ Se cargaron {listaOfac.Count} registros OFAC desde red. " +
                                    $"Clientes procesados: {clientes.Count}. " +
                                    $"Coincidencias encontradas: {coincidencias.Count}.";

                var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
                HiddenListaOfac.Value = serializer.Serialize(coincidencias);

                // ✅ Mostrar en cliente
                ScriptManager.RegisterStartupScript(this, GetType(), "cargarLista", "cargarListaClientes();", true);
            }
            catch (Exception ex)
            {
                lblResultado.ForeColor = System.Drawing.Color.Red;
                lblResultado.Text = $"❌ Error al procesar: {ex.Message}";
            }
        }

        // 🔸 Método para obtener clientes desde BO
        private static List<Entity.Contabilidad.Clientes> ObtenerClientes()
        {
            var origen = MobileBO.Contabilidad.RevisionClientesEnOfacBO.GenerarListaClientes();
            if (origen == null) return new List<Entity.Contabilidad.Clientes>();
            return origen.Cast<Entity.Contabilidad.Clientes>().ToList();
        }

        private static List<Entity.Contabilidad.Clientes> ObtenerBloqueados()
        {
            var origen = MobileBO.Contabilidad.RevisionClientesEnOfacBO.ConsultaBloqueado();
            if (origen == null) return new List<Entity.Contabilidad.Clientes>();
            return origen.Cast<Entity.Contabilidad.Clientes>().ToList();
        }

        private static List<Entity.Contabilidad.Clientes> BuscarCliente(string nombre)
        {
            var origen = MobileBO.Contabilidad.RevisionClientesEnOfacBO.BuscarCliente(nombre);
            if (origen == null) return new List<Entity.Contabilidad.Clientes>();
            return origen.Cast<Entity.Contabilidad.Clientes>().ToList();
        }

        // 🔹 Normalización de texto (quita acentos, signos y pasa a mayúsculas)
        private static string Normalizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return "";
            texto = texto.ToUpperInvariant();
            texto = texto.Normalize(NormalizationForm.FormD);

            var sb = new StringBuilder();
            foreach (char c in texto)
            {
                var cat = CharUnicodeInfo.GetUnicodeCategory(c);
                if (cat != UnicodeCategory.NonSpacingMark && (char.IsLetterOrDigit(c) || c == ' '))
                    sb.Append(c);
            }

            return System.Text.RegularExpressions.Regex
                .Replace(sb.ToString(), @"[^A-Z0-9\s]", "")
                .Replace("  ", " ")
                .Trim();
        }

        // 🔹 WebMethod para búsquedas manuales directas desde cliente (sin barrido)
        [System.Web.Services.WebMethod]
        public static string BuscarClientesPorNombre(string nombre, string apellido)
        {
            try
            {
                nombre = (nombre ?? "").Trim();
                apellido = (apellido ?? "").Trim();

                string nombreCompleto = (nombre + " " + apellido).Trim().ToUpperInvariant();

                var clientes = BuscarCliente(nombreCompleto);

                var coincidencias = clientes
                    .Select(c => new
                    {
                        RazonSocial = c.RazonSocial,
                        Nombre = c.Nombre,
                        Tipo = c.Tipo,
                        Match = "Coincidencia directa en clientes"
                    })
                    .ToList();

                var serializer = new JavaScriptSerializer();
                return serializer.Serialize(coincidencias);
            }
            catch (Exception ex)
            {
                return $"ERROR:{ex.Message}";
            }
        }
    }
}
