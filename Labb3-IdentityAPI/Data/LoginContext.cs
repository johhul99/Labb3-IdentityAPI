using Labb3_IdentityAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Labb3_IdentityAPI.Data
{
    public class LoginContext : DbContext
    {
        public LoginContext(DbContextOptions<LoginContext> options) : base(options) {}

        public DbSet<Login> Logins { get; set; }
    }
}
