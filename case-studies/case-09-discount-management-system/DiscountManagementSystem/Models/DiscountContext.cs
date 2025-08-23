using DiscountManagementSystem.Enums;

namespace DiscountManagementSystem.Models
{
    public class DiscountContext
    {
        public decimal OriginalPrice { get; set; }
        public CustomerType CustomerType { get; set; }
        public int PurchaseQuantity { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string? ProductCategory { get; set; }
        public bool IsFirstTimeCustomer { get; set; }
        public decimal CustomerLifetimeValue { get; set; }
    }
}
