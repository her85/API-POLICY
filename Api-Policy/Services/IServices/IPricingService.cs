using Api_Policy.Models;

public interface IPricingService
{
    decimal CalculateMonthlyPremium(decimal coverageAmount);
    decimal CalculateMonthlyPremium(decimal coverageAmount, int age, PolicyType policyType, int claimHistory = 0, bool hasDiscounts = false);
}