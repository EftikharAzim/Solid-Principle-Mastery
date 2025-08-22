using System;
using System.Collections.Generic;
using System.Linq;

// ============================================================================
// 🏗️ COMPLETE ENTITY MODELS
// ============================================================================

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public CustomerType Type { get; set; }
    public DateTime JoinDate { get; set; }
    public List<int> PurchaseHistory { get; set; } = new List<int>();
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
}

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    public OrderStatus Status { get; set; }
    public decimal TotalAmount => Items.Sum(item => item.Quantity * item.UnitPrice);
    public bool IsFirstOrder { get; set; }
    public decimal AppliedDiscount { get; set; }
    public decimal FinalAmount => TotalAmount - AppliedDiscount;
    public List<int> ProductIds => Items.Select(i => i.ProductId).ToList();
}

public class OrderItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class Invoice
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal Amount { get; set; }
    public string InvoiceNumber { get; set; }
}

public class Shipment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string TrackingNumber { get; set; }
    public string ShippingAddress { get; set; }
    public ShipmentStatus Status { get; set; }
    public DateTime ShipDate { get; set; }
}

public class BundleDeal
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<int> ProductIds { get; set; } = new List<int>();
    public decimal DiscountPercentage { get; set; }
    public decimal MinimumSpend { get; set; }
}

// ============================================================================
// 📋 ENUMS
// ============================================================================

public enum CustomerType
{
    Regular,
    Premium,
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Processing,
    Shipped,
    Delivered,
    Cancelled,
}

public enum ShipmentStatus
{
    Preparing,
    InTransit,
    Delivered,
}

// ============================================================================
// 🔧 CORE SERVICE INTERFACES - Following ISP
// ============================================================================

// Inventory Management
public interface IInventoryService
{
    bool IsProductAvailable(int productId, int quantity);
    void ReserveProduct(int productId, int quantity);
    void ReleaseReservation(int productId, int quantity);
    Product GetProduct(int productId);
}

// Payment Processing - Following OCP & LSP
public interface IPaymentProcessor
{
    PaymentResult ProcessPayment(decimal amount, string paymentDetails);
}

// Notification Services - Following ISP
public interface IOrderConfirmationService
{
    void SendOrderConfirmation(Customer customer, Order order);
}

public interface IShippingNotificationService
{
    void SendShippingNotification(Customer customer, Order order, string trackingNumber);
}

public interface IDeliveryNotificationService
{
    void SendDeliveryNotification(Customer customer, Order order);
}

// Invoice & Shipping
public interface IInvoiceService
{
    Invoice GenerateInvoice(Order order);
}

public interface IShippingService
{
    Shipment CreateShipment(Order order, string shippingAddress);
    void UpdateShipmentStatus(int shipmentId, ShipmentStatus status);
}

// Loyalty & Premium Services
public interface ILoyaltyService
{
    void AwardPoints(Customer customer, decimal orderAmount);
    int GetCustomerPoints(Customer customer);
}

// Premium Customer Services - Following ISP
public interface IWelcomeGiftService
{
    void SendWelcomeGift(Customer customer);
}

public interface IPrioritySupport
{
    void AssignPrioritySupport(Customer customer, Order order);
}

public interface IExclusiveDiscountService
{
    decimal ApplyExclusiveDiscount(Customer customer, decimal orderAmount);
}

public interface IPremiumCustomerService
    : IWelcomeGiftService,
        IPrioritySupport,
        IExclusiveDiscountService { }

// Recommendation Engine - Following ISP
public interface IProductSuggestion
{
    List<Product> GetSuggestedProducts(Customer customer, int maxSuggestions = 5);
}

public interface IOffer
{
    List<BundleDeal> GetBundleDeals(Customer customer, List<int> currentCartProducts);
    decimal CalculateBundleDiscount(BundleDeal bundle);
}

public interface IRecommendation
{
    List<Product> GetSeasonalRecommendations(string season, Customer customer);
    List<Product> GetTrendingProducts(Customer customer);
}

public interface IProductRecommendationEngine : IProductSuggestion, IOffer, IRecommendation
{
    RecommendationSummary GetCompleteRecommendations(
        Customer customer,
        List<int> currentCartProducts
    );
}

// ============================================================================
// 📊 SUPPORTING DATA MODELS
// ============================================================================

public class PaymentResult
{
    public bool IsSuccessful { get; set; }
    public string TransactionId { get; set; }
    public string ErrorMessage { get; set; }
}

public class RecommendationSummary
{
    public List<Product> PersonalizedSuggestions { get; set; } = new List<Product>();
    public List<BundleDeal> AvailableBundles { get; set; } = new List<BundleDeal>();
    public List<Product> SeasonalItems { get; set; } = new List<Product>();
    public List<Product> TrendingItems { get; set; } = new List<Product>();
    public decimal PotentialSavings { get; set; }
}

