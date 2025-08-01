using System.ComponentModel.DataAnnotations;

namespace ClandestineRouter.Data.Models;

public interface IBaseLookupModel : IBaseModel
{
    bool IsActive { get; set; }

    string Name { get; set; } 
}

public abstract class BaseLookupModel : BaseModel, IBaseLookupModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}