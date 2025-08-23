using System;
using System.Collections.Generic;
using System.Linq;

// ========================
// DOMAIN MODELS
// ========================
public class Order
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();

    public decimal GetTotalAmount()
    {
        return Items.Sum(item => item.Price * item.Quantity);
    }
}

public class OrderItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class SaveResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static SaveResult SuccessResult() =>
        new SaveResult { Success = true, Message = "Operation succeeded." };

    public static SaveResult FailureResult(string message) =>
        new SaveResult { Success = false, Message = message };
}

// ========================
// ABSTRACTION (DIP - Both high-level and low-level depend on this)
// ========================
public interface IOrderRepository
{
    void SaveOrder(Order order);
    Order GetOrderById(int orderId);
    List<Order> GetOrdersByUserId(int userId);
    void DeleteOrder(int orderId);
}

// ========================
// HIGH-LEVEL MODULE (Business Logic)
// ========================
public class OrderProcessor
{
    private readonly IOrderRepository _orderRepository;

    // DIP: High-level module depends on abstraction, not concrete implementation
    public OrderProcessor(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public void ProcessNewOrder(int userId, List<OrderItem> items)
    {
        Console.WriteLine("🔄 Processing new order...");

        // Business logic - this stays the same regardless of storage!
        var order = new Order
        {
            OrderId = new Random().Next(1000, 9999), // Simple ID generation
            UserId = userId,
            OrderDate = DateTime.Now,
            Items = items,
        };

        // Validate business rules
        if (order.GetTotalAmount() <= 0)
        {
            throw new InvalidOperationException("Order must have a positive total amount");
        }

        // Save using abstraction - no knowledge of SQL vs NoSQL!
        _orderRepository.SaveOrder(order);

        Console.WriteLine($"✅ Order {order.OrderId} processed successfully!");
        Console.WriteLine($"   Total Amount: ${order.GetTotalAmount():F2}");
    }

    public void DisplayUserOrders(int userId)
    {
        Console.WriteLine($"📋 Retrieving orders for User {userId}...");

        var userOrders = _orderRepository.GetOrdersByUserId(userId);

        if (!userOrders.Any())
        {
            Console.WriteLine("   No orders found.");
            return;
        }

        foreach (var order in userOrders)
        {
            Console.WriteLine(
                $"   Order {order.OrderId}: ${order.GetTotalAmount():F2} ({order.OrderDate:yyyy-MM-dd})"
            );
        }
    }
}

// ========================
// LOW-LEVEL MODULES (Implementation Details)
// ========================

// SQL Implementation
public class SqlOrderRepository : IOrderRepository
{
    private readonly List<Order> _sqlDatabase = new List<Order>(); // Simulated SQL storage

    public void SaveOrder(Order order)
    {
        Console.WriteLine($"💾 [SQL] Saving order to SQL database...");
        _sqlDatabase.Add(order);
        Console.WriteLine($"💾 [SQL] Order {order.OrderId} saved to SQL database");
    }

    public Order? GetOrderById(int orderId)
    {
        Console.WriteLine($"🔍 [SQL] Querying SQL database for order {orderId}...");
        return _sqlDatabase.FirstOrDefault(o => o.OrderId == orderId);
    }

    public List<Order> GetOrdersByUserId(int userId)
    {
        Console.WriteLine($"🔍 [SQL] Querying SQL database for user {userId} orders...");
        return _sqlDatabase.Where(o => o.UserId == userId).ToList();
    }

    public void DeleteOrder(int orderId)
    {
        Console.WriteLine($"🗑️ [SQL] Deleting order {orderId} from SQL database...");
        var order = _sqlDatabase.FirstOrDefault(o => o.OrderId == orderId);
        if (order != null)
        {
            _sqlDatabase.Remove(order);
        }
    }
}

// NoSQL Implementation
public class NoSqlOrderRepository : IOrderRepository
{
    private readonly Dictionary<int, Order> _noSqlDatabase = new Dictionary<int, Order>(); // Simulated NoSQL storage

    public void SaveOrder(Order order)
    {
        Console.WriteLine($"📄 [NoSQL] Saving order to NoSQL database...");
        _noSqlDatabase[order.OrderId] = order;
        Console.WriteLine($"📄 [NoSQL] Order {order.OrderId} saved to NoSQL database");
    }

    public Order? GetOrderById(int orderId)
    {
        Console.WriteLine($"🔍 [NoSQL] Querying NoSQL database for order {orderId}...");
        return _noSqlDatabase.TryGetValue(orderId, out var order) ? order : null;
    }

    public List<Order> GetOrdersByUserId(int userId)
    {
        Console.WriteLine($"🔍 [NoSQL] Querying NoSQL database for user {userId} orders...");
        return _noSqlDatabase.Values.Where(o => o.UserId == userId).ToList();
    }

    public void DeleteOrder(int orderId)
    {
        Console.WriteLine($"🗑️ [NoSQL] Deleting order {orderId} from NoSQL database...");
        _noSqlDatabase.Remove(orderId);
    }
}

// ========================
// DEMONSTRATION - DIP IN ACTION
// ========================
public class Program
{
    public static void Main()
    {
        Console.WriteLine("🎯 SOLID Principles - Dependency Inversion Principle (DIP) Demo");
        Console.WriteLine("================================================================\n");

        // Sample order items
        var orderItems = new List<OrderItem>
        {
            new OrderItem
            {
                ProductId = 101,
                Quantity = 2,
                Price = 25.99M,
            },
            new OrderItem
            {
                ProductId = 102,
                Quantity = 1,
                Price = 15.50M,
            },
        };

        Console.WriteLine("📊 SCENARIO 1: Using SQL Database");
        Console.WriteLine("----------------------------------");

        // Using SQL implementation
        IOrderRepository sqlRepository = new SqlOrderRepository();
        var orderProcessorWithSql = new OrderProcessor(sqlRepository);

        orderProcessorWithSql.ProcessNewOrder(userId: 123, items: orderItems);
        orderProcessorWithSql.DisplayUserOrders(userId: 123);

        Console.WriteLine("\n" + new string('=', 50) + "\n");

        Console.WriteLine("📊 SCENARIO 2: Switching to NoSQL Database");
        Console.WriteLine("-------------------------------------------");

        // Switching to NoSQL - NO CHANGES to OrderProcessor needed!
        IOrderRepository noSqlRepository = new NoSqlOrderRepository();
        var orderProcessorWithNoSql = new OrderProcessor(noSqlRepository);

        orderProcessorWithNoSql.ProcessNewOrder(userId: 456, items: orderItems);
        orderProcessorWithNoSql.DisplayUserOrders(userId: 456);

        Console.WriteLine("\n🎯 KEY INSIGHT:");
        Console.WriteLine("---------------");
        Console.WriteLine("✅ OrderProcessor (high-level) never changed!");
        Console.WriteLine("✅ We switched storage completely without touching business logic!");
        Console.WriteLine(
            "✅ Both high-level and low-level modules depend on IOrderRepository abstraction!"
        );
        Console.WriteLine("\nThis is the power of Dependency Inversion Principle! 🚀");
    }
}