// ============================================================================
// 💳 PAYMENT PROCESSORS - Following OCP & LSP
// ============================================================================

public class CreditCardProcessor : IPaymentProcessor
{
    public PaymentResult ProcessPayment(decimal amount, string paymentDetails)
    {
        Console.WriteLine($"💳 Processing credit card payment of ${amount:F2}");
        return new PaymentResult
        {
            IsSuccessful = true,
            TransactionId = "CC" + Guid.NewGuid().ToString()[..8],
        };
    }
}

public class PayPalProcessor : IPaymentProcessor
{
    public PaymentResult ProcessPayment(decimal amount, string paymentDetails)
    {
        Console.WriteLine($"🔵 Processing PayPal payment of ${amount:F2}");
        return new PaymentResult
        {
            IsSuccessful = true,
            TransactionId = "PP" + Guid.NewGuid().ToString()[..6],
        };
    }
}

public class BankTransferProcessor : IPaymentProcessor
{
    public PaymentResult ProcessPayment(decimal amount, string paymentDetails)
    {
        Console.WriteLine($"🏦 Processing bank transfer of ${amount:F2}");
        Console.WriteLine("⏳ Bank transfers take 2-3 business days...");
        return new PaymentResult
        {
            IsSuccessful = true,
            TransactionId = "BANK" + DateTime.Now.Ticks.ToString()[^6..],
        };
    }
}

// ============================================================================
// 🔔 NOTIFICATION SERVICES - Following SRP & ISP
// ============================================================================

public class EmailNotificationService
    : IOrderConfirmationService,
        IShippingNotificationService,
        IDeliveryNotificationService
{
    public void SendOrderConfirmation(Customer customer, Order order)
    {
        string badge = customer.Type == CustomerType.Premium ? "🌟 PREMIUM" : "👤 REGULAR";
        Console.WriteLine($"📧 Order confirmation sent to {customer.Email} {badge}");
        Console.WriteLine(
            $"📧 Order #{order.Id} details: {order.Items.Count} items, ${order.FinalAmount:F2}"
        );
    }

    public void SendShippingNotification(Customer customer, Order order, string trackingNumber)
    {
        Console.WriteLine($"📧 Shipping notification sent to {customer.Email}");
        Console.WriteLine($"📧 Order #{order.Id} shipped! Tracking: {trackingNumber}");
    }

    public void SendDeliveryNotification(Customer customer, Order order)
    {
        Console.WriteLine($"📧 Delivery confirmation sent to {customer.Email}");
        Console.WriteLine($"📧 Order #{order.Id} has been delivered!");
    }
}

public class SmsNotificationService : IShippingNotificationService
{
    public void SendShippingNotification(Customer customer, Order order, string trackingNumber)
    {
        Console.WriteLine($"📱 SMS to {customer.PhoneNumber}:");
        Console.WriteLine($"📱 Order #{order.Id} shipped! Track: {trackingNumber}");
    }
}

// ============================================================================
// 🏪 CORE BUSINESS SERVICES - Following SRP
// ============================================================================

public class InventoryService : IInventoryService
{
    private readonly Dictionary<int, int> _stock = new Dictionary<int, int>
    {
        { 1, 100 },
        { 2, 50 },
        { 3, 25 },
        { 4, 30 },
        { 5, 40 },
        { 6, 60 },
        { 7, 20 },
        { 8, 35 },
    };

    private readonly Dictionary<int, Product> _products = new Dictionary<int, Product>();

    public InventoryService()
    {
        InitializeProducts();
    }

    public bool IsProductAvailable(int productId, int quantity)
    {
        return _stock.ContainsKey(productId) && _stock[productId] >= quantity;
    }

    public void ReserveProduct(int productId, int quantity)
    {
        if (IsProductAvailable(productId, quantity))
        {
            _stock[productId] -= quantity;
            Console.WriteLine($"📦 Reserved {quantity} units of {GetProduct(productId).Name}");
        }
    }

    public void ReleaseReservation(int productId, int quantity)
    {
        _stock[productId] += quantity;
        Console.WriteLine($"🔄 Released {quantity} units of {GetProduct(productId).Name}");
    }

    public Product GetProduct(int productId)
    {
        return _products.GetValueOrDefault(productId);
    }

