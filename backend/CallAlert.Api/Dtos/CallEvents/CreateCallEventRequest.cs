namespace CallAlert.Api.Dtos.CallEvents;

public record CreateCallEventRequest(
    string CallerNumber,
    DateTime CalledAt,
    string? CallStatus,
    string? DeviceId);


