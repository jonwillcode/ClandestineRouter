using System.ComponentModel.DataAnnotations;

namespace ClandestineRouter.Data.Models;

public class InboundContent : CommonEntityBase
{
    public Guid PersonaId { get; set; }
    
    public Persona Persona { get; set; } = null!;

    [MaxLength(256)]
    public string? ContentUrl { get; set; }

    [MaxLength(2000)]
    public string? ExtractedText { get; set; }

    [Required]
    public bool IsProcessed { get; set; } = false;

    public string? Notes { get; set; }
}
