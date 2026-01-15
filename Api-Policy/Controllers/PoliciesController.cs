using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using Api_Policy.Data;
using Api_Policy.Models;
using Api_Policy.Models.Dtos;

namespace Api_Policy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PoliciesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPricingService _pricingService;

    public PoliciesController(AppDbContext context, IPricingService pricingService)
    {
        _context = context;
        _pricingService = pricingService;
    }

    // GET: api/policies
    [HttpGet]
    [EnableRateLimiting("GetPolicies")]
    public async Task<ActionResult<IEnumerable<PolicyReadDto>>> GetPolicies(
        [FromQuery] string? clientName = null,
        [FromQuery] decimal? minCoverage = null,
        [FromQuery] decimal? maxCoverage = null,
        [FromQuery] string? status = null)
    {
        var query = _context.Policies.AsQueryable();

        // Filtro por nombre de cliente (búsqueda parcial)
        if (!string.IsNullOrWhiteSpace(clientName))
        {
            query = query.Where(p => p.ClientName.Contains(clientName));
        }

        // Filtro por monto mínimo de cobertura
        if (minCoverage.HasValue)
        {
            query = query.Where(p => p.CoverageAmount >= minCoverage.Value);
        }

        // Filtro por monto máximo de cobertura
        if (maxCoverage.HasValue)
        {
            query = query.Where(p => p.CoverageAmount <= maxCoverage.Value);
        }

        // Filtro por estado
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<PolicyStatus>(status, true, out var parsedStatus))
        {
            query = query.Where(p => p.Status == parsedStatus);
        }

        return await query
            .Select(p => new PolicyReadDto(
                p.Id, p.PolicyNumber, p.ClientName, p.ClientAge, p.Type.ToString(), p.CoverageAmount, p.MonthlyPremium, p.Status.ToString()
            ))
            .ToListAsync();
    }

    // GET: api/policies/{policyNumber}
    [HttpGet("{policyNumber}")]
    [EnableRateLimiting("GetPolicies")]
    public async Task<ActionResult<PolicyReadDto>> GetPolicyByNumber(string policyNumber)
    {
        var policy = await _context.Policies
            .FirstOrDefaultAsync(p => p.PolicyNumber == policyNumber);

        if (policy == null)
        {
            return NotFound(new { message = $"No se encontró la póliza con número: {policyNumber}" });
        }

        return new PolicyReadDto(
            policy.Id, 
            policy.PolicyNumber, 
            policy.ClientName,
            policy.ClientAge,
            policy.Type.ToString(),
            policy.CoverageAmount, 
            policy.MonthlyPremium, 
            policy.Status.ToString()
        );
    }

    // POST: api/policies/quote (Solo cotizar, sin crear)
    [HttpPost("quote")]
    public ActionResult<QuoteDto> GetQuote(PolicyCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (dto.CoverageAmount < 1000)
            return BadRequest("La cobertura mínima es de $1,000");

        // Calcular prima mensual
        var monthlyPremium = _pricingService.CalculateMonthlyPremium(
            dto.CoverageAmount,
            dto.ClientAge,
            dto.PolicyType,
            dto.ClaimHistory,
            dto.HasDiscounts
        );

        var totalCost = monthlyPremium * dto.MonthsDuration;

        // Desglose de cálculo
        var breakdown = $"Tipo: {dto.PolicyType}, Edad: {dto.ClientAge}, Cobertura: ${dto.CoverageAmount:N2}, " +
                       $"Reclamos previos: {dto.ClaimHistory}, Descuento: {(dto.HasDiscounts ? "Sí" : "No")}";

        return new QuoteDto(
            monthlyPremium,
            totalCost,
            dto.PolicyType.ToString(),
            dto.CoverageAmount,
            dto.MonthsDuration,
            breakdown
        );
    }

    // POST: api/policies (Cotizar y Crear)
    [HttpPost]
    [EnableRateLimiting("CreatePolicy")]
    public async Task<ActionResult<PolicyReadDto>> CreatePolicy(PolicyCreateDto dto)
    {
        // 1. Validar el modelo
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // 2. Sanitizar el nombre del cliente (remover caracteres peligrosos)
        var sanitizedClientName = SanitizeInput(dto.ClientName);
        
        // 3. Lógica de negocio: Validar cobertura mínima
        if (dto.CoverageAmount < 1000) 
            return BadRequest("La cobertura mínima es de $1,000");

        // Usar el servicio de precios con factores avanzados
        var monthlyPremium = _pricingService.CalculateMonthlyPremium(
            dto.CoverageAmount,
            dto.ClientAge,
            dto.PolicyType,
            dto.ClaimHistory,
            dto.HasDiscounts
        );

        // 4. Mapear a la entidad
        var policy = new Policy
        {
            PolicyNumber = $"POL-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            ClientName = sanitizedClientName,
            ClientAge = dto.ClientAge,
            Type = dto.PolicyType,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(dto.MonthsDuration),
            CoverageAmount = dto.CoverageAmount,
            MonthlyPremium = monthlyPremium,
            Status = PolicyStatus.Activa
        };

        _context.Policies.Add(policy);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPolicyByNumber), new { policyNumber = policy.PolicyNumber }, 
            new PolicyReadDto(policy.Id, policy.PolicyNumber, policy.ClientName, policy.ClientAge, policy.Type.ToString(), policy.CoverageAmount, policy.MonthlyPremium, policy.Status.ToString()));
    }

    /// <summary>
    /// Sanitiza el input del usuario para prevenir inyección de código
    /// </summary>
    private string SanitizeInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Remover caracteres de control y normalizar espacios
        var sanitized = input.Trim();
        sanitized = System.Text.RegularExpressions.Regex.Replace(sanitized, @"\s+", " ");
        
        // Remover caracteres potencialmente peligrosos (HTML, SQL, scripts)
        sanitized = sanitized
            .Replace("<", "")
            .Replace(">", "")
            .Replace("&", "")
            .Replace("\"", "")
            .Replace("'", "")
            .Replace(";", "")
            .Replace("--", "")
            .Replace("/*", "")
            .Replace("*/", "")
            .Replace("=", "");

        return sanitized;
    }
}