using Microsoft.EntityFrameworkCore;
using VapeApp.Models;

namespace VapeApp.Data
{
    public class VapeDbContext : DbContext
    {
        public VapeDbContext(DbContextOptions<VapeDbContext> options) : base(options) { }

        public DbSet<Vape> Vapes { get; set; }
    }
}
 