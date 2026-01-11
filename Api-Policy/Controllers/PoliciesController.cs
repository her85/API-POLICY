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
    public async Task<ActionResult<IEnumerable<PolicyReadDto>>> GetPolicies()
    {
        return await _context.Policies
            .Select(p => new PolicyReadDto(
                p.Id, p.PolicyNumber, p.ClientName, p.MonthlyPremium, p.Status.ToString()
            ))
            .ToListAsync();
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

        // Usar el servicio de precios para calcular la prima
        var monthlyPremium = _pricingService.CalculateMonthlyPremium(dto.CoverageAmount);

        // 4. Mapear a la entidad
        var policy = new Policy
        {
            PolicyNumber = $"POL-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            ClientName = sanitizedClientName,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(dto.MonthsDuration),
            CoverageAmount = dto.CoverageAmount,
            MonthlyPremium = monthlyPremium,
            Status = PolicyStatus.Active
        };

        _context.Policies.Add(policy);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPolicies), new { id = policy.Id }, 
            new PolicyReadDto(policy.Id, policy.PolicyNumber, policy.ClientName, policy.MonthlyPremium, policy.Status.ToString()));
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