    private void InitializeProducts()
    {
        var products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Laptop Pro",
                Price = 1299.99m,
                Category = "Electronics",
                Tags = new List<string> { "tech", "work" },
            },
            new Product
            {
                Id = 2,
                Name = "Wireless Mouse",
                Price = 29.99m,
                Category = "Electronics",
                Tags = new List<string> { "tech", "accessory" },
            },
            new Product
            {
                Id = 3,
                Name = "Coffee Maker",
                Price = 89.99m,
                Category = "Appliances",
                Tags = new List<string> { "kitchen", "coffee" },
            },
            new Product
            {
                Id = 4,
                Name = "Winter Coat",
                Price = 149.99m,
                Category = "Clothing",
                Tags = new List<string> { "winter", "outerwear" },
            },
            new Product
            {
                Id = 5,
                Name = "Bluetooth Headphones",
                Price = 199.99m,
                Category = "Electronics",
                Tags = new List<string> { "tech", "audio" },
            },
            new Product
            {
                Id = 6,
                Name = "Yoga Mat",
                Price = 39.99m,
                Category = "Fitness",
                Tags = new List<string> { "exercise", "wellness" },
            },
            new Product
            {
                Id = 7,
                Name = "Smart Watch",
                Price = 299.99m,
                Category = "Electronics",
                Tags = new List<string> { "tech", "fitness" },
            },
            new Product
            {
                Id = 8,
                Name = "Protein Powder",
                Price = 49.99m,
                Category = "Fitness",
                Tags = new List<string> { "nutrition", "fitness" },
            },
        };

        foreach (var product in products)
        {
            _products[product.Id] = product;
        }
    }
}

public class InvoiceService : IInvoiceService
{
    public Invoice GenerateInvoice(Order order)
    {
        var invoice = new Invoice
        {
            Id = new Random().Next(1000, 9999),
            OrderId = order.Id,
            InvoiceDate = DateTime.Now,
            Amount = order.FinalAmount,
            InvoiceNumber = $"INV-{DateTime.Now.Year}-{order.Id:D6}",
        };

        Console.WriteLine(
            $"📄 Invoice generated: {invoice.InvoiceNumber} for ${invoice.Amount:F2}"
        );
        return invoice;
    }
}

public class ShippingService : IShippingService
{
    public Shipment CreateShipment(Order order, string shippingAddress)
    {
        var shipment = new Shipment
        {
            Id = new Random().Next(1000, 9999),
            OrderId = order.Id,
            TrackingNumber = "TRK" + Guid.NewGuid().ToString()[..8].ToUpper(),
            ShippingAddress = shippingAddress,
            Status = ShipmentStatus.Preparing,
            ShipDate = DateTime.Now.AddDays(1),
        };

        Console.WriteLine($"📦 Shipment created with tracking: {shipment.TrackingNumber}");
        return shipment;
    }

    public void UpdateShipmentStatus(int shipmentId, ShipmentStatus status)
    {
        Console.WriteLine($"📦 Shipment {shipmentId} status updated to: {status}");
    }
}

public class LoyaltyService : ILoyaltyService
{
    private readonly Dictionary<int, int> _customerPoints = new Dictionary<int, int>();

    public void AwardPoints(Customer customer, decimal orderAmount)
    {
        int multiplier = customer.Type == CustomerType.Premium ? 2 : 1;
        int pointsToAward = (int)(orderAmount / 10) * multiplier;

        _customerPoints[customer.Id] =
            _customerPoints.GetValueOrDefault(customer.Id, 0) + pointsToAward;

        Console.WriteLine($"🏆 {pointsToAward} loyalty points awarded to {customer.Name}");
        if (multiplier > 1)
            Console.WriteLine($"🌟 Premium 2x bonus applied!");
        Console.WriteLine($"🏆 Total points: {_customerPoints[customer.Id]}");
    }

    public int GetCustomerPoints(Customer customer)
    {
        return _customerPoints.GetValueOrDefault(customer.Id, 0);
    }
}

// ============================================================================
// 🌟 PREMIUM CUSTOMER SERVICES - Following SRP & ISP
// ============================================================================

public class PremiumCustomerService : IPremiumCustomerService
{
    private readonly HashSet<int> _welcomeGiftsSent = new HashSet<int>();

    public void SendWelcomeGift(Customer customer)
    {
        if (_welcomeGiftsSent.Contains(customer.Id))
        {
            Console.WriteLine($"🎁 Welcome gift already sent to {customer.Name}");
            return;
        }

        Console.WriteLine($"🎁 Premium welcome gift sent to {customer.Name}!");
        Console.WriteLine($"🎁 Includes: Premium mug, $20 voucher, member card");
        _welcomeGiftsSent.Add(customer.Id);
    }

    public void AssignPrioritySupport(Customer customer, Order order)
    {
        Console.WriteLine($"🏆 Priority support assigned to {customer.Name}");
        Console.WriteLine($"🏆 Dedicated agent & 24/7 hotline activated");
    }

    public decimal ApplyExclusiveDiscount(Customer customer, decimal orderAmount)
    {
        decimal discount = orderAmount * 0.15m; // 15% exclusive discount
        Console.WriteLine($"💎 Premium exclusive discount: -${discount:F2} (15%)");
        return discount;
    }
}

