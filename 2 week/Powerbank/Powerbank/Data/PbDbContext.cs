using Microsoft.EntityFrameworkCore;
using PbApp.Models;

namespace PbApp.Data
{
    public class PbDbContext : DbContext
    {
        public PbDbContext(DbContextOptions<PbDbContext> options) : base(options) { }

        public DbSet<Powerbank> Powerbanks { get; set; }
    }
}
 