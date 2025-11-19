namespace CallAlert.Api.Dtos.CallEvents;

public record CallEventDto(
    int Id,
    string CallerNumber,
    DateTime CalledAt,
    string? CallStatus,
    string? DeviceId,
    bool IsWatchedNumber,
    bool MqttPublished,
    DateTime? MqttPublishedAt,
    DateTime CreatedAt);


