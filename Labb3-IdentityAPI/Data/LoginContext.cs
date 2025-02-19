using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Labb3_IdentityAPI.Data
{
    public class LoginContext : IdentityDbContext<IdentityUser>
    {
        public LoginContext(DbContextOptions<LoginContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);  
        }
    }
}
