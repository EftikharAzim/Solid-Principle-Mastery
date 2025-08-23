using DiscountManagementSystem.Interfaces;
using DiscountManagementSystem.Models;

namespace DiscountManagementSystem.Configuration
{
    public class DiscountRule
    {
        public string? Name { get; set; }
        public Func<DiscountContext, bool>? Condition { get; set; }
        public Func<IDiscount>? CreateDiscount { get; set; }
        public int Priority { get; set; }
    }
}
