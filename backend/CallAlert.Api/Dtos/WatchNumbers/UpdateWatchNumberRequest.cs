namespace CallAlert.Api.Dtos.WatchNumbers;

public record UpdateWatchNumberRequest(
    string? Label,
    bool? IsActive);


