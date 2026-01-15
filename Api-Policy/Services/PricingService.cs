using Api_Policy.Models;

public class PricingService : IPricingService
{
    // Método simple (retrocompatibilidad)
    public decimal CalculateMonthlyPremium(decimal coverageAmount)
    {
        // Regla de negocio: 0.5% del monto de cobertura + un fee base de 10$
        return (coverageAmount * 0.005m) + 10m;
    }

    // Método avanzado con múltiples factores
    public decimal CalculateMonthlyPremium(
        decimal coverageAmount, 
        int age, 
        PolicyType policyType, 
        int claimHistory = 0, 
        bool hasDiscounts = false)
    {
        // 1. Base: porcentaje según tipo de póliza
        decimal baseRate = policyType switch
        {
            PolicyType.Vida => 0.008m,      // 0.8% para vida
            PolicyType.Auto => 0.012m,      // 1.2% para auto
            PolicyType.Hogar => 0.006m,      // 0.6% para hogar
            PolicyType.Salud => 0.015m,    // 1.5% para salud
            _ => 0.005m                      // 0.5% por defecto
        };

        decimal basePremium = coverageAmount * baseRate;

        // 2. Factor de edad (más riesgo = más costo)
        decimal ageFactor = age switch
        {
            < 25 => 1.5m,      // Jóvenes: +50%
            < 35 => 1.2m,      // Adultos jóvenes: +20%
            < 50 => 1.0m,      // Adultos: sin cambio
            < 65 => 1.3m,      // Maduros: +30%
            _ => 1.6m          // Adultos mayores: +60%
        };

        basePremium *= ageFactor;

        // 3. Penalización por historial de reclamos
        if (claimHistory > 0)
        {
            decimal claimPenalty = claimHistory switch
            {
                1 => 1.15m,    // +15% por 1 reclamo
                2 => 1.30m,    // +30% por 2 reclamos
                >= 3 => 1.50m  // +50% por 3+ reclamos
,
                _ => throw new NotImplementedException()
            };
            basePremium *= claimPenalty;
        }

        // 4. Descuentos (buen cliente, paquetes, etc.)
        if (hasDiscounts)
        {
            basePremium *= 0.90m; // 10% de descuento
        }

        // 5. Fee base operacional
        decimal operationalFee = policyType switch
        {
            PolicyType.Vida => 15m,
            PolicyType.Auto => 20m,
            PolicyType.Hogar => 25m,
            PolicyType.Salud => 30m,
            _ => 10m
        };

        return basePremium + operationalFee;
    }
}