
// ================================
// PROBLEMATIC DESIGN: Properties in Interfaces
// ================================

// ❌ BAD: Interface with properties
public interface IShippable
{
    string ShippingAddress { get; set; }    // Problem: Forces state management
    string PreferredMethod { get; set; }    // Problem: Assumes single method
    DateTime ShippingDate { get; set; }     // Problem: When is this set?

    void Ship();  // Problem: Where does it get shipping info from?
}

// ❌ BAD: This creates multiple problems
public class PhysicalProduct : IShippable
{
    // Problem 1: Must store state it might not need
    public string ShippingAddress { get; set; }
    public string PreferredMethod { get; set; }
    public DateTime ShippingDate { get; set; }

    public void Ship()
    {
        // Problem 2: Method depends on properties being set correctly
        if (string.IsNullOrEmpty(ShippingAddress))
            throw new InvalidOperationException("Address not set!"); // LSP Violation!

        // Problem 3: Side effects - method changes object state
        ShippingDate = DateTime.Now;

        Console.WriteLine($"Shipping to: {ShippingAddress}");
    }
}

// ❌ WORSE: What about products that determine shipping address dynamically?
public class DropShipProduct : IShippable
{
    public string ShippingAddress { get; set; }  // This makes no sense!
    public string PreferredMethod { get; set; }
    public DateTime ShippingDate { get; set; }

    public void Ship()
    {
        // This product gets shipping address from supplier dynamically
        // But we're forced to have a ShippingAddress property!

        var supplierAddress = GetSupplierAddress(); // Real address comes from here

        // LSP VIOLATION: We ignore the interface property!
        Console.WriteLine($"Drop-shipping from supplier: {supplierAddress}");

        // What should we do with ShippingAddress property? 
        // Leave it null? Set it to supplier address? This violates expectations!
    }

    private string GetSupplierAddress() => "Supplier Warehouse, NY";
}

// ================================
// BETTER DESIGN: Methods with Parameters  
// ================================

// ✅ GOOD: Interface with method parameters
public interface IShippableBetter
{
    ShippingResult Ship(string destinationAddress, string shippingMethod);
    decimal CalculateShippingCost(string destination, decimal weight);
    List<string> GetAvailableShippingMethods();
}

// ✅ GOOD: Clean implementation
public class PhysicalProductBetter : IShippableBetter
{
    private readonly decimal _weight;

    public PhysicalProductBetter(decimal weight)
    {
        _weight = weight;
    }

    public ShippingResult Ship(string destinationAddress, string shippingMethod)
    {
        // Clear contract: method tells us exactly what it needs
        if (string.IsNullOrEmpty(destinationAddress))
            return new ShippingResult { IsSuccess = false, Message = "Address required" };

        if (string.IsNullOrEmpty(shippingMethod))
            return new ShippingResult { IsSuccess = false, Message = "Method required" };

        // No hidden state dependencies!
        return new ShippingResult
        {
            IsSuccess = true,
            TrackingNumber = $"TRK{DateTime.Now.Ticks}",
            Message = "Shipped successfully"
        };
    }

    public decimal CalculateShippingCost(string destination, decimal weight) => 10.0m + weight * 0.5m;

    public List<string> GetAvailableShippingMethods() => new() { "Standard", "Express" };
}

// ✅ GOOD: Drop-ship implementation - no LSP violations!
public class DropShipProductBetter : IShippableBetter
{
    public ShippingResult Ship(string destinationAddress, string shippingMethod)
    {
        // Clean implementation - uses the provided parameters
        var supplierAddress = GetSupplierAddress();

        // Forward the shipment request to supplier
        Console.WriteLine($"Requesting drop-ship from {supplierAddress} to {destinationAddress}");

        return new ShippingResult
        {
            IsSuccess = true,
            TrackingNumber = $"DS{DateTime.Now.Ticks}",
            Message = "Drop-ship initiated"
        };
    }

    public decimal CalculateShippingCost(string destination, decimal weight)
    {
        // Different calculation for drop-ship - but same contract!
        return 15.0m; // Flat rate from supplier
    }

    public List<string> GetAvailableShippingMethods() => new() { "Supplier Standard" };

    private string GetSupplierAddress() => "Supplier Warehouse, NY";
}

// ================================
// LSP VIOLATION DEMONSTRATION
// ================================

public class LSPViolationDemo
{
    public static void DemonstrateProblems()
    {
        Console.WriteLine("=== PROBLEMS WITH PROPERTIES IN INTERFACES ===\n");

        // Problem with property-based interface
        IShippable[] productsWithProperties =
        {
            new PhysicalProduct(),
            new DropShipProduct()
        };

        Console.WriteLine("❌ Property-based interface problems:");
        foreach (var product in productsWithProperties)
        {
            try
            {
                // Problem: We must set properties first!
                product.ShippingAddress = "123 Main St";
                product.PreferredMethod = "Standard";

                product.Ship();

                // LSP Violation Check: Are all implementations behaving the same?
                Console.WriteLine($"Address after shipping: {product.ShippingAddress}");
                Console.WriteLine($"Shipping date: {product.ShippingDate}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            Console.WriteLine();
        }

        Console.WriteLine("\n✅ Method-based interface benefits:");

        IShippableBetter[] productsWithMethods =
        {
            new PhysicalProductBetter(2.5m),
            new DropShipProductBetter()
        };

        foreach (var product in productsWithMethods)
        {
            // Clean contract: method signature tells us exactly what's needed
            var result = product.Ship("123 Main St", "Standard");
            Console.WriteLine($"Result: {result.Message}");
            Console.WriteLine($"Tracking: {result.TrackingNumber}");

            var cost = product.CalculateShippingCost("123 Main St", 2.5m);
            Console.WriteLine($"Cost: ${cost:F2}");
            Console.WriteLine();
        }
    }
}

// ================================
// WHY PROPERTIES VIOLATE LSP
// ================================

/*
LSP VIOLATIONS WITH PROPERTIES:

1. INCONSISTENT STATE HANDLING:
   - PhysicalProduct.Ship() sets ShippingDate
   - DropShipProduct.Ship() might not set it
   - Caller can't rely on consistent behavior

2. PRECONDITION STRENGTHENING:
   - PhysicalProduct requires ShippingAddress to be set before Ship()
   - This adds preconditions not defined in interface
   - Violates "derived classes should be substitutable"

3. HIDDEN DEPENDENCIES:
   - Properties create temporal coupling (set this before calling that)
   - Method parameters make dependencies explicit
   - Interface contract becomes unclear

4. SIDE EFFECTS:
   - Properties can be modified by methods
   - Clients can't predict object state after method calls
   - Breaks substitutability expectations

SUMMARY: Properties in interfaces force implementations to manage state
in ways that may not be natural for their specific use case, leading to
LSP violations through inconsistent behavior and hidden dependencies.
*/