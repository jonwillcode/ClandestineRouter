using System.ComponentModel.DataAnnotations;

namespace ClandestineRouter.Data.Models;

public class SocialMediaApp : BaseModel
{

    [Required, MaxLength(256)]
    public string Name { get; set; } = string.Empty;
}
