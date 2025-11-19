namespace CallAlert.Api.Entities;

public class CallEvent
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string CallerNumber { get; set; } = default!;
    public DateTime CalledAt { get; set; }
    public string? CallStatus { get; set; }
    public string? DeviceId { get; set; }
    public bool IsWatchedNumber { get; set; }
    public bool MqttPublished { get; set; }
    public DateTime? MqttPublishedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}


