using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MyEmailWebApi.Data
{
    public class UserContext(DbContextOptions<UserContext> options) : IdentityDbContext(options)
    {
    }
}
