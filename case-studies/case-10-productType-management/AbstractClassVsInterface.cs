// ================================
// PROBLEMATIC DESIGN: Abstract Class with Abstract Properties
// ================================

// ❌ BAD: Abstract class forces inheritance hierarchy
public abstract class Product
{
    public abstract int Id { get; set; }
    public abstract string Name { get; set; }
    public abstract decimal Price { get; set; }
    public abstract string Description { get; set; }
    public abstract string Category { get; set; }

    // Missing return type - compile error!
    public abstract string GetProductInfo();  // Should be: public abstract string GetProductInfo();
}

// Problem 1: Forces rigid inheritance
public class PhysicalProduct : Product
{
    // Forced to implement ALL abstract properties, even if not needed this way
    public override int Id { get; set; }
    public override string Name { get; set; }
    public override decimal Price { get; set; }
    public override string Description { get; set; }
    public override string Category { get; set; }

    public override string GetProductInfo()
    {
        return $"{Name} - ${Price}";
    }
}

// Problem 2: What if we want different data storage strategies?
public class DatabaseProduct : Product
{
    private readonly IProductRepository _repository;
    private readonly int _productId;

    public DatabaseProduct(IProductRepository repository, int productId)
    {
        _repository = repository;
        _productId = productId;
    }

    // ❌ FORCED to implement properties, even though we get data from database!
    public override int Id
    {
        get => _productId;
        set => throw new NotSupportedException("ID cannot be changed!"); // LSP violation!
    }

    public override string Name
    {
        get => _repository.GetName(_productId);
        set => _repository.UpdateName(_productId, value); // What if repository is read-only?
    }

    public override decimal Price
    {
        get => _repository.GetPrice(_productId);
        set => throw new NotSupportedException("Price managed by pricing service!"); // LSP violation!
    }

    public override string Description
    {
        get => _repository.GetDescription(_productId);
        set => _repository.UpdateDescription(_productId, value);
    }

    public override string Category
    {
        get => _repository.GetCategory(_productId);
        set => throw new NotSupportedException("Category is fixed!"); // LSP violation!
    }

    public override string GetProductInfo()
    {
        return $"{Name} - ${Price} ({Category})";
    }
}

// Problem 3: Can't implement multiple product types!
// This won't work - can't inherit from both!
/*
public class HybridProduct : Product, SomeOtherAbstractClass  // ❌ Compilation error!
{
    // Can't do this - C# doesn't support multiple inheritance
}
*/

// Problem 4: Testing becomes difficult
public class ProductService
{
    public string FormatProduct(Product product)  // Depends on abstract class!
    {
        // Hard to mock - must create concrete implementation
        return product.GetProductInfo();
    }
}

// ================================
// BETTER DESIGN: Interface-Based Approach
// ================================

// ✅ GOOD: Interface defines contract
public interface IProduct
{
    int GetId();
    string GetName();
    decimal GetPrice();
    string GetDescription();
    string GetCategory();
    string GetProductInfo();
}

// ✅ GOOD: Optional base class for common behavior
public abstract class BaseProduct : IProduct
{
    protected readonly int _id;
    protected readonly string _name;
    protected readonly decimal _price;
    protected readonly string _description;
    protected readonly string _category;

    protected BaseProduct(int id, string name, decimal price, string description, string category)
    {
        _id = id;
        _name = name;
        _price = price;
        _description = description;
        _category = category;
    }

    public virtual int GetId() => _id;
    public virtual string GetName() => _name;
    public virtual decimal GetPrice() => _price;
    public virtual string GetDescription() => _description;
    public virtual string GetCategory() => _category;
    public virtual string GetProductInfo() => $"{_name} - ${_price:F2} ({_category})";
}

// ✅ GOOD: Can choose to inherit base class or implement interface directly
public class SimpleProduct : BaseProduct
{
    public SimpleProduct(int id, string name, decimal price, string description, string category)
        : base(id, name, price, description, category)
    {
        // Gets all the common behavior from base class
    }
}

// ✅ GOOD: Can implement interface directly for different strategies
public class DatabaseProductBetter : IProduct
{
    private readonly IProductRepository _repository;
    private readonly int _productId;

    public DatabaseProductBetter(IProductRepository repository, int productId)
    {
        _repository = repository;
        _productId = productId;
    }

    // Clean implementation - no forced property setters!
    public int GetId() => _productId;
    public string GetName() => _repository.GetName(_productId);
    public decimal GetPrice() => _repository.GetPrice(_productId);
    public string GetDescription() => _repository.GetDescription(_productId);
    public string GetCategory() => _repository.GetCategory(_productId);

    public string GetProductInfo()
    {
        return $"{GetName()} - ${GetPrice():F2} ({GetCategory()})";
    }
}

