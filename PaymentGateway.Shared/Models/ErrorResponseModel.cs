using System;

namespace PaymentGateway.Shared.Models
{
    public class ErrorResponseModel
    {
        public ErrorResponseModel(string errorDescription)
        {
            ErrorDescription = errorDescription;
        }

        public string ErrorDescription { get; }
    }
}