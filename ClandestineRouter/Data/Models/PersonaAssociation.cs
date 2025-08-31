using System.ComponentModel.DataAnnotations;

namespace ClandestineRouter.Data.Models;

public class PersonaAssociation : CommonEntityBase
{
    [Required]
    public Guid BasePersonaId { get; set; }

    public Persona BasePersona { get; set; } = null!;

    [Required]
    public Guid AssociatePersonaId { get; set; }

    public Persona AssociatePersona { get; set; } = null!;
}
