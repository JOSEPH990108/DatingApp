
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppUserRole : IdentityUserRole<int>
    {
        public ApplicationUser User { get; set; }
        public AppRole Role { get; set; }
    }
}