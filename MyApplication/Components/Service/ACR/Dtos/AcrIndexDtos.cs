using System;
using System.Collections.Generic;

namespace MyApplication.Components.Service.Acr;

public sealed record AcrRequestListItem(
    int Id,
    int EmployeeId,
    string EmployeeName,
    string AcrType,
    string? AcrStatus,
    DateOnly EffectiveDate,
    DateTime? SubmitTime
);

// Filters
public sealed record AcrIndexFilter(
    int? AcrTypeId,
    int? AcrStatusId,
    string? EmployeeSearch,
    DateOnly? EffectiveFrom,
    DateOnly? EffectiveTo
);

// NEW: Paged Result Wrapper
public sealed record PagedResult<T>(List<T> Items, int TotalCount);