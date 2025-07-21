using System.ComponentModel.DataAnnotations;

namespace ClandestineRouter.Data.Models;

public class Persona : BaseModel
{
    [MaxLength(256)]
    public string? Name { get; set; }

    public string? Notes { get; set; }
}
