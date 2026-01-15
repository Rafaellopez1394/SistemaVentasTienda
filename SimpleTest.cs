using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

class SimpleTest {
    static void Main() {
        TestAsync().GetAwaiter().GetResult();
    }
    
    static async Task TestAsync() {
        try {
            // Habilitar TLS 1.2
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            
            Console.WriteLine("=== TEST SIMPLE FISCALAPI (SIN CERTIFICADOS) ===");
            Console.WriteLine("RFC Emisor: MISC491214B86");
            Console.WriteLine("Tenant: e0a0d1de-d225-46de-b95f-55d04f2787ff");
            Console.WriteLine("");
            
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://test.fiscalapi.com");
            client.DefaultRequestHeaders.Add("X-API-KEY", "sk_test_47126aed_6c71_4060_b05b_932c4423dd00");
            client.DefaultRequestHeaders.Add("X-TENANT-KEY", "e0a0d1de-d225-46de-b95f-55d04f2787ff");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            
            var json = @"{
  ""versionCode"": ""4.0"",
  ""series"": ""F"",
  ""date"": ""2026-01-15T16:00:00"",
  ""paymentFormCode"": ""01"",
  ""currencyCode"": ""MXN"",
  ""typeCode"": ""I"",
  ""expeditionZipCode"": ""81048"",
  ""exportCode"": ""01"",
  ""paymentMethodCode"": ""PUE"",
  ""exchangeRate"": 1.0,
  ""issuer"": {
    ""tin"": ""MISC491214B86"",
    ""legalName"": ""CECILIA MIRANDA SANCHEZ"",
    ""taxRegimeCode"": ""612""
  },
  ""recipient"": {
    ""tin"": ""XAXX010101000"",
    ""legalName"": ""PUBLICO EN GENERAL"",
    ""zipCode"": ""81048"",
    ""taxRegimeCode"": ""616"",
    ""cfdiUseCode"": ""S01""
  },
  ""items"": [{
    ""itemCode"": ""01010101"",
    ""quantity"": 1.0,
    ""unitOfMeasurementCode"": ""H87"",
    ""description"": ""PRODUCTO DE PRUEBA"",
    ""unitPrice"": 100.0,
    ""taxObjectCode"": ""02"",
    ""itemTaxes"": [{
      ""taxCode"": ""002"",
      ""taxTypeCode"": ""Tasa"",
      ""taxRate"": 0.16,
      ""taxFlagCode"": ""T""
    }]
  }]
}";
            
            Console.WriteLine("Enviando request SIN certificados (usando los del dashboard)...");
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/v4/invoices", content);
            
            Console.WriteLine("");
            Console.WriteLine("=============== RESPUESTA ===============");
            Console.WriteLine("Status Code: " + (int)response.StatusCode + " " + response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Length: " + responseBody.Length + " chars");
            Console.WriteLine("");
            
            if (string.IsNullOrEmpty(responseBody)) {
                Console.WriteLine("Response: (VACIO)");
            } else {
                Console.WriteLine("Response:");
                Console.WriteLine(responseBody);
            }
            Console.WriteLine("=========================================");
            
        } catch (Exception ex) {
            Console.WriteLine("ERROR: " + ex.Message);
            if (ex.InnerException != null) {
                Console.WriteLine("Inner: " + ex.InnerException.Message);
            }
        }
    }
}
