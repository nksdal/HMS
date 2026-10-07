namespace HMS.Infrastructure.Email;

public class EmailSettings
{
    public string Sender { get; set; } = string.Empty;
    public string SmtpServer { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string AppBaseUrl { get; set; } = string.Empty;
}
