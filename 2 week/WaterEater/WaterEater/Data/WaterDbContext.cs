using Microsoft.EntityFrameworkCore;
using WaterApp.Models;

namespace WaterApp.Data
{
    public class WaterDbContext : DbContext
    {
        public WaterDbContext(DbContextOptions<WaterDbContext> options) : base(options) { }

        public DbSet<Water> Waters { get; set; }
    }
}
 