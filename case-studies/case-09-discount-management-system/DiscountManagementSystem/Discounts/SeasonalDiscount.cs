using DiscountManagementSystem.Models;

namespace DiscountManagementSystem.Discounts
{
    public class SeasonalDiscount : BaseDiscount
    {
        private readonly decimal _discountRate;
        private readonly string _seasonName;

        public SeasonalDiscount(decimal discountRate, string seasonName)
        {
            _discountRate = discountRate;
            _seasonName = seasonName;
        }

        public override string Name => $"{_seasonName} Seasonal Discount";

        public override DiscountResult ApplyDiscount(DiscountContext context, decimal currentPrice)
        {
            var discountAmount = currentPrice * _discountRate;
            return CreateResult(currentPrice, discountAmount, Name);
        }
    }
}
