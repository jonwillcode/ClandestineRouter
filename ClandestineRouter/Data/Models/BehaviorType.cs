using System.ComponentModel.DataAnnotations;

namespace ClandestineRouter.Data.Models;

public class BehaviorType : BaseLookupModel
{
    public IEnumerable<Encounter>? EncountersBegin { get; set; }

    public IEnumerable<Encounter>? EncountersEnd { get; set; }
}
