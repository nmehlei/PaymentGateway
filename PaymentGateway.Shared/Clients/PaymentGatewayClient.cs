using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PaymentGateway.Shared.Exceptions;
using PaymentGateway.Shared.Models;

namespace PaymentGateway.Shared.Clients
{
    public class PaymentGatewayClient : IPaymentGatewayClient
    {
        public PaymentGatewayClient(Guid merchantId, string apiKey, string paymentGatewayBaseAddress = null,
                                    TimeSpan? defaultRequestTimeout = null)
        {
            if (merchantId == Guid.Empty)
                throw new ArgumentOutOfRangeException(nameof(merchantId), "merchantId was not defined");
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("The API key is invalid", nameof(apiKey));

            _paymentGatewayBaseAddress = paymentGatewayBaseAddress ?? PaymentGatewayConstants.DefaultPaymentGatewayBaseAddress;

            // make sure that base address ends in a slash (/)
            if (_paymentGatewayBaseAddress[_paymentGatewayBaseAddress.Length - 1] != '/')
                _paymentGatewayBaseAddress = _paymentGatewayBaseAddress + '/';
            
            _merchantId = merchantId;
            _apiKey = apiKey;
            _defaultRequestTimeout = defaultRequestTimeout ?? TimeSpan.FromSeconds(60);
            InitializeClient();
        }

        private readonly Guid _merchantId;
        private readonly string _apiKey;
        private readonly string _paymentGatewayBaseAddress;
        private readonly TimeSpan _defaultRequestTimeout;
        private HttpClient _httpClient;

        protected const string UsedMediaType = "application/json";

        private void InitializeClient()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(_paymentGatewayBaseAddress, UriKind.Absolute),
                Timeout = _defaultRequestTimeout
            };

            _httpClient.DefaultRequestHeaders.Add(PaymentGatewayConstants.MerchantHeaderName, _merchantId.ToString());
            _httpClient.DefaultRequestHeaders.Add(PaymentGatewayConstants.ApiKeyHeaderName, 
                Convert.ToBase64String(Encoding.UTF8.GetBytes(_apiKey)));
        }

        private async Task<string> TryGetErrorMessage(HttpResponseMessage responseRaw)
        {
            var errorResponseJson = await responseRaw.Content.ReadAsStringAsync().ConfigureAwait(false);
            var errorResponseObj = JsonSerializer.Deserialize<ErrorResponseModel>(errorResponseJson);
            return errorResponseObj?.ErrorDescription;
        }

        public async Task<GetPaymentResponseModel> GetPaymentRequestAsync(GetPaymentRequestModel requestModel)
        {
            var url = string.Format("{0}", requestModel.PaymentId);
            var responseRaw = await _httpClient.GetAsync(url).ConfigureAwait(false);

            // Possible improvement: Optimize resiliency with retry behavior

            if (!responseRaw.IsSuccessStatusCode)
            {
                var errorMessage = await TryGetErrorMessage(responseRaw).ConfigureAwait(false);
                throw new PaymentGatewayRequestException(responseRaw.StatusCode, responseRaw.ReasonPhrase, errorMessage);
            }

            var responseJson = await responseRaw.Content.ReadAsStringAsync().ConfigureAwait(false);
            var response = JsonSerializer.Deserialize<GetPaymentResponseModel>(responseJson);
            return response;
        }

        public async Task<CreatePaymentResponseModel> PostPaymentRequestAsync(CreatePaymentRequestModel requestModel)
        {
            var url = string.Empty;
            var stringContent = new StringContent(JsonSerializer.Serialize(requestModel), Encoding.UTF8, UsedMediaType);
            var responseRaw = await _httpClient.PostAsync(url, stringContent).ConfigureAwait(false);
            
            // Possible improvement: Optimize resiliency with retry behavior

            if (!responseRaw.IsSuccessStatusCode)
            {
                var errorMessage = await TryGetErrorMessage(responseRaw).ConfigureAwait(false);
                throw new PaymentGatewayRequestException(responseRaw.StatusCode, responseRaw.ReasonPhrase, errorMessage);
            }
            
            var responseJson = await responseRaw.Content.ReadAsStringAsync().ConfigureAwait(false);
            var response = JsonSerializer.Deserialize<CreatePaymentResponseModel>(responseJson);
            return response;
        }
    }
}