// ============================================================================
// 🤖 RECOMMENDATION ENGINE - Following SRP & ISP
// ============================================================================

public class IntelligentRecommendationEngine : IProductRecommendationEngine
{
    private readonly IInventoryService _inventoryService;
    private readonly List<BundleDeal> _bundleDeals;
    private readonly Dictionary<string, List<int>> _seasonalProducts;
    private readonly List<int> _trendingProductIds = new List<int> { 1, 3, 7, 8 };

    public IntelligentRecommendationEngine(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
        _bundleDeals = InitializeBundleDeals();
        _seasonalProducts = InitializeSeasonalData();
    }

    public List<Product> GetSuggestedProducts(Customer customer, int maxSuggestions = 5)
    {
        Console.WriteLine($"🤖 AI analyzing {customer.Name}'s purchase history...");

        var suggestions = new List<Product>();
        var allProducts = GetAllProducts();

        // Based on purchase history
        var purchasedCategories = customer
            .PurchaseHistory.Select(id => _inventoryService.GetProduct(id)?.Category)
            .Where(c => c != null)
            .Distinct()
            .ToList();

        foreach (var category in purchasedCategories)
        {
            var categoryProducts = allProducts
                .Where(p => p.Category == category && !customer.PurchaseHistory.Contains(p.Id))
                .Take(2)
                .ToList();
            suggestions.AddRange(categoryProducts);
        }

        Console.WriteLine($"💡 {suggestions.Count} personalized suggestions found");
        return suggestions.Take(maxSuggestions).ToList();
    }

    public List<BundleDeal> GetBundleDeals(Customer customer, List<int> currentCartProducts)
    {
        Console.WriteLine($"🎁 Checking bundle deals...");

        var applicableDeals = _bundleDeals
            .Where(deal => deal.ProductIds.Intersect(currentCartProducts).Any())
            .ToList();

        if (customer.Type == CustomerType.Premium)
        {
            var premiumDeals = _bundleDeals.Where(d => d.DiscountPercentage > 0.15m).ToList();
            applicableDeals.AddRange(premiumDeals);
        }

        return applicableDeals.Distinct().ToList();
    }

    public decimal CalculateBundleDiscount(BundleDeal bundle)
    {
        var bundleProducts = bundle
            .ProductIds.Select(id => _inventoryService.GetProduct(id))
            .Where(p => p != null);
        var totalValue = bundleProducts.Sum(p => p.Price);
        return totalValue * bundle.DiscountPercentage;
    }

    public List<Product> GetSeasonalRecommendations(string season, Customer customer)
    {
        if (!_seasonalProducts.ContainsKey(season.ToLower()))
            return new List<Product>();

        var seasonalIds = _seasonalProducts[season.ToLower()];
        return seasonalIds
            .Select(id => _inventoryService.GetProduct(id))
            .Where(p => p != null)
            .ToList();
    }

    public List<Product> GetTrendingProducts(Customer customer)
    {
        return _trendingProductIds
            .Select(id => _inventoryService.GetProduct(id))
            .Where(p => p != null)
            .ToList();
    }

    public RecommendationSummary GetCompleteRecommendations(
        Customer customer,
        List<int> currentCartProducts
    )
    {
        Console.WriteLine($"\n🎯 Complete recommendations for {customer.Name}");

        var summary = new RecommendationSummary
        {
            PersonalizedSuggestions = GetSuggestedProducts(customer, 3),
            AvailableBundles = GetBundleDeals(customer, currentCartProducts),
            SeasonalItems = GetSeasonalRecommendations("winter", customer),
            TrendingItems = GetTrendingProducts(customer),
        };

        summary.PotentialSavings = summary.AvailableBundles.Sum(CalculateBundleDiscount);
        return summary;
    }

    private List<Product> GetAllProducts()
    {
        var products = new List<Product>();
        for (int i = 1; i <= 8; i++)
        {
            var product = _inventoryService.GetProduct(i);
            if (product != null)
                products.Add(product);
        }
        return products;
    }

    private List<BundleDeal> InitializeBundleDeals()
    {
        return new List<BundleDeal>
        {
            new BundleDeal
            {
                Id = 1,
                Name = "Tech Bundle",
                ProductIds = new List<int> { 1, 2, 5 },
                DiscountPercentage = 0.10m,
            },
            new BundleDeal
            {
                Id = 2,
                Name = "Fitness Pack",
                ProductIds = new List<int> { 6, 7, 8 },
                DiscountPercentage = 0.15m,
            },
            new BundleDeal
            {
                Id = 3,
                Name = "Winter Comfort",
                ProductIds = new List<int> { 3, 4 },
                DiscountPercentage = 0.20m,
            },
        };
    }

