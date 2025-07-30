namespace ClandestineRouter.Data.Models;
public interface IBaseModel
{
    Guid Id { get; set; }
    DateTime CreatedDateTimeUtc { get; set; }
    public DateTime UpdatedDateTimeUtc { get; set; }
}

public abstract class BaseModel : IBaseModel
{
    public Guid Id { get; set; }

    public DateTime CreatedDateTimeUtc { get; set; }

    public DateTime UpdatedDateTimeUtc { get; set; }
}
