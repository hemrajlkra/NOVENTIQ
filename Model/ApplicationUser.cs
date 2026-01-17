using Microsoft.AspNetCore.Identity;

namespace NOVENTIQ.Model
{
    public class ApplicationUser: IdentityUser
    {
        public required string Name {  get; set; }
    }
}
