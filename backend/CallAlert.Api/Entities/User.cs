namespace CallAlert.Api.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string PasswordHash { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<WatchNumber> WatchNumbers { get; set; } = new List<WatchNumber>();
    public ICollection<CallEvent> CallEvents { get; set; } = new List<CallEvent>();
}