// ✅ GOOD: Can implement multiple interfaces!
public class HybridProductBetter : IProduct, IShippable, IDownloadable
{
    private readonly IProduct _productInfo;
    private readonly IShippingService _shippingService;
    private readonly IDownloadService _downloadService;

    public HybridProductBetter(IProduct productInfo, IShippingService shippingService, IDownloadService downloadService)
    {
        _productInfo = productInfo;
        _shippingService = shippingService;
        _downloadService = downloadService;
    }

    // IProduct implementation - delegates to composed object
    public int GetId() => _productInfo.GetId();
    public string GetName() => _productInfo.GetName();
    public decimal GetPrice() => _productInfo.GetPrice();
    public string GetDescription() => _productInfo.GetDescription();
    public string GetCategory() => _productInfo.GetCategory();
    public string GetProductInfo() => _productInfo.GetProductInfo();

    // IShippable implementation
    public ShippingResult Ship(string destinationAddress, string shippingMethod)
    {
        return _shippingService.Ship(GetId(), destinationAddress, shippingMethod);
    }

    public decimal CalculateShippingCost(string destination, decimal weight)
    {
        return _shippingService.CalculateShippingCost(destination, weight);
    }

    public List<string> GetAvailableShippingMethods()
    {
        return _shippingService.GetAvailableShippingMethods();
    }

    public DateTime GetEstimatedDeliveryDate(string destination)
    {
        return _shippingService.GetEstimatedDeliveryDate(destination);
    }

    // IDownloadable implementation  
    public DownloadResult StartDownload(string userEmail)
    {
        return _downloadService.StartDownload(GetId(), userEmail);
    }

    public DownloadResult PauseDownload(string downloadId)
    {
        return _downloadService.PauseDownload(downloadId);
    }

    public DownloadResult CancelDownload(string downloadId)
    {
        return _downloadService.CancelDownload(downloadId);
    }

    public bool ValidateLicense(string licenseKey)
    {
        return _downloadService.ValidateLicense(GetId(), licenseKey);
    }

    public long GetFileSize()
    {
        return _downloadService.GetFileSize(GetId());
    }

    public string GenerateSecureDownloadLink(string userEmail, TimeSpan expirationTime)
    {
        return _downloadService.GenerateSecureDownloadLink(GetId(), userEmail, expirationTime);
    }
}

// ✅ GOOD: Easy to test with interfaces
public class ProductServiceBetter
{
    public string FormatProduct(IProduct product)  // Depends on interface!
    {
        // Easy to mock, test, and substitute implementations
        return product.GetProductInfo();
    }
}

// ================================
// DEMONSTRATION OF PROBLEMS AND SOLUTIONS
// ================================

public class DesignComparisonDemo
{
    public static void ShowProblems()
    {
        Console.WriteLine("=== ABSTRACT CLASS PROBLEMS ===\n");

        // Problem: Hard to test
        var productService = new ProductService();

        // Must create concrete implementation - can't easily mock
        var physicalProduct = new PhysicalProduct
        {
            Id = 1,
            Name = "Laptop",
            Price = 999.99m,
            Description = "High-end laptop",
            Category = "Electronics"
        };

        Console.WriteLine("❌ Abstract class approach:");
        Console.WriteLine(productService.FormatProduct(physicalProduct));

        // Problem demonstrated with DatabaseProduct
        var mockRepo = new MockProductRepository();
        var dbProduct = new DatabaseProduct(mockRepo, 1);

        try
        {
            // This will throw - LSP violation!
            dbProduct.Id = 999;
        }
        catch (NotSupportedException ex)
        {
            Console.WriteLine($"❌ LSP Violation: {ex.Message}");
        }

        try
        {
            // This will throw - LSP violation!
            dbProduct.Price = 1999.99m;
        }
        catch (NotSupportedException ex)
        {
            Console.WriteLine($"❌ LSP Violation: {ex.Message}");
        }

        Console.WriteLine("\n✅ Interface approach benefits:");

        var betterService = new ProductServiceBetter();

        // Can use any implementation
        var simpleProduct = new SimpleProduct(1, "Laptop", 999.99m, "High-end laptop", "Electronics");
        var dbProductBetter = new DatabaseProductBetter(mockRepo, 1);

        Console.WriteLine(betterService.FormatProduct(simpleProduct));
        Console.WriteLine(betterService.FormatProduct(dbProductBetter));

        // Can easily create test doubles
        var mockProduct = new MockProduct();
        Console.WriteLine(betterService.FormatProduct(mockProduct));

        // Can compose multiple behaviors
        var hybridProduct = new HybridProductBetter(
            simpleProduct,
            new MockShippingService(),
            new MockDownloadService()
        );

        Console.WriteLine($"Hybrid product: {hybridProduct.GetProductInfo()}");
        Console.WriteLine($"Can ship: {hybridProduct is IShippable}");
        Console.WriteLine($"Can download: {hybridProduct is IDownloadable}");
    }
}

