using System;

namespace PaymentGateway.Domain.DomainServices.Randomization
{
    public interface IRandomDomainService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxValue">exclusive upper bound</param>
        /// <returns></returns>
        int GetRandomNumber(int maxValue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minValue">inclusive lower bound</param>
        /// <param name="maxValue">exclusive upper bound</param>
        /// <returns></returns>
        int GetRandomNumber(int minValue, int maxValue);
        double GetRandomFloatingPointNumber();
        string GenerateRandomAlphanumericString(int length);
    }
}