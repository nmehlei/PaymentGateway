using System;
using System.Net;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace PaymentGateway.Shared.Exceptions
{
    public class PaymentGatewayRequestException : Exception
    {
        // Constructors

        internal PaymentGatewayRequestException(HttpStatusCode statusCode, string reasonPhrase, string errorMessage)
            : base("The request to the payment gateway service failed with status code " + statusCode + ", reason \"" + (reasonPhrase ?? "n/a")
                + "\" and error message \"" + errorMessage + "\".")
        {
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
            ErrorMessage = errorMessage;
        }

        // Properties

        public HttpStatusCode StatusCode { get; }
        public string ReasonPhrase { get; }
        public string ErrorMessage { get; }
    }
}