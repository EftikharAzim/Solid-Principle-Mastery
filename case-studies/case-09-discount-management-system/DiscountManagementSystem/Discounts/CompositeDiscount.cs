using DiscountManagementSystem.Interfaces;
using DiscountManagementSystem.Models;

namespace DiscountManagementSystem.Discounts
{
    public class CompositeDiscount : IDiscount
    {
        private readonly List<IDiscount> _discounts;

        public CompositeDiscount(IEnumerable<IDiscount> discounts)
        {
            _discounts = discounts.ToList();
        }

        public string Name => "Combined Discounts";

        public bool IsApplicable(DiscountContext context)
        {
            return _discounts.Any(d => d.IsApplicable(context));
        }

        public DiscountResult ApplyDiscount(DiscountContext context, decimal currentPrice)
        {
            var result = new DiscountResult
            {
                FinalPrice = currentPrice,
                DiscountAmount = 0,
                AppliedDiscounts = new List<string>(),
            };

            decimal workingPrice = currentPrice;

            foreach (var discount in _discounts.Where(d => d.IsApplicable(context)))
            {
                var discountResult = discount.ApplyDiscount(context, workingPrice);

                if (discountResult.DiscountAmount > 0)
                {
                    result.DiscountAmount += discountResult.DiscountAmount;
                    result.AppliedDiscounts.AddRange(discountResult.AppliedDiscounts);
                    workingPrice = discountResult.FinalPrice;
                }
            }

            result.FinalPrice = workingPrice;
            result.Summary = result.AppliedDiscounts.Any()
                ? $"Total discount: ${result.DiscountAmount:F2} | Final price: ${result.FinalPrice:F2}"
                : "No discounts applied";

            return result;
        }
    }
}
