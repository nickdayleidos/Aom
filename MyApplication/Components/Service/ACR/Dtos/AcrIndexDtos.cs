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


// Filters (nullable = All)
public sealed record AcrIndexFilter(
int? AcrTypeId,
int? AcrStatusId,
string? EmployeeSearch, // id or "first last"
DateOnly? EffectiveFrom,
DateOnly? EffectiveTo
);