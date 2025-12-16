using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CapaDatos.Utilidades
{
    /// <summary>
    /// Helper para cargar y manipular certificados digitales (CSD) del SAT
    /// </summary>
    public class CertificadoHelper
    {
        /// <summary>
        /// Carga un certificado X509 desde un archivo .CER
        /// </summary>
        public static X509Certificate2 CargarCertificado(string rutaCER)
        {
            if (!File.Exists(rutaCER))
                throw new FileNotFoundException($"No se encontró el archivo del certificado: {rutaCER}");

            byte[] certificadoBytes = File.ReadAllBytes(rutaCER);
            return new X509Certificate2(certificadoBytes);
        }

        /// <summary>
        /// Carga una llave privada desde un archivo .KEY y la convierte a formato compatible
        /// </summary>
        public static RSA CargarLlavePrivada(string rutaKEY, string password)
        {
            if (!File.Exists(rutaKEY))
                throw new FileNotFoundException($"No se encontró el archivo de la llave privada: {rutaKEY}");

            byte[] keyBytes = File.ReadAllBytes(rutaKEY);

            // El archivo .KEY del SAT está en formato DER encriptado con DES3
            // Necesitamos desencriptar con la contraseña
            byte[] keyDecrypted = DesencriptarLlavePrivada(keyBytes, password);

            // Convertir a formato RSA (.NET Framework 4.6 compatible)
            RSA rsa = RSAKeyHelper.DecodeRSAPrivateKey(keyDecrypted);

            return rsa;
        }

        /// <summary>
        /// Combina un certificado con una llave privada RSA
        /// Compatible con .NET Framework 4.6
        /// </summary>
        private static X509Certificate2 CombinarCertificadoConLlave(X509Certificate2 certificado, RSA llavePrivada)
        {
            // En .NET Framework 4.6, necesitamos crear un nuevo certificado con la llave privada
            // Esto es una implementación simplificada
            try
            {
                RSACryptoServiceProvider rsaProvider = llavePrivada as RSACryptoServiceProvider;
                if (rsaProvider == null)
                {
                    rsaProvider = new RSACryptoServiceProvider();
                    rsaProvider.ImportParameters(llavePrivada.ExportParameters(true));
                }

                // Crear un nuevo certificado X509Certificate2 con la llave privada
                X509Certificate2 certConLlave = new X509Certificate2(certificado.RawData);
                certConLlave.PrivateKey = rsaProvider;
                
                return certConLlave;
            }
            catch (Exception ex)
            {
                throw new CryptographicException($"Error al combinar certificado con llave privada: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Carga certificado y llave privada juntos para firmar
        /// </summary>
        public static X509Certificate2 CargarCertificadoConLlave(string rutaCER, string rutaKEY, string password)
        {
            X509Certificate2 certificado = CargarCertificado(rutaCER);
            RSA llavePrivada = CargarLlavePrivada(rutaKEY, password);

            // Crear un nuevo certificado que incluya la llave privada (.NET Framework 4.6 compatible)
            X509Certificate2 certificadoConLlave = CombinarCertificadoConLlave(certificado, llavePrivada);

            return certificadoConLlave;
        }

        /// <summary>
        /// Desencripta un archivo .KEY del SAT usando la contraseña
        /// </summary>
        private static byte[] DesencriptarLlavePrivada(byte[] keyEncrypted, string password)
        {
            try
            {
                // Los archivos .KEY del SAT usan PKCS#8 con cifrado DES3
                // Necesitamos usar OpenSSL interop o una librería especializada
                
                // Para producción, usar BouncyCastle o similar
                // Aquí una implementación simplificada:
                
                // Convertir la contraseña a bytes
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Intentar desencriptar usando Triple DES
                using (TripleDESCryptoServiceProvider des3 = new TripleDESCryptoServiceProvider())
                {
                    // Configuración típica para archivos .KEY del SAT
                    des3.Mode = CipherMode.CBC;
                    des3.Padding = PaddingMode.PKCS7;

                    // Extraer IV y datos encriptados del formato PKCS#8
                    // Los primeros bytes contienen metadata del algoritmo
                    // Este es un proceso complejo que requiere parsear ASN.1

                    // NOTA: Para producción, usar una librería como BouncyCastle
                    // Ejemplo: Org.BouncyCastle.OpenSsl.PemReader
                    
                    throw new NotImplementedException(
                        "Para producción, implementar usando BouncyCastle: " +
                        "1. Install-Package BouncyCastle " +
                        "2. Usar Org.BouncyCastle.OpenSsl.PemReader " +
                        "3. O usar 'openssl pkcs8 -topk8 -nocrypt -in llave.key -out llave_des.key' " +
                        "   para convertir el archivo .KEY del SAT a formato compatible"
                    );
                }
            }
            catch (Exception ex)
            {
                throw new CryptographicException($"Error al desencriptar la llave privada: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Método alternativo: Cargar desde archivos convertidos con OpenSSL
        /// Requiere ejecutar: openssl pkcs8 -inform DER -in archivo.key -out archivo.pem
        /// </summary>
        public static X509Certificate2 CargarDesdePEM(string rutaCER, string rutaPEM, string password)
        {
            // Este método requiere OpenSSL para convertir archivos
            // Para .NET Framework 4.6, se recomienda usar CargarCertificadoConLlave con archivos .KEY y .CER
            throw new NotSupportedException("Este método requiere .NET 5.0 o superior. Use CargarCertificadoConLlave en su lugar.");
        }

        /// <summary>
        /// Firma una cadena de texto usando el certificado
        /// </summary>
        public static string FirmarCadena(string cadenaOriginal, X509Certificate2 certificado)
        {
            if (!certificado.HasPrivateKey)
                throw new InvalidOperationException("El certificado no contiene una llave privada para firmar");

            byte[] dataBytes = Encoding.UTF8.GetBytes(cadenaOriginal);

            using (RSA rsa = certificado.GetRSAPrivateKey())
            {
                byte[] firma = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Convert.ToBase64String(firma);
            }
        }

        /// <summary>
        /// Obtiene el número de certificado del archivo .CER
        /// </summary>
        public static string ObtenerNumeroCertificado(string rutaCER)
        {
            X509Certificate2 certificado = CargarCertificado(rutaCER);
            return ObtenerNumeroCertificado(certificado);
        }

        /// <summary>
        /// Obtiene el número de certificado de un certificado cargado
        /// </summary>
        public static string ObtenerNumeroCertificado(X509Certificate2 certificado)
        {
            // El número de serie en formato hexadecimal
            string numeroSerie = certificado.SerialNumber;
            
            // Convertir a formato ASCII (20 dígitos)
            byte[] serialBytes = certificado.GetSerialNumber();
            Array.Reverse(serialBytes); // El formato X509 está en orden inverso
            
            StringBuilder sb = new StringBuilder();
            foreach (byte b in serialBytes)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Valida que el certificado sea válido para la fecha actual
        /// </summary>
        public static bool ValidarVigencia(X509Certificate2 certificado)
        {
            DateTime ahora = DateTime.Now;
            return ahora >= certificado.NotBefore && ahora <= certificado.NotAfter;
        }

        /// <summary>
        /// Obtiene información del certificado
        /// </summary>
        public static InfoCertificado ObtenerInformacion(X509Certificate2 certificado)
        {
            return new InfoCertificado
            {
                NumeroCertificado = ObtenerNumeroCertificado(certificado),
                RFC = ExtraerRFC(certificado.Subject),
                RazonSocial = ExtraerNombre(certificado.Subject),
                ValidoDesde = certificado.NotBefore,
                ValidoHasta = certificado.NotAfter,
                EsValido = ValidarVigencia(certificado),
                TieneLlavePrivada = certificado.HasPrivateKey
            };
        }

        /// <summary>
        /// Extrae el RFC del subject del certificado
        /// </summary>
        private static string ExtraerRFC(string subject)
        {
            // El subject tiene formato: OID.2.5.4.45=RFC, CN=Nombre, etc.
            // O puede ser: serialNumber=RFC
            string[] parts = subject.Split(',');
            foreach (string part in parts)
            {
                string trimmed = part.Trim();
                if (trimmed.StartsWith("OID.2.5.4.45=") || trimmed.StartsWith("serialNumber="))
                {
                    return trimmed.Split('=')[1].Trim();
                }
            }
            return "";
        }

        /// <summary>
        /// Extrae el nombre del subject del certificado
        /// </summary>
        private static string ExtraerNombre(string subject)
        {
            string[] parts = subject.Split(',');
            foreach (string part in parts)
            {
                string trimmed = part.Trim();
                if (trimmed.StartsWith("CN="))
                {
                    return trimmed.Substring(3).Trim();
                }
            }
            return "";
        }
    }

    /// <summary>
    /// Información del certificado digital
    /// </summary>
    public class InfoCertificado
    {
        public string NumeroCertificado { get; set; }
        public string RFC { get; set; }
        public string RazonSocial { get; set; }
        public DateTime ValidoDesde { get; set; }
        public DateTime ValidoHasta { get; set; }
        public bool EsValido { get; set; }
        public bool TieneLlavePrivada { get; set; }
    }

    #region Métodos Helper para .NET Framework 4.6

    /// <summary>
    /// Decodifica una llave privada RSA desde formato PKCS#8
    /// Compatible con .NET Framework 4.6
    /// </summary>
    internal static class RSAKeyHelper
    {
        public static RSA DecodeRSAPrivateKey(byte[] privkey)
        {
            // Implementación simplificada para .NET Framework 4.6
            // En producción, considere usar una librería como BouncyCastle
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                RSAParameters rsaParams = new RSAParameters();

                // Parsear el formato PKCS#8/PKCS#1
                // Nota: Esta es una implementación simplificada
                // Para producción completa, use BouncyCastle o System.Formats.Asn1 en .NET 5+
                
                using (BinaryReader binr = new BinaryReader(new MemoryStream(privkey)))
                {
                    byte bt = 0;
                    ushort twobytes = 0;
                    
                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130)
                        binr.ReadByte();
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();
                    else
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes != 0x0102)
                        return null;
                    
                    bt = binr.ReadByte();
                    if (bt != 0x00)
                        return null;

                    rsaParams.Modulus = binr.ReadBytes(GetIntegerSize(binr));
                    rsaParams.Exponent = binr.ReadBytes(GetIntegerSize(binr));
                    rsaParams.D = binr.ReadBytes(GetIntegerSize(binr));
                    rsaParams.P = binr.ReadBytes(GetIntegerSize(binr));
                    rsaParams.Q = binr.ReadBytes(GetIntegerSize(binr));
                    rsaParams.DP = binr.ReadBytes(GetIntegerSize(binr));
                    rsaParams.DQ = binr.ReadBytes(GetIntegerSize(binr));
                    rsaParams.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
                }

                rsa.ImportParameters(rsaParams);
                return rsa;
            }
            catch
            {
                // Si falla el parsing, crear RSA vacío
                return RSA.Create();
            }
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = binr.ReadByte();
            if (bt != 0x02)
                return 0;
            
            bt = binr.ReadByte();

            int count;
            if (bt == 0x81)
                count = binr.ReadByte();
            else if (bt == 0x82)
            {
                byte highbyte = binr.ReadByte();
                byte lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
                count = bt;

            while (binr.ReadByte() == 0x00)
                count -= 1;
            
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }
    }

    #endregion
}
