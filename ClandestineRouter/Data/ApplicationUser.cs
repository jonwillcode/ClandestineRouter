using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ClandestineRouter.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(128)]
        public string LocalTimeZone { get; set; } = "America/New_York";
    }

}
