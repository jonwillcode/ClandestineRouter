using System.ComponentModel.DataAnnotations;

namespace ClandestineRouter.Data.Models;

public class EncounterType : BaseModel
{
    [Required, MaxLength(256)]
    public required string Name { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }
}
