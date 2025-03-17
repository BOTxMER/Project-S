using Microsoft.EntityFrameworkCore;
using TeaApp.Models;

namespace TeaApp.Data
{
    public class TeaDbContext : DbContext
    {
        public TeaDbContext(DbContextOptions<TeaDbContext> options) : base(options) { }

        public DbSet<Tea> Teas { get; set; }
    }
}