    private Dictionary<string, List<int>> InitializeSeasonalData()
    {
        return new Dictionary<string, List<int>>
        {
            ["winter"] = new List<int> { 3, 4 },
            ["summer"] = new List<int> { 6, 8 },
        };
    }
}

// ============================================================================
// 🎯 MASTER ORDER PROCESSOR - All Features Integrated
// ============================================================================

public class MasterOrderProcessor
{
    private readonly IInventoryService _inventoryService;
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IOrderConfirmationService _orderConfirmationService;
    private readonly IShippingNotificationService _shippingNotificationService;
    private readonly IDeliveryNotificationService _deliveryNotificationService;
    private readonly IInvoiceService _invoiceService;
    private readonly IShippingService _shippingService;
    private readonly ILoyaltyService _loyaltyService;
    private readonly IPremiumCustomerService _premiumService;
    private readonly IProductRecommendationEngine _recommendationEngine;

    // DIP: All dependencies are interfaces (abstractions)
    public MasterOrderProcessor(
        IInventoryService inventoryService,
        IPaymentProcessor paymentProcessor,
        IOrderConfirmationService orderConfirmationService,
        IShippingNotificationService shippingNotificationService,
        IDeliveryNotificationService deliveryNotificationService,
        IInvoiceService invoiceService,
        IShippingService shippingService,
        ILoyaltyService loyaltyService,
        IPremiumCustomerService premiumService,
        IProductRecommendationEngine recommendationEngine
    )
    {
        _inventoryService = inventoryService;
        _paymentProcessor = paymentProcessor;
        _orderConfirmationService = orderConfirmationService;
        _shippingNotificationService = shippingNotificationService;
        _deliveryNotificationService = deliveryNotificationService;
        _invoiceService = invoiceService;
        _shippingService = shippingService;
        _loyaltyService = loyaltyService;
        _premiumService = premiumService;
        _recommendationEngine = recommendationEngine;
    }

