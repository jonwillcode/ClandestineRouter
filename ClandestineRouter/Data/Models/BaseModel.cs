namespace ClandestineRouter.Data.Models;

/// <summary>
/// Base interface for entities with Guid IDs
/// </summary>
public interface IBaseModel
{
    Guid Id { get; set; }
    DateTime CreatedDateTimeUtc { get; set; }
    DateTime UpdatedDateTimeUtc { get; set; }
}

public interface IEntity : IBaseModel { }

/// <summary>
/// Interface for entities with user audit tracking
/// </summary>
public interface IAuditableEntity : IEntity
{
    Guid? CreatedById { get; set; }
    Guid? ModifiedById { get; set; }
}

/// <summary>
/// Interface for entities that support soft delete
/// </summary>
public interface ISoftDeletableEntity : IEntity
{
    bool IsActive { get; set; }
}

/// <summary>
/// Combined interface for full-featured entities
/// </summary>
public interface ICommonEntity : IAuditableEntity, ISoftDeletableEntity { }

/// <summary>
/// Interface for lookup entities that have a name and support common entity features.
/// </summary>
public interface ILookupEntity : ICommonEntity
{
    string Name { get; set; }
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

/// <summary>
/// Enhanced base model for entities that need data services
/// </summary>
public abstract class EntityBase : BaseModel, IEntity { }

public abstract class AuditableEntity : BaseModel, IAuditableEntity
{
    public Guid? CreatedById { get; set; }
    public Guid? ModifiedById { get; set ; }
}

public abstract class SoftDeletableEntity : BaseModel, ISoftDeletableEntity
{
    public bool IsActive { get; set; } = true;
}

public abstract class CommonEntityBase : BaseModel, ICommonEntity
{
    public Guid? CreatedById { get; set; }
    public Guid? ModifiedById { get; set; }
    public bool IsActive { get; set; } = true;
}

public abstract class LookupEntityBase : BaseModel, ILookupEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid? CreatedById { get; set; }
    public Guid? ModifiedById { get; set; }
    public bool IsActive { get; set; } = true;
}