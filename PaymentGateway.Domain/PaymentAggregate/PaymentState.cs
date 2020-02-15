using System;

namespace PaymentGateway.Domain.PaymentAggregate
{
    public enum PaymentState
    {
        Created,
        PaymentFailed,
        PaymentSuccessful
    }
}