#pragma warning disable CS8618
namespace CtServer.Data.Models;

public class Subscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Endpoint { get; set; }
    public string P256dh { get; set; }
    public string Auth { get; set; }

    public User User { get; set; }
}