    public bool ProcessCompleteOrder(Order order, Customer customer)
    {
        Console.WriteLine($"\n🏪 MASTER ORDER PROCESSING");
        Console.WriteLine($"Customer: {customer.Name} ({customer.Type})");
        Console.WriteLine(
            $"Order: #{order.Id} | Items: {order.Items.Count} | Amount: ${order.TotalAmount:F2}"
        );
        Console.WriteLine("=" + new string('=', 80));

        try
        {
            // ============================================================================
            // PHASE 1: PRE-CHECKOUT RECOMMENDATIONS
            // ============================================================================
            Console.WriteLine("\n🎯 PHASE 1: INTELLIGENT RECOMMENDATIONS");
            var recommendations = _recommendationEngine.GetCompleteRecommendations(
                customer,
                order.ProductIds
            );
            DisplayRecommendations(recommendations);

            // Auto-add one suggestion for demo
            if (recommendations.PersonalizedSuggestions.Any())
            {
                var suggestion = recommendations.PersonalizedSuggestions.First();
                Console.WriteLine(
                    $"✅ Adding suggested: {suggestion.Name} (+${suggestion.Price:F2})"
                );
                order.Items.Add(
                    new OrderItem
                    {
                        ProductId = suggestion.Id,
                        ProductName = suggestion.Name,
                        Quantity = 1,
                        UnitPrice = suggestion.Price,
                    }
                );
            }

            // ============================================================================
            // PHASE 2: INVENTORY VALIDATION
            // ============================================================================
            Console.WriteLine("\n📦 PHASE 2: INVENTORY VALIDATION");
            foreach (var item in order.Items)
            {
                if (!_inventoryService.IsProductAvailable(item.ProductId, item.Quantity))
                {
                    Console.WriteLine($"❌ Insufficient stock for {item.ProductName}");
                    return false;
                }
            }

            foreach (var item in order.Items)
            {
                _inventoryService.ReserveProduct(item.ProductId, item.Quantity);
            }

            // ============================================================================
            // PHASE 3: PREMIUM CUSTOMER SERVICES
            // ============================================================================
            if (customer.Type == CustomerType.Premium)
            {
                Console.WriteLine("\n🌟 PHASE 3: PREMIUM CUSTOMER SERVICES");

                if (order.IsFirstOrder)
                {
                    _premiumService.SendWelcomeGift(customer);
                }

                var discount = _premiumService.ApplyExclusiveDiscount(customer, order.TotalAmount);
                order.AppliedDiscount += discount;

                _premiumService.AssignPrioritySupport(customer, order);
            }

            // Apply bundle discounts
            var applicableBundles = recommendations.AvailableBundles;
            if (applicableBundles.Any())
            {
                var bestBundle = applicableBundles.First();
                var bundleDiscount = _recommendationEngine.CalculateBundleDiscount(bestBundle);
                Console.WriteLine($"🎁 Bundle discount applied: -{bundleDiscount:C}");
                order.AppliedDiscount += bundleDiscount;
            }

            // ============================================================================
            // PHASE 4: PAYMENT PROCESSING
            // ============================================================================
            Console.WriteLine("\n💳 PHASE 4: PAYMENT PROCESSING");
            Console.WriteLine($"Order Total: ${order.TotalAmount:F2}");
            Console.WriteLine($"Discounts: -${order.AppliedDiscount:F2}");
            Console.WriteLine($"Final Amount: ${order.FinalAmount:F2}");

            var paymentResult = _paymentProcessor.ProcessPayment(
                order.FinalAmount,
                "payment-details"
            );
            if (!paymentResult.IsSuccessful)
            {
                // Release inventory on payment failure
                foreach (var item in order.Items)
                {
                    _inventoryService.ReleaseReservation(item.ProductId, item.Quantity);
                }
                Console.WriteLine($"❌ Payment failed: {paymentResult.ErrorMessage}");
                return false;
            }

            Console.WriteLine($"✅ Payment successful! Transaction: {paymentResult.TransactionId}");
            order.Status = OrderStatus.Confirmed;

            // ============================================================================
            // PHASE 5: POST-PAYMENT OPERATIONS
            // ============================================================================
            Console.WriteLine("\n📋 PHASE 5: POST-PAYMENT OPERATIONS");

            // Generate invoice
            var invoice = _invoiceService.GenerateInvoice(order);

            // Create shipment
            var shipment = _shippingService.CreateShipment(order, customer.Address);

            // Update customer purchase history
            customer.PurchaseHistory.AddRange(order.ProductIds);

            // Award loyalty points
            _loyaltyService.AwardPoints(customer, order.TotalAmount);

            // ============================================================================
            // PHASE 6: CUSTOMER COMMUNICATIONS
            // ============================================================================
            Console.WriteLine("\n📧 PHASE 6: CUSTOMER COMMUNICATIONS");

            _orderConfirmationService.SendOrderConfirmation(customer, order);
            _shippingNotificationService.SendShippingNotification(
                customer,
                order,
                shipment.TrackingNumber
            );

            // Simulate delivery (for demo)
            Console.WriteLine("\n📦 [SIMULATED] Order delivered...");
            _deliveryNotificationService.SendDeliveryNotification(customer, order);
            order.Status = OrderStatus.Delivered;

            Console.WriteLine($"\n🎉 ORDER #{order.Id} COMPLETED SUCCESSFULLY!");
            Console.WriteLine($"💰 Final charged amount: ${order.FinalAmount:F2}");
            Console.WriteLine(
                $"🏆 Customer loyalty points: {_loyaltyService.GetCustomerPoints(customer)}"
            );

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Critical error: {ex.Message}");
            return false;
        }
    }

    private void DisplayRecommendations(RecommendationSummary recommendations)
    {
        if (recommendations.PersonalizedSuggestions.Any())
        {
            Console.WriteLine("💡 AI Suggestions:");
            foreach (var product in recommendations.PersonalizedSuggestions)
            {
                Console.WriteLine($"   • {product.Name} - ${product.Price}");
            }
        }

        if (recommendations.AvailableBundles.Any())
        {
            Console.WriteLine("🎁 Bundle Offers:");
            foreach (var bundle in recommendations.AvailableBundles.Take(2))
            {
                var savings = _recommendationEngine.CalculateBundleDiscount(bundle);
                Console.WriteLine($"   • {bundle.Name} - Save ${savings:F2}");
            }
        }

        Console.WriteLine($"💰 Potential Total Savings: ${recommendations.PotentialSavings:F2}");
    }
}

