using DiscountManagementSystem.Enums;
using DiscountManagementSystem.Models;
using DiscountManagementSystem.Services;

namespace DiscountManagementSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🛒 E-Commerce Discount System - SOLID OCP Demo\n");

            var calculator = new DiscountCalculatorService();

            // Test Case 1: VIP customer with bulk purchase in December
            Console.WriteLine(string.Join("", Enumerable.Repeat('=', 60)));
            Console.WriteLine("Test Case 1: VIP Customer - Holiday Season - Bulk Purchase");
            Console.WriteLine(string.Join("", Enumerable.Repeat('=', 60)));

            var context1 = new DiscountContext
            {
                OriginalPrice = 1000m,
                CustomerType = CustomerType.VIP,
                PurchaseQuantity = 15,
                PurchaseDate = new DateTime(2024, 12, 15),
                IsFirstTimeCustomer = false,
            };

            var result1 = calculator.CalculateDiscount(context1);

            // Test Case 2: First-time regular customer
            Console.WriteLine('\n' + string.Join("", Enumerable.Repeat('=', 60)));
            Console.WriteLine("Test Case 2: First-Time Regular Customer");
            Console.WriteLine(string.Join("", Enumerable.Repeat('=', 60)));

            var context2 = new DiscountContext
            {
                OriginalPrice = 250m,
                CustomerType = CustomerType.Regular,
                PurchaseQuantity = 3,
                PurchaseDate = new DateTime(2024, 7, 10),
                IsFirstTimeCustomer = true,
            };

            var result2 = calculator.CalculateDiscount(context2);

            // Test Case 3: Premium customer with small summer purchase
            Console.WriteLine("\n" + string.Join("", Enumerable.Repeat('=', 60)));
            Console.WriteLine("Test Case 3: Premium Customer - Summer Sale");
            Console.WriteLine(string.Join("", Enumerable.Repeat('=', 60)));

            var context3 = new DiscountContext
            {
                OriginalPrice = 150m,
                CustomerType = CustomerType.Premium,
                PurchaseQuantity = 2,
                PurchaseDate = new DateTime(2024, 7, 20),
                IsFirstTimeCustomer = false,
            };

            var result3 = calculator.CalculateDiscount(context3);

            Console.WriteLine("\n" + string.Join("", Enumerable.Repeat('=', 60)));
            Console.WriteLine(
                "🎉 Demo completed! New discount types can be added without modifying existing code."
            );
            Console.WriteLine(string.Join("", Enumerable.Repeat('=', 60)));
        }
    }
}
