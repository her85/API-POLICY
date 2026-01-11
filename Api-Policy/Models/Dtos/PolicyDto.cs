using System;

namespace Api_Policy.Models.Dtos;

public record PolicyCreateDto(
    string ClientName, 
    decimal CoverageAmount, 
    int MonthsDuration
);

public record PolicyReadDto(
    int Id, 
    string PolicyNumber, 
    string ClientName, 
    decimal MonthlyPremium, 
    string Status
);
