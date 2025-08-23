using System.ComponentModel.DataAnnotations;

namespace ClandestineRouter.Data.DTOs;

public class BehaviorTypeDto
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
}
