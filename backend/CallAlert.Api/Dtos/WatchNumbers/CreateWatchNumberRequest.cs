namespace CallAlert.Api.Dtos.WatchNumbers;

public record CreateWatchNumberRequest(
    string PhoneNumber,
    string? Label);


