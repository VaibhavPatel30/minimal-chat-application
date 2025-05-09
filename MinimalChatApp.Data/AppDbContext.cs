using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Entity.Models;

namespace MinimalChatApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
