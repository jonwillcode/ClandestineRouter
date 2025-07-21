using System.ComponentModel.DataAnnotations;

namespace ClandestineRouter.Data.Models;

public class BehaviorType : BaseModel
{
    [Required, MaxLength(256)]
    public required string Name { get; set; }

    public IEnumerable<Encounter>? EncountersBegin { get; set; }

    public IEnumerable<Encounter>? EncountersEnd { get; set; }
}
