namespace DiscountManagementSystem.Models
{
    public class DiscountResult
    {
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
        public List<string> AppliedDiscounts { get; set; } = new();
        public string? Summary { get; set; }
    }
}
