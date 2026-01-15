public enum PolicyStatus { Borrador, Activa, Expirada, Cancelada }

public enum PolicyType { Vida, Auto, Hogar, Salud, Otro }

public class Policy
{
    public int Id { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public int ClientAge { get; set; }
    public PolicyType Type { get; set; } = PolicyType.Vida;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    // El "Premium" es el costo del seguro
    public decimal CoverageAmount { get; set; }
    public decimal MonthlyPremium { get; set; }
    
    public PolicyStatus Status { get; set; } = PolicyStatus.Borrador;
    
    // Relación con Siniestros
    public List<Claim> Claims { get; set; } = new();
}

