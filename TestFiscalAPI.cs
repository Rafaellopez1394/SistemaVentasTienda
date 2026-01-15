using System;
using System.Threading.Tasks;
using CapaDatos;

class Program {
    static void Main() {
        MainAsync().GetAwaiter().GetResult();
    }
    
    static async Task MainAsync() {
        try {
            Console.WriteLine("==========================================");
            Console.WriteLine("     PRUEBA TIMBRADO CON FISCALAPI       ");
            Console.WriteLine("==========================================");
            Console.WriteLine("VentaID: 9aecc96b-e17d-4dc7-b2b8-4132a2173dc7");
            Console.WriteLine("Emisor: MISC491214B86");
            Console.WriteLine("Receptor: MASO451221PM4");
            Console.WriteLine("");
            Console.WriteLine("Ejecutando GenerarYTimbrarFactura...");
            Console.WriteLine("");
            
            var cd = new CD_Factura();
            var ventaGuid = new Guid("9aecc96b-e17d-4dc7-b2b8-4132a2173dc7");
            
            var resultado = await cd.GenerarYTimbrarFactura(
                ventaID: ventaGuid,
                rfcReceptor: "MASO451221PM4",
                nombreReceptor: "MARIA OLIVIA MARTINEZ SAGAZ",
                codigoPostalReceptor: "80290",
                regimenFiscalReceptor: "612",
                usoCFDI: "G03",
                formaPago: "01",
                metodoPago: "PUE",
                serie: "F",
                usuario: "admin"
            );
            
            Console.WriteLine("==========================================");
            Console.WriteLine("            RESULTADO                     ");
            Console.WriteLine("==========================================");
            Console.WriteLine("Exitoso: " + resultado.Exitoso);
            Console.WriteLine("Mensaje: " + resultado.Mensaje);
            
            if (!string.IsNullOrEmpty(resultado.UUID)) {
                Console.WriteLine("");
                Console.WriteLine(">>> UUID: " + resultado.UUID);
                Console.WriteLine(">>> FechaTimbrado: " + resultado.FechaTimbrado);
                Console.WriteLine(">>> CertificadoSAT: " + resultado.NoCertificadoSAT);
                Console.WriteLine(">>> SelloSAT: " + (string.IsNullOrEmpty(resultado.SelloSAT) ? "(vacio)" : resultado.SelloSAT.Substring(0, Math.Min(50, resultado.SelloSAT.Length)) + "..."));
                Console.WriteLine(">>> XML guardado en base de datos");
            }
            
            Console.WriteLine("==========================================");
            
        } catch (Exception ex) {
            Console.WriteLine("==========================================");
            Console.WriteLine("ERROR: " + ex.Message);
            Console.WriteLine("==========================================");
            if (ex.InnerException != null) {
                Console.WriteLine("Inner: " + ex.InnerException.Message);
            }
        }
    }
}
