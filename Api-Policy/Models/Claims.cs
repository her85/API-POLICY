public class Claim
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime IncidentDate { get; set; }
    public decimal EstimatedLoss { get; set; }
    public bool IsApproved { get; set; }
    public int PolicyId { get; set; }
}