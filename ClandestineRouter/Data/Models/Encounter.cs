using System.ComponentModel.DataAnnotations;

namespace ClandestineRouter.Data.Models;

public class Encounter : BaseModel
{
    [Required]
    public required Guid SocialMediaAccountId { get; set; }

    public SocialMediaAccount SocialMediaAccount { get; set; } = null!;

    [Required]
    public required Guid EncounterTypeId { get; set; }

    public EncounterType EncounterType { get; set; } = null!;

    public IEnumerable<BehaviorType>? BeginBehaviorType { get; set; }

    public IEnumerable<BehaviorType>? EndBehaviorType { get; set; }

    public string? Notes { get; set; }
}
