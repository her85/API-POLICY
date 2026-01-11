public interface IPricingService
{
    decimal CalculateMonthlyPremium(decimal coverageAmount);
}

public class PricingService : IPricingService
{
    public decimal CalculateMonthlyPremium(decimal coverageAmount)
    {
        // Regla de negocio: 0.5% del monto de cobertura + un fee base de 10$
        return (coverageAmount * 0.005m) + 10m;
    }
}