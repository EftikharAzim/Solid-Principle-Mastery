using DiscountManagementSystem.Configuration;
using DiscountManagementSystem.Discounts;
using DiscountManagementSystem.Interfaces;
using DiscountManagementSystem.Models;

namespace DiscountManagementSystem.Services
{
    public class DiscountFactory
    {
        public static IDiscount? CreateDiscountChain(DiscountContext context)
        {
            var applicableDiscounts = new List<IDiscount>();

            // Apply all matching rules
            foreach (var rule in DiscountConfiguration.Rules.OrderBy(r => r.Priority))
            {
                if (rule.Condition?.Invoke(context) == true)
                {
                    var discount = rule.CreateDiscount?.Invoke();
                    if (discount != null)
                    {
                        applicableDiscounts.Add(discount);
                        Console.WriteLine($"✓ Rule matched: {rule.Name}");
                    }
                }
            }

            return applicableDiscounts.Count > 0
                ? new CompositeDiscount(applicableDiscounts)
                : null;
        }
    }
}
