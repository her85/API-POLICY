using System;
using System.ComponentModel.DataAnnotations;

namespace Api_Policy.Models.Dtos;

public record PolicyCreateDto(
    [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 200 caracteres")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s\.'-]+$", ErrorMessage = "El nombre solo puede contener letras, espacios y caracteres válidos")]
    string ClientName,
    
    [Required(ErrorMessage = "La edad del cliente es obligatoria")]
    [Range(18, 100, ErrorMessage = "La edad debe estar entre 18 y 100 años")]
    int ClientAge,
    
    [Required(ErrorMessage = "El tipo de póliza es obligatorio")]
    PolicyType PolicyType,
    
    [Required(ErrorMessage = "El monto de cobertura es obligatorio")]
    [Range(1000, 10000000, ErrorMessage = "El monto de cobertura debe estar entre $1,000 y $10,000,000")]
    decimal CoverageAmount, 
    
    [Required(ErrorMessage = "La duración en meses es obligatoria")]
    [Range(1, 120, ErrorMessage = "La duración debe estar entre 1 y 120 meses")]
    int MonthsDuration,
    
    [Range(0, 50, ErrorMessage = "El historial de reclamos debe estar entre 0 y 50")]
    int ClaimHistory = 0,
    
    bool HasDiscounts = false
);

public record PolicyReadDto(
    int Id, 
    string PolicyNumber, 
    string ClientName,
    int ClientAge,
    string PolicyType,
    decimal CoverageAmount, 
    decimal MonthlyPremium, 
    string Status
);

public record QuoteDto(
    decimal EstimatedMonthlyPremium,
    decimal TotalCost,
    string PolicyType,
    decimal CoverageAmount,
    int DurationMonths,
    string Breakdown
);
