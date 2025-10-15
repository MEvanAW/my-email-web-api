using Microsoft.EntityFrameworkCore;
using MyEmailWebApi.Models;

namespace MyEmailWebApi.Data
{
    public class EmailContext(DbContextOptions<EmailContext> options) : DbContext(options)
    {
        public DbSet<EmailHistory> EmailHistories { get; set; }
    }
}
