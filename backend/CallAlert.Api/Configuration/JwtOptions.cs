namespace CallAlert.Api.Configuration;

public class JwtOptions
{
    public string Issuer { get; set; } = "CallAlert";
    public string Audience { get; set; } = "CallAlertClients";
    public string Key { get; set; } = "CHANGE_ME_SUPER_SECRET_KEY";
    public int ExpiryMinutes { get; set; } = 120;
}


