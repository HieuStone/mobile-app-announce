using System.Text;
using System.Text.Json;
using CallAlert.Api.Configuration;
using CallAlert.Api.Services.Abstractions;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace CallAlert.Api.Services.Implementations;

public class MqttPublisherService : IMqttPublisherService
{
    private readonly ILogger<MqttPublisherService> _logger;
    private readonly MqttOptions _options;

    public MqttPublisherService(IOptions<MqttOptions> options, ILogger<MqttPublisherService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task PublishIncomingCallAsync(object payload, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(payload);
        _logger.LogInformation("Publishing MQTT message to {Host}:{Port}, topic={Topic}", _options.Host, _options.Port, _options.Topic);

        return PublishAsync(json, cancellationToken);
    }

    private async Task PublishAsync(string payload, CancellationToken cancellationToken)
    {
        var factory = new MqttFactory();
        using var client = factory.CreateMqttClient();

        var clientOptionsBuilder = new MqttClientOptionsBuilder()
            .WithTcpServer(_options.Host, _options.Port);

        if (!string.IsNullOrWhiteSpace(_options.Username))
        {
            clientOptionsBuilder = clientOptionsBuilder.WithCredentials(_options.Username, _options.Password);
        }

        var mqttClientOptions = clientOptionsBuilder.Build();

        try
        {
            await client.ConnectAsync(mqttClientOptions, cancellationToken);

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(_options.Topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await client.PublishAsync(message, cancellationToken);
            _logger.LogInformation("MQTT message published successfully to topic: {Topic}", _options.Topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish MQTT message");
            throw;
        }
        finally
        {
            await client.DisconnectAsync(cancellationToken: cancellationToken);
        }
    }
}


