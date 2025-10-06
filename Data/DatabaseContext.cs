using API_Doodles_2._0.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Doodles_2._0.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {}

    public DbSet<Users> Users { get; set; }
}