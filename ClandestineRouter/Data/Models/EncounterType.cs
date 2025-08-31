using System.ComponentModel.DataAnnotations;

namespace ClandestineRouter.Data.Models;

public class EncounterType : LookupEntityBase
{
    [MaxLength(2000)]
    public string? Description { get; set; }
}
