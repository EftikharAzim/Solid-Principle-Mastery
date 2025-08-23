// ========================
// DOMAIN MODELS
// ========================
public class Order
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItem> Items { get; set; } = new();

    public decimal GetTotalAmount()
    {
        return Items.Sum(item => item.Price * item.Quantity);
    }

    // Business validation
    public bool IsValid(out string errorMessage)
    {
        if (UserId <= 0)
        {
            errorMessage = "Invalid UserId";
            return false;
        }

        if (!Items.Any())
        {
            errorMessage = "Order must have at least one item";
            return false;
        }

        if (GetTotalAmount() <= 0)
        {
            errorMessage = "Order total must be greater than zero";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}

public class OrderItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

// ========================
// RESULT MODELS (Advanced Error Handling)
// ========================
public class SaveResult
{
    public int TotalOrders { get; set; }
    public int SuccessfulOrders { get; set; }
    public int FailedOrders { get; set; }
    public List<OrderSaveError> Errors { get; set; } = new();

    public bool IsCompleteSuccess => FailedOrders == 0;
    public bool IsPartialSuccess => SuccessfulOrders > 0 && FailedOrders > 0;
    public bool IsCompleteFailure => SuccessfulOrders == 0;

    public static SaveResult Success(int totalOrders) =>
        new SaveResult
        {
            TotalOrders = totalOrders,
            SuccessfulOrders = totalOrders,
            FailedOrders = 0,
        };

    public static SaveResult CompleteFailure(int totalOrders, string generalError) =>
        new SaveResult
        {
            TotalOrders = totalOrders,
            SuccessfulOrders = 0,
            FailedOrders = totalOrders,
            Errors = new List<OrderSaveError>
            {
                new OrderSaveError
                {
                    OrderId = 0,
                    ErrorMessage = generalError,
                    ErrorCode = "SYSTEM_ERROR",
                },
            },
        };
}

public class OrderSaveError
{
    public int OrderId { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
}

// ========================
// REPOSITORY ABSTRACTION (Advanced)
// ========================
public interface IOrderRepository
{
    Task<SaveResult> SaveOrdersAsync(IEnumerable<Order> orders);
    Task<List<Order>> GetOrdersByIdAsync(IEnumerable<int> orderIds);
    Task<List<Order>> GetOrdersByUserIdsAsync(IEnumerable<int> userIds);
    Task DeleteOrderAsync(int orderId);

    // Additional useful methods
    Task<Order?> GetOrderByIdAsync(int orderId);
    Task<List<Order>> GetOrdersByUserIdAsync(int userId);
}

// ========================
// HIGH-LEVEL MODULE (Advanced Business Logic)
// ========================
public class OrderProcessor
{
    private readonly IOrderRepository _orderRepository;

    public OrderProcessor(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<SaveResult> ProcessOrderBatchAsync(IEnumerable<Order> orders)
    {
        Console.WriteLine($"🔄 Processing batch of {orders.Count()} orders...");

        try
        {
            // Pre-validate orders (business logic)
            var validatedOrders = new List<Order>();
            var validationErrors = new List<OrderSaveError>();

            foreach (var order in orders)
            {
                if (order.IsValid(out string errorMessage))
                {
                    validatedOrders.Add(order);
                }
                else
                {
                    validationErrors.Add(
                        new OrderSaveError
                        {
                            OrderId = order.OrderId,
                            ErrorMessage = errorMessage,
                            ErrorCode = "VALIDATION_ERROR",
                        }
                    );
                }
            }

            // If no valid orders, return early
            if (!validatedOrders.Any())
            {
                Console.WriteLine("❌ No valid orders to process!");
                return new SaveResult
                {
                    TotalOrders = orders.Count(),
                    SuccessfulOrders = 0,
                    FailedOrders = orders.Count(),
                    Errors = validationErrors,
                };
            }

            // Process valid orders
            Console.WriteLine($"✅ {validatedOrders.Count} orders passed validation");

            var result = await _orderRepository.SaveOrdersAsync(validatedOrders);

            // Add validation errors to the result
            result.Errors.AddRange(validationErrors);
            result.TotalOrders = orders.Count();
            result.FailedOrders += validationErrors.Count;

            Console.WriteLine(
                $"📊 Batch Result: {result.SuccessfulOrders} success, {result.FailedOrders} failed"
            );

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"💥 Critical error processing batch: {ex.Message}");
            return SaveResult.CompleteFailure(orders.Count(), $"System error: {ex.Message}");
        }
    }

    public async Task DisplayOrderSummaryAsync(IEnumerable<int> userIds)
    {
        Console.WriteLine($"📋 Retrieving orders for {userIds.Count()} users...");

        try
        {
            var orders = await _orderRepository.GetOrdersByUserIdsAsync(userIds);

            var summary = orders
                .GroupBy(o => o.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    OrderCount = g.Count(),
                    TotalSpent = g.Sum(o => o.GetTotalAmount()),
                })
                .ToList();

            foreach (var userSummary in summary)
            {
                Console.WriteLine(
                    $"   User {userSummary.UserId}: {userSummary.OrderCount} orders, ${userSummary.TotalSpent:F2} total"
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error retrieving orders: {ex.Message}");
        }
    }
}

// ========================
// LOW-LEVEL MODULES (Advanced Implementations)
// ========================

// Advanced SQL Implementation with Batch Processing
public class SqlOrderRepository : IOrderRepository
{
    private readonly List<Order> _sqlDatabase = new List<Order>();

    public async Task<SaveResult> SaveOrdersAsync(IEnumerable<Order> orders)
    {
        Console.WriteLine($"💾 [SQL] Starting batch save of {orders.Count()} orders...");

        // Simulate async database operation
        await Task.Delay(100);

        var result = new SaveResult { TotalOrders = orders.Count() };

        try
        {
            // Simulate batch insert with some failures
            foreach (var order in orders)
            {
                // Simulate occasional database constraint failures
                if (order.OrderId % 13 == 0) // Simulate 1 in 13 orders failing
                {
                    result.Errors.Add(
                        new OrderSaveError
                        {
                            OrderId = order.OrderId,
                            ErrorMessage = "Database constraint violation",
                            ErrorCode = "DB_CONSTRAINT_ERROR",
                        }
                    );
                    result.FailedOrders++;
                }
                else
                {
                    _sqlDatabase.Add(order);
                    result.SuccessfulOrders++;
                }
            }

            Console.WriteLine(
                $"💾 [SQL] Batch save completed: {result.SuccessfulOrders} saved, {result.FailedOrders} failed"
            );
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"💥 [SQL] Critical database error: {ex.Message}");
            return SaveResult.CompleteFailure(orders.Count(), $"SQL Database error: {ex.Message}");
        }
    }

    public async Task<List<Order>> GetOrdersByIdAsync(IEnumerable<int> orderIds)
    {
        Console.WriteLine($"🔍 [SQL] Batch querying {orderIds.Count()} orders...");
        await Task.Delay(50);
        return _sqlDatabase.Where(o => orderIds.Contains(o.OrderId)).ToList();
    }

    public async Task<List<Order>> GetOrdersByUserIdsAsync(IEnumerable<int> userIds)
    {
        Console.WriteLine($"🔍 [SQL] Querying orders for {userIds.Count()} users...");
        await Task.Delay(75);
        return _sqlDatabase.Where(o => userIds.Contains(o.UserId)).ToList();
    }

    public async Task DeleteOrderAsync(int orderId)
    {
        Console.WriteLine($"🗑️ [SQL] Deleting order {orderId}...");
        await Task.Delay(25);
        var order = _sqlDatabase.FirstOrDefault(o => o.OrderId == orderId);
        if (order != null)
            _sqlDatabase.Remove(order);
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        await Task.Delay(10);
        return _sqlDatabase.FirstOrDefault(o => o.OrderId == orderId);
    }

    public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
    {
        await Task.Delay(25);
        return _sqlDatabase.Where(o => o.UserId == userId).ToList();
    }
}

// Advanced NoSQL Implementation
public class NoSqlOrderRepository : IOrderRepository
{
    private readonly Dictionary<int, Order> _noSqlDatabase = new Dictionary<int, Order>();

    public async Task<SaveResult> SaveOrdersAsync(IEnumerable<Order> orders)
    {
        Console.WriteLine($"📄 [NoSQL] Starting batch save of {orders.Count()} orders...");
        await Task.Delay(80);

        var result = new SaveResult { TotalOrders = orders.Count() };

        try
        {
            // NoSQL typically has better batch performance
            foreach (var order in orders)
            {
                _noSqlDatabase[order.OrderId] = order;
                result.SuccessfulOrders++;
            }

            Console.WriteLine(
                $"📄 [NoSQL] Batch save completed successfully: {result.SuccessfulOrders} saved"
            );
            return result;
        }
        catch (Exception ex)
        {
            return SaveResult.CompleteFailure(orders.Count(), $"NoSQL error: {ex.Message}");
        }
    }

    public async Task<List<Order>> GetOrdersByIdAsync(IEnumerable<int> orderIds)
    {
        Console.WriteLine($"🔍 [NoSQL] Batch querying {orderIds.Count()} orders...");
        await Task.Delay(30);
        return orderIds
            .Select(id => _noSqlDatabase.TryGetValue(id, out var order) ? order : null)
            .Where(o => o != null)
            .ToList()!;
    }

    public async Task<List<Order>> GetOrdersByUserIdsAsync(IEnumerable<int> userIds)
    {
        Console.WriteLine($"🔍 [NoSQL] Querying orders for {userIds.Count()} users...");
        await Task.Delay(40);
        return _noSqlDatabase.Values.Where(o => userIds.Contains(o.UserId)).ToList();
    }

    public async Task DeleteOrderAsync(int orderId)
    {
        Console.WriteLine($"🗑️ [NoSQL] Deleting order {orderId}...");
        await Task.Delay(15);
        _noSqlDatabase.Remove(orderId);
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        await Task.Delay(5);
        return _noSqlDatabase.TryGetValue(orderId, out var order) ? order : null;
    }

    public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
    {
        await Task.Delay(20);
        return _noSqlDatabase.Values.Where(o => o.UserId == userId).ToList();
    }
}

// ========================
// MOCK REPOSITORY FOR TESTING
// ========================
public class MockOrderRepository : IOrderRepository
{
    private readonly List<Order> _mockData = new();
    public bool ShouldSimulateFailure { get; set; } = false;

    public async Task<SaveResult> SaveOrdersAsync(IEnumerable<Order> orders)
    {
        await Task.Delay(1); // Simulate async without real delay

        if (ShouldSimulateFailure)
        {
            return SaveResult.CompleteFailure(orders.Count(), "Mock failure for testing");
        }

        _mockData.AddRange(orders);
        return SaveResult.Success(orders.Count());
    }

    public async Task<List<Order>> GetOrdersByIdAsync(IEnumerable<int> orderIds)
    {
        await Task.Delay(1);
        return _mockData.Where(o => orderIds.Contains(o.OrderId)).ToList();
    }

    public async Task<List<Order>> GetOrdersByUserIdsAsync(IEnumerable<int> userIds)
    {
        await Task.Delay(1);
        return _mockData.Where(o => userIds.Contains(o.UserId)).ToList();
    }

    public async Task DeleteOrderAsync(int orderId)
    {
        await Task.Delay(1);
        var order = _mockData.FirstOrDefault(o => o.OrderId == orderId);
        if (order != null)
            _mockData.Remove(order);
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        await Task.Delay(1);
        return _mockData.FirstOrDefault(o => o.OrderId == orderId);
    }

    public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
    {
        await Task.Delay(1);
        return _mockData.Where(o => o.UserId == userId).ToList();
    }
}

// ========================
// ADVANCED DEMONSTRATION
// ========================
public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("🎯 ADVANCED SOLID DIP - Production-Ready Order System");
        Console.WriteLine("=====================================================\n");

        // Create sample orders (with some invalid ones for testing)
        var orders = CreateSampleOrders();

        Console.WriteLine("📊 SCENARIO 1: Batch Processing with SQL");
        Console.WriteLine("=========================================");
        await DemonstrateRepository(new SqlOrderRepository(), orders);

        Console.WriteLine("\n" + new string('=', 60) + "\n");

        Console.WriteLine("📊 SCENARIO 2: Switching to NoSQL");
        Console.WriteLine("==================================");
        await DemonstrateRepository(new NoSqlOrderRepository(), orders);

        Console.WriteLine("\n" + new string('=', 60) + "\n");

        Console.WriteLine("🧪 SCENARIO 3: Unit Testing with Mock");
        Console.WriteLine("=====================================");
        await DemonstrateUnitTesting();

        Console.WriteLine("\n🎯 KEY INSIGHTS:");
        Console.WriteLine("================");
        Console.WriteLine("✅ Async/await for non-blocking operations");
        Console.WriteLine("✅ Batch processing for performance");
        Console.WriteLine("✅ Detailed error handling with partial failures");
        Console.WriteLine("✅ Same business logic works with any storage!");
        Console.WriteLine("✅ Easy unit testing with mock implementations");
        Console.WriteLine("✅ Production-ready error handling and validation");
    }

    private static async Task DemonstrateRepository(IOrderRepository repository, List<Order> orders)
    {
        var processor = new OrderProcessor(repository);

        // Process batch
        var result = await processor.ProcessOrderBatchAsync(orders);

        Console.WriteLine($"\n📋 Batch Result Summary:");
        Console.WriteLine($"   Total: {result.TotalOrders}");
        Console.WriteLine($"   Successful: {result.SuccessfulOrders}");
        Console.WriteLine($"   Failed: {result.FailedOrders}");

        if (result.Errors.Any())
        {
            Console.WriteLine($"   Errors:");
            foreach (var error in result.Errors.Take(5)) // Show first 5 errors
            {
                Console.WriteLine($"     Order {error.OrderId}: {error.ErrorMessage}");
            }
        }

        // Display summary
        var userIds = orders.Select(o => o.UserId).Distinct();
        await processor.DisplayOrderSummaryAsync(userIds);
    }

    private static async Task DemonstrateUnitTesting()
    {
        var mockRepo = new MockOrderRepository();
        var processor = new OrderProcessor(mockRepo);

        var testOrders = CreateSampleOrders().Take(3).ToList();

        // Test successful scenario
        Console.WriteLine("🧪 Testing successful batch save...");
        var result = await processor.ProcessOrderBatchAsync(testOrders);
        Console.WriteLine(
            $"   Result: {result.SuccessfulOrders} succeeded, {result.FailedOrders} failed"
        );

        // Test failure scenario
        Console.WriteLine("\n🧪 Testing failure scenario...");
        mockRepo.ShouldSimulateFailure = true;
        var failureResult = await processor.ProcessOrderBatchAsync(testOrders);
        Console.WriteLine(
            $"   Result: {failureResult.SuccessfulOrders} succeeded, {failureResult.FailedOrders} failed"
        );
        Console.WriteLine($"   Error: {failureResult.Errors.FirstOrDefault()?.ErrorMessage}");
    }

    private static List<Order> CreateSampleOrders()
    {
        var random = new Random();
        var orders = new List<Order>();

        for (int i = 1; i <= 20; i++)
        {
            var order = new Order
            {
                OrderId = 1000 + i,
                UserId = random.Next(100, 105), // 5 different users
                OrderDate = DateTime.Now.AddDays(-random.Next(0, 30)),
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = random.Next(1, 10),
                        Quantity = random.Next(1, 5),
                        Price = (decimal)(random.NextDouble() * 50 + 10),
                    },
                },
            };

            // Make some orders invalid for testing
            if (i % 7 == 0)
                order.UserId = 0; // Invalid user ID
            if (i % 11 == 0)
                order.Items.Clear(); // No items

            orders.Add(order);
        }

        return orders;
    }
}
