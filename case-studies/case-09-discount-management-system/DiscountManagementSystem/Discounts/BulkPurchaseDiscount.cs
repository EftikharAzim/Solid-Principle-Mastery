using DiscountManagementSystem.Models;

namespace DiscountManagementSystem.Discounts
{
    public class BulkPurchaseDiscount : BaseDiscount
    {
        private readonly int _minimumQuantity;
        private readonly decimal _discountRate;

        public BulkPurchaseDiscount(int minimumQuantity, decimal discountRate)
        {
            _minimumQuantity = minimumQuantity;
            _discountRate = discountRate;
        }

        public override string Name => $"Bulk Purchase Discount ({_minimumQuantity}+ items)";

        public override bool IsApplicable(DiscountContext context)
        {
            return context.PurchaseQuantity >= _minimumQuantity;
        }

        public override DiscountResult ApplyDiscount(DiscountContext context, decimal currentPrice)
        {
            if (!IsApplicable(context))
                return CreateResult(currentPrice, 0, Name + " (Minimum quantity not met)");

            var discountAmount = currentPrice * _discountRate;
            return CreateResult(currentPrice, discountAmount, Name);
        }
    }
}
