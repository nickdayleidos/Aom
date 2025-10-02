namespace MyApplication.Components.Services.Email;

public sealed class OiEventContext
{
    public int EventId { get; init; }
    public string Summary { get; init; } = "";
    public string? ServicesAffected { get; init; }
    public string? TicketNumber { get; init; }
    public int CategoryId { get; init; }
    public int SeverityId { get; init; }
    public int SiteId { get; init; }
    public int? UsersAffected { get; init; } = 0;
    public DateTime PostedTime { get; init; }
    public DateTime StartTime { get; init; }
    public string? Description { get; init; }
    public string? EstimatedTimeToResolve { get; init; }
    public DateTime? ResolutionTime { get; init; }
}
