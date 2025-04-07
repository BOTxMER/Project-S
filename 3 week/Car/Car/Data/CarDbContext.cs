using Microsoft.EntityFrameworkCore;
using CarApp.Models;

namespace CarApp.Data
{
    public class CarDbContext : DbContext
    {
        public CarDbContext(DbContextOptions<CarDbContext> options) : base(options) { }

        public DbSet<Car> Cars { get; set; }
    }
}
 