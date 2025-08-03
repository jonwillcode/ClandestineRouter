namespace ClandestineRouter.Data.Models;
public interface IBaseModel
{
    Guid Id { get; set; }
    DateTime CreatedDateTimeUtc { get; set; }
    public DateTime UpdatedDateTimeUtc { get; set; }
}

public abstract class BaseModel : IBaseModel
{
    protected BaseModel()
    {
        Id = Guid.NewGuid();
        CreatedDateTimeUtc = DateTime.UtcNow;
        UpdatedDateTimeUtc = DateTime.UtcNow;
    }
    protected BaseModel(Guid existingGuid)
    {
        Id = existingGuid;
        UpdatedDateTimeUtc = DateTime.UtcNow;
    }
    public Guid Id { get; set; }

    public DateTime CreatedDateTimeUtc { get; set; }

    public DateTime UpdatedDateTimeUtc { get; set; }
}