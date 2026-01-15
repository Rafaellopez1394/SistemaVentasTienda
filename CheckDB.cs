using System;
using System.Data.SqlClient;

class CheckDB {
    static void Main() {
        try {
            Console.WriteLine("=== VERIFICANDO CONFIGURACION EN BASE DE DATOS ===");
            Console.WriteLine("");
            
            string connString = @"Data Source=.\SQLEXPRESS;Initial Catalog=DB_TIENDA;Integrated Security=True";
            using (var conn = new SqlConnection(connString)) {
                conn.Open();
                
                var cmd = new SqlCommand(@"
                    SELECT 
                        TenantKey,
                        LEFT(ApiKey, 30) + '...' AS ApiKey,
                        Ambiente,
                        RfcEmisor,
                        NombreEmisor,
                        RegimenFiscal,
                        CodigoPostal,
                        LEN(CertificadoBase64) AS CerLength,
                        LEFT(CertificadoBase64, 50) + '...' AS CerStart,
                        LEN(LlavePrivadaBase64) AS KeyLength,
                        LEFT(LlavePrivadaBase64, 50) + '...' AS KeyStart,
                        PasswordLlave,
                        Activo
                    FROM ConfiguracionFiscalAPI 
                    WHERE Activo = 1
                ", conn);
                
                using (var reader = cmd.ExecuteReader()) {
                    if (reader.Read()) {
                        Console.WriteLine("Tenant: " + reader["TenantKey"]);
                        Console.WriteLine("ApiKey: " + reader["ApiKey"]);
                        Console.WriteLine("Ambiente: " + reader["Ambiente"]);
                        Console.WriteLine("RfcEmisor: " + reader["RfcEmisor"]);
                        Console.WriteLine("NombreEmisor: " + reader["NombreEmisor"]);
                        Console.WriteLine("RegimenFiscal: " + reader["RegimenFiscal"]);
                        Console.WriteLine("CodigoPostal: " + reader["CodigoPostal"]);
                        Console.WriteLine("");
                        Console.WriteLine("Certificado (.cer):");
                        Console.WriteLine("  Length: " + reader["CerLength"] + " chars");
                        Console.WriteLine("  Start: " + reader["CerStart"]);
                        Console.WriteLine("");
                        Console.WriteLine("Llave Privada (.key):");
                        Console.WriteLine("  Length: " + reader["KeyLength"] + " chars");
                        Console.WriteLine("  Start: " + reader["KeyStart"]);
                        Console.WriteLine("");
                        Console.WriteLine("Password: " + reader["PasswordLlave"]);
                        Console.WriteLine("Activo: " + reader["Activo"]);
                    } else {
                        Console.WriteLine("NO SE ENCONTRO CONFIGURACION ACTIVA");
                    }
                }
            }
            
            Console.WriteLine("");
            Console.WriteLine("===================================================");
            
        } catch (Exception ex) {
            Console.WriteLine("ERROR: " + ex.Message);
        }
    }
}
