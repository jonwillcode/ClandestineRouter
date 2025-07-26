using System.ComponentModel.DataAnnotations;

namespace ClandestineRouter.Data.Models;

public class EncounterType : BaseLookupModel
{
    [MaxLength(2000)]
    public string? Description { get; set; }
}
