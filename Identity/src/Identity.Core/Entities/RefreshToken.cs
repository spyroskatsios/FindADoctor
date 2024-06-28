namespace Identity.Core.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    
    public string JwtId { get; set; } = default!;

    public string Value { get; set; } = default!;

    public DateTime CreationDateTime { get; set; }

    public DateTime ExpirationDateTime { get; set; }

    public bool Invalidated { get; set; } 
    public bool Used { get; set; } 
    public string UserId { get; set; } = default!;
}