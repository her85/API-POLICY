using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public async Task<ActionResult<PolicyReadDto>> CreatePolicy(PolicyCreateDto dto)
    {
        // 1. Lógica de negocio: Validar cobertura mínima
        if (dto.CoverageAmount < 1000) 
            return BadRequest("La cobertura mínima es de $1,000");

        // 2. Usar el servicio de precios para calcular la prima
        var monthlyPremium = _pricingService.CalculateMonthlyPremium(dto.CoverageAmount);

        // 3. Mapear a la entidad
        var policy = new Policy
        {
            PolicyNumber = $"POL-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            ClientName = dto.ClientName,
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
}