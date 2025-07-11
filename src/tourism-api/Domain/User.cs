using System.Net;

namespace tourism_api.Domain;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
    }
}