// ============================================================================
// 🎮 COMPLETE DEMONSTRATION PROGRAM
// ============================================================================

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("🏪 COMPLETE SOLID E-COMMERCE SYSTEM");
        Console.WriteLine(
            "All features integrated: Inventory, Payments, Premium Services, AI Recommendations"
        );
        Console.WriteLine("=" + new string('=', 90));

        // ============================================================================
        // DEPENDENCY INJECTION SETUP - All services configured
        // ============================================================================
        Console.WriteLine("🔧 Initializing all services...\n");

        // Core services
        var inventoryService = new InventoryService();
        var invoiceService = new InvoiceService();
        var shippingService = new ShippingService();
        var loyaltyService = new LoyaltyService();

        // Payment processors (can easily swap)
        var paymentProcessor = new CreditCardProcessor(); // Could be PayPalProcessor or BankTransferProcessor

        // Notification services (following ISP - can mix and match)
        var emailService = new EmailNotificationService();
        var smsService = new SmsNotificationService();

        // Premium services
        var premiumService = new PremiumCustomerService();

        // AI Recommendation engine
        var recommendationEngine = new IntelligentRecommendationEngine(inventoryService);

        // Master order processor with ALL dependencies
        var orderProcessor = new MasterOrderProcessor(
            inventoryService,
            paymentProcessor, // Payment method
            emailService, // Order confirmations
            smsService, // Shipping notifications (SMS for quick alerts)
            emailService, // Delivery notifications (email for detailed info)
            invoiceService,
            shippingService,
            loyaltyService,
            premiumService,
            recommendationEngine
        );

        // ============================================================================
        // TEST SCENARIO 1: New Premium Customer - First Order
        // ============================================================================

        Console.WriteLine("📋 TEST SCENARIO 1: New Premium Customer (First Order)");
        Console.WriteLine("-" + new string('-', 70));

        var premiumCustomer = new Customer
        {
            Id = 1,
            Name = "Emma Wilson",
            Email = "emma.wilson@email.com",
            PhoneNumber = "+1-555-0199",
            Address = "123 Premium Ave, Tech City, TC 12345",
            Type = CustomerType.Premium,
            JoinDate = DateTime.Now,
            PurchaseHistory = new List<int>(), // New customer, no history
        };

        var premiumOrder = new Order
        {
            Id = 5001,
            CustomerId = premiumCustomer.Id,
            OrderDate = DateTime.Now,
            Status = OrderStatus.Pending,
            IsFirstOrder = true, // Triggers welcome gift
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 1,
                    ProductName = "Laptop Pro",
                    Quantity = 1,
                    UnitPrice = 1299.99m,
                },
                new OrderItem
                {
                    ProductId = 2,
                    ProductName = "Wireless Mouse",
                    Quantity = 1,
                    UnitPrice = 29.99m,
                },
            },
        };

        bool result1 = orderProcessor.ProcessCompleteOrder(premiumOrder, premiumCustomer);
        Console.WriteLine($"\n📊 SCENARIO 1 RESULT: {(result1 ? "✅ SUCCESS" : "❌ FAILED")}");

        // ============================================================================
        // TEST SCENARIO 2: Regular Customer with Purchase History
        // ============================================================================

        Console.WriteLine("\n\n📋 TEST SCENARIO 2: Regular Customer (With Purchase History)");
        Console.WriteLine("-" + new string('-', 70));

        var regularCustomer = new Customer
        {
            Id = 2,
            Name = "Mike Johnson",
            Email = "mike.johnson@email.com",
            PhoneNumber = "+1-555-0123",
            Address = "456 Regular St, Normal City, NC 54321",
            Type = CustomerType.Regular,
            JoinDate = DateTime.Now.AddMonths(-6),
            PurchaseHistory = new List<int> { 3, 6 }, // Previously bought Coffee Maker and Yoga Mat
        };

        var regularOrder = new Order
        {
            Id = 5002,
            CustomerId = regularCustomer.Id,
            OrderDate = DateTime.Now,
            Status = OrderStatus.Pending,
            IsFirstOrder = false,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 7,
                    ProductName = "Smart Watch",
                    Quantity = 1,
                    UnitPrice = 299.99m,
                },
                new OrderItem
                {
                    ProductId = 8,
                    ProductName = "Protein Powder",
                    Quantity = 2,
                    UnitPrice = 49.99m,
                },
            },
        };

        bool result2 = orderProcessor.ProcessCompleteOrder(regularOrder, regularCustomer);
        Console.WriteLine($"\n📊 SCENARIO 2 RESULT: {(result2 ? "✅ SUCCESS" : "❌ FAILED")}");

        // ============================================================================
        // TEST SCENARIO 3: Demonstrate Payment Method Flexibility (OCP)
        // ============================================================================

        Console.WriteLine("\n\n📋 TEST SCENARIO 3: Different Payment Method (PayPal)");
        Console.WriteLine("-" + new string('-', 70));

        // Create new order processor with PayPal instead of Credit Card
        var paypalProcessor = new PayPalProcessor();
        var orderProcessorWithPayPal = new MasterOrderProcessor(
            inventoryService,
            paypalProcessor, // ✨ Different payment processor (OCP in action!)
            emailService,
            emailService, // Using email for both shipping and delivery
            emailService,
            invoiceService,
            shippingService,
            loyaltyService,
            premiumService,
            recommendationEngine
        );

        var paypalOrder = new Order
        {
            Id = 5003,
            CustomerId = regularCustomer.Id,
            OrderDate = DateTime.Now,
            Status = OrderStatus.Pending,
            IsFirstOrder = false,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 4,
                    ProductName = "Winter Coat",
                    Quantity = 1,
                    UnitPrice = 149.99m,
                },
            },
        };

        bool result3 = orderProcessorWithPayPal.ProcessCompleteOrder(paypalOrder, regularCustomer);
        Console.WriteLine($"\n📊 SCENARIO 3 RESULT: {(result3 ? "✅ SUCCESS" : "❌ FAILED")}");

        // ============================================================================
        // FINAL STATISTICS & SOLID PRINCIPLES SUMMARY
        // ============================================================================

        Console.WriteLine("\n" + new string('=', 90));
        Console.WriteLine("📈 FINAL SYSTEM STATISTICS");
        Console.WriteLine("=" + new string('=', 90));

        Console.WriteLine($"🎯 Orders Processed: 3");
        Console.WriteLine(
            $"🏆 Emma's Loyalty Points: {loyaltyService.GetCustomerPoints(premiumCustomer)}"
        );
        Console.WriteLine(
            $"🏆 Mike's Loyalty Points: {loyaltyService.GetCustomerPoints(regularCustomer)}"
        );
        Console.WriteLine($"💳 Payment Methods Used: Credit Card, PayPal");
        Console.WriteLine($"📧 Notifications Sent: Email + SMS hybrid approach");
        Console.WriteLine(
            $"🎁 Premium Services Activated: Welcome gifts, exclusive discounts, priority support"
        );
        Console.WriteLine(
            $"🤖 AI Recommendations: Personalized suggestions, bundle deals, seasonal items"
        );

        Console.WriteLine("\n✨ SOLID PRINCIPLES DEMONSTRATED:");
        Console.WriteLine("=" + new string('=', 90));

        Console.WriteLine("✅ SRP (Single Responsibility Principle):");
        Console.WriteLine(
            "   • Each service has ONE clear purpose (InventoryService, PaymentProcessor, etc.)"
        );
        Console.WriteLine("   • Each class has only one reason to change");

        Console.WriteLine("\n✅ OCP (Open/Closed Principle):");
        Console.WriteLine(
            "   • Added new features (Premium services, Recommendations) without modifying existing code"
        );
        Console.WriteLine(
            "   • Easy to add new payment methods (CreditCard → PayPal → BankTransfer)"
        );
        Console.WriteLine("   • Can add new notification channels without breaking existing ones");

        Console.WriteLine("\n✅ LSP (Liskov Substitution Principle):");
        Console.WriteLine(
            "   • Any IPaymentProcessor can replace another (CreditCard ↔ PayPal ↔ BankTransfer)"
        );
        Console.WriteLine("   • Any notification service can be substituted seamlessly");

        Console.WriteLine("\n✅ ISP (Interface Segregation Principle):");
        Console.WriteLine(
            "   • Small, focused interfaces (IOrderConfirmation, IShippingNotification, IDeliveryNotification)"
        );
        Console.WriteLine("   • SMS service only implements what makes sense for SMS");
        Console.WriteLine(
            "   • Premium services split into logical interfaces (IWelcomeGift, IPrioritySupport, etc.)"
        );

        Console.WriteLine("\n✅ DIP (Dependency Inversion Principle):");
        Console.WriteLine("   • MasterOrderProcessor depends on interfaces, not concrete classes");
        Console.WriteLine("   • Easy to inject different implementations via constructor");
        Console.WriteLine("   • System is testable with mock objects");

        Console.WriteLine("\n🎓 ARCHITECTURAL BENEFITS ACHIEVED:");
        Console.WriteLine("=" + new string('=', 90));
        Console.WriteLine("🔧 Maintainability: Each component can be updated independently");
        Console.WriteLine("🧪 Testability: Easy to mock dependencies for unit testing");
        Console.WriteLine(
            "🚀 Extensibility: New features can be added without breaking existing code"
        );
        Console.WriteLine("🔄 Flexibility: Components can be mixed and matched as needed");
        Console.WriteLine("📈 Scalability: System can grow to handle more complex requirements");

        Console.WriteLine("\n🎯 REAL-WORLD PRODUCTION BENEFITS:");
        Console.WriteLine("=" + new string('=', 90));
        Console.WriteLine(
            "👥 Team Development: Different teams can work on different services independently"
        );
        Console.WriteLine("🐛 Bug Isolation: Issues in one service don't affect others");
        Console.WriteLine(
            "⚡ Performance: Can optimize individual services without affecting the whole system"
        );
        Console.WriteLine(
            "🔐 Security: Can apply different security measures to different services"
        );
        Console.WriteLine(
            "☁️ Deployment: Services can be deployed independently (microservices ready)"
        );

        Console.WriteLine("\n🏆 CONGRATULATIONS! You've mastered SOLID principles!");
        Console.WriteLine(
            "This complete system demonstrates professional-grade software architecture."
        );
        Console.WriteLine("You're now ready to design complex, maintainable software systems! 🌟");
    }
}
