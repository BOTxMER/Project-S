using Microsoft.EntityFrameworkCore;
using GameApp.Models;

namespace GameApp.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

        public DbSet<Game> Games { get; set; }
    }
}
 