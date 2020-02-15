using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGateway.API.Helpers;
using PaymentGateway.Domain.DomainServices.BankConnector;
using PaymentGateway.Domain.MerchantAggregate;
using PaymentGateway.Domain.PaymentAggregate;
using PaymentGateway.Domain.SharedValueTypes;
using PaymentGateway.Persistence.Repositories;
using PaymentGateway.Shared;
using PaymentGateway.Shared.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace PaymentGateway.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentsController : ControllerBase
    {
        public PaymentsController(ILogger<PaymentsController> logger, IPaymentRepository paymentRepository, IMerchantRepository merchantRepository,
                                  IBankConnectorDomainService bankConnectorDS, ModelMapper modelMapper)
        {
            _logger = logger;
            _paymentRepository = paymentRepository;
            _merchantRepository = merchantRepository;
            _bankConnectorDS = bankConnectorDS;
            _modelMapper = modelMapper;
        }

        private readonly ILogger<PaymentsController> _logger;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMerchantRepository _merchantRepository;
        private readonly IBankConnectorDomainService _bankConnectorDS;
        private readonly ModelMapper _modelMapper;
        
        [HttpGet("{paymentId}")]
        [SwaggerOperation("Retrieve the details of a previously made payment")]
        [SwaggerResponse(200, "The payment was found", typeof(GetPaymentResponseModel))]
        [SwaggerResponse(400, "The request data is invalid", typeof(ErrorResponseModel))]
        public async Task<ActionResult> Get([FromRoute]GetPaymentRequestModel requestModel)
        {
            if (!EnsureMerchantValidity(out var merchant))
                return BadRequest(new ErrorResponseModel("missing or invalid merchant header"));

            _logger.LogInformation(requestModel.PaymentId.ToString());

            var payment = await _paymentRepository.GetByPaymentIdAsync(requestModel.PaymentId);

            if (payment == null)
                return NotFound();

            // Protect against access of payment data from other merchants
            if (payment.MerchantId != merchant.MerchantId)
                return BadRequest(new ErrorResponseModel("Merchant mismatch"));

            var responseModel = new GetPaymentResponseModel
            {
                Payment = _modelMapper.MapPayment(payment)
            };

            return Ok(responseModel);
        }

        [HttpPost("create")]
        [SwaggerOperation("Process a payment through the payment gateway and receive either a successful or unsuccessful response")]
        [SwaggerResponse(202, "The payment was created", typeof(CreatePaymentResponseModel))]
        [SwaggerResponse(400, "The request data is invalid", typeof(ErrorResponseModel))]
        public async Task<ActionResult> Create([FromBody]CreatePaymentRequestModel requestModel)
        {
            if (!EnsureMerchantValidity(out var merchant))
                return BadRequest(new ErrorResponseModel("missing or invalid merchant header"));

            var creditCardInformation = new CreditCardInformation(requestModel.CreditCardNumber,
                new ExpiryDate(requestModel.CreditCardExpiryMonth, requestModel.CreditCardExpiryYear),
                requestModel.CreditCardCcv);

            var paymentAmount = new PaymentAmount(requestModel.Amount, requestModel.CurrencyCode);
            var payment = Payment.Create(paymentAmount, creditCardInformation, merchant.MerchantId, requestModel.ExternalShopperIdentifier);

            bool wasPaymentSuccessful = await payment.AttemptPayment(merchant.CreditCardInformation, _bankConnectorDS);

            _paymentRepository.Add(payment);
            await _paymentRepository.PersistAsync();

            _logger.LogInformation($"New payment was created with PaymentId: {payment.PaymentId}");

            var responseModel = new CreatePaymentResponseModel
            {
                WasPaymentSuccessful = wasPaymentSuccessful,
                PaymentId = payment.PaymentId
            };

            return Accepted(responseModel);
        }

        /// <summary>
        /// Handle authentication (via Merchant ID) and authorization (via API key).
        /// MerchantId is delivered as a header since it will (probably) be necessary for
        /// every call and should not pollute all request-specific models.
        /// 
        /// Improvement Idea: Allow granular error messages to return to the caller,
        /// though this can in some instances be a slight security risk.
        /// Also: Introduce caching for enhanced performance and scalability.
        /// </summary>
        /// <param name="merchant">The Merchant based on the Merchant ID that was read from the headers</param>
        /// <returns></returns>
        private bool EnsureMerchantValidity(out Merchant merchant)
        {
            merchant = null;

            // check merchant ID
            if (!Request.Headers.ContainsKey(PaymentGatewayConstants.MerchantHeaderName))
                return false;

            var rawMerchantId = Request.Headers[PaymentGatewayConstants.MerchantHeaderName].First();
            if (!Guid.TryParse(rawMerchantId, out var parsedMerchantId))
                return false;

            // check if merchant with the merchant ID exists and is enabled
            merchant = _merchantRepository.GetByMerchantId(parsedMerchantId);
            if (merchant == null)
                return false;

            if (!merchant.IsEnabled)
                return false;

            // check api key
            if (!Request.Headers.ContainsKey(PaymentGatewayConstants.ApiKeyHeaderName))
                return false;

            var rawApiKey = Request.Headers[PaymentGatewayConstants.ApiKeyHeaderName].First();
            var apiKey = Encoding.UTF8.GetString(Convert.FromBase64String(rawApiKey));

            if (apiKey != merchant.ApiKey)
                return false;

            return true;
        }
    }
}