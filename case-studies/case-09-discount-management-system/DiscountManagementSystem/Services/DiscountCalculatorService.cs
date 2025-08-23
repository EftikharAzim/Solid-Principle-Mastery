using DiscountManagementSystem.Models;

namespace DiscountManagementSystem.Services
{
    public class DiscountCalculatorService
    {
        public DiscountResult CalculateDiscount(DiscountContext context)
        {
            Console.WriteLine("\n🔍 Analyzing discount eligibility...");
            Console.WriteLine(
                $"Customer: {context.CustomerType} | Quantity: {context.PurchaseQuantity} | Amount: ${context.OriginalPrice}"
            );

            var discountChain = DiscountFactory.CreateDiscountChain(context);

            if (discountChain == null)
            {
                Console.WriteLine("❌ No applicable discounts found");
                return new DiscountResult
                {
                    FinalPrice = context.OriginalPrice,
                    DiscountAmount = 0,
                    Summary = "No discounts applied",
                };
            }

            var result = discountChain.ApplyDiscount(context, context.OriginalPrice);

            Console.WriteLine("\n💰 Discount Calculation Results:");
            Console.WriteLine($"Original Price: ${context.OriginalPrice:F2}");
            Console.WriteLine($"Total Discount: ${result.DiscountAmount:F2}");
            Console.WriteLine($"Final Price: ${result.FinalPrice:F2}");
            Console.WriteLine($"Applied Discounts: {string.Join(", ", result.AppliedDiscounts)}");

            return result;
        }
    }
}
