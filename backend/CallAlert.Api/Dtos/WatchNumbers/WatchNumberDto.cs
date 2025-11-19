namespace CallAlert.Api.Dtos.WatchNumbers;

public record WatchNumberDto(
    int Id,
    string PhoneNumber,
    string? Label,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);


