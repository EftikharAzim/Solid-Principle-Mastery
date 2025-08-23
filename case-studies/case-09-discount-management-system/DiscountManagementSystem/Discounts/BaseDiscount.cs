using DiscountManagementSystem.Interfaces;
using DiscountManagementSystem.Models;

namespace DiscountManagementSystem.Discounts
{
    public abstract class BaseDiscount : IDiscount
    {
        public abstract string Name { get; }

        public virtual bool IsApplicable(DiscountContext context)
        {
            return true; // Override in derived classes for specific conditions
        }

        public abstract DiscountResult ApplyDiscount(DiscountContext context, decimal currentPrice);

        protected DiscountResult CreateResult(
            decimal originalPrice,
            decimal discountAmount,
            string discountName
        )
        {
            return new DiscountResult
            {
                DiscountAmount = discountAmount,
                FinalPrice = originalPrice - discountAmount,
                AppliedDiscounts = new List<string> { discountName },
                Summary = $"{discountName}: ${discountAmount:F2} discount applied",
            };
        }
    }
}
