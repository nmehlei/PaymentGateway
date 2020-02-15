using System;

namespace PaymentGateway.Shared
{
    public static class PaymentGatewayConstants
    {
        public const string MerchantHeaderName = "PG-Merchant";
        public const string ApiKeyHeaderName = "PG-ApiKey";

        //TODO: change as soon as there is an official DNS name set for a testing/staging/production environment
        public const string DefaultPaymentGatewayBaseAddress = "https://localhost:32768/";
    }
}