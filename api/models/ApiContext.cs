using Microsoft.EntityFrameworkCore;

namespace api.Models
{
  public class ApiContext : DbContext
  {
    public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Server> Servers { get; set; }
  }
}