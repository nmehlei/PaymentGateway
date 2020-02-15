using System;

namespace PaymentGateway.Domain.DomainServices.BankConnector
{
    // NOTE: implemented as enum as this could easily evolve into different (sub-)states
    public enum PaymentOrderResultStatus
    {
        Successful,
        FailedDueToUnknownReason,
        FailedDueToRejectedCreditCard
    }
}