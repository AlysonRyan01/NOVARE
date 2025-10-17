namespace SharedService.Shared.Settings;

public class RabbitMqSettings
{
    public string ConnectionString { get; set; } = null!;
    public string User { get; set; } = null!;
    public string Password { get; set; } = null!;
}