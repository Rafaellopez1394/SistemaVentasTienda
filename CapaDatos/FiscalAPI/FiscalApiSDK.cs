// FiscalAPI SDK Implementation - Integración completa con API de FiscalAPI
// Basado en: https://github.com/fiscalapi/fiscalapi-net
// Documentación: https://docs.fiscalapi.com
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Fiscalapi
{
    #region Configuración y Cliente Principal

    /// <summary>
    /// Configuración para conectar con FiscalAPI
    /// </summary>
    public class FiscalapiSettings
    {
        public string ApiUrl { get; set; }
        public string ApiKey { get; set; }
        public string ApiVersion { get; set; } = "v4";
        public string Tenant { get; set; }
        public string TimeZone { get; set; } = "America/Mexico_City";
    }

    // Alias para compatibilidad
    public class FiscalApiOptions : FiscalapiSettings { }

    /// <summary>
    /// Cliente principal para interactuar con FiscalAPI
    /// </summary>
    public class FiscalApiClient : IFiscalApiClient
    {
        public IInvoiceService Invoices { get; private set; }
        public ICatalogService Catalogs { get; private set; }
        public IProductService Products { get; private set; }
        public IPersonService Persons { get; private set; }
        public ISatCfdiService SatCfdi { get; private set; }
        public ITaxFileService TaxFiles { get; private set; }
        public IDownloadRequestService DownloadRequests { get; private set; }

        private FiscalApiClient(FiscalapiSettings settings)
        {
            var httpClient = CreateHttpClient(settings);
            var apiVersion = settings.ApiVersion ?? "v4";

            Invoices = new InvoiceService(httpClient, apiVersion);
            Catalogs = new CatalogService(httpClient, apiVersion);
            Products = new ProductService(httpClient, apiVersion);
            Persons = new PersonService(httpClient, apiVersion);
            SatCfdi = new SatCfdiService(httpClient, apiVersion);
            TaxFiles = new TaxFileService(httpClient, apiVersion);
            DownloadRequests = new DownloadRequestService(httpClient, apiVersion);
        }

        public static IFiscalApiClient Create(FiscalapiSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            ValidateSettings(settings);
            return new FiscalApiClient(settings);
        }

        private static void ValidateSettings(FiscalapiSettings settings)
        {
            if (string.IsNullOrEmpty(settings.ApiUrl))
                throw new ArgumentException("ApiUrl is required", nameof(settings.ApiUrl));
            if (string.IsNullOrEmpty(settings.ApiKey))
                throw new ArgumentException("ApiKey is required", nameof(settings.ApiKey));
            if (string.IsNullOrEmpty(settings.Tenant))
                throw new ArgumentException("Tenant is required", nameof(settings.Tenant));
        }

        private static IFiscalApiHttpClient CreateHttpClient(FiscalapiSettings settings)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(settings.ApiUrl);
            // Headers exactos según documentación oficial: https://docs.fiscalapi.com/authentication
            httpClient.DefaultRequestHeaders.Add("X-API-KEY", settings.ApiKey);  // CORREGIDO: era "X-API-Key"
            httpClient.DefaultRequestHeaders.Add("X-TENANT-KEY", settings.Tenant);  // CORREGIDO: era "X-Tenant"
            httpClient.DefaultRequestHeaders.Add("X-API-VERSION", settings.ApiVersion ?? "v4");
            httpClient.DefaultRequestHeaders.Add("X-TIME-ZONE", settings.TimeZone ?? "America/Mexico_City");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.Timeout = TimeSpan.FromSeconds(120);
            
            return new FiscalApiHttpClient(httpClient);
        }
    }

    #endregion

    #region HTTP Client

    public interface IFiscalApiHttpClient
    {
        Task<ApiResponse<T>> GetAsync<T>(string endpoint);
        Task<ApiResponse<T>> GetByIdAsync<T>(string id);
        Task<ApiResponse<T>> PostAsync<T>(string endpoint, object payload);
        Task<ApiResponse<T>> PutAsync<T>(string endpoint, object payload);
        Task<ApiResponse<T>> DeleteAsync<T>(string endpoint, object payload);
        Task<ApiResponse<bool>> DeleteAsync(string endpoint);
    }

    public class FiscalApiHttpClient : IFiscalApiHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerSettings _jsonSettings;

        public FiscalApiHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
        {
            return await SendRequestAsync<T>(HttpMethod.Get, endpoint);
        }

        public async Task<ApiResponse<T>> GetByIdAsync<T>(string id)
        {
            return await SendRequestAsync<T>(HttpMethod.Get, id);
        }

        public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object payload)
        {
            return await SendRequestAsync<T>(HttpMethod.Post, endpoint, payload);
        }

        public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object payload)
        {
            return await SendRequestAsync<T>(HttpMethod.Put, endpoint, payload);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(string endpoint)
        {
            return await SendRequestAsync<bool>(HttpMethod.Delete, endpoint);
        }

        public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint, object payload)
        {
            return await SendRequestAsync<T>(HttpMethod.Delete, endpoint, payload);
        }

        private async Task<ApiResponse<T>> SendRequestAsync<T>(HttpMethod method, string endpoint, object content = null)
        {
            try
            {
                var request = new HttpRequestMessage(method, endpoint);

                if (content != null)
                {
                    var json = JsonConvert.SerializeObject(content, _jsonSettings);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent, _jsonSettings);
                    return apiResponse ?? new ApiResponse<T> { Succeeded = true, Data = default(T) };
                }
                else
                {
                    return HandleFailureResponse<T>(responseContent, (int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<T>
                {
                    Succeeded = false,
                    Message = $"Error en la petición HTTP: {ex.Message}",
                    ErrorCode = "HTTP_ERROR"
                };
            }
        }

        private ApiResponse<T> HandleFailureResponse<T>(string responseContent, int statusCode)
        {
            try
            {
                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent, _jsonSettings);
                if (errorResponse != null)
                {
                    errorResponse.Succeeded = false;
                    return errorResponse;
                }
            }
            catch { }

            return new ApiResponse<T>
            {
                Succeeded = false,
                Message = $"Error HTTP {statusCode}: {responseContent}",
                ErrorCode = $"HTTP_{statusCode}"
            };
        }
    }

    #endregion

    #region Models y DTOs

    public class ApiResponse<T>
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public T Data { get; set; }
    }

    public class BaseDto
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class PagedList<T>
    {
        public List<T> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
    }

    #endregion

    #region Invoice Models

    public class Invoice : BaseDto
    {
        public string VersionCode { get; set; } = "4.0";
        public string Series { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public string PaymentFormCode { get; set; }
        public string CurrencyCode { get; set; } = "MXN";
        public string TypeCode { get; set; } = "I";
        public string ExpeditionZipCode { get; set; }
        public string ExportCode { get; set; } = "01";
        public string PaymentMethodCode { get; set; } = "PUE";
        public decimal ExchangeRate { get; set; } = 1;
        public string PaymentConditions { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        
        public InvoiceIssuer Issuer { get; set; }
        public InvoiceRecipient Recipient { get; set; }
        public List<InvoiceItem> Items { get; set; }
        public List<InvoiceResponse> Responses { get; set; }
        public List<InvoicePayment> Payments { get; set; }
        public string SatStatus { get; set; }
    }

    public class InvoiceIssuer : BaseDto
    {
        public string Tin { get; set; }
        public string LegalName { get; set; }
        public string TaxRegimeCode { get; set; }
        public List<TaxCredential> TaxCredentials { get; set; }
    }

    public class InvoiceRecipient : BaseDto
    {
        public string Tin { get; set; }
        public string LegalName { get; set; }
        public string ZipCode { get; set; }
        public string TaxRegimeCode { get; set; }
        public string CfdiUseCode { get; set; }
        public string Email { get; set; }
    }

    public class InvoiceItem : BaseDto
    {
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }
        public string UnitOfMeasurementCode { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public string TaxObjectCode { get; set; }
        public decimal Discount { get; set; }
        public List<InvoiceItemTax> ItemTaxes { get; set; }
    }

    public class InvoiceItemTax
    {
        public decimal Base { get; set; }
        public string TaxCode { get; set; }
        public string TaxTypeCode { get; set; }
        public decimal TaxRate { get; set; }
        public string TaxFlagCode { get; set; }
    }

    public class InvoicePayment
    {
        public DateTime PaymentDate { get; set; }
        public string PaymentFormCode { get; set; }
        public string CurrencyCode { get; set; }
        public decimal ExchangeRate { get; set; } = 1;
        public decimal Amount { get; set; }
        public List<PaidInvoice> PaidInvoices { get; set; }
    }

    public class PaidInvoice
    {
        public string InvoiceId { get; set; }
        public string InvoiceUuid { get; set; }
        public string CurrencyCode { get; set; }
        public decimal ExchangeRate { get; set; } = 1;
        public int InstallmentNumber { get; set; }
        public decimal PreviousBalanceAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingBalanceAmount { get; set; }
        public List<PaidInvoiceTax> Taxes { get; set; }
    }

    public class PaidInvoiceTax
    {
        public decimal Base { get; set; }
        public string TaxCode { get; set; }
        public string TaxTypeCode { get; set; }
        public decimal TaxRate { get; set; }
        public string TaxFlagCode { get; set; }
    }

    public class InvoiceResponse : BaseDto
    {
        public string InvoiceId { get; set; }
        public string InvoiceUuid { get; set; }
        public string InvoiceCertificateNumber { get; set; }
        public string InvoiceBase64Sello { get; set; }
        public DateTime InvoiceSignatureDate { get; set; }
        public string InvoiceBase64QrCode { get; set; }
        public string InvoiceBase64 { get; set; }
        public string SatBase64Sello { get; set; }
        public string SatBase64OriginalString { get; set; }
        public string SatCertificateNumber { get; set; }
    }

    public class TaxCredential
    {
        public string Base64File { get; set; }
        public FileType FileType { get; set; }
        public string Password { get; set; }
    }

    public enum FileType
    {
        CertificateCsd,
        PrivateKeyCsd
    }

    public class CancelInvoiceRequest
    {
        public string Id { get; set; }
        public string InvoiceUuid { get; set; }
        public string Tin { get; set; }
        public string CancellationReasonCode { get; set; }
        public string ReplacementUuid { get; set; }
        public List<TaxCredential> TaxCredentials { get; set; }

        // Alias para compatibilidad con código existente
        public string Uuid
        {
            get => InvoiceUuid;
            set => InvoiceUuid = value;
        }

        public string Rfc
        {
            get => Tin;
            set => Tin = value;
        }

        public string ReasonCode
        {
            get => CancellationReasonCode;
            set => CancellationReasonCode = value;
        }

        public string SubstitutionFolio
        {
            get => ReplacementUuid;
            set => ReplacementUuid = value;
        }
    }

    public class CancelInvoiceResponse : BaseDto
    {
        public string Base64CancellationAcknowledgement { get; set; }
        public Dictionary<string, string> InvoiceUuids { get; set; }
        public string Status { get; set; }
    }

    public class InvoiceStatusRequest
    {
        public string Id { get; set; }
        public string IssuerTin { get; set; }
        public string RecipientTin { get; set; }
        public decimal InvoiceTotal { get; set; }
        public string InvoiceUuid { get; set; }
        public string Last8DigitsIssuerSignature { get; set; }
    }

    public class InvoiceStatusResponse
    {
        public string StatusCode { get; set; }
        public string Status { get; set; }
        public string CancelableStatus { get; set; }
        public string CancellationStatus { get; set; }
        public string EfosValidation { get; set; }
    }

    public class CreatePdfRequest
    {
        public string InvoiceId { get; set; }
        public string Base64Logo { get; set; }
        public string BandColor { get; set; }
        public string FontColor { get; set; }
    }

    public class SendInvoiceRequest
    {
        public string InvoiceId { get; set; }
        public string Base64Logo { get; set; }
        public string BandColor { get; set; }
        public string FontColor { get; set; }
        public string ToEmail { get; set; }
    }

    public class FileResponse
    {
        public string Base64Content { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }

    #endregion

    #region Catalog Models

    public class CatalogDto : BaseDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    #endregion

    #region Product Models

    public class Product : BaseDto
    {
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public string SatUnitMeasurementId { get; set; }
        public string SatTaxObjectId { get; set; }
        public string SatProductCodeId { get; set; }
        public List<ProductTax> ProductTaxes { get; set; }
        
        // Aliases para compatibilidad
        public string Name
        {
            get => Description;
            set => Description = value;
        }
        public decimal Price
        {
            get => UnitPrice;
            set => UnitPrice = value;
        }
        public string ProductCode
        {
            get => SatProductCodeId;
            set => SatProductCodeId = value;
        }
        public string UnitCode
        {
            get => SatUnitMeasurementId;
            set => SatUnitMeasurementId = value;
        }
    }

    public class ProductTax
    {
        public decimal Rate { get; set; }
        public string TaxId { get; set; }
        public string TaxFlagId { get; set; }
        public string TaxTypeId { get; set; }
    }

    #endregion

    #region Person Models

    public class Person : BaseDto
    {
        public string LegalName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Tin { get; set; }
        public string TaxRegimeCode { get; set; }
        public string ZipCode { get; set; }
    }

    public class SatCfdiSearchResult
    {
        public List<SatCfdiItem> Items { get; set; }
        public int TotalRecords { get; set; }
    }

    public class SatCfdiItem
    {
        public string Uuid { get; set; }
        public string IssuerTin { get; set; }
        public string RecipientTin { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public string TypeCode { get; set; }
    }

    #endregion

    #region TaxFile Models (Certificados CSD/FIEL)

    public class TaxFile : BaseDto
    {
        public string PersonId { get; set; }
        public string Tin { get; set; } // RFC
        public string Base64File { get; set; }
        public FileType FileType { get; set; }
        public string Password { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int Sequence { get; set; }
    }

    #endregion

    #region DownloadRequest Models (Descarga Masiva SAT)

    public class DownloadRequest : BaseDto
    {
        public string IssuerTin { get; set; }
        public string RequesterTin { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SatQueryTypeId { get; set; }
        public string SatInvoiceTypeId { get; set; }
        public string SatInvoiceStatusId { get; set; }
        public string SatInvoiceComplementId { get; set; }
        public string SatRequestStatusId { get; set; }
        public string DownloadRequestStatusId { get; set; }
        public DateTime? LastAttemptDate { get; set; }
        public DateTime? NextAttemptDate { get; set; }
        public int InvoiceCount { get; set; }
        public int PackageCount { get; set; }
    }

    public class DownloadPackage : BaseDto
    {
        public string DownloadRequestId { get; set; }
        public string PackageId { get; set; }
        public string Status { get; set; }
        public int InvoiceCount { get; set; }
    }

    public class MetadataItem : BaseDto
    {
        public string InvoiceUuid { get; set; }
        public string IssuerTin { get; set; }
        public string IssuerName { get; set; }
        public string RecipientTin { get; set; }
        public string RecipientName { get; set; }
        public string PacTin { get; set; }
        public DateTime Date { get; set; }
        public DateTime CertificationDate { get; set; }
        public decimal Total { get; set; }
        public string EffectCode { get; set; }
        public int Status { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string DownloadPackageId { get; set; }
        public string DownloadRequestId { get; set; }
    }

    #endregion

    #region Service Interfaces

    public interface IFiscalApiClient
    {
        IInvoiceService Invoices { get; }
        ICatalogService Catalogs { get; }
        IProductService Products { get; }
        IPersonService Persons { get; }
        ISatCfdiService SatCfdi { get; }
        ITaxFileService TaxFiles { get; }
        IDownloadRequestService DownloadRequests { get; }
    }

    public interface IFiscalApiService<T> where T : BaseDto
    {
        Task<ApiResponse<PagedList<T>>> GetListAsync(int pageNumber, int pageSize);
        Task<ApiResponse<T>> GetByIdAsync(string id, bool details = false);
        Task<ApiResponse<T>> CreateAsync(T model);
        Task<ApiResponse<T>> UpdateAsync(string id, T model);
        Task<ApiResponse<bool>> DeleteAsync(string id);
    }

    public interface IInvoiceService : IFiscalApiService<Invoice>
    {
        Task<ApiResponse<CancelInvoiceResponse>> CancelAsync(CancelInvoiceRequest requestModel);
        Task<ApiResponse<FileResponse>> GetPdfAsync(CreatePdfRequest requestModel);
        Task<ApiResponse<FileResponse>> GetXmlAsync(string id);
        Task<ApiResponse<bool>> SendAsync(SendInvoiceRequest requestModel);
        Task<ApiResponse<bool>> SendByEmailAsync(string invoiceId, string email);
        Task<ApiResponse<InvoiceStatusResponse>> GetStatusAsync(InvoiceStatusRequest requestModel);
    }

    public interface ICatalogService : IFiscalApiService<CatalogDto>
    {
        Task<ApiResponse<List<string>>> GetListAsync();
        Task<ApiResponse<PagedList<CatalogDto>>> SearchCatalogAsync(string catalogName, string searchTerm, int pageNumber, int pageSize);
    }

    public interface IProductService : IFiscalApiService<Product>
    {
        Task<ApiResponse<List<ProductTax>>> GetTaxesAsync(string id);
        Task<ApiResponse<Product>> GetAsync(string id);
        Task<ApiResponse<PagedList<Product>>> ListAsync(int pageNumber = 1, int pageSize = 100);
    }

    public interface IPersonService : IFiscalApiService<Person>
    {
        Task<ApiResponse<PagedList<Person>>> SearchAsync(string searchTerm, int pageNumber = 1, int pageSize = 100);
        Task<ApiResponse<PagedList<Person>>> GetAllAsync(int pageNumber = 1, int pageSize = 100);
    }

    public interface ISatCfdiService
    {
        Task<ApiResponse<SatCfdiSearchResult>> SearchAsync(Dictionary<string, string> filtros);
        Task<ApiResponse<byte[]>> DownloadZipAsync(Dictionary<string, string> filtros);
    }

    public interface ITaxFileService : IFiscalApiService<TaxFile>
    {
        Task<ApiResponse<List<TaxFile>>> GetDefaultReferencesAsync(string personId);
        Task<ApiResponse<List<TaxFile>>> GetDefaultValuesAsync(string personId);
    }

    public interface IDownloadRequestService : IFiscalApiService<DownloadRequest>
    {
        Task<ApiResponse<List<MetadataItem>>> GetMetadataAsync(string requestId);
        Task<ApiResponse<List<FileResponse>>> DownloadPackageAsync(string requestId);
        Task<ApiResponse<FileResponse>> DownloadSatRequestAsync(string requestId);
        Task<ApiResponse<FileResponse>> DownloadSatResponseAsync(string requestId);
        Task<ApiResponse<List<DownloadRequest>>> SearchAsync(DateTime createdAt);
    }

    #endregion

    #region Service Implementations

    public abstract class BaseFiscalApiService<T> : IFiscalApiService<T> where T : BaseDto
    {
        protected readonly IFiscalApiHttpClient HttpClient;
        protected readonly string ResourcePath;
        protected readonly string ApiVersion;

        protected BaseFiscalApiService(IFiscalApiHttpClient httpClient, string resourcePath, string apiVersion)
        {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            ResourcePath = resourcePath ?? throw new ArgumentNullException(nameof(resourcePath));
            ApiVersion = apiVersion ?? "v4";
        }

        protected virtual string BuildEndpoint(string path = "", IDictionary<string, string> queryParams = null)
        {
            var endpoint = $"api/{ApiVersion}/{ResourcePath}";
            if (!string.IsNullOrEmpty(path))
                endpoint += $"/{path}";

            if (queryParams != null && queryParams.Count > 0)
            {
                var queryString = string.Join("&", 
                    Array.ConvertAll(new List<string>(queryParams.Keys).ToArray(), 
                        key => $"{key}={queryParams[key]}"));
                endpoint += $"?{queryString}";
            }

            return endpoint;
        }

        public virtual Task<ApiResponse<PagedList<T>>> GetListAsync(int pageNumber, int pageSize)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "PageNumber", pageNumber.ToString() },
                { "PageSize", pageSize.ToString() }
            };
            return HttpClient.GetAsync<PagedList<T>>(BuildEndpoint(queryParams: queryParams));
        }

        public virtual Task<ApiResponse<T>> GetByIdAsync(string id, bool details = false)
        {
            var parameters = new Dictionary<string, string>
            {
                { "details", details.ToString().ToLower() }
            };
            return HttpClient.GetByIdAsync<T>(BuildEndpoint(id, parameters));
        }

        public virtual Task<ApiResponse<T>> CreateAsync(T entity)
        {
            return HttpClient.PostAsync<T>(BuildEndpoint(), entity);
        }

        public virtual Task<ApiResponse<T>> UpdateAsync(string id, T entity)
        {
            return HttpClient.PutAsync<T>(BuildEndpoint(id), entity);
        }

        public virtual Task<ApiResponse<bool>> DeleteAsync(string id)
        {
            return HttpClient.DeleteAsync(BuildEndpoint(id));
        }
    }

    public class InvoiceService : BaseFiscalApiService<Invoice>, IInvoiceService
    {
        private const string IncomeEndpoint = "income";
        private const string CreditNoteEndpoint = "credit-note";
        private const string PaymentEndpoint = "payment";

        public InvoiceService(IFiscalApiHttpClient httpClient, string apiVersion)
            : base(httpClient, "invoices", apiVersion)
        {
        }

        public override async Task<ApiResponse<Invoice>> CreateAsync(Invoice requestModel)
        {
            if (requestModel == null)
                throw new ArgumentNullException(nameof(requestModel));

            string endpoint;
            switch (requestModel.TypeCode)
            {
                case "I":
                    endpoint = BuildEndpoint(IncomeEndpoint);
                    break;
                case "E":
                    endpoint = BuildEndpoint(CreditNoteEndpoint);
                    break;
                case "P":
                    endpoint = BuildEndpoint(PaymentEndpoint);
                    break;
                default:
                    throw new InvalidOperationException($"Tipo de factura no soportado: {requestModel.TypeCode}");
            }

            return await HttpClient.PostAsync<Invoice>(endpoint, requestModel);
        }

        public async Task<ApiResponse<CancelInvoiceResponse>> CancelAsync(CancelInvoiceRequest requestModel)
        {
            if (requestModel == null)
                throw new ArgumentNullException(nameof(requestModel));

            return await HttpClient.DeleteAsync<CancelInvoiceResponse>(BuildEndpoint(), requestModel);
        }

        public async Task<ApiResponse<FileResponse>> GetPdfAsync(CreatePdfRequest requestModel)
        {
            if (requestModel == null)
                throw new ArgumentNullException(nameof(requestModel));

            return await HttpClient.PostAsync<FileResponse>(BuildEndpoint("pdf"), requestModel);
        }

        public Task<ApiResponse<FileResponse>> GetXmlAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            return HttpClient.GetAsync<FileResponse>(BuildEndpoint($"{id}/xml"));
        }

        public Task<ApiResponse<bool>> SendAsync(SendInvoiceRequest requestModel)
        {
            return HttpClient.PostAsync<bool>(BuildEndpoint("send"), requestModel);
        }

        public Task<ApiResponse<bool>> SendByEmailAsync(string invoiceId, string email)
        {
            var request = new SendInvoiceRequest
            {
                InvoiceId = invoiceId,
                ToEmail = email
            };
            return SendAsync(request);
        }

        public Task<ApiResponse<InvoiceStatusResponse>> GetStatusAsync(InvoiceStatusRequest requestModel)
        {
            return HttpClient.PostAsync<InvoiceStatusResponse>(BuildEndpoint("status"), requestModel);
        }
    }

    public class CatalogService : BaseFiscalApiService<CatalogDto>, ICatalogService
    {
        public CatalogService(IFiscalApiHttpClient httpClient, string apiVersion)
            : base(httpClient, "catalogs", apiVersion)
        {
        }

        public Task<ApiResponse<List<string>>> GetListAsync()
        {
            return HttpClient.GetAsync<List<string>>(BuildEndpoint());
        }

        public Task<ApiResponse<PagedList<CatalogDto>>> SearchCatalogAsync(string catalogName, string searchTerm, int pageNumber, int pageSize)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "search", searchTerm },
                { "PageNumber", pageNumber.ToString() },
                { "PageSize", pageSize.ToString() }
            };
            return HttpClient.GetAsync<PagedList<CatalogDto>>(BuildEndpoint($"{catalogName}", queryParams));
        }
    }

    public class ProductService : BaseFiscalApiService<Product>, IProductService
    {
        public ProductService(IFiscalApiHttpClient httpClient, string apiVersion)
            : base(httpClient, "products", apiVersion)
        {
        }

        public Task<ApiResponse<List<ProductTax>>> GetTaxesAsync(string id)
        {
            var path = $"{id}/taxes";
            var endpoint = BuildEndpoint(path);
            return HttpClient.GetAsync<List<ProductTax>>(endpoint);
        }

        public Task<ApiResponse<Product>> GetAsync(string id)
        {
            return GetByIdAsync(id);
        }

        public Task<ApiResponse<PagedList<Product>>> ListAsync(int pageNumber = 1, int pageSize = 100)
        {
            return GetListAsync(pageNumber, pageSize);
        }
    }

    public class PersonService : BaseFiscalApiService<Person>, IPersonService
    {
        public PersonService(IFiscalApiHttpClient httpClient, string apiVersion)
            : base(httpClient, "persons", apiVersion)
        {
        }

        public Task<ApiResponse<PagedList<Person>>> SearchAsync(string searchTerm, int pageNumber = 1, int pageSize = 100)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "search", searchTerm },
                { "PageNumber", pageNumber.ToString() },
                { "PageSize", pageSize.ToString() }
            };
            return HttpClient.GetAsync<PagedList<Person>>(BuildEndpoint(queryParams: queryParams));
        }

        public Task<ApiResponse<PagedList<Person>>> GetAllAsync(int pageNumber = 1, int pageSize = 100)
        {
            return GetListAsync(pageNumber, pageSize);
        }
    }

    public class SatCfdiService : ISatCfdiService
    {
        private readonly IFiscalApiHttpClient _httpClient;
        private readonly string _apiVersion;

        public SatCfdiService(IFiscalApiHttpClient httpClient, string apiVersion)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiVersion = apiVersion ?? "v4";
        }

        public Task<ApiResponse<SatCfdiSearchResult>> SearchAsync(Dictionary<string, string> filtros)
        {
            var endpoint = BuildEndpoint("search", filtros);
            return _httpClient.GetAsync<SatCfdiSearchResult>(endpoint);
        }

        public Task<ApiResponse<byte[]>> DownloadZipAsync(Dictionary<string, string> filtros)
        {
            var endpoint = BuildEndpoint("download-zip", filtros);
            return _httpClient.GetAsync<byte[]>(endpoint);
        }

        private string BuildEndpoint(string path = "", IDictionary<string, string> queryParams = null)
        {
            var endpoint = $"api/{_apiVersion}/sat-cfdi";
            if (!string.IsNullOrEmpty(path))
                endpoint += $"/{path}";

            if (queryParams != null && queryParams.Count > 0)
            {
                var queryString = string.Join("&", 
                    Array.ConvertAll(new List<string>(queryParams.Keys).ToArray(), 
                        key => $"{key}={queryParams[key]}"));
                endpoint += $"?{queryString}";
            }

            return endpoint;
        }
    }

    public class TaxFileService : BaseFiscalApiService<TaxFile>, ITaxFileService
    {
        public TaxFileService(IFiscalApiHttpClient httpClient, string apiVersion)
            : base(httpClient, "tax-files", apiVersion)
        {
        }

        public Task<ApiResponse<List<TaxFile>>> GetDefaultReferencesAsync(string personId)
        {
            // GET /api/v4/tax-files/{personId}/default-references
            var path = $"{personId}/default-references";
            var endpoint = BuildEndpoint(path);
            return HttpClient.GetAsync<List<TaxFile>>(endpoint);
        }

        public Task<ApiResponse<List<TaxFile>>> GetDefaultValuesAsync(string personId)
        {
            // GET /api/v4/tax-files/{personId}/default-values
            var path = $"{personId}/default-values";
            var endpoint = BuildEndpoint(path);
            return HttpClient.GetAsync<List<TaxFile>>(endpoint);
        }
    }

    public class DownloadRequestService : BaseFiscalApiService<DownloadRequest>, IDownloadRequestService
    {
        public DownloadRequestService(IFiscalApiHttpClient httpClient, string apiVersion)
            : base(httpClient, "download-requests", apiVersion)
        {
        }

        public Task<ApiResponse<List<MetadataItem>>> GetMetadataAsync(string requestId)
        {
            // GET /api/v4/download-requests/{requestId}/metadata
            var path = $"{requestId}/metadata";
            var endpoint = BuildEndpoint(path);
            return HttpClient.GetAsync<List<MetadataItem>>(endpoint);
        }

        public Task<ApiResponse<List<FileResponse>>> DownloadPackageAsync(string requestId)
        {
            // GET /api/v4/download-requests/{requestId}/download-package
            var path = $"{requestId}/download-package";
            var endpoint = BuildEndpoint(path);
            return HttpClient.GetAsync<List<FileResponse>>(endpoint);
        }

        public Task<ApiResponse<FileResponse>> DownloadSatRequestAsync(string requestId)
        {
            // GET /api/v4/download-requests/{requestId}/download-sat-request
            var path = $"{requestId}/download-sat-request";
            var endpoint = BuildEndpoint(path);
            return HttpClient.GetAsync<FileResponse>(endpoint);
        }

        public Task<ApiResponse<FileResponse>> DownloadSatResponseAsync(string requestId)
        {
            // GET /api/v4/download-requests/{requestId}/download-sat-response
            var path = $"{requestId}/download-sat-response";
            var endpoint = BuildEndpoint(path);
            return HttpClient.GetAsync<FileResponse>(endpoint);
        }

        public Task<ApiResponse<List<DownloadRequest>>> SearchAsync(DateTime createdAt)
        {
            // GET /api/v4/download-requests/search?createdAt=2025-08-21
            var path = $"search?createdAt={createdAt:yyyy-MM-dd}";
            var endpoint = BuildEndpoint(path);
            return HttpClient.GetAsync<List<DownloadRequest>>(endpoint);
        }
    }

    #endregion
}
