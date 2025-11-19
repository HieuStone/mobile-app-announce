namespace CallAlert.Api.Services.Abstractions;

public interface IMqttPublisherService
{
    Task PublishIncomingCallAsync(object payload, CancellationToken cancellationToken = default);
}


