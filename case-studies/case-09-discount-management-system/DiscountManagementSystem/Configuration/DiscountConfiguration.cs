using DiscountManagementSystem.Discounts;
using DiscountManagementSystem.Enums;

namespace DiscountManagementSystem.Configuration
{
    public static class DiscountConfiguration
    {
        public static readonly List<DiscountRule> Rules = new()
        {
            new DiscountRule
            {
                Name = "First Time Customer Welcome",
                Priority = 1,
                Condition = ctx => ctx.IsFirstTimeCustomer,
                CreateDiscount = () => new FirstTimeCustomerDiscount(0.15m),
            },
            new DiscountRule
            {
                Name = "VIP Loyalty Program",
                Priority = 2,
                Condition = ctx => ctx.CustomerType == CustomerType.VIP,
                CreateDiscount = () => new LoyaltyDiscount(0.20m, CustomerType.VIP),
            },
            new DiscountRule
            {
                Name = "Premium Member Benefits",
                Priority = 3,
                Condition = ctx => ctx.CustomerType == CustomerType.Premium,
                CreateDiscount = () => new LoyaltyDiscount(0.10m, CustomerType.Premium),
            },
            new DiscountRule
            {
                Name = "Bulk Purchase Incentive",
                Priority = 4,
                Condition = ctx => ctx.PurchaseQuantity >= 10,
                CreateDiscount = () => new BulkPurchaseDiscount(10, 0.08m),
            },
            new DiscountRule
            {
                Name = "Holiday Season Sale",
                Priority = 5,
                Condition = ctx => ctx.PurchaseDate.Month == 12,
                CreateDiscount = () => new SeasonalDiscount(0.12m, "Holiday"),
            },
            new DiscountRule
            {
                Name = "Summer Sale",
                Priority = 6,
                Condition = ctx => ctx.PurchaseDate.Month >= 6 && ctx.PurchaseDate.Month <= 8,
                CreateDiscount = () => new SeasonalDiscount(0.05m, "Summer"),
            },
        };
    }
}
