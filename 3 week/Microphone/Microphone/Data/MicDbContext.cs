using Microsoft.EntityFrameworkCore;
using MicApp.Models;

namespace MicApp.Data
{
    public class MicDbContext : DbContext
    {
        public MicDbContext(DbContextOptions<MicDbContext> options) : base(options) { }

        public DbSet<Mic> Mics { get; set; }
    }
}
 