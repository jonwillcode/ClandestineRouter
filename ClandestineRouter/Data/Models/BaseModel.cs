namespace ClandestineRouter.Data.Models;

public abstract class BaseModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedDateTimeUtc { get; set; }

    public DateTime UpdatedDateTimeUtc { get; set; }
}
