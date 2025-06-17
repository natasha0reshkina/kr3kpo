using System.ComponentModel.DataAnnotations;
namespace BillingService.Persistence.Entities
{
    public class Account
    {
        [Key]
        public Guid UserId { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}