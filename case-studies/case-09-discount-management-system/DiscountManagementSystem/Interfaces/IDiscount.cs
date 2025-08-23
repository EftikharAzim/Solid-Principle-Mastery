using DiscountManagementSystem.Models;

namespace DiscountManagementSystem.Interfaces
{
    public interface IDiscount
    {
        string Name { get; }
        DiscountResult ApplyDiscount(DiscountContext context, decimal currentPrice);
        bool IsApplicable(DiscountContext context);
    }
}
