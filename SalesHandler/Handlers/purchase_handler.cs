using Microsoft.EntityFrameworkCore;
using SalesService.Persistence.Entities;
using SalesService.Persistence;
using System.Text.Json;
namespace SalesService.Handlers
{
    public class OrderService
    {
        private readonly OrdersDbContext _dbContext;
        public OrderService(OrdersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Purchase> CreateOrderAsync(Guid userId, decimal amount, string description)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            var purchase = new Purchase
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = amount,
                Description = description,
                Status = "NEW",
                CreatedAt = DateTime.UtcNow
            };
            await _dbContext.Purchases.AddAsync(purchase);
            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                AggregateId = purchase.Id,
                Type = "OrderPaymentRequested",
                Payload = JsonSerializer.Serialize(new
                {
                    OrderId = purchase.Id,
                    UserId = purchase.UserId,
                    Amount = purchase.Amount
                }),
                CreatedAt = DateTime.UtcNow
            };
            await _dbContext.OutboxMessages.AddAsync(outboxMessage);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return purchase;
        }
        public async Task<List<Purchase>> GetOrdersAsync(Guid userId)
        {
            return await _dbContext.Purchases
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
        public async Task<Purchase?> GetOrderByIdAsync(Guid orderId)
        {
            return await _dbContext.Purchases.FindAsync(orderId);
        }
        public async Task MarkOrderAsPaidAsync(Guid orderId)
        {
            var purchase = await _dbContext.Purchases.FindAsync(orderId);
            if (purchase != null && purchase.Status == "NEW") 
            {
                purchase.Status = "FINISHED";
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task MarkOrderAsCancelledAsync(Guid orderId)
        {
            var purchase = await _dbContext.Purchases.FindAsync(orderId);
            if (purchase != null && purchase.Status == "NEW")
            {
                purchase.Status = "CANCELLED";
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}