namespace RadiusSearch.Api.Models;

public sealed record ErrorResponse(
    string Code,
    string Reason,
    string Message,
    string Status,
    string Timestamp);
