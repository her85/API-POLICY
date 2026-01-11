public enum PolicyStatus { Draft, Active, Expired, Cancelled }

public class Policy
{
    public int Id { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    // El "Premium" es el costo del seguro
    public decimal CoverageAmount { get; set; }
    public decimal MonthlyPremium { get; set; }
    
    public PolicyStatus Status { get; set; } = PolicyStatus.Draft;
    
    // Relación con Siniestros
    public List<Claim> Claims { get; set; } = new();
}

