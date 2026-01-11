using System;
using System.ComponentModel.DataAnnotations;

namespace Api_Policy.Models.Dtos;

public record PolicyCreateDto(
    [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 200 caracteres")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s\.'-]+$", ErrorMessage = "El nombre solo puede contener letras, espacios y caracteres válidos")]
    string ClientName, 
    
    [Required(ErrorMessage = "El monto de cobertura es obligatorio")]
    [Range(1000, 10000000, ErrorMessage = "El monto de cobertura debe estar entre $1,000 y $10,000,000")]
    decimal CoverageAmount, 
    
    [Required(ErrorMessage = "La duración en meses es obligatoria")]
    [Range(1, 120, ErrorMessage = "La duración debe estar entre 1 y 120 meses")]
    int MonthsDuration
);

public record PolicyReadDto(
    int Id, 
    string PolicyNumber, 
    string ClientName, 
    decimal MonthlyPremium, 
    string Status
);
