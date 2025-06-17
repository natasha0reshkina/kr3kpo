using Microsoft.EntityFrameworkCore;
using SalesService.Persistence.Entities;
namespace SalesService.Persistence
{
    public class OrdersDbContext : DbContext
    {
        public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}