// Supporting mock classes for demonstration
public class MockProduct : IProduct
{
    public int GetId() => 999;
    public string GetName() => "Mock Product";
    public decimal GetPrice() => 1.00m;
    public string GetDescription() => "Test product";
    public string GetCategory() => "Test";
    public string GetProductInfo() => "Mock Product - $1.00 (Test)";
}

public interface IProductRepository
{
    string GetName(int id);
    decimal GetPrice(int id);
    string GetDescription(int id);
    string GetCategory(int id);
    void UpdateName(int id, string name);
    void UpdateDescription(int id, string description);
}

public class MockProductRepository : IProductRepository
{
    public string GetName(int id) => "Database Product";
    public decimal GetPrice(int id) => 299.99m;
    public string GetDescription(int id) => "Product from database";
    public string GetCategory(int id) => "Database";
    public void UpdateName(int id, string name) { /* Update logic */ }
    public void UpdateDescription(int id, string description) { /* Update logic */ }
}

// Mock services for hybrid product
public interface IShippingService
{
    ShippingResult Ship(int productId, string destinationAddress, string shippingMethod);
    decimal CalculateShippingCost(string destination, decimal weight);
    List<string> GetAvailableShippingMethods();
    DateTime GetEstimatedDeliveryDate(string destination);
}

public interface IDownloadService
{
    DownloadResult StartDownload(int productId, string userEmail);
    DownloadResult PauseDownload(string downloadId);
    DownloadResult CancelDownload(string downloadId);
    bool ValidateLicense(int productId, string licenseKey);
    long GetFileSize(int productId);
    string GenerateSecureDownloadLink(int productId, string userEmail, TimeSpan expirationTime);
}

public class MockShippingService : IShippingService
{
    public ShippingResult Ship(int productId, string destinationAddress, string shippingMethod)
    {
        return new ShippingResult { IsSuccess = true, Message = "Shipped via mock service", TrackingNumber = "MOCK123" };
    }

    public decimal CalculateShippingCost(string destination, decimal weight) => 10.00m;
    public List<string> GetAvailableShippingMethods() => new() { "Mock Standard", "Mock Express" };
    public DateTime GetEstimatedDeliveryDate(string destination) => DateTime.Now.AddDays(3);
}

public class MockDownloadService : IDownloadService
{
    public DownloadResult StartDownload(int productId, string userEmail)
    {
        return new DownloadResult { IsSuccess = true, DownloadId = "MOCK-DL-123", Message = "Download started" };
    }

    public DownloadResult PauseDownload(string downloadId)
    {
        return new DownloadResult { IsSuccess = true, Message = "Download paused" };
    }

    public DownloadResult CancelDownload(string downloadId)
    {
        return new DownloadResult { IsSuccess = true, Message = "Download cancelled" };
    }

    public bool ValidateLicense(int productId, string licenseKey) => true;
    public long GetFileSize(int productId) => 1024 * 1024 * 100; // 100MB

    public string GenerateSecureDownloadLink(int productId, string userEmail, TimeSpan expirationTime)
    {
        return $"https://mock-download.com/{productId}?expires={DateTime.Now.Add(expirationTime).Ticks}";
    }
}

/*
================================
SUMMARY: WHY ABSTRACT CLASSES WITH ABSTRACT PROPERTIES ARE PROBLEMATIC
================================

1. INHERITANCE LIMITATIONS:
   - Forces single inheritance hierarchy
   - Can't compose multiple abstract classes
   - Rigid structure that's hard to change

2. LSP VIOLATIONS:
   - Implementations may throw exceptions for setters they don't support
   - Different implementations handle properties inconsistently
   - Breaks substitutability

3. TESTING DIFFICULTIES:
   - Hard to create mocks/test doubles
   - Must create concrete implementations for testing
   - Dependencies on abstract classes are harder to isolate

4. INFLEXIBLE DATA STORAGE:
   - Forces specific storage patterns (properties with getters/setters)
   - Doesn't work well with different data access strategies
   - Can't easily adapt to external systems or APIs

5. VIOLATION OF ISP:
   - All implementations forced to implement ALL abstract members
   - May implement properties they don't need or can't support properly
   - Creates unnecessary coupling

BETTER APPROACH:
- Use interfaces to define contracts
- Provide optional base classes for common implementations
- Allow flexibility in how implementations satisfy the contract
- Enable composition over inheritance
- Make testing easier with interface-based design
*/