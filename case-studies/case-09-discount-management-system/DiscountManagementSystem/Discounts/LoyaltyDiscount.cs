using DiscountManagementSystem.Enums;
using DiscountManagementSystem.Models;

namespace DiscountManagementSystem.Discounts
{
    public class LoyaltyDiscount : BaseDiscount
    {
        private readonly decimal _discountRate;
        private readonly CustomerType _eligibleCustomerType;

        public LoyaltyDiscount(decimal discountRate, CustomerType eligibleCustomerType)
        {
            _discountRate = discountRate;
            _eligibleCustomerType = eligibleCustomerType;
        }

        public override string Name => $"{_eligibleCustomerType} Loyalty Discount";

        public override bool IsApplicable(DiscountContext context)
        {
            return context.CustomerType == _eligibleCustomerType;
        }

        public override DiscountResult ApplyDiscount(DiscountContext context, decimal currentPrice)
        {
            if (!IsApplicable(context))
                return CreateResult(currentPrice, 0, Name + " (Not Eligible)");

            var discountAmount = currentPrice * _discountRate;
            return CreateResult(currentPrice, discountAmount, Name);
        }
    }
}
