using DiscountManagementSystem.Models;

namespace DiscountManagementSystem.Discounts
{
    public class FirstTimeCustomerDiscount : BaseDiscount
    {
        private readonly decimal _discountRate;

        public FirstTimeCustomerDiscount(decimal discountRate)
        {
            _discountRate = discountRate;
        }

        public override string Name => "First Time Customer Discount";

        public override bool IsApplicable(DiscountContext context)
        {
            return context.IsFirstTimeCustomer;
        }

        public override DiscountResult ApplyDiscount(DiscountContext context, decimal currentPrice)
        {
            if (!IsApplicable(context))
                return CreateResult(currentPrice, 0, Name + " (Not a first-time customer)");

            var discountAmount = currentPrice * _discountRate;
            return CreateResult(currentPrice, discountAmount, Name);
        }
    }
}
